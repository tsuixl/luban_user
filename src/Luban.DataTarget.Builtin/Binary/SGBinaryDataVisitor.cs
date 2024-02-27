using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Location;
using Luban.Serialization;
using Luban.Types;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.Binary;

public class SGBinaryDataVisitor : IDataActionVisitor2<SGBinaryDataVisitorContext>
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    public static SGBinaryDataVisitor Ins { get; } = new();

    public void Accept(DBool data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteBool(data.Value);
    }

    public void Accept(DByte data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteByte(data.Value);
    }

    public void Accept(DShort data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteShort(data.Value);
    }

    public void Accept(DInt data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteInt(data.Value);
    }

    public void Accept(DLong data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteLong(data.Value);
    }

    public void Accept(DFloat data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteFloat(data.Value);
    }

    public void Accept(DDouble data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteDouble(data.Value);
    }

    public void Accept(DEnum data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteInt(data.Value);
    }

    public void Accept(DString data, TType type, SGBinaryDataVisitorContext x)
    {
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
            if (string.IsNullOrEmpty(data.Value))
            {
                // x.byteBuf.WriteSize(0);
                x.textIndexBuf.WriteSize(0);
                x.textIndexList.Add(0);
            }
            else
            {
                var fullKey = SGTextKeyCollectionData.GetFullKey(data.Value, x.table, x.record);
                if (x.locationTextMap.TryGetValue(fullKey, out var id))
                {
                    // x.byteBuf.WriteSize(id);
                    x.textIndexBuf.WriteSize(id);
                    x.textIndexList.Add(id);
                }
                else
                {
                    s_logger.Error($"找不到 text id value:{data.Value}");
                    throw new Exception($"找不到 text id value:{data.Value}");
                }
            }
        }
        else
        {
            AcceptStringRaw(data, type, x);
        }
    }
    
    public void AcceptStringRaw(DString data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteString(data.Value);
    }

    public void Accept(DDateTime data, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteLong(data.UnixTimeOfCurrentContext);
    }

    public void Accept(DBean data, TType type, SGBinaryDataVisitorContext x)
    {
        var bean = data.Type;
        if (bean.IsAbstractType)
        {
            x.byteBuf.WriteInt(data.ImplType.Id);
        }

        var defFields = data.ImplType.HierarchyFields;
        int index = 0;
        foreach (var field in data.Fields)
        {
            var defField = (DefField)defFields[index++];
            if (!defField.NeedExport())
            {
                continue;
            }
            var fieldType = defField.CType;
            if (defField.CType.IsNullable)
            {
                if (field != null)
                {
                    x.byteBuf.WriteBool(true);
                    field.Apply(this, fieldType, x);
                }
                else
                {
                    x.byteBuf.WriteBool(false);
                }
            }
            else
            {
                field.Apply(this, fieldType, x);
            }
        }
    }

    public void WriteList(List<DType> datas, TType type, SGBinaryDataVisitorContext x)
    {
        x.byteBuf.WriteSize(datas.Count);
        foreach (var d in datas)
        {
            d.Apply(this, type, x);
        }
    }

    public void Accept(DArray data, TType type, SGBinaryDataVisitorContext x)
    {
        WriteList(data.Datas, data.Type.ElementType, x);
    }

    public void Accept(DList data, TType type, SGBinaryDataVisitorContext x)
    {
        WriteList(data.Datas, data.Type.ElementType, x);
    }

    public void Accept(DSet data, TType type, SGBinaryDataVisitorContext x)
    {
        WriteList(data.Datas, data.Type.ElementType, x);
    }

    public void Accept(DMap data, TType type, SGBinaryDataVisitorContext x)
    {
        Dictionary<DType, DType> datas = data.Datas;
        x.byteBuf.WriteSize(datas.Count);
        foreach (var e in datas)
        {
            e.Key.Apply(this, data.Type.KeyType, x);
            e.Value.Apply(this, data.Type.ValueType, x);
        }
    }
}
