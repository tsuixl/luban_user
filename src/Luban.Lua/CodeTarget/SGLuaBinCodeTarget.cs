using Luban.CodeTarget;
using Luban.Location;
using Luban.Lua.TemplateExtensions;
using Scriban;
using Scriban.Runtime;

namespace Luban.Lua.CodeTarget;

[CodeTarget("sg-lua-bin")]
public class SGLuaBinCodeTarget : LuaCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new LuaBinTemplateExtension());

    }
}
