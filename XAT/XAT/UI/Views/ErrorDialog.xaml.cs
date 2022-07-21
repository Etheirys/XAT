using System.Windows.Controls;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Views;

public partial class ErrorDialog : UserControl
{
    public static readonly DependencyBinding<string> MessageProperty = Binder.Register<string, FileChooser>(nameof(Message));

    public string Message
    {
        get => MessageProperty.Get(this);
        set => MessageProperty.Set(this, value);
    }

    public ErrorDialog()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }
}
