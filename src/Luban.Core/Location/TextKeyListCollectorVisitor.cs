

using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Types;

namespace Luban.Location;


/// <summary>
/// 检查 相同key的text,原始值必须相同
/// </summary>
public class SGTextKeyListCollectorVisitor : IDataActionVisitor2<SGTextKeyCollection>
{
    public static SGTextKeyListCollectorVisitor Ins { get; } = new SGTextKeyListCollectorVisitor();

    public void Accept(DBool data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DByte data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DShort data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DInt data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DLong data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DFloat data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DDouble data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DEnum data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DString data, TType type, SGTextKeyCollection x)
    {
        if (data != null &&  LocationManager.IsTextField(type, false))
        {
            x.AddKey(data.Value, x);
        }
    }

    public void Accept(DDateTime data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DBean data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DArray data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DList data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DSet data, TType type, SGTextKeyCollection x)
    {

    }

    public void Accept(DMap data, TType type, SGTextKeyCollection x)
    {

    }
}
