using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    public string DefaultLanguage { get; set; } = "zh";
    public List<string> AllLanguages => m_AllLanguages;

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
        bool buildLocation = string.IsNullOrEmpty(locationFile) == false;
        m_IsNeedBuildLocation = buildLocation;
        if (m_AllData == null)
        {
            if (IsNeedBuildLocation)
            {
                m_AllLanguages = new();
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
                s_logger.Error($"LocationFile 没找到默认key {data.ToDebugString()}");
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
                sb.Append( JsonSerializer.Serialize(item));
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
    }
}
