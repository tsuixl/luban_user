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
        string src = $"{type.DefBean.FullName}.Deserialize{type.DefBean.Name}({bufName}, textList)";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }
}
