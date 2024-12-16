using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	public class UserManager2012Client : IssuedTokenClientBase<IUserManager2012>, IUserManagerClient
	{
		private const string SERVICE_PATH = "/UserManager2012Service";

		public UserManager2012Client()
			: base(IdentityInfoCache.Default, "/UserManager2012Service", (IdentityInfoCache.Default.Count > 0) ? IdentityInfoCache.Default.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2012Client(string baseAddress)
			: base(IdentityInfoCache.Default, "/UserManager2012Service", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2012Client(string baseAddress, UserCredentials credentials)
			: base(IdentityInfoCache.Default, "/UserManager2012Service", baseAddress, credentials, useCompression: false)
		{
		}

		public UserManager2012Client(string baseAddress, UserCredentials credentials, bool useCompression)
			: base(IdentityInfoCache.Default, "/UserManager2012Service", baseAddress, credentials, useCompression)
		{
		}

		public UserManager2012Client(IdentityInfoCache identityCache)
			: base(identityCache, "/UserManager2012Service", (identityCache.Count > 0) ? identityCache.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2012Client(IdentityInfoCache identityCache, string baseAddress)
			: base(identityCache, "/UserManager2012Service", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2012Client(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
			: base(identityCache, "/UserManager2012Service", baseAddress, credentials, useCompression: false)
		{
		}

		public UserManager2012Client(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
			: base(identityCache, "/UserManager2012Service", baseAddress, credentials, useCompression)
		{
		}

		public Guid CreateResourceGroup(Guid parentId, ResourceGroup resourceGroup)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.CreateResourceGroup(parentId, resourceGroup);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public ResourceGroup[] GetResourceGroupHierarchy()
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceGroupHierarchy();
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid GetResourceGroupId(string resourceGroupPath)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceGroupId(resourceGroupPath);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid GetOrganizationId(string organizationPath)
		{
			return GetResourceGroupId(organizationPath);
		}

		public Organization GetOrganization(Guid organizationId)
		{
			throw new NotSupportedException("This method is not supported. Use GetResourceGroup instead.");
		}

		public ResourceGroup GetResourceGroup(Guid resourceGroupId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceGroup(resourceGroupId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User[] GetOrganizationUsers(Guid organizationId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetOrganizationUsers(organizationId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public MembershipEntry[] GetResourceGroupMembershipEntries(Guid resourceGroupId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceGroupMembershipEntries(resourceGroupId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public MembershipEntry[] GetUserMembershipEntries(Guid userId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetUserMembershipEntries(userId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void AddMembershipEntries(MembershipEntry[] entries)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.AddMembershipEntries(entries);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void DeleteMembershipEntries(MembershipEntry[] entries)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.DeleteMembershipEntries(entries);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Resource[] GetResourceGroupResources(Guid resourceGroupId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceGroupResources(resourceGroupId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid UpdateResourceGroup(ResourceGroup resourceGroup)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.UpdateResourceGroup(resourceGroup);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void DeleteResourceGroup(Guid resourceGroupId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.DeleteResourceGroup(resourceGroupId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User GetUser(Guid userId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetUser(userId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User[] GetAllUsers()
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetAllUsers();
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User[] GetUsers(Guid[] userIds)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetUsers(userIds);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User[] GetAllUsersByType(UserType[] types)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetAllUsersByType(types);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User[] GetUsersByType(Guid[] userIds, UserType[] types)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetUsersByType(userIds, types);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User[] FindUsers(string searchText)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.FindUsers(searchText);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void MoveResource(Guid resourceId, Guid resourceGroupId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.MoveResource(resourceId, resourceGroupId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid AddWindowsUser(Guid organizationId, User user)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.AddWindowsUser(organizationId, user);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid AddUser(Guid organizationId, User user, string password)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.AddUser(organizationId, user, password);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid UpdateUser(User user)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.UpdateUser(user);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void SetPassword(Guid userId, string password)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.SetPassword(userId, password);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void MoveUsers(Guid[] userIds, Guid organizationId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.MoveUsers(userIds, organizationId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void DeleteUsers(Guid[] userIds)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.DeleteUsers(userIds);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid AddRole(Role role)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.AddRole(role);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Guid UpdateRole(Role role)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.UpdateRole(role);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void DeleteRole(Guid roleId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.DeleteRole(roleId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Role[] GetRoles()
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetRoles();
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void LinkResource(Guid resourceId, Guid resourceGroupId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.LinkResource(resourceId, resourceGroupId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void UnLinkResource(Guid resourceId, Guid resourceGroupId)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.UnLinkResource(resourceId, resourceGroupId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public Permission[] GetPermissions()
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetPermissions();
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public User GetUserByUserName(string userName)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetUserByUserName(userName);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public bool IsUserNameInUse(string userName)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.IsUserNameInUse(userName);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public bool IsResourceNameUnique(Guid resourceGroupId, string name)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.IsResourceNameUnique(resourceGroupId, name);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}

		public void ChangePassword(string userName, string oldPassword, string newPassword)
		{
			IUserManager2012 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.ChangePassword(userName, oldPassword, newPassword);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(userManager as ICommunicationObject, connectionError);
			}
		}
	}
}
