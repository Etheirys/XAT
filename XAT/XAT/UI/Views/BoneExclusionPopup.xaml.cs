using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using XAT.UI.ViewModels;

namespace XAT.UI.Views;

[AddINotifyPropertyChangedInterface]
public partial class BoneExclusionPopup : UserControl
{
    public BoneExclusionPopup()
    {
        InitializeComponent();
        this.DataContext = new BoneExclusionViewModel();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        this.ContentArea.DataContext = this.DataContext;
    }
}
