using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using XAT.Services;

namespace XAT.Core.Services;

public class SettingsService : ServiceBase<SettingsService>
{
    public Settings Settings { get; private set; } = new Settings();
    public override async Task Initialize()
    {
        if (File.Exists(FileService.Instance.SettingsPath))
        {
            await Load();
        }
        else
        {
            await Save();
        }

        this.Settings.PropertyChanged += this.OnSettingsChanged;

        await base.Initialize();
    }

    public async Task Load()
    {
        string json = await File.ReadAllTextAsync(FileService.Instance.SettingsPath);
        this.Settings = SerializerService.Deserialize<Settings>(json);
    }

    public async Task Save()
    {
        string json = SerializerService.Serialize(Settings);
        await File.WriteAllTextAsync(FileService.Instance.SettingsPath, json);
    }

    private async void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (this.Settings == null)
            return;

        if (sender is Settings)
            await Save();
    }
}
