using Microsoft.Win32;
using PropertyChanged;
using Serilog;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Game.Formats.Pap;
using XAT.Game.Formats.Sklb;
using XAT.Game.Interop;
using XAT.UI.Utils;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Animation;

[AddINotifyPropertyChangedInterface]
public partial class AnimationExportView : UserControl
{
    public static readonly DependencyBinding<PapAnimation?> AnimationProperty = Binder.Register<PapAnimation?, AnimationExportView>(nameof(Animation));
    public static readonly DependencyBinding<SklbFormat?> SkeletonProperty = Binder.Register<SklbFormat?, AnimationExportView>(nameof(Skeleton));

    public PapAnimation? Animation
    {
        get => AnimationProperty.Get(this);
        set => AnimationProperty.Set(this, value);
    }

    public SklbFormat? Skeleton
    {
        get => SkeletonProperty.Get(this);
        set => SkeletonProperty.Set(this, value);
    }

    public RawAnimationFileTypeAttribute[] FileTypes => RawAnimationFileTypeExtensions.GetFileFormatAttributes();
    public RawAnimationFileTypeAttribute? FileTypeAttribute { get; set; }

    [DependsOn(nameof(FileTypeAttribute))]
    public RawAnimationFileType? FileType => RawAnimationFileTypeExtensions.GetTypeFromExtension(FileTypeAttribute?.Extension) ?? null;

    public bool HavokBundleSkeleton { get; set; }

    public AnimationExportView()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
    }

    public ICommand ExportAnimation => new Command(async (_) =>
    {


        using (new ProgressWrapper())
        {
            Log.Information("Exporting animation...");


            if (this.FileType == null || this.Animation == null || this.Skeleton == null)
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
                                var result = await AnimationInterop.ExportFBX(this.Animation, this.Skeleton, path);
                                string resultText = $"Generated FBX from {this.Animation.Data.Name} with {result.framesConverted} frames across {result.bonesConverted} bones.";
                                Log.Information(resultText);
                                DialogUtils.ShowSnackbar(resultText);
                            }
                            break;

                        case RawAnimationFileType.HavokTagFile:
                        case RawAnimationFileType.HavokPackFile:
                        case RawAnimationFileType.HavokXMLFile:
                            {
                                Log.Information("Exporting Havok file...");
                                await AnimationInterop.ExportHavok((RawAnimationFileType)this.FileType, this.Animation, this.Skeleton, this.HavokBundleSkeleton, path);
                                string resultText = $"Exported havok file from {this.Animation.Data.Name}.";
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
