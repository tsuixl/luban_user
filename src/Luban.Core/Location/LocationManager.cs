using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.L10N;
using Luban.Types;
using Scriban.Runtime;

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
        public string extendKey;
        public List<LocationItem> items { get; set; } = new();

        public string ToDebugString()
        {
            var language = LocationManager.Ins.ConfigFileLanguage;
            var item = GetItem(language);
            string key = item?.content;
            return $"id:{id} key:{key}";
        }

        public string GetKey()
        {
            var language = LocationManager.Ins.ConfigFileLanguage;
            var item = GetItem(language);
            if (item == null)
            {
                Console.WriteLine($"找不到默认语言文本 id:{id} {language} ");
            }

            if (string.IsNullOrEmpty(extendKey))
            {
                return item != null ? item.content : "";
            }
            else
            {
                return item != null ? extendKey +"_"+ item.content : "";
            }
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
    private List<string> m_AllLanguages = new();
    private List<string> m_ExportLanguages = new();

    public string ConfigFileLanguage { get; set; } = "zh";
    
    public string ExportDefaultLanguage { get; set; } = "zh";
    public List<string> AllLanguages => m_AllLanguages;
    public List<string> ExportLanguages => m_ExportLanguages;

    private bool m_IsNeedBuildLocation = false;

    private LocationAllData m_OldDdatas = new LocationAllData();
    private List<List<string>> m_OldRawDatas = null;

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
                ConfigFileLanguage = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationConfigFileLanguage, "zh");
                ExportDefaultLanguage = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationExportDefaultLanguage, "zh");

                m_AllData = new LocationAllData();
                if (File.Exists(locationFile))
                {
                    Type SheetLoadUtil = null;
                    SheetLoadUtil = Type.GetType("Luban.DataLoader.Builtin.Excel.SheetLoadUtil");
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        SheetLoadUtil = assembly.GetType("Luban.DataLoader.Builtin.Excel.SheetLoadUtil");
                        if(SheetLoadUtil!=null)
                            break;
                    }
                    if (SheetLoadUtil != null)
                    {
                        var methods = SheetLoadUtil.GetMethods();
                        var method = SheetLoadUtil.GetMethod("ReadExcel");
                        var rowsObj = method.Invoke(null, new object[] {locationFile});
                        var rows = rowsObj as List<List<string>>;
                        m_OldRawDatas = rows;
                        var headRow = rows[0];
                        var emptyHeadIdx = headRow.FindIndex((v) => string.IsNullOrEmpty(v));
                        emptyHeadIdx = emptyHeadIdx < 0 ? headRow.Count : emptyHeadIdx;
                        for (int i = 1; i < rows.Count; i++)
                        {
                            var row = rows[i];
                            var idV = row[0];
                            if (string.IsNullOrEmpty(idV))
                            {
                                break;
                            }
                            
                            LocationContent content = new();
                            content.id = i;
                            content.extendKey = row[1];
                            for (int j = 2; j < emptyHeadIdx; j++)
                            {
                                LocationItem item = new();
                                item.content = row[j];
                                item.language = headRow[j];
                                content.items.Add(item);
                            }
                            m_AllData.datas.Add(content);
                        }
                    }
                }

                m_OldDdatas = new LocationAllData()
                {
                    datas = new(m_AllData.datas),
                };

                HandleData();

            }
        }
        
        s_logger.Info($"LocationManager buildLocation:{buildLocation} ExportDefaultLanguage:{ExportDefaultLanguage} ConfigFileLanguage:{ConfigFileLanguage}" +
                       $" m_AllLanguages:{string.Join(',',m_AllLanguages)}" +
                       $" m_ExportLanguages:{string.Join(',',m_ExportLanguages)} ");
    }

    private void HandleData()
    {
        m_AllDataMap.Clear();
        m_AllLanguages.Clear();
        AddToAllLanguage(ConfigFileLanguage);
        AddToAllLanguage(ExportDefaultLanguage);
        for (int i = 0; i < m_AllData.datas.Count; i++)
        {
            var data = m_AllData.datas[i];
            data.id = i + 1;
            var items = data.items;
            var defaultItem = data.GetItem(ConfigFileLanguage);
            if (defaultItem == null)
            {
                s_logger.Error($"LocationFile 有数据没找到默认文本 language:{ConfigFileLanguage} data:{data.ToDebugString()}");
                throw new Exception();
            }

            var key = data.GetKey();
            if (m_AllDataMap.ContainsKey(key))
            {
                s_logger.Error($"LocationFile 重复key {data.ToDebugString()}");
                m_AllData.datas.RemoveAt(i);
                i--;
                continue;
            }

            m_AllDataMap.Add(key, data);
            foreach (var item in items)
            {
                AddToAllLanguage(item.language);
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
            m_ExportLanguages.Add(ExportDefaultLanguage);
            var lanList = exportLanguage.Split('|');
            foreach (var lan in lanList)
            {
                if (m_AllLanguages.Contains(lan) == false)
                {
                    throw new Exception($"{BuiltinOptionNames.LocationExportLanguage} error, language not find，{lan}");
                    return;
                }
                AddToListUnique(m_ExportLanguages, lan);
            }
        }
    }

    private void AddToAllLanguage(string lan)
    {
        AddToListUnique(m_AllLanguages, lan);
    }
    
    private static void AddToListUnique<T>(List<T> list, T lan)
    {
        if (list.Contains(lan) == false)
        {
            list.Add(lan);
        }
    }

    public LocationAllData GetCurLocationAllData()
    {
        return m_AllData;
    }

    public void AddText(string text, DefTable table, Record record)
    {
        var fullKey = GetContentFullKey(text, table, record);
        
        if (m_AllDataMap.TryGetValue(fullKey, out var content))
        {
        }
        else
        {
            var data = new LocationContent();
            data.extendKey = SGTextKeyCollectionData.GetExtendKey(text, table, record);
            foreach (var language in m_AllLanguages)
            {
                data.items.Add(new LocationItem() { language = language, content = language == ConfigFileLanguage ? text : "" });
            }

            m_AllData.datas.Add(data);
            m_AllDataMap.Add(data.GetKey(), data);
        }
    }

    public string GetContentValue(string key, string language, DefTable table, Record record, bool returnSelfIfNotExist = true)
    {
        if (language == ConfigFileLanguage)
        {
            return key;
        }
        var fullKey = GetContentFullKey(key, table, record);
        if (m_AllDataMap.TryGetValue(fullKey, out var content))
        {
            var item = content.GetItem(language);
            if (item != null)
            {
                return item.content;
            }

            return returnSelfIfNotExist? key :  "";
        }

        return returnSelfIfNotExist? key :  "";
    }

    public string GetContentValueByFullKey(string fullKey, string language, bool returnSelfIfNotExist = true)
    {
        if (m_AllDataMap.TryGetValue(fullKey, out var content))
        {
            var item = content.GetItem(language);
            if (item != null)
            {
                return item.content;
            }

            return returnSelfIfNotExist? fullKey :  "";
        }

        return returnSelfIfNotExist? fullKey :  "";
    }

    public ScriptObject CreateExportScriptObject()
    {
        var tableExtension = new ScriptObject
        {
            {"__tables_extension", LocationManager.Ins.ExtensionDataMap },
            {"__location_build", LocationManager.Ins.IsNeedBuildLocation },
            {"__location_config_file_language", LocationManager.Ins.ConfigFileLanguage },
            {"__location_export_default_language", LocationManager.Ins.ExportDefaultLanguage },
            {"__location_export_languages", LocationManager.Ins.ExportLanguages },
            {"__xargs", EnvManager.Current.GetAllOptions()},
        };
        return tableExtension;
    }

    public void WriteLocationFile()
    {
        if (IsNeedBuildLocation == false)
        {
            return;
        }

        var addList = m_AllData.datas.Where((d)=>m_OldDdatas.datas.Contains(d)==false).ToList() ;
        addList.Sort((d1, d2) =>
        {
            return d1.GetKey().CompareTo(d2.GetKey());
        });
        m_AllData.datas.Clear();
        m_AllData.datas.AddRange(m_OldDdatas.datas);
        m_AllData.datas.AddRange(addList);
        for (int i = 0; i < m_AllData.datas.Count; i++)
        {
            var data = m_AllData.datas[i];
            data.id = i + 1;
        }

        var locationFile = EnvManager.Current.GetOptionOrDefaultRaw(BuiltinOptionNames.LocationFile, "");
        var dir = Path.GetDirectoryName(locationFile);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        

        var newFileExcel = Path.GetFileNameWithoutExtension(locationFile) + "_new" +".xlsx";
        newFileExcel = Path.Combine(dir, newFileExcel);
        List<List<string>> excelDatas = m_OldRawDatas.Select((sub) => { return new List<string>(sub);}).ToList();
        List<string> headRow = new(){"id","extend_key"};
        headRow.AddRange(m_AllLanguages);
        ListSetValue(excelDatas, 0, headRow);
        for(int i=0; i<m_AllData.datas.Count; i++)
        {
            var content = m_AllData.datas[i];
            List<string> row = new();
            row.Add(content.id.ToString());
            row.Add(content.extendKey.ToString());
            foreach (var language in m_AllLanguages)
            {
                var item = content.GetItem(language);
                var v = item != null ? item.content : "";
                if (string.IsNullOrEmpty(v))
                {
                    v = content.GetKey();
                }
                row.Add(v);
            }
            ListSetValue(excelDatas, i+1, row);
        }

        Type SheetLoadUtil = null;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            SheetLoadUtil = assembly.GetType("Luban.DataLoader.Builtin.Excel.SheetLoadUtil");
            if(SheetLoadUtil!=null)
                break;
        }
        if (SheetLoadUtil != null)
        {
            var methods = SheetLoadUtil.GetMethods();
            var method = SheetLoadUtil.GetMethod("WriteExcelStringList");
            method.Invoke(null, new object[] {newFileExcel, excelDatas});
        }
    }

    private void ListSetValue<T>(List<List<T>> arr, int idx1, List<T> v)
    {
        for (int i = 0; i < v.Count; i++)
        {
            ListSetValue(arr, idx1, i, v[i]);
        }
    }
    
    private void ListSetValue<T>(List<List<T>> arr, int idx1, int idx2, T v)
    {
        ExpandList(arr, idx1+1);
        var list1 = arr[idx1];
        if (list1 == null)
        {
            list1 = new List<T>();
            arr[idx1] = list1;
        }
        ExpandList(list1, idx2+1);
        list1[idx2] = v;
    }
    private void ExpandList<T>(IList<T> list, int size)
    {
        while (list.Count < size)
        {
            list.Add(default(T));
        }
    }
    
    
    
    public class TableExtensionData
    {
        public DefTable table;
        public bool hasText = false;
        public Dictionary<string, int> locationTextMap;
        public List<SGTextKeyCollectionData> locationTextList;
        public List<DefField> textFields;
        public List<TType> textTTypes;
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
            List<SGTextKeyCollectionData> textList = null;
            Dictionary<string, int> textMap = buildLocation? GetTableAllText(table, records, null, out textList) : null;
            var extensionData = new TableExtensionData()
            {
                table = table, 
                locationTextMap = textMap, 
                locationTextList = textList,
            };
            extensionData.hasText = textList != null && textList.Count > 1;
            m_ExtensionDataMap.Add(table, extensionData);
            if (extensionData.locationTextList != null)
            {
                foreach (var text in extensionData.locationTextList)
                {
                    LocationManager.Ins.AddText(text.text, text.table, text.record);
                }
            }

            List<DefField> textFields = new();
            foreach (var field in table.ValueTType.DefBean.Fields)
            {
                if (IsTextField(field.CType, false))
                {
                    textFields.Add(field);
                }
            }

            var textTTypes = textFields.Select((f) => f.CType).ToList();

            extensionData.textFields = textFields;
            extensionData.textTTypes = textTTypes;
        }
    }

    public static string GetContentFullKey(string key, DefTable table, Record record)
    {
        return SGTextKeyCollectionData.GetFullKey(key, table, record);
    }
    public static SGTextKeyCollectionData GetSGTextKeyCollectionData(string key, DefTable table, Record record)
    {
        var data = SGTextKeyCollectionData.Get(key, table, record);
        return data;
    }

    public static bool IsTextField(TType type, bool checkNeedBuilLocation = true)
    {
        if (checkNeedBuilLocation && !LocationManager.Ins.IsNeedBuildLocation)
        {
            return false;
        }
        if (type is TString)
        {
            if (type != null && type.HasTag("text"))
            {
                return true;
            }
        }
        return false;
    }

    private static int s_IsShouldReLoadTextFieldCalls = 0;
    public static bool IsShouldReLoadTextField(TType type)
    {
        s_IsShouldReLoadTextFieldCalls = 0;
        var ret = IsShouldReLoadTextFieldPrivate(type);
        s_IsShouldReLoadTextFieldCalls = 0;
        return ret;
    }
    
    private static bool IsShouldReLoadTextFieldPrivate(TType type)
    {
        s_IsShouldReLoadTextFieldCalls++;
        if (s_IsShouldReLoadTextFieldCalls > 100)
        {
            throw new Exception("IsShouldReLoadTextField call max");
        }
        
        if (type is TList tList)
        {
            return IsShouldReLoadTextFieldPrivate(tList.ElementType);
        }
        else if(type is TArray tArray)
        {
            return IsShouldReLoadTextFieldPrivate(tArray.ElementType);
        }
        else if(type is TSet tSet)
        {
            return IsShouldReLoadTextFieldPrivate(tSet.ElementType);
        }
        else if(type is TMap tMap)
        {
            return IsShouldReLoadTextFieldPrivate(tMap.ElementType);
        }
        else if (type is TBean tBean)
        {
            var fields = tBean.DefBean.HierarchyExportFields;
            foreach (var field in fields)
            {
                if (IsShouldReLoadTextFieldPrivate(field.CType))
                {
                    return true;
                }
            }

            return false;
        }
        if (type.IsValueType && !LocationManager.IsTextField(type))
        {
            return false;
        }
        return true;
    }
    
    
    
    private Dictionary<string, int> GetTableAllText(DefTable table, List<Record> records, List<SGTextKeyCollectionData> oldData, out List<SGTextKeyCollectionData> textList)
    {
        var textCollection = new SGTextKeyCollection();

        var visitor = new DataActionHelpVisitor2<SGTextKeyCollection>(SGTextKeyListCollectorVisitor.Ins);

        // TableVisitor.Ins.Visit(table, visitor, textCollection);
        foreach (Record r in records)
        {
            DBean data = r.Data;
            textCollection.table = table;
            textCollection.curRecord = r;
            data.Apply(visitor, table.ValueTType, textCollection);
        }

        
        var keys = textCollection.textMap.Keys.ToList();
        keys.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

        var datas = new List<SGTextKeyCollectionData>();
        textList = datas;
        datas.Add(new SGTextKeyCollectionData(){fullKey = "", text = "", extendTextKey = ""});
        if (oldData != null)
        {
            foreach (var data in oldData)
            {
                if (SGTextKeyCollectionData.GetFromList(datas, data.fullKey) == null)
                {
                    datas.Add(data);
                }
            }
        }

        foreach (var key in keys)
        {
            if (SGTextKeyCollectionData.GetFromList(datas, key) == null)
            {
                datas.Add(textCollection.textMap[key]);
            }
        }

        var map = new Dictionary<string, int>();
        for(int i=0; i<datas.Count; i++)
        {
            map.Add(datas[i].fullKey, i);
        }

        // string outputFile = EnvManager.Current.GetOption(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextKeyListFile, false);

        return map;
    }
}
