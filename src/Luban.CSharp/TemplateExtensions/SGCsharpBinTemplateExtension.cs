using Luban.CSharp.TypeVisitors;
using Luban.Location;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class SGCsharpBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(SGBinaryDeserializeVisitor.Ins, bufName, fieldName);
    }

    public static string ReDeserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(SGBinaryReDeserializeVisitor.Ins, bufName, fieldName);
    }
}


public class SGBinaryDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static SGBinaryDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string bufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({bufName}.ReadBool()){{ {type.Apply(SGBinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)} }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return type.Apply(SGBinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0);
        }
    }
}


public class SGBinaryUnderlyingDeserializeVisitor:BinaryUnderlyingDeserializeVisitor
{
    public static SGBinaryUnderlyingDeserializeVisitor Ins { get; } = new();
    
    public override string Accept(TString type, string bufName, string fieldName, int depth)
    {
        if (LocationManager.Ins.IsNeedBuildLocation)
        {
            if (LocationManager.IsTextField(type))
            {
                return $"{fieldName} = ReadText({bufName}, textList);";
            }
        }

        return $"{fieldName} = {bufName}.ReadString();";
    }  
    
    public override string Accept(TBean type, string bufName, string fieldName, int depth)
    {
        string src = $"{type.DefBean.FullName}.S_Deserialize({bufName}, textList)";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }
}


public class SGBinaryReDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static SGBinaryReDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string bufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({bufName}.ReadBool()){{ {type.Apply(SGBinaryUnderlyingReDeserializeVisitor.Ins, bufName, fieldName, 0)} }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return type.Apply(SGBinaryUnderlyingReDeserializeVisitor.Ins, bufName, fieldName, 0);
        }
    }
}


public class SGBinaryUnderlyingReDeserializeVisitor:BinaryUnderlyingDeserializeVisitor
{
    public static SGBinaryUnderlyingReDeserializeVisitor Ins { get; } = new();
    
    public override string Accept(TString type, string bufName, string fieldName, int depth)
    {
        if (LocationManager.Ins.IsNeedBuildLocation)
        {
            if (LocationManager.IsTextField(type))
            {
                return $"{fieldName} = ReadText({bufName}, textList);";
            }
        }

        return $"{fieldName} = {bufName}.ReadString();";
    }  
    
    public override string Accept(TBean type, string bufName, string fieldName, int depth)
    {
        
        string src = $"{type.DefBean.FullName}.S_ReDeserialize({fieldName}, {bufName}, textList);";
        return src;
    }

    public override string Accept(TArray type, string bufName, string fieldName, int depth)
    {
        string num = $"__n{depth}";
        string item = $"__e{depth}";
        string index = $"__index{depth}";
        string typeStr = $"{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}[{num}]";
        if (type.Dimension > 1)
        {
            typeStr = $"{type.FinalElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}[{num}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }

        string getStr = type.ElementType.IsValueType ? "" : $" = {fieldName}[{index}]";
        string tmp =
            @$"
{{
    int {num} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);
    for(var {index} = 0 ; {index} < {num} ; {index}++)
    {{
        {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {item}{getStr};
        {type.ElementType.Apply(this, bufName, $"{item}", depth + 1)}
        {fieldName}[{index}] = {item};
    }}
}}
";

        return tmp;
    }

    public override string Accept(TList type, string bufName, string fieldName, int depth)
    {
        string num = $"n{depth}";
        string item = $"_e{depth}";
        string index = $"i{depth}";

        string getStr = type.ElementType.IsValueType ? "" : $" = {fieldName}[{index}]";
        string tmp =
@$"
{{
    int {num} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);
    for(var {index} = 0 ; {index} < {num} ; {index}++)
    {{
        {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {item}{getStr};
        {type.ElementType.Apply(this, bufName, $"{item}", depth + 1)}
        {fieldName}[{index}] = {item};
    }}
}}
";
        return tmp;
        
    }

    public override string Accept(TSet type, string bufName, string fieldName, int depth)
    {
        
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{\nint {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);" + "\n" +
               $"{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(/*{n} * 3 / 2*/);" + "\n" +
               $"for(var {i} = 0 ; {i} < {n} ; {i}++) " + "\n" +
               $"{{\n" +
               $" {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {e}; " + "\n" +
               $" {type.ElementType.Apply(this, bufName, $"{e}", +1)} " + "\n" +
               $"{fieldName}.Add({e});" + "\n" +
               $"}}}}";
    }

    public override string Accept(TMap type, string bufName, string fieldName, int depth)
    {
        string num = $"n{depth}";
        string key = $"_k{depth}";
        string item = $"_v{depth}";
        string index = $"i{depth}";
        
        string getStr = type.ElementType.IsValueType ? "" : $" = {fieldName}[{index}]";
        string tmp =
@$"
{{
    int {num} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);
    for(var {index} = 0 ; {index} < {num} ; {index}++)
    {{
        {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)} {key};
        {type.KeyType.Apply(this, bufName, key, depth + 1)}
        {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {item}{getStr};
        {type.ElementType.Apply(this, bufName, $"{item}", depth + 1)}
        {fieldName}[{index}] = {item};
    }}
}}
";

        return tmp;
    }
}
