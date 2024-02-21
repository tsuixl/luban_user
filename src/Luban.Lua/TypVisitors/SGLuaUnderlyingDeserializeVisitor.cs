using Luban.Location;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class SGLuaUnderlyingDeserializeVisitorContext
{
    public int methodType = 1;
    public string fieldName;
    public string paramStr;
    public int depth = 0;
    public Stack<TType> callStack = new();

    public SGLuaUnderlyingDeserializeVisitorContext()
    {
    }

    public void ParseParam(string arg)
    {
        var arr = arg.Split('|');
        methodType = int.Parse(arr[0]);
        fieldName = arr[1];
        paramStr = arr[2];
    }

    public SGLuaUnderlyingDeserializeVisitorContext Clone()
    {
        SGLuaUnderlyingDeserializeVisitorContext obj = new();
        obj.methodType = methodType;
        obj.fieldName = fieldName;
        obj.paramStr = paramStr;
        obj.depth = depth;
        obj.callStack = new(callStack);
        return obj;
    }
}

public class SGLuaUnderlyingDeserializeVisitor : DecoratorFuncVisitor<SGLuaUnderlyingDeserializeVisitorContext, string>
{
    public static SGLuaUnderlyingDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType tpye, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        throw new NotImplementedException();
    }

    public override string Accept(TBool type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readBool({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readBool({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readBool({paramStr})";
        }

        return "error";
        return "readBool";
    }

    public override string Accept(TByte type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readByte({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readByte({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readByte({paramStr})";
        }

        return "error";
        return "readByte";
    }

    public override string Accept(TShort type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readShort({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readShort({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readShort({paramStr})";
        }

        return "error";
        return "readShort";
    }

    public override string Accept(TInt type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readInt({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readInt({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readInt({paramStr})";
        }

        return "error";
        return "readInt";
    }

    public override string Accept(TLong type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readLong({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readLong({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readLong({paramStr})";
        }

        return "error";
        return "readLong";
    }

    public override string Accept(TFloat type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readFloat({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readFloat({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readFloat({paramStr})";
        }

        return "error";
        return "readFloat";
    }

    public override string Accept(TDouble type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readDouble({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readDouble({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readDouble({paramStr})";
        }

        return "error";
        return "readDouble";
    }

    public override string Accept(TEnum type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readInt({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readInt({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readInt({paramStr})";
        }

        return "error";
        return "readInt";
    }

    public override string Accept(TString type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            if (LocationManager.IsTextField(type))
            {
                return $"{fieldName} = readText({paramStr})";
            }

            return $"{fieldName} = readString({paramStr})";
        }
        else if (methodType == 2)
        {
            if (LocationManager.IsTextField(type))
            {
                return $"{fieldName} = readText({paramStr})";
            }

            return $"--{fieldName} = readString({paramStr})";
        }
        else if (methodType == 3)
        {
            if (LocationManager.IsTextField(type))
            {
                return $"{fieldName} = readText({paramStr})";
            }

            return $"--{fieldName} = readString({paramStr})";
        }

        return "error";
        return "readString";
    }

    public override string Accept(TBean type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = beans.{type.DefBean.FullName}.__deserialize({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"beans.{type.DefBean.FullName}.__reDeserializeText({fieldName}, {paramStr})";
        }
        else if (methodType == 3)
        {
            return $"beans.{type.DefBean.FullName}.__reDeserializeTextLua({fieldName}, {paramStr})";
        }

        return "error";
        return $"beans.{type.DefBean.FullName}.__deserialize";
    }

    public override string Accept(TArray type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        var depth = x.callStack.Count;
        x.callStack.Push(type);
        
        string result = "";
        string countName = $"_n{depth}";
        string itemName = $"_e{depth}";
        string indexName = $"_i{depth}";

        var itemContext = x.Clone();
        itemContext.fieldName = itemName;
        
        if (methodType == 1)
        {
            result = $@"
do
    {fieldName} = {{}}
    local {countName} = readSize()
    for {indexName}=1, {countName} do
        local {itemName}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        tinsert({fieldName}, {itemName})
    end
end";
        }
        else if (methodType == 2)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {

                string getStr = type.ElementType.IsValueType ? "nil" : $"{fieldName}[{indexName}]";
                result = $@"
do
    local {countName} = #{fieldName}
    for {indexName}=1, {countName} do
        local {itemName} = {getStr}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        {(type.ElementType.IsValueType?"":"--")}{fieldName}[{indexName}] = {indexName}
    end
end";
            }
        }
        else if (methodType == 3)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {

                string getStr = type.ElementType.IsValueType ? "nil" : $"{fieldName}[{indexName}]";
                result = $@"
do
    local {countName} = #{fieldName}
    for {indexName}=1, {countName} do
        local {itemName} = {getStr}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        {(type.ElementType.IsValueType?"":"--")}{fieldName}[{indexName}] = {indexName}
    end
end";
            }
        }

        x.callStack.Pop();
        return result;
        return "readList";
    }

    public override string Accept(TList type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        var depth = x.callStack.Count;
        x.callStack.Push(type);
        
        string result = "";
        string countName = $"_n{depth}";
        string itemName = $"_e{depth}";
        string indexName = $"_i{depth}";

        var itemContext = x.Clone();
        itemContext.fieldName = itemName;
        
        if (methodType == 1)
        {
            result = $@"
do
    {fieldName} = {{}}
    local {countName} = readSize()
    for {indexName}=1, {countName} do
        local {itemName}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        tinsert({fieldName}, {itemName})
    end
end";
        }
        else if (methodType == 2)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {

                string getStr = type.ElementType.IsValueType ? "nil" : $"{fieldName}[{indexName}]";
                result = $@"
do
    local {countName} = #{fieldName}
    for {indexName}=1, {countName} do
        local {itemName} = {getStr}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        {(type.ElementType.IsValueType?"":"--")}{fieldName}[{indexName}] = {indexName}
    end
end";
            }
        }
        else if (methodType == 3)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {

                string getStr = type.ElementType.IsValueType ? "nil" : $"{fieldName}[{indexName}]";
                result = $@"
do
    local {countName} = #{fieldName}
    for {indexName}=1, {countName} do
        local {itemName} = {getStr}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        {(type.ElementType.IsValueType?"":"--")}{fieldName}[{indexName}] = {indexName}
    end
end";
            }
        }

        x.callStack.Pop();
        return result;
        return "readList";
    }

    public override string Accept(TSet type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
                var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        var depth = x.callStack.Count;
        x.callStack.Push(type);
        
        string result = "";
        string countName = $"_n{depth}";
        string itemName = $"_e{depth}";
        string indexName = $"_i{depth}";

        var itemContext = x.Clone();
        itemContext.fieldName = itemName;
        
        if (methodType == 1)
        {
            result = $@"
do
    {fieldName} = {{}}
    local {countName} = readSize()
    for {indexName}=1, {countName} do
        local {itemName}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        tinsert({fieldName}, {itemName})
    end
end";
        }
        else if (methodType == 2)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {

                string getStr = type.ElementType.IsValueType ? "nil" : $"{fieldName}[{indexName}]";
                result = $@"
do
    local {countName} = #{fieldName}
    for {indexName}=1, {countName} do
        local {itemName} = {getStr}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        {(type.ElementType.IsValueType?"":"--")}{fieldName}[{indexName}] = {indexName}
    end
end";
            }
        }
        else if (methodType == 3)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {

                string getStr = type.ElementType.IsValueType ? "nil" : $"{fieldName}[{indexName}]";
                result = $@"
do
    local {countName} = #{fieldName}
    for {indexName}=1, {countName} do
        local {itemName} = {getStr}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        {(type.ElementType.IsValueType?"":"--")}{fieldName}[{indexName}] = {indexName}
    end
end";
            }
        }

        x.callStack.Pop();
        return result;
        return "readSet";
    }

    public override string Accept(TMap type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
                var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        var depth = x.callStack.Count;
        x.callStack.Push(type);
        
        string result = "";
        string countName = $"_n{depth}";
        string itemName = $"_e{depth}";
        string indexName = $"_i{depth}";
        string keyName = $"_k{depth}";

        var itemContext = x.Clone();
        itemContext.fieldName = itemName;
        var keyContext = x.Clone();
        keyContext.fieldName = keyName;
        
        if (methodType == 1)
        {
            result = $@"
do
    {fieldName} = {{}}
    local {countName} = readSize()
    for {indexName}=1, {countName} do
        local {keyName}
        {type.KeyType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, keyContext)}
        local {itemName}
        {type.ElementType.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, itemContext)}
        {fieldName}[{keyName}] = {itemName}
    end
end";
        }
        else if (methodType == 2)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {
                result = $"--{fieldName} not implement";
            }
        }
        else if (methodType == 3)
        {
            if (!LocationManager.IsShouldReLoadTextField(type))
            {
                result = $"--{fieldName}";
            }
            else
            {
                result = $"--{fieldName} not implement";
            }
        }

        x.callStack.Pop();
        return result;
        return "readMap";
    }

    public override string Accept(TTable type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"--{fieldName} = readTable({paramStr}) not implement";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readTable({paramStr}) not implement";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readTable({paramStr}) not implement";
        }

        return "error";
        return "readTable";
    }

    public override string Accept(TDateTime type, SGLuaUnderlyingDeserializeVisitorContext x)
    {
        var methodType = x.methodType;
        var fieldName = x.fieldName;
        var paramStr = x.paramStr;
        if (methodType == 1)
        {
            return $"{fieldName} = readLong({paramStr})";
        }
        else if (methodType == 2)
        {
            return $"--{fieldName} = readLong({paramStr})";
        }
        else if (methodType == 3)
        {
            return $"--{fieldName} = readLong({paramStr})";
        }

        return "error";
        return "readLong";
    }
}
