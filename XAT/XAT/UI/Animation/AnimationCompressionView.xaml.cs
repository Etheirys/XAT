using PropertyChanged;
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
public partial class AnimationCompressionView : UserControl
{
    public static readonly DependencyBinding<PapAnimation?> AnimationProperty = Binder.Register<PapAnimation?, AnimationCompressionView>(nameof(Animation));
    public static readonly DependencyBinding<SklbFormat?> SkeletonProperty = Binder.Register<SklbFormat?, AnimationCompressionView>(nameof(Skeleton));

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

    public CompressionType[] CompressionTypes => (CompressionType[])Enum.GetValues(typeof(CompressionType));
    public CompressionType? SelectedCompressionType { get; set; }
    public CompressionTolerance CompressionTolerances { get; } = new();

    public ICommand Compress => new Command(async (_) =>
    {
        if (this.Animation == null || this.Skeleton == null)
            throw new Exception("Unexpected null value"); // Should never happen

        using (new ProgressWrapper())
        {
            try
            {
                int havokSizeBefore = this.Animation.Container.HavokData.Length;

                switch (this.SelectedCompressionType)
                {
                    case CompressionType.Predictive:
                        {
                            await AnimationInterop.PredictiveCompress(this.Animation, this.Skeleton, 0, this.CompressionTolerances.StaticTranslationTolerance, this.CompressionTolerances.StaticRotationTolerance, this.CompressionTolerances.StaticScaleTolerance, 0, this.CompressionTolerances.DynamicTranslationTolerance, this.CompressionTolerances.DynamicRotationTolerance, this.CompressionTolerances.DynamicScaleTolerance);
                        }
                        break;

                    case CompressionType.Quantized:
                        {
                            await AnimationInterop.QuantizedCompress(this.Animation, this.Skeleton, 0, this.CompressionTolerances.StaticTranslationTolerance, this.CompressionTolerances.StaticRotationTolerance, this.CompressionTolerances.StaticScaleTolerance);
                        }
                        break;
                }

                int havokSizeNow = this.Animation.Container.HavokData.Length;
                DialogUtils.ShowSnackbar($"Saved {havokSizeBefore - havokSizeNow} bytes during compression.");

            }
            catch (Exception e)
            {
                await DialogUtils.ShowErrorPopup(e.Message);
            }
        }
    });

    public AnimationCompressionView()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
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
