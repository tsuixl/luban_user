using Luban.Location;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class SGLuaUnderlyingDeserializeVisitor : DecoratorFuncVisitor<string, string>
{
    public class SGLuaDeserializeMethodNameVisitor:LuaDeserializeMethodNameVisitor
    {
        public static SGLuaDeserializeMethodNameVisitor Ins { get; } = new();
        public override string Accept(TString type)
        {
            if (LocationManager.IsTextField(type))
            {
                return $"readText";
            }
            return "readString";
        }
    }
    
    public static SGLuaUnderlyingDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string x)
    {
        return $"{type.Apply(SGLuaDeserializeMethodNameVisitor.Ins)}({x})";
    }
    
    public override string Accept(TString type, string x)
    {
        if (LocationManager.IsTextField(type))
        {
            return $"readText({x})";
        }
        return "readString({x})";
    }
    
    public override string Accept(TArray type, string x)
    {
        return $"readArray({x}, {type.ElementType.Apply(SGLuaDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TList type, string x)
    {
        return $"readList({x}, {type.ElementType.Apply(SGLuaDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TSet type, string x)
    {
        return $"readSet({x}, {type.ElementType.Apply(SGLuaDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TMap type, string x)
    {
        return $"readMap({x}, {type.KeyType.Apply(SGLuaDeserializeMethodNameVisitor.Ins)}, {type.ValueType.Apply(SGLuaDeserializeMethodNameVisitor.Ins)})";
    }
}
