using Sdl.Enterprise2.MultiTerm.Client.Discovery;
using Sdl.Enterprise2.Platform.Contracts.Discovery;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	internal class UserManagerClientFactory
	{
		private enum UserManagerVersion
		{
			UserManager2011,
			UserManager2012
		}

		private static ConcurrentDictionary<string, UserManagerVersion> cachedServerVersions = new ConcurrentDictionary<string, UserManagerVersion>();

		internal static IUserManagerClient Create()
		{
			return new UserManager2012Client();
		}

		internal static IUserManagerClient Create(string baseAddress)
		{
			if (GetUserManagerVersion(baseAddress) == UserManagerVersion.UserManager2011)
			{
				return new UserManager2011Client(baseAddress);
			}
			return new UserManager2012Client(baseAddress);
		}

		internal static IUserManagerClient Create(string baseAddress, UserCredentials credentials)
		{
			if (GetUserManagerVersion(baseAddress, credentials) == UserManagerVersion.UserManager2011)
			{
				return new UserManager2011Client(baseAddress, credentials);
			}
			return new UserManager2012Client(baseAddress, credentials);
		}

		internal static IUserManagerClient Create(string baseAddress, UserCredentials credentials, bool useCompression)
		{
			if (GetUserManagerVersion(baseAddress, credentials) == UserManagerVersion.UserManager2011)
			{
				return new UserManager2011Client(baseAddress, credentials, useCompression);
			}
			return new UserManager2012Client(baseAddress, credentials, useCompression);
		}

		internal static IUserManagerClient Create(IdentityInfoCache identityCache)
		{
			if (GetUserManagerVersion(identityCache) == UserManagerVersion.UserManager2011)
			{
				return new UserManager2011Client(identityCache);
			}
			return new UserManager2012Client(identityCache);
		}

		internal static IUserManagerClient Create(IdentityInfoCache identityCache, string baseAddress)
		{
			if (GetUserManagerVersion(baseAddress, identityCache) == UserManagerVersion.UserManager2011)
			{
				return new UserManager2011Client(identityCache, baseAddress);
			}
			return new UserManager2012Client(identityCache, baseAddress);
		}

		internal static IUserManagerClient Create(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
		{
			if (GetUserManagerVersion(baseAddress, identityCache) == UserManagerVersion.UserManager2011)
			{
				return new UserManager2011Client(identityCache, baseAddress, credentials);
			}
			return new UserManager2012Client(identityCache, baseAddress, credentials);
		}

		internal static IUserManagerClient Create(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
		{
			if (GetUserManagerVersion(baseAddress, identityCache) == UserManagerVersion.UserManager2011)
			{
				return new UserManager2011Client(identityCache, baseAddress, credentials, useCompression);
			}
			return new UserManager2012Client(identityCache, baseAddress, credentials, useCompression);
		}

		private static UserManagerVersion GetUserManagerVersion(string baseAddress)
		{
			if (cachedServerVersions.TryGetValue(baseAddress, out UserManagerVersion value))
			{
				return value;
			}
			value = UserManagerVersion.UserManager2012;
			using (DiscoveryServiceClient discoveryServiceClient = new DiscoveryServiceClient(baseAddress))
			{
				if (!discoveryServiceClient.GetServiceEndpoints().Any((ServiceEndpointInfo s) => s.Address.AbsolutePath.Contains("/UserManager2012Service")))
				{
					value = UserManagerVersion.UserManager2011;
				}
			}
			AddVersionToCache(baseAddress, value);
			return value;
		}

		private static UserManagerVersion GetUserManagerVersion(string baseAddress, UserCredentials credentials)
		{
			if (cachedServerVersions.TryGetValue(baseAddress, out UserManagerVersion value))
			{
				return value;
			}
			value = UserManagerVersion.UserManager2012;
			SetCacheCredentials(baseAddress, credentials);
			using (DiscoveryServiceClient discoveryServiceClient = new DiscoveryServiceClient(baseAddress))
			{
				if (!discoveryServiceClient.GetServiceEndpoints().Any((ServiceEndpointInfo s) => s.Address.AbsolutePath.Contains("/UserManager2012Service")))
				{
					value = UserManagerVersion.UserManager2011;
				}
			}
			AddVersionToCache(baseAddress, value);
			return value;
		}

		private static UserManagerVersion GetUserManagerVersion(IdentityInfoCache identityCache)
		{
			string text = identityCache.GetBaseAddress(identityCache.DefaultKey).ToString();
			if (cachedServerVersions.TryGetValue(text, out UserManagerVersion value))
			{
				return value;
			}
			value = UserManagerVersion.UserManager2012;
			using (DiscoveryServiceClient discoveryServiceClient = new DiscoveryServiceClient(identityCache))
			{
				if (!discoveryServiceClient.GetServiceEndpoints().Any((ServiceEndpointInfo s) => s.Address.AbsolutePath.Contains("/UserManager2012Service")))
				{
					value = UserManagerVersion.UserManager2011;
				}
			}
			AddVersionToCache(text, value);
			return value;
		}

		private static UserManagerVersion GetUserManagerVersion(string baseAddress, IdentityInfoCache identityCache)
		{
			if (cachedServerVersions.TryGetValue(baseAddress, out UserManagerVersion value))
			{
				return value;
			}
			value = UserManagerVersion.UserManager2012;
			using (DiscoveryServiceClient discoveryServiceClient = new DiscoveryServiceClient(baseAddress, identityCache))
			{
				if (!discoveryServiceClient.GetServiceEndpoints().Any((ServiceEndpointInfo s) => s.Address.AbsolutePath.Contains("/UserManager2012Service")))
				{
					value = UserManagerVersion.UserManager2011;
				}
			}
			AddVersionToCache(baseAddress, value);
			return value;
		}

		private static void AddVersionToCache(string baseAddress, UserManagerVersion version)
		{
			cachedServerVersions[baseAddress] = version;
		}

		private static void SetCacheCredentials(string baseAddress, UserCredentials credentials)
		{
			Uri baseAddress2 = new Uri(baseAddress);
			IdentityInfoCache @default = IdentityInfoCache.Default;
			switch (credentials.UserType)
			{
			case UserManagerTokenType.CustomUser:
				@default.SetCustomIdentity(baseAddress2, credentials.UserName, credentials.Password);
				break;
			case UserManagerTokenType.WindowsUser:
				@default.SetWindowsIdentity(baseAddress2);
				break;
			case UserManagerTokenType.CustomWindowsUser:
				@default.SetWindowsIdentity(baseAddress2, credentials.UserName, credentials.Password);
				break;
			case UserManagerTokenType.Saml2User:
				@default.SetCustomIdentity(baseAddress2, credentials.UserName, credentials.SsoCredentials);
				break;
			}
		}
	}
}
