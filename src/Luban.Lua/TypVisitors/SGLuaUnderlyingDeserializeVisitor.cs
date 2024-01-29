using Luban.Location;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class SGLuaUnderlyingDeserializeVisitor : DecoratorFuncVisitor<string, string>
{
    public static SGLuaUnderlyingDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string x)
    {
        return $"{type.Apply(LuaDeserializeMethodNameVisitor.Ins)}({x})";
    }

    public override string Accept(TString type, string x)
    {
        if (LocationManager.IsTextField(type))
        {
            
        }
        return $"readArray({x})";
    }
    
    public override string Accept(TArray type, string x)
    {
        return $"readArray({x}, {type.ElementType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TList type, string x)
    {
        return $"readList({x}, {type.ElementType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TSet type, string x)
    {
        return $"readSet({x}, {type.ElementType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
    }

    public override string Accept(TMap type, string x)
    {
        return $"readMap({x}, {type.KeyType.Apply(LuaDeserializeMethodNameVisitor.Ins)}, {type.ValueType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
    }
}
