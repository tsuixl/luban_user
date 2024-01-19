
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Luban;



namespace cfg
{
    public sealed partial class TestItem2 : Luban.BeanBase
    {
        public TestItem2(ByteBuf _buf, string[] textList) 
        {
            Id = _buf.ReadInt();
            Name = _buf.ReadString();
            Desc = ReadText(_buf, textList);
            LocationText = LocationText.DeserializeLocationText(_buf, textList);
            Desc3 = ReadText(_buf, textList);
            {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);
            DescList = new System.Collections.Generic.List<LocationText>(n0);
            for(var i0 = 0 ; i0 < n0 ; i0++) 
            { LocationText _e0;  
            _e0 = LocationText.DeserializeLocationText(_buf, textList);
             DescList.Add(_e0);
            }}
            {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);
            DescList2 = new System.Collections.Generic.List<string>(n0);
            for(var i0 = 0 ; i0 < n0 ; i0++) 
            { string _e0;  
            _e0 = ReadText(_buf, textList);
             DescList2.Add(_e0);
            }}
        }

        public static TestItem2 DeserializeTestItem2(ByteBuf _buf, string[] textList)
        {
            return new TestItem2(_buf, textList);
        }

        /// <summary>
        /// 这是id
        /// </summary>
        public readonly int Id;
        /// <summary>
        /// 名字
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 描述
        /// </summary>
        public readonly string Desc;
        public readonly LocationText LocationText;
        public readonly string Desc3;
        public readonly System.Collections.Generic.List<LocationText> DescList;
        public readonly System.Collections.Generic.List<string> DescList2;

        public const int __ID__ = 803528173;
        public override int GetTypeId() => __ID__;

        public  void ResolveRef(Tables tables)
        {
            
            
            
            LocationText?.ResolveRef(tables);
            
            foreach (var _e in DescList) { _e?.ResolveRef(tables); }
            
        }

        public override string ToString()
        {
            return "{ "
            + "id:" + Id + ","
            + "name:" + Name + ","
            + "desc:" + Desc + ","
            + "locationText:" + LocationText + ","
            + "desc3:" + Desc3 + ","
            + "descList:" + Luban.StringUtil.CollectionToString(DescList) + ","
            + "descList2:" + Luban.StringUtil.CollectionToString(DescList2) + ","
            + "}";
        }
    }

}

