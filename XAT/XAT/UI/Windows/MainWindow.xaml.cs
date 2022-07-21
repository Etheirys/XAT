using System.Windows;
using XAT.UI.Utils;

namespace XAT.UI.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DialogUtils.MainSnackbar = this.MainSnackbar;
    }

}
