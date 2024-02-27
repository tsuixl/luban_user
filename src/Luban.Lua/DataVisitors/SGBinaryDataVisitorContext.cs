using Luban.Defs;
using Luban.Serialization;
using NLog.LayoutRenderers.Wrappers;

namespace Luban.Lua;

public class SGLuaDataVisitorContext
{
    public Dictionary<string, int> locationTextMap;
    public bool buildLocation = false;
    public string language;
    public List<int> textIndexList = new();

    public DefTable table;
    public Record record;
}
