using Serilog;
using System.Windows;
using XAT.Services;

namespace XAT;

public partial class App : Application
{
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        ServiceManager.Create(this);

        await ServiceManager.Instance.InitializeServices();

        await ServiceManager.Instance.StartServices();

    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error("Fatal error {e}", e);
    }
}