using Luban.Location;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class SGLuaUnderlyingReDeserializeVisitor : DecoratorFuncVisitor<string, string>
{
    public class SGLuaReDeserializeMethodNameVisitor:LuaDeserializeMethodNameVisitor
    {
        public static SGLuaReDeserializeMethodNameVisitor Ins { get; } = new();
        public override string Accept(TString type)
        {
            if (LocationManager.IsTextField(type))
            {
                return $"readText";
            }
            return "readString";
        }
        
        public override string Accept(TBean type)
        {
            return $"beans['{type.DefBean.FullName}']._re_deserialize";
        }
    }
    
    public static SGLuaUnderlyingReDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string x)
    {
        return $"re_{type.Apply(SGLuaReDeserializeMethodNameVisitor.Ins)}({x})";
    }
    
    public override string Accept(TBean type, string x)
    {
        return $"{type.Apply(SGLuaReDeserializeMethodNameVisitor.Ins)}({x})";
    }

    public override string Accept(TArray type, string x)
    {
        return $"re_readArray({x}, {type.ElementType.Apply(SGLuaReDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TList type, string x)
    {
        return $"re_readList({x}, {type.ElementType.Apply(SGLuaReDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TSet type, string x)
    {
        return $"re_readSet({x}, {type.ElementType.Apply(SGLuaReDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TMap type, string x)
    {
        return $"re_readMap({x}, {type.KeyType.Apply(SGLuaReDeserializeMethodNameVisitor.Ins)}, {type.ValueType.Apply(SGLuaReDeserializeMethodNameVisitor.Ins)})";
    }
}
