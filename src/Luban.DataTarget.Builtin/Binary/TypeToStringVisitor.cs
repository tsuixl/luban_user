using System.Text;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using YamlDotNet.Core.Tokens;

namespace Luban.DataExporter.Builtin;

public class TypeToStringVisitor : DecoratorFuncVisitor<string, Dictionary<string, object>>
{
    public static TypeToStringVisitor Ins { get; } = new();

    public string SerializeAllType()
    {
        var tables = GenerationContext.Current.ExportTables;
        var beans = GenerationContext.Current.ExportBeans;
        var enums = GenerationContext.Current.ExportEnums;
        Dictionary<string, object> allData = new();
        {
            List<object> objList = new();
            foreach (var table in tables)
            {
                if(table.NeedExport()==false)
                    continue;
                Dictionary<string, object> tObj = new()
                {
                    { "FullName", table.FullName },
                    { "Name", table.Name },
                    { "OutputDataFile", table.OutputDataFile },
                    { "IsLazy", table.IsLazy },
                    { "IsOnlyLua", table.IsOnlyLua },
                    { "IsExported", table.IsExported },
                    { "Mode", table.Mode },
                    { "Index", table.Index },
                    { "ValueType", table.ValueType },
                    { "KeyTType", table.KeyTType.TypeName },
                    { "Tags", table.Tags },
                };
                objList.Add(tObj);
            }
            allData.Add("Tables", objList);
        }
        
        {
            List<object> objList = new();
            foreach (var bean in beans)
            {
                Dictionary<string, object> tObj = new()
                {
                    { "FullName", bean.FullName },
                    { "Name", bean.Name },
                    { "Parent", bean.Parent },
                    { "Alias", bean.Alias },
                    { "Id", bean.Id },
                    { "Tags", bean.Tags },
                };
                objList.Add(tObj);
                
                List<object> objList2 = new();
                foreach (var field in bean.ExportFields)
                {
                    Dictionary<string, object> tObj2 = field.CType.Apply(this, field.Name);
                    objList2.Add(tObj2);
                }
                tObj.Add("Fields", objList2);
            }
            allData.Add("Beans", objList);
        }
        
        {
            List<object> objList = new();
            foreach (var defEnum in enums)
            {
                Dictionary<string, object> tObj = new()
                {
                    { "FullName", defEnum.FullName },
                    { "Name", defEnum.Name },
                    { "IsFlags", defEnum.IsFlags},
                    { "Tags", defEnum.Tags},
                };
                objList.Add(tObj);
                
                List<object> objList2 = new();
                foreach (var item in defEnum.Items)
                {
                    Dictionary<string, object> tObj2 = new()
                    {
                        {"Name", item.Name},
                        {"Alias", item.Alias},
                        {"Value", item.Value},
                        {"IntValue", item.IntValue},
                    };
                    objList2.Add(tObj2);
                }
                tObj.Add("Items", objList2);
            }
            allData.Add("Enums", objList);
        }

        string ret = ObjectToStringJson(allData);
        return ret;
    }
    
    private string AddBlockString(string value)
    {
        return "{" + value + "}";
    }

    private string ObjectToStringJson(object v)
    {
        StringBuilder sb = new();
        if (v == null)
        {
            sb.Append("null");
        }
        else if (v is string str)
        {
            sb.Append($"\"{str}\"");
        }
        else if (v is IDictionary<string, object> map)
        {
            sb.Append($"{MapToStringJson(map)}");
        }
        else if (v is Dictionary<string, string> map2)
        {
            var map3 = ToMapObject(map2);
            sb.Append($"{MapToStringJson(map3)}");
        }
        else if (v is IList<object> list)
        {
            HandleNewLine(sb, list);
            sb.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                HandleNewLineIdx(sb, i);
                if (i > 0) sb.Append(",");
                var subV = list[i];
                sb.Append(ObjectToStringJson(subV));
            }
            sb.Append("]");
            HandleNewLine(sb, list);
        }
        else
        {
            sb.Append(v.ToString());
        }

        return sb.ToString();
    }
    
    private string MapToStringJson(IDictionary<string, object> map)
    {
        StringBuilder sb = new();
        HandleNewLine(sb, map);
        sb.Append("{");
        int idx = -1;
        foreach (var kp in map)
        {
            idx++;
            HandleNewLineIdx(sb, idx);
            if (idx > 0)
            {
                sb.Append(",");
            }
            sb.Append($"\"{kp.Key}\":");
            sb.Append($"{ObjectToStringJson(kp.Value)}");
        }
        sb.Append("}");
        HandleNewLine(sb, map);
        return sb.ToString();
    }

    private void HandleNewLine(StringBuilder sb, object v)
    {
        int checkNum = 3;
        if (v is IDictionary<string, object> map)
        {
            if (map.Count > checkNum) sb.Append("\n");
        }
        if (v is IList<object> list)
        {
            if (list.Count > checkNum) sb.Append("\n");
        }
    }
    private void HandleNewLineIdx(StringBuilder sb, int idx)
    {
        int checkNum = 5;
        if (idx > 1 && idx % checkNum == 0)
        {
            sb.Append("\n");
        }
    }

    private IDictionary<string, object> ToMapObject<T, K>(IDictionary<T, K> map)
    {
        if (map is IDictionary<string, object> map1)
        {
            return map1;
        }

        Dictionary<string, object> map2 = new();
        foreach (var kp in map)
        {
            string key = kp.Key.ToString();
            object v = kp.Value;
            if (key != null)
            {
                map2.Add(key, v);
            }
        }
        return map2;
    }

    public override Dictionary<string, object> DoAccept(TType type, string fieldName)
    {
        return TTotring(type, fieldName, null);
    }

    private Dictionary<string, object> TTotring(TType type, string fieldName, Dictionary<string, object> other)
    {
        Dictionary<string, object> obj = new()
        {
            { "TypeName", type.TypeName }, 
            { "FieldName,", fieldName }, 
            { "IsNullable", type.IsNullable },
            { "IsCollection", type.IsCollection },
            { "IsBean", type.IsBean },
            { "Tags", type.Tags},
        };
        if (other != null)
        {
            foreach (var kp in other)
            {
                obj.Add(kp.Key, kp.Value);
            }
        }

        return obj;
    }

    public override Dictionary<string, object> Accept(TEnum type, string fieldName)
    {
        Dictionary<string, object> other = new()
        {
            {"FullName", type.DefEnum.FullName}
        };
        return TTotring(type, fieldName, other);
    }

    public override Dictionary<string, object> Accept(TBean type, string fieldName)
    {
        Dictionary<string, object> other = new()
        {
            {"FullName", type.DefBean.FullName}
        };
        return TTotring(type, fieldName, other);
    }

    public override Dictionary<string, object> Accept(TArray type, string fieldName)
    {
        Dictionary<string, object> other = new()
        {
            {"ElementType", type.ElementType.Apply(this, "$0")}
        };
        return TTotring(type, fieldName, other);
    }

    public override Dictionary<string, object> Accept(TList type, string fieldName)
    {
        Dictionary<string, object> other = new()
        {
            {"ElementType", type.ElementType.Apply(this, "$0")}
        };
        return TTotring(type, fieldName, other);
    }

    public override Dictionary<string, object> Accept(TSet type, string fieldName)
    {
        Dictionary<string, object> other = new()
        {
            {"ElementType", type.ElementType.Apply(this, "$0")}
        };
        return TTotring(type, fieldName, other);
    }

    public override Dictionary<string, object> Accept(TMap type, string fieldName)
    {
        Dictionary<string, object> other = new()
        {
            {"KeyType", type.KeyType.Apply(this, "$0")},
            {"ElementType", type.ElementType.Apply(this, "$1")},

        };
        return TTotring(type, fieldName, other);
    }
}
