using Luban.Datas;
using Luban.Defs;
using Luban.Lua.TypVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Lua.TemplateExtensions;

public class SGLuaDataTemplateExtension : ScriptObject
{
    public static string DeserializeRecord(Record r, SGLuaDataVisitorContext context)
    {
        DBean d = r.Data;
        context.record = r;
        var ret = d.Apply(SGLuaDataVisitor.Ins, d.TType, context);
        return ret;
    }
    public static string DeserializeRecordKeyStr(DefTable t, Record r, SGLuaDataVisitorContext context)
    {
        var d = r.Data;
        context.record = r;
        string keyStr = d.GetField(t.Index).Apply(SGLuaDataVisitor.Ins, t.KeyTType, context);
        return keyStr;
    }
    
    
    
}
