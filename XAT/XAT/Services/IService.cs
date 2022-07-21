using System.Threading.Tasks;

namespace XAT.Services;

public interface IService
{
	Task Initialize();
	Task Start();
	Task Shutdown();
}