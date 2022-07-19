using System.Windows;
using XAT.Utils.Dialog;

namespace XAT;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DialogUtils.MainSnackbar = this.MainSnackbar;
    }

}
