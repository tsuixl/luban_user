using Luban.Serialization;
using NLog.LayoutRenderers.Wrappers;

namespace Luban.DataExporter.Builtin.Binary;

public class SGBinaryDataVisitorContext
{
    public ByteBuf byteBuf;
    public Dictionary<string, int> localtionTextMap;
    public bool buildLocation = false;
}
