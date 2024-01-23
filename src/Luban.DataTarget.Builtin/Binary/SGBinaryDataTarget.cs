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



    public override void BeforeExport(List<DefTable> tables)
    {
        
    }

    public override void AfterExport(List<DefTable> tables)
    { 
        bool buildLocation = LocationManager.Ins.IsNeedBuildLocation;
        if (buildLocation)
        {
            LocationManager.Ins.WriteLocationFile();
        }

        var outputAllType = EnvManager.Current.GetOptionRaw("outputAllType");
        if (string.IsNullOrEmpty(outputAllType) == false)
        {
            var alltypStr = TypeToStringVisitor.Ins.SerializeAllType();
            File.WriteAllText(outputAllType, alltypStr);
        }
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        if (table.IsOnlyLua)
        {
            return null;
        }

        s_logger.Debug($"[Debug] ExportTable => {table.Name} {table.IsLazy} {table.IsOnlyLua}");

        var extension = LocationManager.Ins.GetExtensionData(table);
        List<OutputFile> subFile = new();
        bool buildLocation = LocationManager.Ins.IsNeedBuildLocation;
        Dictionary<string, int> tableText = extension.locationTextMap;
        List<string> textList = extension.locationTextList;

        var isLazy = table.IsLazy;
        var offsetBuf = isLazy ? new ByteBuf() : null;
        
        ByteBuf dataBuf = new ByteBuf(10 * 1024);
        if (offsetBuf != null)
        {
            offsetBuf.WriteSize(records.Count);
        }

        if (buildLocation)
        {
            foreach (var language in LocationManager.Ins.ExportLanguages)
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
                    File = $"location/{table.OutputDataFile}_{language}.{OutputFileExt}", 
                    Content = textBuf.CopyData(), OtherFiles = null,
                });
            }
            

        }


        SGBinaryDataVisitorContext visitorContext = new SGBinaryDataVisitorContext()
        {
            byteBuf = dataBuf, 
            locationTextMap = tableText,
            buildLocation = buildLocation,
        };
        dataBuf.WriteSize(records.Count);
        int lastOffset = 0;
        foreach (var d in records)
        {
            if (isLazy)
            {
                foreach (var indexInfo in table.IndexList)
                {
                    DType keyData = d.Data.Fields[indexInfo.IndexFieldIdIndex];
                    keyData.Apply(BinaryDataVisitor.Ins, offsetBuf);
                }
            }

            int offset = dataBuf.Size;
            if (isLazy)
            {
                offsetBuf.WriteSize(offset);
            }

            d.Data.Apply(SGBinaryDataVisitor.Ins, table.ValueTType, visitorContext);
            int length = dataBuf.Size - lastOffset;
            if (isLazy)
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

}
