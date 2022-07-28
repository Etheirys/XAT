using PropertyChanged;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Game.Formats.Sklb;
using XAT.Game.Interop;
using XAT.UI.Utils;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Skeleton;

[AddINotifyPropertyChangedInterface]
public partial class SkeletonImportView : UserControl
{
    public static readonly DependencyBinding<SklbFormat?> SkeletonProperty = Binder.Register<SklbFormat?, SkeletonImportView>(nameof(Skeleton));

    public SklbFormat? Skeleton
    {
        get => SkeletonProperty.Get(this);
        set => SkeletonProperty.Set(this, value);
    }

    public string FileTypeFilters => RawAnimationFileTypeExtensions.GetFileFormatFilters(true);

    [DependsOn(nameof(ImportPath))]
    public RawAnimationFileType? FileType => RawAnimationFileTypeExtensions.GetTypeFromExtension(Path.GetExtension(this.ImportPath));

    public string ImportPath { get; set; } = String.Empty;

    public bool PreserveImportCompatibility { get; set; } = true;

    public SkeletonImportView()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
    }

    public ICommand ImportSkeleton => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {
            Log.Information("Importing skeleton...");

            if (this.FileType == null || this.Skeleton == null || string.IsNullOrEmpty(this.ImportPath))
                throw new Exception("Unexpected null value"); // Should never happen

            try
            {
                switch (this.FileType)
                {
                    case RawAnimationFileType.FBX:
                        {
                            Log.Information("Importing FBX...");
                            var result = await SkeletonInterop.ImportFBX(this.Skeleton, this.ImportPath, this.PreserveImportCompatibility);
                            string resultText = $"Generated havok data from {result} bones.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;

                    case RawAnimationFileType.HavokTagFile:
                    case RawAnimationFileType.HavokPackFile:
                    case RawAnimationFileType.HavokXMLFile:
                        {
                            Log.Information("Importing Havok file...");
                            await SkeletonInterop.ImportHavok(this.Skeleton, (RawAnimationFileType)this.FileType, this.ImportPath);
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
}
