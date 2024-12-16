using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using System;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	public interface IUserManagerClient
	{
		Guid CreateResourceGroup(Guid parentId, ResourceGroup resourceGroup);

		ResourceGroup[] GetResourceGroupHierarchy();

		Guid GetResourceGroupId(string resourceGroupPath);

		[Obsolete("Please call GetResourceGroupId instead")]
		Guid GetOrganizationId(string organizationPath);

		ResourceGroup GetResourceGroup(Guid resourceGroupId);

		User[] GetOrganizationUsers(Guid organizationId);

		MembershipEntry[] GetResourceGroupMembershipEntries(Guid resourceGroupId);

		MembershipEntry[] GetUserMembershipEntries(Guid userId);

		void AddMembershipEntries(MembershipEntry[] entries);

		void DeleteMembershipEntries(MembershipEntry[] entries);

		Resource[] GetResourceGroupResources(Guid resourceGroupId);

		Guid UpdateResourceGroup(ResourceGroup resourceGroup);

		void DeleteResourceGroup(Guid resourceGroupId);

		User GetUser(Guid userId);

		User[] GetAllUsers();

		User[] GetUsers(Guid[] userIds);

		User[] FindUsers(string searchText);

		void MoveResource(Guid resourceId, Guid resourceGroupId);

		Guid AddWindowsUser(Guid organizationId, User user);

		Guid AddUser(Guid organizationId, User user, string password);

		Guid UpdateUser(User user);

		void SetPassword(Guid userId, string password);

		void MoveUsers(Guid[] userIds, Guid organizationId);

		void DeleteUsers(Guid[] userIds);

		Guid AddRole(Role role);

		Guid UpdateRole(Role role);

		void DeleteRole(Guid roleId);

		Role[] GetRoles();

		void LinkResource(Guid resourceId, Guid resourceGroupId);

		void UnLinkResource(Guid resourceId, Guid resourceGroupId);

		Permission[] GetPermissions();

		User GetUserByUserName(string userName);

		bool IsUserNameInUse(string userName);

		Organization GetOrganization(Guid organizationId);

		void ChangePassword(string userName, string oldPassword, string newPassword);

		bool IsResourceNameUnique(Guid resourceGroupId, string name);
	}
}
