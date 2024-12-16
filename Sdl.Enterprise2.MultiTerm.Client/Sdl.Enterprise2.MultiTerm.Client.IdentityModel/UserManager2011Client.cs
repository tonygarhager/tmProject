using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	public class UserManager2011Client : IssuedTokenClientBase<IUserManager2011>, IUserManagerClient
	{
		private const string SERVICE_PATH = "/UserManager2011Service";

		public UserManager2011Client()
			: base(IdentityInfoCache.Default, "/UserManager2011Service", (IdentityInfoCache.Default.Count > 0) ? IdentityInfoCache.Default.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2011Client(string baseAddress)
			: base(IdentityInfoCache.Default, "/UserManager2011Service", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2011Client(string baseAddress, UserCredentials credentials)
			: base(IdentityInfoCache.Default, "/UserManager2011Service", baseAddress, credentials, useCompression: false)
		{
		}

		public UserManager2011Client(string baseAddress, UserCredentials credentials, bool useCompression)
			: base(IdentityInfoCache.Default, "/UserManager2011Service", baseAddress, credentials, useCompression)
		{
		}

		public UserManager2011Client(IdentityInfoCache identityCache)
			: base(identityCache, "/UserManager2011Service", (identityCache.Count > 0) ? identityCache.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2011Client(IdentityInfoCache identityCache, string baseAddress)
			: base(identityCache, "/UserManager2011Service", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public UserManager2011Client(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
			: base(identityCache, "/UserManager2011Service", baseAddress, credentials, useCompression: false)
		{
		}

		public UserManager2011Client(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
			: base(identityCache, "/UserManager2011Service", baseAddress, credentials, useCompression)
		{
		}

		public Dictionary<Guid, Permission[]> GetResourceGroupsAuthorizationData(Guid[] resourceGroupIds)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceGroupsAuthorizationData(resourceGroupIds);
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

		public Dictionary<Guid, Permission[]> GetAllAuthorizationData()
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetAllAuthorizationData();
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
			IUserManager2011 userManager = null;
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

		public Role2011[] GetRoles2011()
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetRoles2011();
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

		public string GetUserName(Guid userId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetUserName(userId);
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

		public Permission[] GetAuthorizationData()
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetAuthorizationData();
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

		public Permission[] GetResourceGroupAuthorizationData(Guid resourceGroupId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceGroupAuthorizationData(resourceGroupId);
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
			IUserManager2011 userManager = null;
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

		public Guid ImportUser(Guid organizationId, User2011 user, int salt, string passwordHash)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.ImportUser(organizationId, user, salt, passwordHash);
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
			IUserManager2011 userManager = null;
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

		public Guid UpdateUser(User user)
		{
			IUserManager2011 userManager = null;
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

		public void DeleteUser(Guid userId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.DeleteUser(userId);
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
			IUserManager2011 userManager = null;
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

		public User GetUser(Guid userId)
		{
			IUserManager2011 userManager = null;
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

		public User GetUserByUserName(string userName)
		{
			IUserManager2011 userManager = null;
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

		public void ChangePassword(string userName, string oldPassword, string newPassword)
		{
			IUserManager2011 userManager = null;
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

		public void SetPassword(Guid userId, string password)
		{
			IUserManager2011 userManager = null;
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

		public bool IsUserNameInUse(string userName)
		{
			IUserManager2011 userManager = null;
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

		public Guid GetUserId(string userName)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetUserId(userName);
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
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				User2011[] allUsers = userManager.GetAllUsers();
				return Array.ConvertAll(allUsers, (Converter<User2011, User>)((User2011 input) => input));
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
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				User2011[] users = userManager.GetUsers(userIds);
				return Array.ConvertAll(users, (Converter<User2011, User>)((User2011 input) => input));
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

		public Guid[] GetAllUserIds()
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetAllUserIds();
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

		public void MoveUser(Guid userId, Guid organizationId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.MoveUser(userId, organizationId);
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
			IUserManager2011 userManager = null;
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

		public User[] FindUsers(string searchText)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				User2011[] array = userManager.FindUsers(searchText);
				return Array.ConvertAll(array, (Converter<User2011, User>)((User2011 input) => input));
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

		public Guid CreateOrganization(Guid parentId, Organization organization)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.CreateOrganization(parentId, organization);
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

		public Organization GetOrganization(Guid organizationId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetOrganization(organizationId);
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

		public Guid UpdateOrganization(Organization organization)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.UpdateOrganization(organization);
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
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				User2011[] organizationUsers = userManager.GetOrganizationUsers(organizationId);
				return Array.ConvertAll(organizationUsers, (Converter<User2011, User>)((User2011 input) => input));
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
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetOrganizationId(organizationPath);
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

		public Guid CreateResourceGroup(Guid parentId, ResourceGroup resourceGroup)
		{
			IUserManager2011 userManager = null;
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

		public Guid GetResourceGroupId(string resourceGroupPath)
		{
			return GetOrganizationId(resourceGroupPath);
		}

		public ResourceGroup GetResourceGroup(Guid resourceGroupId)
		{
			IUserManager2011 userManager = null;
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

		public Guid UpdateResourceGroup(ResourceGroup resourceGroup)
		{
			IUserManager2011 userManager = null;
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
			IUserManager2011 userManager = null;
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

		public ResourceGroup[] GetResourceGroupHierarchy()
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				ResourceGroup2011[] resourceGroupHierarchy = userManager.GetResourceGroupHierarchy();
				return Array.ConvertAll(resourceGroupHierarchy, (Converter<ResourceGroup2011, ResourceGroup>)((ResourceGroup2011 input) => input));
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
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				Role2009[] roles = userManager.GetRoles();
				return Array.ConvertAll(roles, (Converter<Role2009, Role>)((Role2009 input) => input));
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
			IUserManager2011 userManager = null;
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

		public Role2009 GetRole(Guid roleId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetRole(roleId);
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
			IUserManager2011 userManager = null;
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
			IUserManager2011 userManager = null;
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

		public Permission[] GetPermissions()
		{
			IUserManager2011 userManager = null;
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

		public Guid AddPermission(Permission permission)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.AddPermission(permission);
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

		public Guid UpdatePermission(Permission permission)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.UpdatePermission(permission);
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

		public void DeletePermission(Guid permissionId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.DeletePermission(permissionId);
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
			IUserManager2011 userManager = null;
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
			IUserManager2011 userManager = null;
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

		public MembershipEntry[] GetResourceGroupMembershipEntries(Guid resourceGroupId)
		{
			IUserManager2011 userManager = null;
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
			IUserManager2011 userManager = null;
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

		public Guid AddResource(Guid parentId, Resource2011 resource)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.AddResource(parentId, resource);
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

		public Guid UpdateResource(Resource2011 resource)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.UpdateResource(resource);
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

		public void DeleteResource(Guid resourceId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				userManager.DeleteResource(resourceId);
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
			IUserManager2011 userManager = null;
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

		public Resource2011 GetResource(Guid resourceId)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResource(resourceId);
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

		public Guid? GetResourceId(string resourcePath)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResourceId(resourcePath);
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
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				Resource2011[] resourceGroupResources = userManager.GetResourceGroupResources(resourceGroupId);
				return Array.ConvertAll(resourceGroupResources, (Converter<Resource2011, Resource>)((Resource2011 input) => input));
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

		public Resource2011[] GetResources(Guid[] resourceIds)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetResources(resourceIds);
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

		public Resource2011[] GetAllResources(string resourceType)
		{
			IUserManager2011 userManager = null;
			bool connectionError = false;
			try
			{
				userManager = GetProxy();
				return userManager.GetAllResources(resourceType);
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
			IUserManager2011 userManager = null;
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
			IUserManager2011 userManager = null;
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
	}
}
