using System.Diagnostics;
using System.Windows.Controls;

namespace XAT.UI.Misc;

public partial class HelpTab : UserControl
{
    public HelpTab()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }

    private void GitHub_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/Etheirys/XAT",
            UseShellExecute = true
        });
    }

    private void Discord_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://discord.gg/KvGJCCnG8t",
            UseShellExecute = true
        });
    }

    private void KoFi_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://ko-fi.com/Asgard",
            UseShellExecute = true
        });
    }
}
