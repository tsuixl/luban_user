using System.Text;
using Luban.DataLoader;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Location;
using Luban.Lua.DataVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.Lua;

public class SGLuaDataVisitor : IDataFuncVisitor<TType, SGLuaDataVisitorContext, string>
{
    public static SGLuaDataVisitor Ins { get; } = new();

    public string Accept(DBool data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DByte data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DShort data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DInt data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DLong data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DFloat data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DDouble data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DEnum data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DString data, TType type, SGLuaDataVisitorContext x)
    {
        var v = data.Value;
        string finnalV = v;
        bool isText = false;
        if (x.buildLocation)
        {
            if (data != null && type.HasTag("text"))
            {
                isText = true;
            }
        }

        if (isText)
        {
            if (string.IsNullOrEmpty(v))
            {
                finnalV = "";
            }
            else if (x.locationTextMap.TryGetValue(v, out var id))
            {
                finnalV = LocationManager.Ins.GetContentValue(v, x.language);
            }
            else
            {
                Console.WriteLine($"找不到 text id value:{v}");
            }
        }

        
        return DataUtil.EscapeLuaStringWithQuote(finnalV);
    }
    

    public string Accept(DDateTime data, TType type, SGLuaDataVisitorContext x)
    {
        return data.Apply(ToLuaLiteralVisitor.Ins);
    }

    public string Accept(DBean data, TType type, SGLuaDataVisitorContext x)
    {
        var sb = new StringBuilder();
        if (data.Type.IsAbstractType)
        {
            sb.Append($"{{ {FieldNames.LuaTypeNameKey}='{DataUtil.GetImplTypeName(data)}',");
        }
        else
        {
            sb.Append('{');
        }

        int index = 0;
        foreach (var f in data.Fields)
        {
            var defField = (DefField)data.ImplType.HierarchyFields[index++];
            if (f == null || !defField.NeedExport())
            {
                continue;
            }
            var fieldType = defField.CType;
            sb.Append(defField.Name).Append('=');
            sb.Append(f.Apply(this, fieldType, x));
            sb.Append(',');
        }
        sb.Append('}');
        return sb.ToString();
    }

    private string WriteList(List<DType> datas, TType type, SGLuaDataVisitorContext x)
    {
        StringBuilder sb = new();
        sb.Append('{');
        foreach (var e in datas)
        {
            sb.Append(e.Apply(this, type, x));
            sb.Append(',');
        }
        sb.Append('}');
        return sb.ToString();
    }

    public string Accept(DArray data, TType type, SGLuaDataVisitorContext x)
    {
        return WriteList(data.Datas, data.Type.ElementType, x);
    }

    public string Accept(DList data, TType type, SGLuaDataVisitorContext x)
    {
        return WriteList(data.Datas, data.Type.ElementType, x);
    }

    public string Accept(DSet data, TType type, SGLuaDataVisitorContext x)
    {
        return WriteList(data.Datas, data.Type.ElementType, x);
    }

    public string Accept(DMap data, TType type, SGLuaDataVisitorContext x)
    {
        Dictionary<DType, DType> datas = data.Datas;
        var sb = new StringBuilder();
        sb.Append('{');
        foreach (var e in datas)
        {
            sb.Append('[');
            sb.Append(e.Key.Apply(this, data.Type.KeyType, x));
            sb.Append(']');
            sb.Append('=');
            sb.Append(e.Value.Apply(this, data.Type.ValueType, x));
            sb.Append(',');
        }
        sb.Append('}');
        return sb.ToString();
    }

    public string Accept(DTable type, TType x, SGLuaDataVisitorContext y)
    {
        return "{"+ type.Value + "}";
    }
}
