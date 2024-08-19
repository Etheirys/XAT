using System;
using System.Windows;
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

}
