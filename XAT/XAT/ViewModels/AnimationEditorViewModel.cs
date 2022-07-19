using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using XAT.Common.FFXIV.Files;
using XAT.Common.Havok;
using XAT.Logic;
using XAT.Utils;
using XAT.Utils.Dialog;
using Serilog;
using System.IO;
using System.Collections.ObjectModel;
using XAT.Views;
using System.Linq;

namespace XAT.ViewModels;

[AddINotifyPropertyChangedInterface]
class AnimationEditorViewModel
{
    public string FileTypeFilters => AnimationFileTypeExtensions.GetFileFormatFilters();
    public AnimationFileType? SelectedExportType { get; set; }
    public string ExportPath { get; set; } = string.Empty;

    public AnimationFileType? SelectedImportType { get; set; }
    public string ImportPath { get; set; } = string.Empty;
    public ObservableCollection<string> ImportTracks { get; private set; } = new();
    public string? SelectedImportTrack { get; set; }

    [DependsOn(nameof(SelectedImportTrack))]
    public int? SelectedImportTrackIndex
    {
        get
        {
            if (SelectedImportTrack == null)
                return null;

            var index = this.ImportTracks.IndexOf(this.SelectedImportTrack);

            return index == -1 ? null : index;
        }
    }

    public string SklbPath { get; set; } = string.Empty;
    public string PapPath { get; set; } = string.Empty;

    public Pap? LoadedPap { get; set; }
    public Sklb? LoadedSklb { get; set; }
    public PapAnimInfo? SelectedAnimation { get; set; }

    public string OutputPath { get; set; } = string.Empty;

    public bool HavokBundleSkeleton { get; set; } = true;

    public ObservableCollection<string> ExcludedBones { get; private set; } = new();

    public CompressionType[] CompressionTypes => (CompressionType[])Enum.GetValues(typeof(CompressionType));
    public CompressionType? SelectedCompressionType { get; set; }
    public CompressionTolerance CompressionTolerances { get; } = new();


    [DependsOn(nameof(LoadedPap), nameof(LoadedSklb))]
    public bool IsLoaded => this.LoadedPap != null && this.LoadedSklb != null;

    public ICommand LoadSource => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {

            if (string.IsNullOrEmpty(this.PapPath))
            {
                await DialogUtils.ShowErrorPopup("Must specify a pap path.");
                return;
            }

            if (string.IsNullOrEmpty(this.SklbPath))
            {
                await DialogUtils.ShowErrorPopup("Must specify a sklb path.");
                return;
            }


            Log.Information($"Attempting to import pap '{this.PapPath}' and sklb '{this.SklbPath}'...");

            try
            {
                this.LoadedPap = Pap.FromFile(this.PapPath);
                this.LoadedSklb = Sklb.FromFile(this.SklbPath);
            }
            catch (Exception e)
            {
                Log.Error($"Error importing: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error importing: {e.Message}");
                return;
            }

            DialogUtils.ShowSnackbar("Successfully loaded source pap and sklb.");
            Log.Information("Successfully loaded source pap and sklb.");
        }
    });

    public ICommand ExportAnimation => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {
            Log.Information("Exporting animation...");

            if (this.SelectedExportType == null || this.LoadedPap == null || this.SelectedAnimation == null || this.LoadedSklb == null)
                throw new Exception("Unexpected null value"); // Should never happen

            try
            {
                switch (this.SelectedExportType)
                {
                    case AnimationFileType.FBX:
                        {
                            Log.Information("Exporting FBX...");
                            var result = await AnimationInterop.ExportFBX(this.LoadedPap, this.SelectedAnimation, this.LoadedSklb, this.ExportPath);
                            string resultText = $"Generated FBX from {this.SelectedAnimation.Name} with {result.framesConverted} frames across {result.bonesConverted} bones.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;

                    case AnimationFileType.HavokTagFile:
                    case AnimationFileType.HavokPackFile:
                    case AnimationFileType.HavokXMLFile:
                        {
                            Log.Information("Exporting Havok file...");
                            await AnimationInterop.ExportHavok((AnimationFileType)this.SelectedExportType, this.LoadedPap, this.SelectedAnimation, this.LoadedSklb, this.HavokBundleSkeleton, this.ExportPath);
                            string resultText = $"Exported havok file from {this.SelectedAnimation.Name}.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error exporting: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error exporting: {e.Message}");
            }


        }
    });
    public ICommand ImportAnimation => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {
            Log.Information("Importing animation...");

            if (this.SelectedImportType == null || this.SelectedImportTrackIndex == null || this.LoadedPap == null || this.SelectedAnimation == null || this.LoadedSklb == null)
                throw new Exception("Unexpected null value"); // Should never happen

            try
            {

                switch (this.SelectedImportType)
                {
                    case AnimationFileType.FBX:
                        {
                            Log.Information("Importing FBX...");
                            var result = await AnimationInterop.ImportFBX(this.LoadedPap, this.SelectedAnimation, this.LoadedSklb, this.ImportPath, (int)this.SelectedImportTrackIndex, new(this.ExcludedBones));
                            string resultText = $"Imported {result.framesConverted} frames across {result.bonesBound} bones from FBX to {this.SelectedAnimation.Name}.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;

                    case AnimationFileType.HavokTagFile:
                    case AnimationFileType.HavokPackFile:
                    case AnimationFileType.HavokXMLFile:
                        {
                            Log.Information("Importing Havok animation...");
                            await AnimationInterop.ImportHavok(this.LoadedPap, this.SelectedAnimation, this.LoadedSklb, this.ImportPath, (int)this.SelectedImportTrackIndex);
                            string resultText = $"Imported havok file.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error exporting: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error exporting: {e.Message}");
            }
        }
    });

    public ICommand SaveOutput => new Command(async (_) =>
    {
        if (this.LoadedPap == null || this.OutputPath == null)
            throw new Exception("Unexpected null value"); // Should never happen

        using (new ProgressWrapper())
        {
            Log.Information("Saving pap...");
            await File.WriteAllBytesAsync(this.OutputPath, this.LoadedPap.ToBytes());
            string resultText = $"Pap saved.";
            Log.Information(resultText);
            DialogUtils.ShowSnackbar(resultText);
        }
    });

    public ICommand ShowExcludedBonesPopup => new Command(async (_) =>
    {
        if (this.LoadedSklb == null)
            throw new Exception("Unexpected null value"); // Should never happen

        List<string> boneList = new();

        using (new ProgressWrapper())
        {
            string tmpFile = Path.GetTempFileName();
            await File.WriteAllBytesAsync(tmpFile, this.LoadedSklb.HavokData);

            boneList = await RawHavokInterop.ListBones(tmpFile, 0);

            File.Delete(tmpFile);
        }

        var popup = new BoneExclusionPopup()
        {
            AllBones = new(boneList),
            ExcludedBones = this.ExcludedBones
        };

        await DialogUtils.ShowRaw(popup);
    });

    public ICommand Compress => new Command(async (_) =>
    {
        if (this.LoadedSklb == null || this.LoadedPap == null || this.SelectedAnimation == null || this.SelectedCompressionType == null)
            throw new Exception("Unexpected null value"); // Should never happen

        using (new ProgressWrapper())
        {
            try
            {
                int havokSizeBefore = this.LoadedPap.HavokData.Length;

                switch (this.SelectedCompressionType)
                {
                    case CompressionType.Predictive:
                        {
                            await AnimationInterop.PredictiveCompress(this.LoadedPap, this.SelectedAnimation, this.LoadedSklb, 0, this.CompressionTolerances.StaticTranslationTolerance, this.CompressionTolerances.StaticRotationTolerance, this.CompressionTolerances.StaticScaleTolerance, 0, this.CompressionTolerances.DynamicTranslationTolerance, this.CompressionTolerances.DynamicRotationTolerance, this.CompressionTolerances.DynamicScaleTolerance);
                        }
                        break;

                    case CompressionType.Quantized:
                        {
                            await AnimationInterop.QuantizedCompress(this.LoadedPap, this.SelectedAnimation, this.LoadedSklb, 0, this.CompressionTolerances.StaticTranslationTolerance, this.CompressionTolerances.StaticRotationTolerance, this.CompressionTolerances.StaticScaleTolerance);
                        }
                        break;
                }

                int havokSizeNow = this.LoadedPap.HavokData.Length;
                DialogUtils.ShowSnackbar($"Saved {havokSizeBefore - havokSizeNow} bytes during compression.");

            }
            catch (Exception e)
            {
                await DialogUtils.ShowErrorPopup(e.Message);
            }
        }
    });


    public void OnSklbPathChanged()
    {
        this.LoadedSklb = null;
    }

    public void OnPapPathChanged()
    {
        this.LoadedPap = null;
    }

    public void OnExportPathChanged()
    {
        this.SelectedExportType = AnimationFileTypeExtensions.GetTypeFromExtension(Path.GetExtension(this.ExportPath));
    }

    public async void OnImportPathChanged()
    {
        this.SelectedImportType = AnimationFileTypeExtensions.GetTypeFromExtension(Path.GetExtension(this.ImportPath));

        this.ExcludedBones.Clear();

        if (this.SelectedImportType == null && this.ImportPath != null)
        {
            this.ImportTracks = new();
            return;
        }

        Log.Information("Identifying animation tracks...");

        using (new ProgressWrapper())
        {
            switch (this.SelectedImportType)
            {
                case AnimationFileType.FBX:
                    {
                        this.ImportTracks = new(await RawHavokInterop.ListFbxStacks(this.ImportPath!));
                    }
                    break;

                case AnimationFileType.HavokTagFile:
                case AnimationFileType.HavokPackFile:
                case AnimationFileType.HavokXMLFile:
                    {
                        var stats = await RawHavokInterop.GetStats(this.ImportPath!);
                        this.ImportTracks = new();
                        for (int i = 0; i < stats.animCount; ++i)
                            this.ImportTracks.Add(i.ToString());

                    }
                    break;
            }

            var resultText = $"Found {this.ImportTracks.Count} animation track(s).";
            Log.Information(resultText);
            DialogUtils.ShowSnackbar(resultText);
        }
    }

    public class CompressionTolerance
    {
        public float StaticTranslationTolerance { get; set; } = 0.0f;
        public float StaticRotationTolerance { get; set; } = 0.0f;
        public float StaticScaleTolerance { get; set; } = 0.0f;
        public float DynamicTranslationTolerance { get; set; } = 0.0f;
        public float DynamicRotationTolerance { get; set; } = 0.0f;
        public float DynamicScaleTolerance { get; set; } = 0.0f;

    }

    public enum CompressionType
    {
        Quantized,
        Predictive
    }
}