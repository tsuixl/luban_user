using Luban.Datas;
using Luban.DataTarget;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.L10N;
using Luban.Location;
using Luban.Serialization;
using YamlDotNet.Core.Tokens;

namespace Luban.DataExporter.Builtin.Binary;

[DataTarget("sg-bin")]
public class SGBinaryDataTarget : DataTargetBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    protected override string OutputFileExt => "bytes";

    private class TableExtensionData
    {
        public DefTable table;
        public Dictionary<string, int> localtionTextMap;
        public List<string> localtionTextList;
    }

    private Dictionary<DefTable, TableExtensionData> m_ExtensionDataMap = new();

    public override void BeforeExport(List<DefTable> tables)
    {
        bool buildLocation = LocationManager.Ins.IsNeedBuildLocation;
       m_ExtensionDataMap.Clear();
       foreach (var table in tables)
       {
           var records = GenerationContext.Current.GetTableExportDataList(table);
           List<string> textList = null;
           Dictionary<string, int> textMap = buildLocation? GetTableAllText(table, records, null, out textList) : null;
           var extensionData = new TableExtensionData() { table = table, localtionTextMap = textMap, localtionTextList = textList};
           m_ExtensionDataMap.Add(table, extensionData);
           if (extensionData.localtionTextMap != null)
           {
               foreach (var text in extensionData.localtionTextMap.Keys)
               {
                   LocationManager.Ins.AddText(text);
               }
           }
       }
    }

    public override void AfterExport(List<DefTable> tables)
    { 
        bool buildLocation = LocationManager.Ins.IsNeedBuildLocation;
        if (buildLocation)
        {
            LocationManager.Ins.WriteLocationFile();
        }
        
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        if (table.IsOnlyLua)
        {
            return null;
        }

        s_logger.Debug($"[Debug] ExportTable => {table.Name} {table.IsLazy} {table.IsOnlyLua}");

        var extension = m_ExtensionDataMap[table];
        List<OutputFile> subFile = new();
        bool buildLocation = LocationManager.Ins.IsNeedBuildLocation;
        Dictionary<string, int> tableText = extension.localtionTextMap;
        List<string> textList = extension.localtionTextList;

        var isLazy = table.IsLazy;
        var offsetBuf = isLazy ? new ByteBuf() : null;
        
        ByteBuf dataBuf = new ByteBuf(10 * 1024);
        if (offsetBuf != null)
        {
            offsetBuf.WriteSize(records.Count);
        }

        if (buildLocation)
        {
            foreach (var language in LocationManager.Ins.AllLanguages)
            {
                var textBuf = buildLocation ? new ByteBuf() : null;
                textBuf.WriteSize(textList.Count);
                foreach (var text in textList)
                {
                    var textT = LocationManager.Ins.GetContentValue(text, language);
                    textBuf.WriteString(textT);
                }
                subFile.Add(new OutputFile()
                {
                    File = $"localtion/{table.OutputDataFile}_{language}.{OutputFileExt}", 
                    Content = textBuf.CopyData(), OtherFiles = null,
                });
            }
            

        }


        SGBinaryDataVisitorContext visitorContext = new SGBinaryDataVisitorContext()
        {
            byteBuf = dataBuf, 
            localtionTextMap = tableText,
            buildLocation = buildLocation,
        };
        dataBuf.WriteSize(records.Count);
        int lastOffset = 0;
        foreach (var d in records)
        {
            foreach (var indexInfo in table.IndexList)
            {
                DType keyData = d.Data.Fields[indexInfo.IndexFieldIdIndex];
                keyData.Apply(BinaryDataVisitor.Ins, dataBuf);
            }
            int offset = dataBuf.Size;
            if (offsetBuf != null)
            {
                offsetBuf.WriteSize(offset);
            }

            d.Data.Apply(SGBinaryDataVisitor.Ins, table.ValueTType, visitorContext);
            int length = dataBuf.Size - lastOffset;
            if (offsetBuf != null)
            {
                offsetBuf.WriteSize(length);
            }

            lastOffset = dataBuf.Size;
        }

       
        if (isLazy)
        {
            subFile.Add(new OutputFile()
            {
                File = $"offset/{table.OutputDataFile}.{OutputFileExt}",
                Content = offsetBuf.CopyData(),
                OtherFiles = null,
            });
        }


        var file = new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = dataBuf.CopyData(),
            OtherFiles = subFile.Count>0 ? subFile:null,
        };

        return file;
    }

    private Dictionary<string, int> GetTableAllText(DefTable table, List<Record> records, List<string> oldData, out List<string> textList)
    {
        var textCollection = new TextKeyCollection();

        var visitor = new DataActionHelpVisitor2<TextKeyCollection>(TextKeyListCollectorVisitor.Ins);

        TableVisitor.Ins.Visit(table, visitor, textCollection);

        var keys = textCollection.Keys.ToList();
        keys.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

        var datas = new List<string>();
        textList = datas;
        datas.Add("");
        if (oldData != null)
        {
            foreach (var key in oldData)
            {
                if (datas.Contains(key) == false)
                {
                    datas.Add(key);
                }
            }
        }

        foreach (var key in keys)
        {
            if (datas.Contains(key) == false)
            {
                datas.Add(key);
            }
        }

        var map = new Dictionary<string, int>();
        for(int i=0; i<datas.Count; i++)
        {
            map.Add(datas[i], i);
        }

        // string outputFile = EnvManager.Current.GetOption(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextKeyListFile, false);

        return map;
    }
}
