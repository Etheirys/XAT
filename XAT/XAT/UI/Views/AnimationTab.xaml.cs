using PropertyChanged;
using System.Windows.Controls;
using XAT.UI.ViewModels;

namespace XAT.UI.Views;

public partial class AnimationTab : UserControl
{
    public AnimationTab()
    {
        InitializeComponent();
        this.ContentArea.DataContext = new AnimationEditorViewModel();
    }
}
