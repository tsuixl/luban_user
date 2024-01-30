using Luban.CodeTarget;
using Luban.CSharp.TemplateExtensions;
using Luban.Location;
using Scriban;
using Scriban.Runtime;

namespace Luban.CSharp.CodeTarget;

[CodeTarget("sg-cs-bin")]
public class SGCsharpBinCodeTarget : CsharpCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new SGCsharpBinTemplateExtension());
        var tableExtension = LocationManager.Ins.CreateExportScriptObject();
        ctx.PushGlobal(tableExtension);
    }
}
