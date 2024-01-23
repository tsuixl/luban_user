using System.Text;
using Luban.Datas;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Location;
using Luban.Lua.DataVisitors;

namespace Luban.Lua;

[DataTarget("sg-lua")]
public class SGLuaDataTarget : DataTargetBase
{
    

    protected override string OutputFileExt => "lua";

    public override void BeforeExport(List<DefTable> tables)
    {
    }

    public override void AfterExport(List<DefTable> tables)
    {
    }
    

    public override OutputFile ExportTables(List<DefTable> tables)
    {
        return null;
    }

    public override OutputFile ExportRecord(DefTable table, Record record)
    {
        return null;
    }

    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        if (!table.IsOnlyLua)
        {
            return null;
        }

        var extension = LocationManager.Ins.GetExtensionData(table);
        List<OutputFile> subFile = new();
        bool buildLocation = LocationManager.Ins.IsNeedBuildLocation && extension.hasText;
        Dictionary<string, int> tableText = extension.locationTextMap;
        List<string> textList = extension.locationTextList;

        List<OutputFile> files = new();
        List<SGLuaDataVisitorContext> contexts = new();
        
        if (buildLocation)
        {
            foreach (var language in LocationManager.Ins.ExportLanguages)
            {
                SGLuaDataVisitorContext context = new SGLuaDataVisitorContext();
                context.buildLocation = buildLocation;
                context.locationTextMap = tableText;
                context.language = language;
                contexts.Add(context);
            }
        }
        else
        {
            SGLuaDataVisitorContext context = new SGLuaDataVisitorContext();
            context.buildLocation = buildLocation;
            context.locationTextMap = tableText;
            contexts.Add(context);
        }

        foreach (var context in contexts)
        {
            var ss = new StringBuilder();
            if (table.IsMapTable)
            {
                ExportTableMap(table, records, ss, context);
            }
            else if (table.IsSingletonTable)
            {
                ExportTableSingleton(table, records[0], ss, context);
            }
            else
            {
                ExportTableList(table, records, ss, context);
            }

            string lanExt = "";
            if (buildLocation && context.language != LocationManager.Ins.DefaultLanguage)
            {
                lanExt = "_" + context.language;
            }
            var file = new OutputFile()
            {
                File = $"{table.OutputDataFile}{lanExt}.{OutputFileExt}",
                Content = ss.ToString(),
            };
            files.Add(file);
        }

        var ret = new OutputFile()
        {
            File = files[0].File,
            Content = files[0].Content,
            OtherFiles = files.Count>1? files.Skip(1).ToList():null,
        };
        return ret;
    }
    
    public void ExportTableSingleton(DefTable t, Record record, StringBuilder result, SGLuaDataVisitorContext context)
    {
        result.Append("return ").AppendLine();
        result.Append(record.Data.Apply(SGLuaDataVisitor.Ins, record.Data.TType, context));
    }

    public void ExportTableMap(DefTable t, List<Record> records, StringBuilder s, SGLuaDataVisitorContext context)
    {
        s.Append("return").AppendLine();
        s.Append('{').AppendLine();
        foreach (Record r in records)
        {
            DBean d = r.Data;
            string keyStr = d.GetField(t.Index).Apply(SGLuaDataVisitor.Ins, t.KeyTType, context);
            if (!keyStr.StartsWith("[", StringComparison.Ordinal))
            {
                s.Append($"[{keyStr}] = ");
            }
            else
            {
                s.Append($"[ {keyStr} ] = ");
            }
            s.Append(d.Apply(SGLuaDataVisitor.Ins, d.TType, context));
            s.Append(',').AppendLine();
        }
        s.Append('}');
    }

    public void ExportTableList(DefTable t, List<Record> records, StringBuilder s, SGLuaDataVisitorContext context)
    {
        s.Append("return").AppendLine();
        s.Append('{').AppendLine();
        foreach (Record r in records)
        {
            DBean d = r.Data;
            s.Append(d.Apply(SGLuaDataVisitor.Ins, d.TType, context));
            s.Append(',').AppendLine();
        }
        s.Append('}');
    }
}
