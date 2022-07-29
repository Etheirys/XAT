using Microsoft.Win32;
using PropertyChanged;
using Serilog;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Game.Formats.Sklb;
using XAT.Game.Interop;
using XAT.UI.Utils;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Skeleton;

[AddINotifyPropertyChangedInterface]
public partial class SkeletonExportView : UserControl
{
    public static readonly DependencyBinding<SklbFormat?> SkeletonProperty = Binder.Register<SklbFormat?, SkeletonExportView>(nameof(Skeleton));

    public SklbFormat? Skeleton
    {
        get => SkeletonProperty.Get(this);
        set => SkeletonProperty.Set(this, value);
    }

    public RawAnimationFileTypeAttribute[] FileTypes => RawAnimationFileTypeExtensions.GetFileFormatAttributes();
    public RawAnimationFileTypeAttribute? FileTypeAttribute { get; set; }

    [DependsOn(nameof(FileTypeAttribute))]
    public RawAnimationFileType? FileType => RawAnimationFileTypeExtensions.GetTypeFromExtension(FileTypeAttribute?.Extension) ?? null;


    public SkeletonExportView()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
    }

    public ICommand ExportSkeleton => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {
            Log.Information("Exporting skeleton...");

            if (this.FileType == null || this.Skeleton == null)
                throw new Exception("Unexpected null value"); // Should never happen

            try
            {
                SaveFileDialog dialog = new();

                dialog.Filter = ((RawAnimationFileType)FileType).GetFileFilter();

                if (dialog.ShowDialog() == true)
                {
                    string path = dialog.FileName;


                    switch (this.FileType)
                    {
                        case RawAnimationFileType.FBX:
                            {
                                Log.Information("Exporting FBX...");
                                var result = await SkeletonInterop.ExportFBX(this.Skeleton, path);
                                string resultText = $"Generated FBX from {result} bones.";
                                Log.Information(resultText);
                                DialogUtils.ShowSnackbar(resultText);
                            }
                            break;

                        case RawAnimationFileType.HavokTagFile:
                        case RawAnimationFileType.HavokPackFile:
                        case RawAnimationFileType.HavokXMLFile:
                            {
                                Log.Information("Exporting Havok file...");
                                await SkeletonInterop.ExportHavok(this.Skeleton, (RawAnimationFileType)this.FileType, path);
                                string resultText = $"Exported havok file.";
                                Log.Information(resultText);
                                DialogUtils.ShowSnackbar(resultText);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error exporting: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error exporting: {e.Message}");
            }
        }
    });
}
