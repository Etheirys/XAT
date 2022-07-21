using System;
using System.IO;
using System.Threading.Tasks;
using XAT.Services;

namespace XAT.Core.Services;

public class FileService : ServiceBase<FileService>
{
    private static readonly string AppDataRawPath = "%AppData%/XAT/";
    private static readonly string LogsRawPath = Path.Combine(AppDataRawPath, "Logs", "xat-log.txt");
    private static readonly string SettingsRawPath = Path.Combine(AppDataRawPath, "Settings.json");

    private static string? appDataPath;

    public FileService()
    {
        AppDataPath = ParseToFilePath(AppDataRawPath);
        LogsPath = ParseToFilePath(LogsRawPath);
        SettingsPath = ParseToFilePath(SettingsRawPath);
    }

    public string AppDataPath { get; init; }
    public string LogsPath { get; init; }
    public string SettingsPath { get; init; }

    public string ParseToFilePath(string path)
    {
        path = path.Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        return path;
    }
}
