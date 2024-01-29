using Luban.CodeTarget;
using Luban.Location;
using Luban.Lua.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.Lua.CodeTarget;

public abstract class LuaCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_LUA;

    protected override string FileSuffixName => "lua";

    protected override string DefaultOutputFileName => "schema.lua";

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        var tableExtension = new ScriptObject
        {
            {"__tables_extension", LocationManager.Ins.ExtensionDataMap },
            {"__location_build", LocationManager.Ins.IsNeedBuildLocation },
            {"__location_config_file_language", LocationManager.Ins.ConfigFileLanguage },
            {"__location_export_default_language", LocationManager.Ins.ExportDefaultLanguage },
        };
        ctx.PushGlobal(tableExtension);
        ctx.PushGlobal(new LuaCommonTemplateExtension());
    }
}
