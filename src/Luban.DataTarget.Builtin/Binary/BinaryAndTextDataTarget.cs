using Luban.DataTarget;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.L10N;
using Luban.Serialization;

namespace Luban.DataExporter.Builtin.Binary;

[DataTarget("bin")]
public class BinaryAndTextDataTarget : DataTargetBase
{
    protected override string OutputFileExt => "bytes";

    private void WriteList(DefTable table, List<Record> datas, ByteBuf x)
    {
        if (table.IsOnlyLua)
        {
            return;
        }
        x.WriteSize(datas.Count);
        foreach (var d in datas)
        {
            d.Data.Apply(BinaryDataVisitor.Ins, x);
        }
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        if (table.IsOnlyLua)
        {
            return null;
        }
        
        var textCollection = new TextKeyCollection();

        var visitor = new DataActionHelpVisitor2<TextKeyCollection>(TextKeyListCollectorVisitor.Ins);

        TableVisitor.Ins.Visit(table, visitor, textCollection);

        var keys = textCollection.Keys.ToList();
        keys.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));
        
        var bytes = new ByteBuf();
        WriteList(table, records, bytes);
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = bytes.CopyData(),
        };
    }
}
