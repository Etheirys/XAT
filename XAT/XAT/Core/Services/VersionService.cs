using Serilog;
using System;
using System.Threading.Tasks;
using XAT.Services;

namespace XAT.Core.Services;

public class VersionService : ServiceBase<VersionService>
{
    public override Task Initialize()
    {
        Log.Information($"XAT Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version}");
        Log.Information($"Dotnet Runtime: {Environment.Version}");

        return base.Initialize();
    }
}
