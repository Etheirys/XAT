using PropertyChanged;
using System.Windows.Controls;

namespace XAT.UI.Timeline;

[AddINotifyPropertyChangedInterface]
public partial class TimelineTab : UserControl
{
    public TimelineTab()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
    }
}
