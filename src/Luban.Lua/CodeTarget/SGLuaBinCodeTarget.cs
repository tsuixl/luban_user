using Luban.CodeTarget;
using Luban.Location;
using Luban.Lua.TemplateExtensions;
using Scriban;
using Scriban.Runtime;

namespace Luban.Lua.CodeTarget;

[CodeTarget("sg-lua-bin")]
public class SGLuaBinCodeTarget : LuaCodeTargetBase
{
    
    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        base.Handle(ctx, manifest);
        // var tasks = new List<Task<OutputFile>>();
        //
        // foreach (var table in ctx.ExportTables)
        // {
        //     tasks.Add(Task.Run(() =>
        //     {
        //         var writer = new CodeWriter();
        //         GenerateTable(ctx, table, writer);
        //         return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(table.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
        //     }));
        // }
        //
        // Task.WaitAll(tasks.ToArray());
        // foreach (var task in tasks)
        // {
        //     manifest.AddFile(task.Result);
        // }
    }
    
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new LuaBinTemplateExtension());

    }
    
    
}
