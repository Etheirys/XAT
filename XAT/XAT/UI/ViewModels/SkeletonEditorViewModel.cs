using PropertyChanged;
using Serilog;
using System;
using System.IO;
using System.Windows.Input;
using XAT.Common.FFXIV.Files;
using XAT.Common.Interop;
using XAT.UI.Services;
using XAT.UI.Utils;

namespace XAT.UI.ViewModels;

[AddINotifyPropertyChangedInterface]

public class SkeletonEditorViewModel
{
    public string FileTypeFilters => ContainerFIleTypeExtensions.GetFileFormatFilters();
    public ContainerFileType? SelectedExportType { get; set; }
    public string ExportPath { get; set; } = string.Empty;

    public ContainerFileType? SelectedImportType { get; set; }
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
                await DialogService.Instance.ShowErrorPopup("Must specify a sklb path.");
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
                await DialogService.Instance.ShowErrorPopup($"Error importing: {e.Message}");
                return;
            }

            DialogService.Instance.ShowSnackbar("Successfully loaded sklb.");
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
            DialogService.Instance.ShowSnackbar(resultText);
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
                    case ContainerFileType.FBX:
                        {
                            Log.Information("Exporting FBX...");
                            var result = await SkeletonInterop.ExportFBX(this.LoadedSklb, this.ExportPath);
                            string resultText = $"Generated FBX from {result} bones.";
                            Log.Information(resultText);
                            DialogService.Instance.ShowSnackbar(resultText);
                        }
                        break;

                    case ContainerFileType.HavokTagFile:
                    case ContainerFileType.HavokPackFile:
                    case ContainerFileType.HavokXMLFile:
                        {
                            Log.Information("Exporting Havok file...");
                            await SkeletonInterop.ExportHavok(this.LoadedSklb, (ContainerFileType)this.SelectedExportType, this.ExportPath);
                            string resultText = $"Exported havok file.";
                            Log.Information(resultText);
                            DialogService.Instance.ShowSnackbar(resultText);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error exporting: {e}", e);
                await DialogService.Instance.ShowErrorPopup($"Error exporting: {e.Message}");
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
                    case ContainerFileType.FBX:
                        {
                            Log.Information("Importing FBX...");
                            var result = await SkeletonInterop.ImportFBX(this.LoadedSklb, this.ImportPath, this.PreserveImportCompatibility);
                            string resultText = $"Generated havok data from {result} bones.";
                            Log.Information(resultText);
                            DialogService.Instance.ShowSnackbar(resultText);
                        }
                        break;

                    case ContainerFileType.HavokTagFile:
                    case ContainerFileType.HavokPackFile:
                    case ContainerFileType.HavokXMLFile:
                        {
                            Log.Information("Importing Havok file...");
                            await SkeletonInterop.ImportHavok(this.LoadedSklb, (ContainerFileType)this.SelectedImportType, this.ImportPath);
                            string resultText = $"Imported havok file.";
                            Log.Information(resultText);
                            DialogService.Instance.ShowSnackbar(resultText);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error importing: {e}", e);
                await DialogService.Instance.ShowErrorPopup($"Error importing: {e.Message}");
            }
        }
    });

    public void OnSklbPathChanged()
    {
        this.LoadedSklb = null;
    }
    public void OnImportPathChanged()
    {
        this.SelectedImportType = ContainerFIleTypeExtensions.GetTypeFromExtension(Path.GetExtension(this.ImportPath));
    }

    public void OnExportPathChanged()
    {
        this.SelectedExportType = ContainerFIleTypeExtensions.GetTypeFromExtension(Path.GetExtension(this.ExportPath));
    }
}
