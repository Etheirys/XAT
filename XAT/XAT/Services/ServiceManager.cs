using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XAT.Core.Services;
using XAT.UI.Services;
using XAT.UI.Utils;

namespace XAT.Services;

public class ServiceManager
{
	public async Task InitializeServices()
	{
		// Core
		await Add<FileService>();
		await Add<LogService>();
		await Add<VersionService>();

		// UI
		await Add<MainWindowService>();
	}

	private readonly List<IService> Services = new();
	public App App { get; init; }

	public async Task StartServices()
	{
		foreach (IService service in Services)
		{
			await service.Start();
		}
	}

	public async Task ShutdownServices()
	{
		Services.Reverse();

		foreach (var service in Services)
		{
			await service.Shutdown();
		}
	}

	public ServiceManager(App app)
	{
		this.App = app;
	}

	private async Task Add<T>() where T : IService, new()
	{
		try
		{
			IService service = Activator.CreateInstance<T>();
			Services.Add(service);
			await service.Initialize();
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, $"{typeof(T).Name} Error: {ex.Message}");
		}
	}

	public static void Create(App app)
	{
		Instance = new ServiceManager(app);
	}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public static ServiceManager Instance { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}