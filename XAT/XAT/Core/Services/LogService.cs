using Serilog;
using System.Threading.Tasks;
using XAT.Services;

namespace XAT.Core.Services;

public class LogService : ServiceBase<LogService>
{
    public override Task Initialize()
    {
        var log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File(FileService.Instance.LogsPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Logger = log;

        return base.Initialize();
    }
}
