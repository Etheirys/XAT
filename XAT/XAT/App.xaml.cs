using Serilog;
using System;
using System.Windows;

namespace XAT;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File("Logs/xat.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Logger = log;

        Log.Information($"XAT Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version}");
        Log.Information($"Dotnet Runtime: {Environment.Version}");
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error("Fatal error {e}", e);
    }
}
