using System.Reflection;

namespace Luban.OutputSaver;

public abstract class OutputSaverBase : IOutputSaver
{
    public virtual string Name => GetType().GetCustomAttribute<OutputSaverAttribute>().Name;

    protected virtual string GetOutputDir(OutputFileManifest manifest)
    {
        string optionName = manifest.OutputType == OutputType.Code
            ? BuiltinOptionNames.OutputCodeDir
            : BuiltinOptionNames.OutputDataDir;
        return EnvManager.Current.GetOption($"{manifest.TargetName}", optionName, true);
    }

    protected virtual void BeforeSave(OutputFileManifest outputFileManifest, string outputDir)
    {

    }

    protected virtual void PostSave(OutputFileManifest outputFileManifest, string outputDir)
    {

    }
    
    public virtual void Save(OutputFileManifest outputFileManifest)
    {
        string outputDir = GetOutputDir(outputFileManifest);
        BeforeSave(outputFileManifest, outputDir);
        var tasks = new List<Task>();
        var tableList = GenerationContext.Current.Tables;
        foreach (var outputFile in outputFileManifest.DataFiles)
        {
            bool IsOnlyLua = false;
            foreach (var table in tableList)
            {
                string[] array = outputFile.File.Split(".");
                if (table.ValueType == array[0] || table.FullName == array[0])
                {
                    if (table.IsOnlyLua)
                    {
                        IsOnlyLua = true;
                    }
                }
            }
            
            if (!IsOnlyLua)
            {
                tasks.Add(Task.Run(() =>
                {
                    SaveFile(outputFileManifest, outputDir, outputFile);
                }));
            }
        }
        Task.WaitAll(tasks.ToArray());
        PostSave(outputFileManifest, outputDir);
    }

    public abstract void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile);
}