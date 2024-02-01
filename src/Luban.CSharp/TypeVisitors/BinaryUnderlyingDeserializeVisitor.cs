using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

public class BinaryUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static BinaryUnderlyingDeserializeVisitor Ins { get; } = new();

    public virtual string Accept(TBool type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadBool();";
    }

    public virtual string Accept(TByte type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadByte();";
    }

    public virtual string Accept(TShort type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadShort();";
    }
    public virtual string Accept(TInt type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadInt();";
    }

    public virtual string Accept(TLong type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadLong();";
    }

    public virtual string Accept(TFloat type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadFloat();";
    }

    public virtual string Accept(TDouble type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadDouble();";
    }

    public virtual string Accept(TEnum type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = ({type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}){bufName}.ReadInt();";
    }

    public virtual string Accept(TString type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadString();";
    }  
    
    public virtual string Accept(TTable type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.ReadString();";
    }

    public virtual string Accept(TDateTime type, string bufName, string fieldName, int depth)
    {
        string src = $"{bufName}.ReadLong()";
        return $"{fieldName} = {src};";
    }

    public virtual string Accept(TBean type, string bufName, string fieldName, int depth)
    {
        string src = $"{type.DefBean.FullName}.Deserialize{type.DefBean.Name}({bufName})";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public virtual string Accept(TArray type, string bufName, string fieldName, int depth)
    {
        string n = $"__n{depth}";
        string e = $"__e{depth}";
        string index = $"__index{depth}";
        string typeStr = $"{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}[{n}]";
        if (type.Dimension > 1)
        {
            typeStr = $"{type.FinalElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}[{n}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }
        
        return 
               $"{{\nint {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);" + "\n" +
               $"{fieldName} = new {typeStr};" + "\n" +
               $"for(var {index} = 0 ; {index} < {n} ; {index}++) " + "\n" +
               $"{{ " + "\n" +
               $"{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {e};" + "\n" +
               $"{type.ElementType.Apply(this, bufName, $"{e}", depth + 1)} " + "\n" +
               $"{fieldName}[{index}] = {e};" + "\n" +
               $"}}}} ";
    }

    public virtual string Accept(TList type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string e = $"_e{depth}";
        string i = $"i{depth}";
        return $"{{\nint {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);" + "\n" +
               $"{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({n});" + "\n" +
               $"for(var {i} = 0 ; {i} < {n} ; {i}++) " + "\n" +
               $"{{\n" +
               $" {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {e};  " + "\n" +
               $"{type.ElementType.Apply(this, bufName, $"{e}", depth + 1)}" + "\n" +
               $" {fieldName}.Add({e});" + "\n" +
               $"}}}}";
    }

    public virtual string Accept(TSet type, string bufName, string fieldName, int depth)
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

    public virtual string Accept(TMap type, string bufName, string fieldName, int depth)
    {
        string n = $"n{depth}";
        string k = $"_k{depth}";
        string v = $"_v{depth}";
        string i = $"i{depth}";
        return $"{{\nint {n} = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);" + "\n" +
               $"{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({n} * 3 / 2);" + "\n" +
               $"for(var {i} = 0 ; {i} < {n} ; {i}++) " + "\n" +
               $"{{\n" +
               $" {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)} {k};  " + "\n" +
               $"{type.KeyType.Apply(this, bufName, k, depth + 1)} {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)} {v};" + "\n" +
               $"  {type.ValueType.Apply(this, bufName, v, depth + 1)}" + "\n" +
               $"     {fieldName}.Add({k}, {v});" + "\n" +
               $"}}}}";
    }
}
