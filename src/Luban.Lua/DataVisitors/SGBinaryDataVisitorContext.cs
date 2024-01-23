using Luban.Serialization;
using NLog.LayoutRenderers.Wrappers;

namespace Luban.Lua;

public class SGLuaDataVisitorContext
{
    public Dictionary<string, int> locationTextMap;
    public bool buildLocation = false;
    public string language;
}
