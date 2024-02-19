using Luban.Defs;
using Luban.Lua.TypVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Lua.TemplateExtensions;

public class LuaBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, TType type)
    {
        return type.Apply(LuaUnderlyingDeserializeVisitor.Ins, bufName);
    }
    
    public static string SGDeserialize(string bufName, TType type)
    {
        return type.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, bufName);
    }
    
    public static string SgDeserialize(string bufName, TType type)
    {
        return type.Apply(SGLuaUnderlyingDeserializeVisitor.Ins, bufName);
    }
    
    public static string SgReDeserialize(string bufName, TType type)
    {
        return type.Apply(SGLuaUnderlyingReDeserializeVisitor.Ins, bufName);
    }
    
}
