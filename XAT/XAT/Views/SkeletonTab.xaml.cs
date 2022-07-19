using System.Windows.Controls;
using XAT.ViewModels;

namespace XAT.Views;

public partial class SkeletonTab : UserControl
{
    public SkeletonTab()
    {
        InitializeComponent();
        this.ContentArea.DataContext = new SkeletonEditorViewModel();
    }
}
