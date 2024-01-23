
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using System.Linq;





namespace Luban.Config
{
public partial class TestConfig2 : Luban.TableBase
{
    private System.Collections.Generic.Dictionary<int, TestItem2> _dataMap;
    private System.Collections.Generic.List<TestItem2> _dataList;
    
    public override string TableName { get { return "TestConfig2"; } }
    
    public override string FileName { get { return "Test/TestConfig2"; } }
    
    public override bool IsLazy { get { return false; } }
    
    public override bool HasLocationText { get { return true; } }
    
    public TestConfig2()
    {
    
    }
    
    public void LoadData(ByteBuf _buf, string[] textList)
    {
        bool isReload = Loaded;
        _dataMap = new System.Collections.Generic.Dictionary<int, TestItem2>();
        _dataList = new System.Collections.Generic.List<TestItem2>();

        m_TextList = textList;
        Count = _buf.ReadSize();
        for(int n = Count ; n > 0 ; --n)
        {
            TestItem2 _v;
            _v = TestItem2.DeserializeTestItem2(_buf, textList);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        
        Loaded = true;
    }

    public System.Collections.Generic.Dictionary<int, TestItem2> DataMap => _dataMap;
    public System.Collections.Generic.List<TestItem2> DataList => _dataList;

    public TestItem2 GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public TestItem2 Get(int key) => _dataMap[key];
    public TestItem2 this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}


