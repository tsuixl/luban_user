﻿using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.L10N;
using Luban.Types;

namespace Luban.Location;

public class LocationManager
{
    [Serializable]
    public class LocationItem
    {
        public string language { get; set; } = "";
        public string content { get; set; } = "";
    }

    [Serializable]
    public class LocationContent
    {
        // public string key;
        public int id { get; set; } = 0;
        public List<LocationItem> items { get; set; } = new();

        public string ToDebugString()
        {
            var language = LocationManager.Ins.DefaultLanguage;
            var item = GetItem(language);
            string key = item?.content;
            return $"id:{id} key:{key}";
        }

        public string GetKey()
        {
            var language = LocationManager.Ins.DefaultLanguage;
            var item = GetItem(language);
            if (item == null)
            {
                Console.WriteLine($"找不到默认语言文本 id:{id} {language} ");
            }

            return item != null ? item.content : "";
        }

        public LocationItem GetItem(string language)
        {
            var item = items.Find((item) => item.language == language);
            return item;
        }
    }

    [Serializable]
    public class LocationAllData
    {
        public List<LocationContent> datas { get; set; } = new();
    }

    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static LocationManager Ins { get; } = new();

    private LocationAllData m_AllData = null;
    private Dictionary<string, LocationContent> m_AllDataMap = null;
    private List<string> m_AllLanguages = null;
    private List<string> m_ExportLanguages = null;

    public string DefaultLanguage { get; set; } = "zh";
    public List<string> AllLanguages => m_AllLanguages;
    public List<string> ExportLanguages => m_ExportLanguages;

    private bool m_IsNeedBuildLocation = false;

    public bool IsNeedBuildLocation
    {
        get
        {
            return m_IsNeedBuildLocation;
        }
    }

    public void Init()
    {
        var locationFile = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationFile, "");
        var exportLanguage = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationExportLanguage, "all");
        bool buildLocation = exportLanguage=="none" || string.IsNullOrEmpty(locationFile) == false;
        m_IsNeedBuildLocation = buildLocation;
        if (m_AllData == null)
        {
            if (IsNeedBuildLocation)
            {
                m_AllLanguages = new();
                m_ExportLanguages = new();
                m_AllDataMap = new();
                DefaultLanguage = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationDefaultLanguage, "zh");
                m_AllLanguages.Add(DefaultLanguage);
                var text = File.Exists(locationFile) ? File.ReadAllText(locationFile) : null;
                if (string.IsNullOrEmpty(text))
                {
                    m_AllData = new LocationAllData();
                }
                else
                {
                    m_AllData = JsonSerializer.Deserialize<LocationAllData>(text);
                }

                HandleData();

                WriteLocationFile();
            }
        }
    }

    private void HandleData()
    {
        m_AllDataMap.Clear();
        m_AllLanguages.Clear();
        m_AllLanguages.Add(DefaultLanguage);
        for (int i = 0; i < m_AllData.datas.Count; i++)
        {
            var data = m_AllData.datas[i];
            data.id = i + 1;
            var items = data.items;
            var defaultItem = data.GetItem(DefaultLanguage);
            if (defaultItem == null)
            {
                s_logger.Error($"LocationFile 有数据没找到默认文本 language:{DefaultLanguage} data:{data.ToDebugString()}");
                throw new Exception();
            }

            var key = defaultItem.content;
            if (m_AllDataMap.ContainsKey(key))
            {
                s_logger.Error($"LocationFile 重复key {data.ToDebugString()}");
                m_AllData.datas.RemoveAt(i);
                i--;
                continue;
            }

            m_AllDataMap.Add(data.GetKey(), data);
            foreach (var item in items)
            {
                if (m_AllLanguages.Contains(item.language) == false)
                {
                    m_AllLanguages.Add(item.language);
                }
            }
        }
        
        m_ExportLanguages.Clear();
        var exportLanguage = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationExportLanguage, "all");
        if (exportLanguage == "all")
        {
            m_ExportLanguages.AddRange(m_AllLanguages);
        }
        else
        {
            m_ExportLanguages.Add(DefaultLanguage);
            var lanList = exportLanguage.Split('|');
            foreach (var lan in lanList)
            {
                if (m_AllLanguages.Contains(lan) == false)
                {
                    throw new Exception($"{BuiltinOptionNames.LocationExportLanguage} error, language not find，{lan}");
                    return;
                }
                if (m_ExportLanguages.Contains(lan) == false)
                {
                    m_ExportLanguages.Add(lan);
                }
            }
        }
    }

    public LocationAllData GetCurLocationAllData()
    {
        return m_AllData;
    }

    public void AddText(string text)
    {
        if (m_AllDataMap.TryGetValue(text, out var content))
        {
        }
        else
        {
            var data = new LocationContent();
            foreach (var language in m_AllLanguages)
            {
                data.items.Add(new LocationItem() { language = language, content = language == DefaultLanguage ? text : "" });
            }

            m_AllData.datas.Add(data);
            m_AllDataMap.Add(text, data);
        }
    }

    public string GetContentValue(string key, string language)
    {
        if (language == DefaultLanguage)
        {
            return key;
        }

        if (m_AllDataMap.TryGetValue(key, out var content))
        {
            var item = content.GetItem(language);
            if (item != null)
            {
                return item.content;
            }

            return "";
        }

        return "";
    }

    public void WriteLocationFile()
    {
        if (IsNeedBuildLocation == false)
        {
            return;
        }

        HandleData();
        var locationFile = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationFile, "");
        var dir = Path.GetDirectoryName(locationFile);
        var newFile = Path.GetFileNameWithoutExtension(locationFile) + "_new" +Path.GetExtension(locationFile);
        newFile = Path.Combine(dir, newFile);
        StringBuilder sb = new();
        sb.Append("{\"datas\":[\n");
        int idx1 = -1;
        foreach (var content in m_AllData.datas)
        {
            idx1++;
            sb.Append("{\"id\":" + content.id + ",\"items\":[\n");
            int idx2 = -1;
            foreach (var item in content.items)
            {
                idx2++;
                sb.Append("\t");
                // sb.Append( JsonSerializer.Serialize(item));
                sb.Append( $"{{\"language\":\"{item.language}\", \"content\":\"{item.content}\"}}");
                if (idx2 != content.items.Count-1)
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append("]}");
            if (idx1 != m_AllData.datas.Count-1)
            {
                sb.Append(",");
            }

            sb.Append("\n");
        }
        sb.Append("\n]}");
        // var text = JsonSerializer.Serialize(m_AllData);
        var text = sb.ToString();
        
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(newFile, text);

        List<List<string>> excelDatas = new();
    }
    
    
    public class TableExtensionData
    {
        public DefTable table;
        public bool hasText = false;
        public Dictionary<string, int> locationTextMap;
        public List<string> locationTextList;
        public List<DefField> textFields;
    }

    private Dictionary<DefTable, TableExtensionData> m_ExtensionDataMap = new();

    public Dictionary<DefTable, TableExtensionData> ExtensionDataMap => m_ExtensionDataMap;
    
    public TableExtensionData GetExtensionData(DefTable table)
    {
        if (m_ExtensionDataMap.TryGetValue(table, out var extensionData))
        {
            return extensionData;
        }
        return null;
    }

    public void OnLoadDatas()
    {
        var tables = GenerationContext.Current.ExportTables;
        bool buildLocation = LocationManager.Ins.IsNeedBuildLocation;
        m_ExtensionDataMap.Clear();
        foreach (var table in tables)
        {
            var records = GenerationContext.Current.GetTableExportDataList(table);
            List<string> textList = null;
            Dictionary<string, int> textMap = buildLocation? GetTableAllText(table, records, null, out textList) : null;
            var extensionData = new TableExtensionData() { table = table, locationTextMap = textMap, locationTextList = textList};
            extensionData.hasText = textList != null && textList.Count > 1;
            m_ExtensionDataMap.Add(table, extensionData);
            if (extensionData.locationTextMap != null)
            {
                foreach (var text in extensionData.locationTextMap.Keys)
                {
                    LocationManager.Ins.AddText(text);
                }
            }

            List<DefField> textFields = new();
            foreach (var field in table.ValueTType.DefBean.Fields)
            {
                if (IsTextField(field.CType))
                {
                    textFields.Add(field);
                }
            }

            extensionData.textFields = textFields;
        }
    }

    public static bool IsTextField(TType type)
    {
        if (type is TString)
        {
            if (type != null && type.HasTag("text"))
            {
                return true;
            }
        }
        return false;
    }
    
    private Dictionary<string, int> GetTableAllText(DefTable table, List<Record> records, List<string> oldData, out List<string> textList)
    {
        var textCollection = new TextKeyCollection();

        var visitor = new DataActionHelpVisitor2<TextKeyCollection>(TextKeyListCollectorVisitor.Ins);

        TableVisitor.Ins.Visit(table, visitor, textCollection);

        var keys = textCollection.Keys.ToList();
        keys.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

        var datas = new List<string>();
        textList = datas;
        datas.Add("");
        if (oldData != null)
        {
            foreach (var key in oldData)
            {
                if (datas.Contains(key) == false)
                {
                    datas.Add(key);
                }
            }
        }

        foreach (var key in keys)
        {
            if (datas.Contains(key) == false)
            {
                datas.Add(key);
            }
        }

        var map = new Dictionary<string, int>();
        for(int i=0; i<datas.Count; i++)
        {
            map.Add(datas[i], i);
        }

        // string outputFile = EnvManager.Current.GetOption(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextKeyListFile, false);

        return map;
    }
}
