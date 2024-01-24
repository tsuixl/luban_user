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
        var tableExtension = new ScriptObject
        {
            {"__tables_extension", LocationManager.Ins.ExtensionDataMap },
            {"__location_build", LocationManager.Ins.IsNeedBuildLocation },
            {"__location_config_file_language", LocationManager.Ins.ConfigFileLanguage },
            {"__location_export_default_language", LocationManager.Ins.ExportDefaultLanguage },
        };
        ctx.PushGlobal(tableExtension);
    }
}
