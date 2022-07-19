using PropertyChanged;
using Serilog;
using System;
using System.IO;
using System.Windows.Input;
using XAT.Common.FFXIV.Files;
using XAT.Logic;
using XAT.Utils;
using XAT.Utils.Dialog;

namespace XAT.ViewModels;

[AddINotifyPropertyChangedInterface]

public class SkeletonEditorViewModel
{
    public string FileTypeFilters => AnimationFileTypeExtensions.GetFileFormatFilters();
    public AnimationFileType? SelectedExportType { get; set; }
    public string ExportPath { get; set; } = string.Empty;

    public AnimationFileType? SelectedImportType { get; set; }
    public string ImportPath { get; set; } = string.Empty;

    public string SklbPath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;

    public bool PreserveImportCompatibility { get; set; } = true;

    public Sklb? LoadedSklb { get; set; }

    [DependsOn(nameof(LoadedSklb))]
    public bool IsLoaded => this.LoadedSklb != null;

    public ICommand LoadSource => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {

            if (string.IsNullOrEmpty(this.SklbPath))
            {
                await DialogUtils.ShowErrorPopup("Must specify a sklb path.");
                return;
            }


            Log.Information($"Attempting to import sklb '{this.SklbPath}'...");

            try
            {
                this.LoadedSklb = Sklb.FromFile(this.SklbPath);
            }
            catch (Exception e)
            {
                Log.Error($"Error importing: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error importing: {e.Message}");
                return;
            }

            DialogUtils.ShowSnackbar("Successfully loaded sklb.");
            Log.Information("Successfully loaded sklb.");
        }
    });

    public ICommand SaveOutput => new Command(async (_) =>
    {
        if (this.OutputPath == null || this.LoadedSklb == null)
            throw new Exception("Unexpected null value"); // Should never happen

        using (new ProgressWrapper())
        {
            Log.Information("Saving sklb...");
            await File.WriteAllBytesAsync(this.OutputPath, this.LoadedSklb.ToBytes());
            string resultText = $"Sklb saved.";
            Log.Information(resultText);
            DialogUtils.ShowSnackbar(resultText);
        }
    });

    public ICommand ExportSkeleton => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {
            Log.Information("Exporting skeleton...");

            if (this.SelectedExportType == null || this.LoadedSklb == null)
                throw new Exception("Unexpected null value"); // Should never happen

            try
            {
                switch (this.SelectedExportType)
                {
                    case AnimationFileType.FBX:
                        {
                            Log.Information("Exporting FBX...");
                            var result = await SkeletonInterop.ExportFBX(this.LoadedSklb, this.ExportPath);
                            string resultText = $"Generated FBX from {result} bones.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;

                    case AnimationFileType.HavokTagFile:
                    case AnimationFileType.HavokPackFile:
                    case AnimationFileType.HavokXMLFile:
                        {
                            Log.Information("Exporting Havok file...");
                            await SkeletonInterop.ExportHavok(this.LoadedSklb, (AnimationFileType)this.SelectedExportType, this.ExportPath);
                            string resultText = $"Exported havok file.";
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

    public ICommand ImportSkeleton => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {
            Log.Information("Importing skeleton...");

            if (this.SelectedImportType == null || this.LoadedSklb == null)
                throw new Exception("Unexpected null value"); // Should never happen

            try
            {
                switch (this.SelectedImportType)
                {
                    case AnimationFileType.FBX:
                        {
                            Log.Information("Importing FBX...");
                            var result = await SkeletonInterop.ImportFBX(this.LoadedSklb, this.ImportPath, this.PreserveImportCompatibility);
                            string resultText = $"Generated havok data from {result} bones.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;

                    case AnimationFileType.HavokTagFile:
                    case AnimationFileType.HavokPackFile:
                    case AnimationFileType.HavokXMLFile:
                        {
                            Log.Information("Importing Havok file...");
                            await SkeletonInterop.ImportHavok(this.LoadedSklb, (AnimationFileType)this.SelectedImportType, this.ImportPath);
                            string resultText = $"Imported havok file.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error importing: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error importing: {e.Message}");
            }
        }
    });

    public void OnSklbPathChanged()
    {
        this.LoadedSklb = null;
    }
    public void OnImportPathChanged()
    {
        this.SelectedImportType = AnimationFileTypeExtensions.GetTypeFromExtension(Path.GetExtension(this.ImportPath));
    }

    public void OnExportPathChanged()
    {
        this.SelectedExportType = AnimationFileTypeExtensions.GetTypeFromExtension(Path.GetExtension(this.ExportPath));
    }
}
