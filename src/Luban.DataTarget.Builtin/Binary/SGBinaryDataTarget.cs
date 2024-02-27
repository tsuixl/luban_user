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
        var textList = extension.locationTextList;

        var isLazy = table.IsLazy;
        var isLazyAndText = table.IsLazy && extension.hasText;
        var offsetBuf = isLazy ? new ByteBuf() : null;
        var textIndexBuf = buildLocation ? new ByteBuf() : null;
        
        ByteBuf dataBuf = new ByteBuf(10 * 1024);
        if (offsetBuf != null)
        {
            offsetBuf.WriteSize(records.Count);
        }

        SGBinaryDataVisitorContext visitorContext = new SGBinaryDataVisitorContext()
        {
            byteBuf = dataBuf, 
            locationTextMap = tableText,
            textIndexBuf =  textIndexBuf,
            buildLocation = buildLocation,
        };
        visitorContext.table = table;
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

            int textIndexCount = isLazyAndText ? visitorContext.textIndexList.Count : 0;
            visitorContext.record = d;
            d.Data.Apply(SGBinaryDataVisitor.Ins, table.ValueTType, visitorContext);
            int length = dataBuf.Size - lastOffset;
            if (isLazy)
            {
                offsetBuf.WriteSize(length);
                if (isLazyAndText)
                {
                    offsetBuf.WriteSize(textIndexCount);
                }
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
        
        #region textList
 
        if (buildLocation)
        {
            if (extension.hasText)
            {
                foreach (var language in LocationManager.Ins.ExportLanguages)
                {
                    var textBuf = buildLocation ? new ByteBuf() : null;
                    textBuf.WriteSize(textList.Count);
                    foreach (var text in textList)
                    {
                        var textT = LocationManager.Ins.GetContentValueByFullKey(text.fullKey, language);
                        textBuf.WriteString(textT);
                    }

                    subFile.Add(new OutputFile()
                    {
                        File = $"location/{table.OutputDataFile}_{language}.{OutputFileExt}", 
                        Content = textBuf.CopyData(), OtherFiles = null,
                    });
                }
                
                subFile.Add(new OutputFile()
                {
                    File = $"location/{table.OutputDataFile}_text_index.{OutputFileExt}", 
                    Content = visitorContext.textIndexBuf.CopyData(), OtherFiles = null,
                });
            }

        }
        #endregion


        var file = new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = dataBuf.CopyData(),
            OtherFiles = subFile.Count>0 ? subFile:null,
        };

        return file;
    }

}
