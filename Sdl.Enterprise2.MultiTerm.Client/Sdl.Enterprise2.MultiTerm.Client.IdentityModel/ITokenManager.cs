using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.Discovery;
using System.ServiceModel;
using System.Xml.Linq;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	[ServiceContract(Name = "TokenManager", Namespace = "http://sdl.com/identity/2010")]
	public interface ITokenManager : IRouter
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		XElement IssueToken();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		XElement IssueTokenFromSaml(string samlToken);
	}
}
