using Sdl.Enterprise2.Platform.Contracts.Communication;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.IdentityModel
{
	[ServiceContract(Name = "UserManager", Namespace = "http://sdl.com/identity/2010")]
	public interface IUserManager
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Permission[] GetAuthorizationData();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Permission[] GetResourceGroupAuthorizationData(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddUser(Guid organizationId, User2011 user, string password);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid ImportUser(Guid organizationId, User2011 user, int salt, string passwordHash);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddWindowsUser(Guid organizationId, User2011 user);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateUser(User2011 user);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteUser(Guid userId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteUsers(Guid[] userIds);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User2011 GetUser(Guid userId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User2011 GetUserByUserName(string userName);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void ChangePassword(string userName, string oldPassword, string newPassword);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void SetPassword(Guid userId, string password);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool IsUserNameInUse(string userName);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid GetUserId(string userName);

		[Obsolete("This Method could potentially return too much data. In preference use GetAllUserIds, and then use GetUsers to retrieve manageable chunks of data.")]
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User2011[] GetAllUsers();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User2011[] GetUsers(Guid[] userIds);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid[] GetAllUserIds();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void MoveUser(Guid userId, Guid organizationId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void MoveUsers(Guid[] userIds, Guid organizationId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User2011[] FindUsers(string searchText);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid CreateOrganization(Guid parentId, Organization organization);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Organization GetOrganization(Guid organizationId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateOrganization(Organization organization);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		User2011[] GetOrganizationUsers(Guid organizationId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid GetOrganizationId(string organizationPath);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid CreateResourceGroup(Guid parentId, ResourceGroup2011 resourceGroup);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ResourceGroup2011 GetResourceGroup(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateResourceGroup(ResourceGroup2011 resourceGroup);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteResourceGroup(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		ResourceGroup2011[] GetResourceGroupHierarchy();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Role2009[] GetRoles();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddRole(Role2009 role);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Role2009 GetRole(Guid roleId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateRole(Role2009 role);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteRole(Guid roleId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Permission[] GetPermissions();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddPermission(Permission permission);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdatePermission(Permission permission);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeletePermission(Guid permissionId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void AddMembershipEntries(MembershipEntry[] entries);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteMembershipEntries(MembershipEntry[] entries);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		MembershipEntry[] GetResourceGroupMembershipEntries(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		MembershipEntry[] GetUserMembershipEntries(Guid userId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid AddResource(Guid parentId, Resource2011 resource);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid UpdateResource(Resource2011 resource);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void DeleteResource(Guid resourceId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void MoveResource(Guid resourceId, Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Resource2011 GetResource(Guid resourceId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid? GetResourceId(string resourcePath);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Resource2011[] GetResourceGroupResources(Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Resource2011[] GetResources(Guid[] resourceIds);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Resource2011[] GetAllResources(string resourceType);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void LinkResource(Guid resourceId, Guid resourceGroupId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void UnLinkResource(Guid resourceId, Guid resourceGroupId);
	}
}
