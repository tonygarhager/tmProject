using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using System;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	public class UserManagerClient : IUserManagerClient, IDisposable
	{
		private IUserManagerClient proxy;

		public UserManagerClient()
		{
			proxy = UserManagerClientFactory.Create();
		}

		public UserManagerClient(string baseAddress)
		{
			proxy = UserManagerClientFactory.Create(baseAddress);
		}

		public UserManagerClient(string baseAddress, UserCredentials credentials)
		{
			proxy = UserManagerClientFactory.Create(baseAddress, credentials);
		}

		public UserManagerClient(string baseAddress, UserCredentials credentials, bool useCompression)
		{
			proxy = UserManagerClientFactory.Create(baseAddress, credentials, useCompression);
		}

		public UserManagerClient(IdentityInfoCache identityCache)
		{
			proxy = UserManagerClientFactory.Create(identityCache);
		}

		public UserManagerClient(IdentityInfoCache identityCache, string baseAddress)
		{
			proxy = UserManagerClientFactory.Create(identityCache, baseAddress);
		}

		public UserManagerClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
		{
			proxy = UserManagerClientFactory.Create(identityCache, baseAddress, credentials);
		}

		public UserManagerClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
		{
			proxy = UserManagerClientFactory.Create(identityCache, baseAddress, credentials, useCompression);
		}

		public Guid CreateResourceGroup(Guid parentId, ResourceGroup resourceGroup)
		{
			return proxy.CreateResourceGroup(parentId, resourceGroup);
		}

		public ResourceGroup[] GetResourceGroupHierarchy()
		{
			return proxy.GetResourceGroupHierarchy();
		}

		public Guid GetResourceGroupId(string resourceGroupPath)
		{
			return proxy.GetResourceGroupId(resourceGroupPath);
		}

		[Obsolete("Please call UserManagerClient.GetResourceGroupId instead")]
		public Guid GetOrganizationId(string organizationPath)
		{
			return proxy.GetOrganizationId(organizationPath);
		}

		public ResourceGroup GetResourceGroup(Guid resourceGroupId)
		{
			return proxy.GetResourceGroup(resourceGroupId);
		}

		public User[] GetOrganizationUsers(Guid organizationId)
		{
			return proxy.GetOrganizationUsers(organizationId);
		}

		public MembershipEntry[] GetResourceGroupMembershipEntries(Guid resourceGroupId)
		{
			return proxy.GetResourceGroupMembershipEntries(resourceGroupId);
		}

		public MembershipEntry[] GetUserMembershipEntries(Guid userId)
		{
			return proxy.GetUserMembershipEntries(userId);
		}

		public void AddMembershipEntries(MembershipEntry[] entries)
		{
			proxy.AddMembershipEntries(entries);
		}

		public void DeleteMembershipEntries(MembershipEntry[] entries)
		{
			proxy.DeleteMembershipEntries(entries);
		}

		public Resource[] GetResourceGroupResources(Guid resourceGroupId)
		{
			return proxy.GetResourceGroupResources(resourceGroupId);
		}

		public Guid UpdateResourceGroup(ResourceGroup resourceGroup)
		{
			return proxy.UpdateResourceGroup(resourceGroup);
		}

		public void DeleteResourceGroup(Guid resourceGroupId)
		{
			proxy.DeleteResourceGroup(resourceGroupId);
		}

		public User GetUser(Guid userId)
		{
			return proxy.GetUser(userId);
		}

		public User[] GetAllUsers()
		{
			return GetAllUsersByType(new UserType[0]);
		}

		public User[] GetUsers(Guid[] userIds)
		{
			return proxy.GetUsersByType(userIds, new UserType[0]);
		}

		public User[] GetAllUsersByType(UserType[] types)
		{
			try
			{
				return proxy.GetAllUsersByType(types);
			}
			catch (Exception)
			{
				return proxy.GetAllUsers();
			}
		}

		public User[] GetUsersByType(Guid[] userIds, UserType[] types)
		{
			try
			{
				return proxy.GetUsersByType(userIds, types);
			}
			catch (Exception)
			{
				return proxy.GetUsers(userIds);
			}
		}

		public User[] FindUsers(string searchText)
		{
			return proxy.FindUsers(searchText);
		}

		public void MoveResource(Guid resourceId, Guid resourceGroupId)
		{
			proxy.MoveResource(resourceId, resourceGroupId);
		}

		public Guid AddWindowsUser(Guid organizationId, User user)
		{
			return proxy.AddWindowsUser(organizationId, user);
		}

		public Guid AddUser(Guid organizationId, User user, string password)
		{
			return proxy.AddUser(organizationId, user, password);
		}

		public Guid UpdateUser(User user)
		{
			return proxy.UpdateUser(user);
		}

		public void SetPassword(Guid userId, string password)
		{
			proxy.SetPassword(userId, password);
		}

		public void MoveUsers(Guid[] userIds, Guid organizationId)
		{
			proxy.MoveUsers(userIds, organizationId);
		}

		public void DeleteUsers(Guid[] userIds)
		{
			proxy.DeleteUsers(userIds);
		}

		public Guid AddRole(Role role)
		{
			return proxy.AddRole(role);
		}

		public Guid UpdateRole(Role role)
		{
			return proxy.UpdateRole(role);
		}

		public void DeleteRole(Guid roleId)
		{
			proxy.DeleteRole(roleId);
		}

		public Role[] GetRoles()
		{
			return proxy.GetRoles();
		}

		public void LinkResource(Guid resourceId, Guid resourceGroupId)
		{
			proxy.LinkResource(resourceGroupId, resourceGroupId);
		}

		public void UnLinkResource(Guid resourceId, Guid resourceGroupId)
		{
			proxy.UnLinkResource(resourceId, resourceGroupId);
		}

		public Permission[] GetPermissions()
		{
			return proxy.GetPermissions();
		}

		public User GetUserByUserName(string userName)
		{
			return proxy.GetUserByUserName(userName);
		}

		public bool IsUserNameInUse(string userName)
		{
			return proxy.IsUserNameInUse(userName);
		}

		public Organization GetOrganization(Guid organizationId)
		{
			return proxy.GetOrganization(organizationId);
		}

		public void ChangePassword(string userName, string oldPassword, string newPassword)
		{
			proxy.ChangePassword(userName, oldPassword, newPassword);
		}

		public bool IsResourceNameUnique(Guid resourceGroupId, string name)
		{
			return proxy.IsResourceNameUnique(resourceGroupId, name);
		}

		public void Dispose()
		{
		}
	}
}
