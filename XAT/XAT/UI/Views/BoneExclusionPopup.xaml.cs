using PropertyChanged;
using System.Windows.Controls;

namespace XAT.UI.Views;

[AddINotifyPropertyChangedInterface]
public partial class BoneExclusionPopup : UserControl
{
    public BoneExclusionPopup()
    {
        InitializeComponent();
    }
}
