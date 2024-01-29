using System.Text;
using System.Text.Json;
using Luban.RawDefs;
using Luban.Schema;
using Luban.Utils;

namespace Luban;

public class GlobalConfigLoader : IConfigLoader
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private string _curDir;

    public GlobalConfigLoader()
    {

    }


    private class Group
    {
        public List<string> Names { get; set; }

        public bool Default { get; set; }
    }

    private class SchemaFile
    {
        public string FileName { get; set; }

        public string Type { get; set; }
    }

    private class Target
    {
        public string Name { get; set; }

        public string Manager { get; set; }

        public List<string> Groups { get; set; }

        public string TopModule { get; set; }
    }

    private class LubanConf
    {
        public List<Group> Groups { get; set; }

        public List<SchemaFile> SchemaFiles { get; set; }

        public string DataDir { get; set; }

        public List<Target> Targets { get; set; }
    }

    public LubanConfig Load(string fileName)
    {
        s_logger.Debug("load config file:{}", fileName);
        _curDir = Directory.GetParent(fileName).FullName;
        var excelPath = EnvManager.Current.GetOptionRaw(BuiltinOptionNames.ExcelDataDir);
        if (string.IsNullOrEmpty(excelPath) == false)
        {
            // _curDir = excelPath;
        }
        s_logger.Debug($" data path :{_curDir} {excelPath}");

        Dictionary<string, string> replaceMap = new();
        replaceMap["confFileDir"] = _curDir;
        foreach (var kp in EnvManager.Current.GetAllOptions())
        {
            replaceMap[kp.Key] = kp.Value;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        var globalConf = JsonSerializer.Deserialize<LubanConf>(File.ReadAllText(fileName, Encoding.UTF8), options);

        List<RawGroup> groups = globalConf.Groups.Select(g => new RawGroup() { Names = g.Names, IsDefault = g.Default }).ToList();
        List<RawTarget> targets = globalConf.Targets.Select(t => new RawTarget() { Name = t.Name, Manager = t.Manager, Groups = t.Groups, TopModule = t.TopModule }).ToList();

        List<SchemaFileInfo> importFiles = new();
        foreach (var schemaFile in globalConf.SchemaFiles)
        {
            string fileOrDirectory = ParsePath(schemaFile.FileName, replaceMap);
            foreach (var subFile in FileUtil.GetFileOrDirectory(fileOrDirectory))
            {
                importFiles.Add(new SchemaFileInfo() { FileName = subFile, Type = schemaFile.Type });
            }
        }

        var dataPath = ParsePath(globalConf.DataDir, replaceMap);
        return new LubanConfig()
        {
            InputDataDir = dataPath,
            Groups = groups,
            Targets = targets,
            Imports = importFiles,
        };
    }

    private string ParsePath(string path, Dictionary<string, string> map)
    {
        var ret = path;
        if (path.Contains("%") == false)
        {
            ret = "%confFileDir%/" + ret;
        }

        ret = FileUtil.ParsePath(ret, map);
        return ret;
    }

    private string GetExistFile(params string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            var path = paths[i];
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            if (File.Exists(paths[i]))
            {
                return paths[i];
            }
        }
        s_logger.Error($"file not exists {string.Join(',', paths)}");
        return null;
    }

}
