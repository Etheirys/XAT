using AutoUpdaterDotNET;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using XAT.Services;

namespace XAT.Core.Services;

public class VersionService : ServiceBase<VersionService>
{
    public override Task Initialize()
    {
        Log.Information($"XAT Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version}");
        Log.Information($"Dotnet Runtime: {Environment.Version}");

        return base.Initialize();
    }

    public override Task Start()
    {
        if (!Debugger.IsAttached)
        {
            AutoUpdater.ParseUpdateInfoEvent += AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.HttpUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) XAT Client";
            AutoUpdater.Start("https://api.github.com/repos/Etheirys/XAT/releases/latest");
        }
        
        return base.Start();
    }

    private void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
    {
        var release = JsonSerializer.Deserialize<Release>(args.RemoteData);

        if (release == null || release.Assets == null)
            return;

        Release.Asset? asset = null;
        foreach (Release.Asset tAsset in release.Assets)
        {
            if (tAsset.Name == null)
                continue;

            if (!tAsset.Name.EndsWith(".zip", StringComparison.InvariantCulture))
                continue;

            asset = tAsset;
        }

        if (asset == null)
            return;

        if (asset.Url == null)
            return;

        args.UpdateInfo = new UpdateInfoEventArgs
        {
            CurrentVersion = release.Name,
            DownloadURL = asset.Url,
        };
    }

    public class Release
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        [JsonPropertyName("html_url")]
        public string? ChangesURL { get; set; }

        [JsonPropertyName("published_at")]
        public DateTimeOffset? Published { get; set; }

        [JsonPropertyName("body")]
        public string? Changes { get; set; }

        [JsonPropertyName("assets")]
        public List<Asset>? Assets { get; set; }

        public class Asset
        {
            [JsonPropertyName("browser_download_url")]
            public string? Url { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }
    }
}
