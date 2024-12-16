using Sdl.Enterprise2.Platform.Contracts.Communication;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.Discovery
{
	[ServiceContract(Name = "Discovery", Namespace = "http://sdl.com/router/2010")]
	public interface IDiscoveryService
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ServiceEndpointInfo[] GetServiceEndpoints();
	}
}
