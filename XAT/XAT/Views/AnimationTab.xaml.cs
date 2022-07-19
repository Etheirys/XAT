using PropertyChanged;
using System.Windows.Controls;
using XAT.ViewModels;

namespace XAT.Views;

public partial class AnimationTab : UserControl
{
    public AnimationTab()
    {
        InitializeComponent();
        this.ContentArea.DataContext = new AnimationEditorViewModel();
    }
}
