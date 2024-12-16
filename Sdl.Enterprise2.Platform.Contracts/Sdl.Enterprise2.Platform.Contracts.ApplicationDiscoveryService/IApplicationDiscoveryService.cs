using System.Collections.Generic;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.ApplicationDiscoveryService
{
	[ServiceContract(Namespace = "http://sdl.com/applicationdiscoveryservice/2014")]
	public interface IApplicationDiscoveryService
	{
		[OperationContract]
		List<ApplicationServiceEndpointInfo> GetEndPoints();
	}
}
