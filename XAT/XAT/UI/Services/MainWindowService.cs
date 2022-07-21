using System.Threading.Tasks;
using XAT.Services;
using XAT.UI;

namespace XAT.UI.Services;

public class MainWindowService : ServiceBase<MainWindowService>
{
    public override Task Start()
    {
        var newMainWindow = new MainWindow();
        ServiceManager.Instance.App.MainWindow = newMainWindow;
        newMainWindow.Show();

        return base.Start();
    }
}
