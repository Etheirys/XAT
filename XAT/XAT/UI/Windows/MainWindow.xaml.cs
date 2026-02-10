using System;
using System.Windows;
using System.Windows.Interop;
using XAT.Core;
using XAT.UI.Utils;

namespace XAT.UI.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainSnackbar.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromMilliseconds(10000));
        DialogUtils.MainSnackbar = this.MainSnackbar;
    }

    protected override void OnActivated(EventArgs e)
    {
        if (OperatingSystem.IsWindows())
        {
            Win32.WindowTheming.SetWindowThemeAware(new WindowInteropHelper(this).Handle);
        }

        base.OnActivated(e);
    }
}
