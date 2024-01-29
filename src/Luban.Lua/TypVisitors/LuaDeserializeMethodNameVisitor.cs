using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class LuaDeserializeMethodNameVisitor : ITypeFuncVisitor<string>
{
    public static LuaDeserializeMethodNameVisitor Ins { get; } = new();

    public virtual string Accept(TBool type)
    {
        return "readBool";
    }

    public virtual string Accept(TByte type)
    {
        return "readByte";
    }

    public virtual string Accept(TShort type)
    {
        return "readShort";
    }

    public virtual string Accept(TInt type)
    {
        return "readInt";
    }

    public virtual string Accept(TLong type)
    {
        return "readLong";
    }

    public virtual string Accept(TFloat type)
    {
        return "readFloat";
    }

    public virtual string Accept(TDouble type)
    {
        return "readDouble";
    }

    public virtual string Accept(TEnum type)
    {
        return "readInt";
    }

    public virtual string Accept(TString type)
    {
        return "readString";
    }

    public virtual string Accept(TBean type)
    {
        return $"beans['{type.DefBean.FullName}']._deserialize";
    }

    public virtual string Accept(TArray type)
    {
        return "readList";
    }

    public virtual string Accept(TList type)
    {
        return "readList";
    }

    public virtual string Accept(TSet type)
    {
        return "readSet";
    }

    public virtual string Accept(TMap type)
    {
        return "readMap";
    }

    public virtual string Accept(TTable type)
    {
        return "readTable";
    }

    public virtual string Accept(TDateTime type)
    {
        return "readLong";
    }
}
