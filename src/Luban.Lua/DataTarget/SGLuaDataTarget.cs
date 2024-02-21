using System.Reflection;
using System.Text;
using Luban.CodeTarget;
using Luban.Datas;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Location;
using Luban.Lua.DataVisitors;
using Luban.Lua.TemplateExtensions;
using Luban.TemplateExtensions;
using Luban.Tmpl;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;

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
            // if (table.IsMapTable)
            // {
            //     ExportTableMap(table, records, ss, context);
            // }
            // else if (table.IsSingletonTable)
            // {
            //     ExportTableSingleton(table, records[0], ss, context);
            // }
            // else
            // {
            //     ExportTableList(table, records, ss, context);
            // }
            
            GenerateTable(table, records, ss, context);

            string lanExt = "";
            if (buildLocation && context.language != LocationManager.Ins.ExportDefaultLanguage)
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

        s.Append(",\n");
        
        s.Append('{').AppendLine();
        foreach (Record r in records)
        {
            DBean d = r.Data;
            string keyStr = d.GetField(t.Index).Apply(SGLuaDataVisitor.Ins, t.KeyTType, context);
            s.Append($"{keyStr} ");
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
    
    public string TargetName => GetType().GetCustomAttribute<DataTargetAttribute>().Name;
    
    private  void GenerateTable(DefTable table, List<Record> records, StringBuilder s, SGLuaDataVisitorContext context)
    {
        GenerationContext ctx = GenerationContext.Current;
        var template = GetTemplate("table_data");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__top_module", ctx.Target.TopModule },
            { "__manager_name", ctx.Target.Manager },
            { "__manager_name_with_top_module", TypeUtil.MakeFullName(ctx.TopModule, ctx.Target.Manager) },
            { "__name", table.Name },
            { "__namespace", table.Namespace },
            { "__namespace_with_top_module", table.NamespaceWithTopModule },
            { "__full_name_with_top_module", table.FullNameWithTopModule },
            { "__table", table },
            { "__this", table },
            { "__key_type", table.KeyTType},
            { "__value_type", table.ValueTType},
            // { "__code_style", CodeStyle},
            
            { "__records", records },
            { "__context", context },
        };
        tplCtx.PushGlobal(extraEnvs);
        var result = template.Render(tplCtx);
        s.Append(result);
    }
    
    protected TemplateContext CreateTemplateContext(Template template)
    {
        var ctx = new TemplateContext()
        {
            LoopLimit = 0,
            NewLine = "\n",
        };
        ctx.PushGlobal(new ContextTemplateExtension());
        ctx.PushGlobal(new TypeTemplateExtension());
        var tableExtension = LocationManager.Ins.CreateExportScriptObject();
        ctx.PushGlobal(tableExtension);
        ctx.PushGlobal(new LuaCommonTemplateExtension());
        ctx.PushGlobal(new LuaBinTemplateExtension());
        ctx.PushGlobal(new SGLuaDataTemplateExtension());
        return ctx;
    }
    
    private Scriban.Template GetTemplate(string name)
    {
        if (TemplateManager.Ins.TryGetTemplate($"{TargetName}/{name}", out var template))
        {
            return template;
        }
        throw new Exception($"template:{name} not found");
    }
    
}
