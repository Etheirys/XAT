using System.Windows.Controls;
using XAT.UI.ViewModels;

namespace XAT.UI.Views;

public partial class SkeletonTab : UserControl
{
    public SkeletonTab()
    {
        InitializeComponent();
        this.ContentArea.DataContext = new SkeletonEditorViewModel();
    }
}
