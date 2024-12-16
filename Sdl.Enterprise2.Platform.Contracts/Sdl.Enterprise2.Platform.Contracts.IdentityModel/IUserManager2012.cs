using Sdl.Enterprise2.Platform.Contracts.Communication;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[ServiceContract(Name = "UserManager", Namespace = "http://sdl.com/identity/2012")]
	public interface IUserManager2012
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid CreateResourceGroup(Guid parentId, ResourceGroup resourceGroup);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ResourceGroup[] GetResourceGroupHierarchy();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid GetResourceGroupId(string resourceGroupPath);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ResourceGroup GetResourceGroup(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User[] GetOrganizationUsers(Guid organizationId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		MembershipEntry[] GetResourceGroupMembershipEntries(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		MembershipEntry[] GetUserMembershipEntries(Guid userId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void AddMembershipEntries(MembershipEntry[] entries);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteMembershipEntries(MembershipEntry[] entries);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Resource[] GetResourceGroupResources(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateResourceGroup(ResourceGroup resourceGroup);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteResourceGroup(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User GetUser(Guid userId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User[] GetAllUsers();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User[] GetUsers(Guid[] userIds);

		[OperationContract]
		[FaultContract(typeof(ServiceFault))]
		User[] GetAllUsersByType(UserType[] types);

		[OperationContract]
		[FaultContract(typeof(ServiceFault))]
		User[] GetUsersByType(Guid[] userIds, UserType[] types);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User[] FindUsers(string searchText);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void MoveResource(Guid resourceId, Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddWindowsUser(Guid organizationId, User user);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddUser(Guid organizationId, User user, string password);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateUser(User user);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void SetPassword(Guid userId, string password);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void MoveUsers(Guid[] userIds, Guid organizationId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteUsers(Guid[] userIds);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddRole(Role role);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateRole(Role role);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteRole(Guid roleId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Role[] GetRoles();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void LinkResource(Guid resourceId, Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void UnLinkResource(Guid resourceId, Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Permission[] GetPermissions();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User GetUserByUserName(string userName);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void ChangePassword(string userName, string oldPassword, string newPassword);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool IsUserNameInUse(string userName);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool IsResourceNameUnique(Guid resourceGroupId, string name);
	}
}
