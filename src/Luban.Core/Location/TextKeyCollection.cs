using System.Globalization;
using Luban.DataVisitors;
using Luban.Defs;

namespace Luban.Location;


public class SGTextKeyCollectionData
{
    public string fullKey;
    public string text;
    public string extendTextKey;

    public DefTable table;
    public Record record;

    public static string GetExtendKey(string text, DefTable table, Record record)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }
        string textKey = text;
        var extendTextKey = "";
        var fullKey = "";
        string tableKey = table.GetTag("text_key");
        if (tableKey == null)
            tableKey = "0";
            
        if (tableKey == "0")
        {
            fullKey = textKey;
        }
        else if (tableKey == "1")
        {
            extendTextKey = table.Name;
            textKey = extendTextKey + "_" + text;
            fullKey = textKey;
        }
        else if(tableKey == "2")
        {
            var dfield = record.Data.GetField(table.Index);
            var indexStr = dfield.Apply(ToStringVisitor.Ins);
            extendTextKey = table.Name + "_" + indexStr;
            textKey = extendTextKey + "_" + text;
            fullKey = textKey;
        }

        return extendTextKey;
    }
    public static string GetFullKey(string text, DefTable table, Record record)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }
        string textKey = text;
        var extendTextKey = "";
        var fullKey = "";
        string tableKey = table.GetTag("text_key");
        if (tableKey == null)
            tableKey = "0";
            
        if (tableKey == "0")
        {
            fullKey = textKey;
        }
        else if (tableKey == "1")
        {
            extendTextKey = table.Name;
            textKey = extendTextKey + "_" + text;
            fullKey = textKey;
        }
        else if(tableKey == "2")
        {
            var dfield = record.Data.GetField(table.Index);
            var indexStr = dfield.Apply(ToStringVisitor.Ins);
            extendTextKey = table.Name + "_" + indexStr;
            textKey = extendTextKey + "_" + text;
            fullKey = textKey;
        }

        return fullKey;
    }
    
    public static SGTextKeyCollectionData GetFromList(IList<SGTextKeyCollectionData> list, string fullKey)
    {
        foreach (var item in list)
        {
            if (item.fullKey == fullKey)
            {
                return item;
            }
        }

        return null;
    }
    
    public static SGTextKeyCollectionData Get(string text, DefTable table, Record record)
    {
        SGTextKeyCollectionData value = new();
        value.InitByData(text, table, record);
        return value;
    }
    public SGTextKeyCollectionData InitByData(string text, DefTable table, Record record)
    {
        this.table = table;
        this.record = record;
        string textKey = text;
        this.text = text;
        this.extendTextKey = GetExtendKey(text, table, record);
        this.fullKey = GetFullKey(text, table, record);
        return this;
    }

    public string GetContent(string language)
    {
        return LocationManager.Ins.GetContentValueByFullKey(this.fullKey, language);
    }
}


public class SGTextKeyCollection
{
    public DefTable table;
    public Record curRecord;

    public Dictionary<string, SGTextKeyCollectionData> textMap = new();


    public void AddKey(string key, SGTextKeyCollection x)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            var data = SGTextKeyCollectionData.Get(key, x.table, x.curRecord);
            textMap[data.fullKey] = data;
        }
    }

}
