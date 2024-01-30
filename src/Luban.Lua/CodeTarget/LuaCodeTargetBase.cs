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
        var tableExtension = LocationManager.Ins.CreateExportScriptObject();
        ctx.PushGlobal(tableExtension);
        ctx.PushGlobal(new LuaCommonTemplateExtension());
    }
}
