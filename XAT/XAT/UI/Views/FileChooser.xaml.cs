using Microsoft.Win32;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.UI.Utils;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Views;

public partial class FileChooser : UserControl
{
    public static readonly DependencyBinding<string> HintProperty = Binder.Register<string, FileChooser>(nameof(Hint));
    public static readonly DependencyBinding<string> HelperTextProperty = Binder.Register<string, FileChooser>(nameof(HelperText));
    public static readonly DependencyBinding<string> TextProperty = Binder.Register<string, FileChooser>(nameof(Text));
    public static readonly DependencyBinding<bool> IsReadOnlyProperty = Binder.Register<bool, FileChooser>(nameof(IsReadOnly));
    public static readonly DependencyBinding<FileDialogModes> FileDialogModeProperty = Binder.Register<FileDialogModes, FileChooser>(nameof(FileDialogMode), FileDialogModes.Open);
    public static readonly DependencyBinding<string> FileFilterProperty = Binder.Register<string, FileChooser>(nameof(FileFilter));


    public string Hint
    {
        get => HintProperty.Get(this);
        set => HintProperty.Set(this, value);
    }

    public string HelperText
    {
        get => HelperTextProperty.Get(this);
        set => HelperTextProperty.Set(this, value);
    }

    public string Text
    {
        get => TextProperty.Get(this);
        set => TextProperty.Set(this, value);
    }

    public bool IsReadOnly
    {
        get => IsReadOnlyProperty.Get(this);
        set => IsReadOnlyProperty.Set(this, value);
    }


    public FileDialogModes FileDialogMode
    {
        get => FileDialogModeProperty.Get(this);
        set => FileDialogModeProperty.Set(this, value);
    }

    public string FileFilter
    {
        get => FileFilterProperty.Get(this);
        set => FileFilterProperty.Set(this, value);
    }

    public ICommand ShowFileDialog => new Command((_) =>
    {
        FileDialog dialog;

        switch (this.FileDialogMode)
        {
            case FileDialogModes.Open:
                {
                    dialog = new OpenFileDialog();
        }
                break;

            case FileDialogModes.Save:
                {
                    SaveFileDialog sfd = new();
                    sfd.OverwritePrompt = false;
                    dialog = sfd;
                }
                break;
            default: 
                throw new ArgumentOutOfRangeException(nameof(this.FileDialogMode));
        }

        dialog.Filter = this.FileFilter;

        if(dialog.ShowDialog() == true)
        {
            this.Text = dialog.FileName;
        }

    });

    public FileChooser()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }

    public enum FileDialogModes
    {
        Open,
        Save
    }
}
