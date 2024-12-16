using Sdl.Enterprise2.Platform.Contracts.Communication;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[ServiceContract(Name = "UserManager", Namespace = "http://sdl.com/identity/2011")]
	public interface IUserManager2011 : IUserManager
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Dictionary<Guid, Permission[]> GetResourceGroupsAuthorizationData(Guid[] resourceGroupIds);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Dictionary<Guid, Permission[]> GetAllAuthorizationData();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool IsResourceNameUnique(Guid resourceGroupId, string name);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Role2011[] GetRoles2011();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		string GetUserName(Guid userId);
	}
}
