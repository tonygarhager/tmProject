using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	internal class AuthorizationHelper
	{
		private class PermissionComparer : IEqualityComparer<Permission>
		{
			public bool Equals(Permission x, Permission y)
			{
				return x.UniqueId == y.UniqueId;
			}

			public int GetHashCode(Permission obj)
			{
				return obj.UniqueId.GetHashCode();
			}
		}

		private readonly IdentityInfo _identityInfo;

		public AuthorizationHelper(IdentityInfo idInfo)
		{
			if (idInfo == null)
			{
				throw new ArgumentNullException("idInfo");
			}
			_identityInfo = idInfo;
		}

		private static void ProcessResourceGroups(ICollection<Guid> resourceGroupIds, IEnumerable<ResourceGroup> resourceGroups)
		{
			foreach (ResourceGroup resourceGroup in resourceGroups)
			{
				resourceGroupIds.Add(resourceGroup.UniqueId);
				ProcessResourceGroups(resourceGroupIds, resourceGroup.ChildResourceGroups);
			}
		}

		private Dictionary<Guid, Permission[]> CalculateAllPermissions(MembershipEntry[] entries, ResourceGroup[] orgs, Role[] roles)
		{
			Dictionary<Guid, Permission[]> dictionary = new Dictionary<Guid, Permission[]>();
			List<Permission> list = new List<Permission>();
			List<MembershipEntry> allMembershipEntries = GetAllMembershipEntries(entries, orgs);
			foreach (IGrouping<Guid, MembershipEntry> item in from e in allMembershipEntries
				group e by e.ResourceGroupId)
			{
				List<Permission> list2 = new List<Permission>();
				foreach (MembershipEntry entry in item)
				{
					Permission[] permissions = roles.First((Role r) => r.UniqueId == entry.RoleId).Permissions;
					list2.AddRange(permissions);
				}
				IEnumerable<Permission> source = list2.Distinct(new PermissionComparer());
				list.AddRange(source.Where((Permission p) => string.IsNullOrEmpty(GetResourceType(p))));
				dictionary[item.Key] = source.Where((Permission p) => !string.IsNullOrEmpty(GetResourceType(p))).ToArray();
			}
			dictionary[Guid.Empty] = list.Distinct(new PermissionComparer()).ToArray();
			return dictionary;
		}

		private static string GetResourceType(Permission p)
		{
			string[] array = p.Name.Split('.');
			if (array.Length != 2)
			{
				return string.Empty;
			}
			return array[0].ToUpperInvariant();
		}

		private List<MembershipEntry> GetAllMembershipEntries(MembershipEntry[] entries, ResourceGroup[] orgs)
		{
			List<MembershipEntry> list = new List<MembershipEntry>();
			foreach (MembershipEntry membershipEntry in entries)
			{
				ResourceGroup rg = FindResourceGroup(membershipEntry.ResourceGroupId, orgs);
				AddAllEntries(rg, membershipEntry, list);
			}
			return list;
		}

		private ResourceGroup FindResourceGroup(Guid id, ResourceGroup[] hierarchy)
		{
			foreach (ResourceGroup resourceGroup in hierarchy)
			{
				if (resourceGroup.UniqueId == id)
				{
					return resourceGroup;
				}
				ResourceGroup resourceGroup2 = FindResourceGroup(id, resourceGroup.ChildResourceGroups);
				if (resourceGroup2 != null)
				{
					return resourceGroup2;
				}
			}
			return null;
		}

		private void AddAllEntries(ResourceGroup rg, MembershipEntry entry, List<MembershipEntry> allEntries)
		{
			allEntries.Add(entry);
			ResourceGroup[] childResourceGroups = rg.ChildResourceGroups;
			foreach (ResourceGroup resourceGroup in childResourceGroups)
			{
				MembershipEntry entry2 = new MembershipEntry
				{
					ResourceGroupId = resourceGroup.UniqueId,
					RoleId = entry.RoleId,
					UserId = entry.UserId
				};
				AddAllEntries(resourceGroup, entry2, allEntries);
			}
		}

		private IUserManagerClient CreateUserManagerClient()
		{
			string cacheKey = _identityInfo.CacheKey;
			return UserManagerClientFactory.Create(cacheKey);
		}

		public void GetAllAuthorizationData()
		{
			IUserManagerClient userManagerClient = CreateUserManagerClient();
			User userByUserName = userManagerClient.GetUserByUserName(GetCurrentUser());
			Role[] roles = userManagerClient.GetRoles();
			ResourceGroup[] resourceGroupHierarchy = userManagerClient.GetResourceGroupHierarchy();
			MembershipEntry[] userMembershipEntries = userManagerClient.GetUserMembershipEntries(userByUserName.UniqueId);
			Dictionary<Guid, Permission[]> allPermissions = CalculateAllPermissions(userMembershipEntries, resourceGroupHierarchy, roles);
			_identityInfo.SetAllPermissions(allPermissions);
		}

		private string GetCurrentUser()
		{
			if (_identityInfo.Credentials.UserType == UserManagerTokenType.WindowsUser)
			{
				return WindowsIdentity.GetCurrent()?.Name;
			}
			return _identityInfo.Credentials.UserName;
		}

		public List<Guid> GetResourceGroupIds()
		{
			IUserManagerClient userManagerClient = CreateUserManagerClient();
			ResourceGroup[] resourceGroups = Array.ConvertAll(userManagerClient.GetResourceGroupHierarchy(), (ResourceGroup input) => input);
			List<Guid> list = new List<Guid>();
			ProcessResourceGroups(list, resourceGroups);
			return list;
		}
	}
}
