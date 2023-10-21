using Luban.Utils;

namespace Luban.OutputSaver;

[OutputSaver("local")]
public class LocalFileSaver : OutputSaverBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    protected override void BeforeSave(OutputFileManifest outputFileManifest, string outputDir)
    {
        if (!EnvManager.Current.GetBoolOptionOrDefault($"{BuiltinOptionNames.OutputSaver}.{outputFileManifest.TargetName}", BuiltinOptionNames.CleanUpOutputDir,
                true, true))
        {
            return;
        }
        FileCleaner.Clean(outputDir, outputFileManifest.DataFiles.Select(f => f.File).ToList());
        // TSUIL
        // 解决windows上SaveFile()会存盘失败的问题
        // SaveFile是多线程执行，有概率会出现文件被占用的情况，已经被删除的文件，但是文件句柄还在（File.Exists() == true），导致SaveFile失败
        Thread.Sleep(1000);
    }

    public override void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile)
    {
        var id = Thread.CurrentThread.ManagedThreadId;
        string fullOutputPath = $"{outputDir}/{outputFile.File}";
        Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));
        if (FileUtil.WriteAllBytes(fullOutputPath, outputFile.GetContentBytes()))
        {
            s_logger.Info("save file:{} {}", fullOutputPath, id);
        }
    }
}