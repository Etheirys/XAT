using System;
using System.Threading.Tasks;

namespace XAT.Services;

public abstract class ServiceBase<T> : IService
	where T : ServiceBase<T>
{
	private static T? instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
				throw new Exception($"No service found: {typeof(T)}");

			return instance;
		}
	}

	public virtual Task Initialize()
	{
		instance = (T)this;
		return Task.CompletedTask;
	}

	public virtual Task Shutdown()
	{
		return Task.CompletedTask;
	}

	public virtual Task Start()
	{
		return Task.CompletedTask;
	}
}