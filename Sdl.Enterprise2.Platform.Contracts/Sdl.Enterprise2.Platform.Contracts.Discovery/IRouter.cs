using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.Discovery
{
	[ServiceContract(Name = "Router", Namespace = "http://sdl.com/router/2010")]
	public interface IRouter
	{
		[OperationContract]
		bool Ping();
	}
}
