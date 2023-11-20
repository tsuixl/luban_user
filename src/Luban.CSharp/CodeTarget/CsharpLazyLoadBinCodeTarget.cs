using Luban.CodeTarget;
using Luban.CSharp.TemplateExtensions;
using Luban.Defs;
using Scriban;
using Scriban.Runtime;

namespace Luban.CSharp.CodeTarget;

[CodeTarget("cs-lazyload-bin")]
public class CsharpLazyLoadBinCodeTarget : CsharpCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CsharpBinTemplateExtension());
    }
    
    public override void GenerateTables(GenerationContext ctx, List<DefTable> tables, CodeWriter writer)
    {
        var template = GetTemplate("tables");
        var tplCtx = CreateTemplateContext(template);
        List<DefTable> tempDefTableList = new List<DefTable>();
        foreach (var table in tables)
        {
            if (!table.IsOnlyLua)
            {
                tempDefTableList.Add(table);
            }
        }
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager },
            { "__namespace", ctx.Target.TopModule },
            { "__tables", tempDefTableList },
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }
    
}