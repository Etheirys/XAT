using System.Windows;
using XAT.UI.Services;

namespace XAT.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DialogService.Instance.MainSnackbar = this.MainSnackbar;
    }

}
