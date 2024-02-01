
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
    public partial class TestConfig : Luban.TableBase
    {
        private System.Func<string, int, int, ByteBuf> _byteBufLoader;
        
        private Tables _tables;
        

        private System.Collections.Generic.Dictionary<int, TestItem> _dataMap;
        private System.Collections.Generic.Dictionary<int, int> _offsetMap;
        private System.Collections.Generic.Dictionary<int, int> _lengthMap;
        private System.Collections.Generic.Dictionary<int, ByteBuf> _byteBufMap;
        
        private System.Collections.Generic.List<TestItem> _dataList;

        public override string TableName { get { return "TestConfig"; } }
        
        public override string FileName { get { return "Test/TestConfig"; } }
        
        public override bool IsLazy { get { return true; } }
        
        public override bool HasLocationText { get { return true; } }

        public TestConfig()
        {
        
        }

        public void LoadData(ByteBuf buff, System.Func<string, int, int, ByteBuf> byteBufLoader, string[] textList)
        {
            bool isReload = Loaded;
            _dataMap = new System.Collections.Generic.Dictionary<int, TestItem>(Count);
            _byteBufMap = new System.Collections.Generic.Dictionary<int, ByteBuf>(Count);
            _offsetMap = new System.Collections.Generic.Dictionary<int, int>(Count);
            _lengthMap = new System.Collections.Generic.Dictionary<int, int>(Count);
            _byteBufLoader = byteBufLoader;
            
            m_TextList =  textList;
            Count = buff.ReadSize();

            for (int n = Count; n > 0; --n)
            {
                int key;
                key = buff.ReadInt();
                int offset = buff.ReadInt();
                int length = buff.ReadInt();
                _offsetMap.Add(key, offset);
                _lengthMap.Add(key, length);
            }
            
            Loaded = true;
        }
        
        public void LoadData(ByteBuf buff, string[] textList)
        {
            bool isReload = Loaded;
            _dataMap = new System.Collections.Generic.Dictionary<int, TestItem>(Count);
            _dataList = new System.Collections.Generic.List<TestItem>(Count);
   
            m_TextList =  textList;
            Count = buff.ReadSize();   
            for(int n = Count ; n > 0 ; --n)
            {
                TestItem _v;
                _v = TestItem.S_Deserialize(buff, textList);
                _dataList.Add(_v);
                _dataMap.Add(_v.Id, _v);
            }
            
            Loaded = true;
        }

        public void LoadAll(System.Action<int,TestItem> onLoad = null)
        {
            foreach(var (key,offset) in _offsetMap)
            {
                var value = this.Get(key);
                if (value != null)
                {
                    onLoad?.Invoke(key, value);
                }
            }
        }

        public TestItem GetOrDefault(int key) => this.Get(key) ?? default;
        public TestItem Get(int key)
        {
            if (_dataMap.TryGetValue(key, out var v))
            {
                return v;
            }
            int offset = _offsetMap[key];
            int length = _lengthMap[key];
            ByteBuf buf = this._byteBufLoader(FileName, offset, length);
            var textList = m_TextList;
            v = TestItem.S_Deserialize(buf, textList);;
            _dataMap[key] = v;
            v.ResolveRef(_tables);
            return v;
        }
        
        public ByteBuf GetByteBuf(int key)
        {
            if (_byteBufMap.TryGetValue(key, out var v))
            {
                return v;
            }
            int offset = _offsetMap[key];
            int length = _lengthMap[key];
            ByteBuf buf = this._byteBufLoader(FileName, offset, length);
            _byteBufMap.Add(key,buf);
            return buf;
        }
        
        public TestItem this[int key] => this.Get(key);

        public void ResolveRef(Tables tables)
        {
            this._tables = tables;
        }


    }


}


