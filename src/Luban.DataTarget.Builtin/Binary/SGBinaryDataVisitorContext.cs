using Luban.Serialization;
using NLog.LayoutRenderers.Wrappers;

namespace Luban.DataExporter.Builtin.Binary;

public class SGBinaryDataVisitorContext
{
    public ByteBuf byteBuf;
    public ByteBuf textIndexBuf;
    public List<int> textIndexList = new();
    public Dictionary<string, int> locationTextMap;
    public bool buildLocation = false;
}
