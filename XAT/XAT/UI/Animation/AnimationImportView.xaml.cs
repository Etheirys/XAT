using PropertyChanged;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Game.Formats.Pap;
using XAT.Game.Formats.Sklb;
using XAT.Game.Havok;
using XAT.Game.Interop;
using XAT.UI.Skeleton;
using XAT.UI.Utils;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Animation;

[AddINotifyPropertyChangedInterface]
public partial class AnimationImportView : UserControl
{
    public static readonly DependencyBinding<PapAnimation?> AnimationProperty = Binder.Register<PapAnimation?, AnimationImportView>(nameof(Animation));
    public static readonly DependencyBinding<SklbFormat?> SkeletonProperty = Binder.Register<SklbFormat?, AnimationImportView>(nameof(Skeleton));

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

    public string FileTypeFilters => RawAnimationFileTypeExtensions.GetFileFormatFilters(true);
    public RawAnimationFileType? FileType { get; set; }
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

    public ObservableCollection<string> ExcludedBones { get; private set; } = new();

    public string? ImportPath { get; set; }

    public AnimationImportView()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
    }

    private async Task<bool> OnImportFileLoaded(string filePath)
    {
        this.ImportPath = null;

        using (new ProgressWrapper())
        {
            Log.Information("Identifying animation tracks...");

            this.FileType = RawAnimationFileTypeExtensions.GetTypeFromExtension(Path.GetExtension(filePath));

            this.ExcludedBones.Clear();

            switch (this.FileType)
            {
                case RawAnimationFileType.FBX:
                    {
                        this.ImportTracks = new(await RawHavokInterop.ListFbxStacks(filePath));
                    }
                    break;

                case RawAnimationFileType.HavokTagFile:
                case RawAnimationFileType.HavokPackFile:
                case RawAnimationFileType.HavokXMLFile:
                    {
                        var stats = await RawHavokInterop.GetStats(filePath);
                        this.ImportTracks = new();
                        for (int i = 0; i < stats.animCount; ++i)
                            this.ImportTracks.Add(i.ToString());

                    }
                    break;
            }

            this.ImportPath = filePath;

            var resultText = $"Found {this.ImportTracks.Count} animation track(s).";
            Log.Information(resultText);
            DialogUtils.ShowSnackbar(resultText);
        }

        return true;
    }

    public ICommand ImportAnimation => new Command(async (_) =>
    {
        using (new ProgressWrapper())
        {
            Log.Information("Importing animation...");

            if (this.FileType == null || this.SelectedImportTrackIndex == null || this.Animation == null || this.Skeleton == null || this.ImportPath == null)
                throw new Exception("Unexpected null value"); // Should never happen

            try
            {

                switch (this.FileType)
                {
                    case RawAnimationFileType.FBX:
                        {
                            Log.Information("Importing FBX...");
                            var result = await AnimationInterop.ImportFBX(this.Animation, this.Skeleton, this.ImportPath, (int)this.SelectedImportTrackIndex, new(this.ExcludedBones));
                            string resultText = $"Imported {result.framesConverted} frames across {result.bonesBound} bones from FBX to {this.Animation.Data.Name}.";
                            Log.Information(resultText);
                            DialogUtils.ShowSnackbar(resultText);
                        }
                        break;

                    case RawAnimationFileType.HavokTagFile:
                    case RawAnimationFileType.HavokPackFile:
                    case RawAnimationFileType.HavokXMLFile:
                        {
                            Log.Information("Importing Havok animation...");
                            await AnimationInterop.ImportHavok(this.Animation, this.Skeleton, this.ImportPath, (int)this.SelectedImportTrackIndex);
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

    public ICommand ShowExcludedBonesPopup => new Command(async (_) =>
    {
        if (this.Skeleton == null)
            throw new Exception("Unexpected null value"); // Should never happen

        List<string> boneList = new();

        using (new ProgressWrapper())
        {
            string tmpFile = Path.GetTempFileName();
            await File.WriteAllBytesAsync(tmpFile, this.Skeleton.HavokData);

            boneList = await RawHavokInterop.ListBones(tmpFile, 0);

            File.Delete(tmpFile);
        }

        var popup = new BoneExclusionPopup()
        {
            AllBones = new(boneList),
            ExcludedBones = this.ExcludedBones
        };

        await DialogUtils.Show(popup);
    });
}
