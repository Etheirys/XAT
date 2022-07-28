using PropertyChanged;
using System.Windows.Controls;
using XAT.Game.Formats.Pap;
using XAT.Game.Formats.Sklb;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Animation;

[AddINotifyPropertyChangedInterface]
public partial class AnimationEditor : UserControl
{
public static readonly DependencyBinding<PapAnimation?> AnimationProperty = Binder.Register<PapAnimation?, AnimationEditor>(nameof(Animation));
    public static readonly DependencyBinding<SklbFormat?> SkeletonProperty = Binder.Register<SklbFormat?, AnimationEditor>(nameof(Skeleton));

    public PapAnimation? Animation { get; set; }

    public SklbFormat? Skeleton { get; set; }

    public AnimationEditor()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
    }
}
