using Shake.Core.Utils.ConfigModels;
using System.Text.Json;

namespace Shake.Core.Utils;

public static class ConfigUtils
{
    public static string GetConfigLocation()
    {
        return Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile), ".shake\\settings.json");
    }

    public static ShakeOptions GetConfig()
    {
        var location = GetConfigLocation();
        Directory.CreateDirectory(Path.GetDirectoryName(location));
        using var stream = File.Open(location, FileMode.OpenOrCreate, FileAccess.Read);

        if (stream.Length == 0)
        {
            return new ShakeOptions();
        }

        return JsonSerializer.Deserialize<ShakeOptions>(stream) ?? new ShakeOptions();

    }

    public static string GetConfigAsString()
    {
        var location = GetConfigLocation();
        Directory.CreateDirectory(Path.GetDirectoryName(location));

        return File.ReadAllText(location);
    }

    public static FileSystemWatcher SetupConfigWatch(FileSystemEventHandler onChanged)
    {
        var location = GetConfigLocation();
        var directory = Path.GetDirectoryName(location);
        var filename = Path.GetFileName(location);
        Directory.CreateDirectory(directory);
        var watcher = new FileSystemWatcher(directory, filename);

        watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
        
        watcher.Changed += onChanged;

        watcher.EnableRaisingEvents = true;

        return watcher;
    }

    public static void SaveConfig(ShakeOptions options)
    {
        using var stream = File.Open(GetConfigLocation(), FileMode.Create, FileAccess.ReadWrite);

        JsonSerializer.Serialize(stream, options, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        stream.Flush();
    }

    public static void Save(this ShakeOptions options)
    {
        ConfigUtils.SaveConfig(options);
    }
}