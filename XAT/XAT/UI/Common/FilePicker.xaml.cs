using Microsoft.Win32;
using PropertyChanged;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.UI.Utils;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Common;

[AddINotifyPropertyChangedInterface]
public partial class FilePicker : UserControl
{
    protected static readonly DependencyBinding<string> PathProperty = Binder.Register<string, FilePicker>(nameof(Path));
    protected static readonly DependencyBinding<string> HintProperty = Binder.Register<string, FilePicker>(nameof(Hint));
    protected static readonly DependencyBinding<string> HelperTextProperty = Binder.Register<string, FilePicker>(nameof(HelperText));
    protected static readonly DependencyBinding<bool> ShowRefreshProperty = Binder.Register<bool, FilePicker>(nameof(ShowRefresh), true);
    protected static readonly DependencyBinding<string> FileFilterProperty = Binder.Register<string, FilePicker>(nameof(FileFilter), "All Files|*.*");


    public string Path
    {
        get => PathProperty.Get(this);
        set => PathProperty.Set(this, value);
    }

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

    public bool ShowRefresh
    {
        get => ShowRefreshProperty.Get(this);
        set => ShowRefreshProperty.Set(this, value);
    }

    public string FileFilter
    {
        get => FileFilterProperty.Get(this);
        set => FileFilterProperty.Set(this, value);
    }

    public delegate Task<bool> FileSelectedEventHandler(string filePath);

    public event FileSelectedEventHandler? FileSelected = null;

    public ICommand ShowFileDialog => new Command(async (_) =>
    {
        OpenFileDialog dialog = new();
        dialog.Filter = this.FileFilter;

        if (dialog.ShowDialog() == true)
        {
            try
            {
                string filePath = dialog.FileName;
                bool worked = true;

                if (FileSelected != null)
                    worked = await FileSelected.Invoke(filePath);

                if(worked)
                {
                    this.Path = filePath;
                }
                else
                {
                    this.Path = string.Empty;
                }
            } 
            catch
            {
                throw;
            }
            
        }
    });

    public ICommand RefreshFile => new Command((_) =>
    {
        this.FileSelected?.Invoke(this.Path);
    });

    public FilePicker()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }
}
