using Sdl.Enterprise2.MultiTerm.Client.IdentityModel;
using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using Sdl.Enterprise2.Studio.Platform.Client.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	public class IdentityInfoCache
	{
		private const string ns = "http://sdl.com/identity/2010";

		private const string cacheFolderName = "SdlIdentityCredentials";

		private const string ssoCacheFolderName = "SsoIdentityCredentials";

		private static readonly IdentityInfoCache defaultCache = new IdentityInfoCache(isDefault: true);

		private readonly bool isDefaultCache;

		private readonly object syncLock = new object();

		private readonly Dictionary<string, IdentityInfo> identities = new Dictionary<string, IdentityInfo>(1, StringComparer.OrdinalIgnoreCase);

		public static IdentityInfoCache Default => defaultCache;

		public int Count => identities.Count;

		public ReadOnlyCollection<string> IdentityKeys => new ReadOnlyCollection<string>(new List<string>(identities.Keys));

		public string DefaultKey
		{
			get
			{
				lock (syncLock)
				{
					if (identities.Count != 1)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Can't return the default base address because the identity cache has {0} entries.", Count));
					}
					return identities.Keys.First();
				}
			}
		}

		public event EventHandler<AuthenticationFailureEventArgs> AuthenticationFailed;

		public event EventHandler<IdentityInfoEventArgs> ConnectionStatusChanged;

		public event EventHandler<IdentityInfoEventArgs> AuthenticationStatusChanged;

		private IdentityInfoCache(bool isDefault)
		{
			isDefaultCache = isDefault;
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.ConnectionStatusChanged += MtIdentityInfoCacheChanged;
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.AuthenticationFailed += MtIdentityInfoCacheChanged;
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.AuthenticationStatusChanged += MtIdentityInfoCacheChanged;
			LoadAllIdentities();
		}

		public IdentityInfoCache()
		{
		}

		public bool ContainsKey(string key)
		{
			try
			{
				ValidateKey(key);
				return true;
			}
			catch (KeyNotFoundException)
			{
				return false;
			}
		}

		internal IdentityInfo GetIdentityInfo(string key)
		{
			key = ValidateKey(key);
			lock (syncLock)
			{
				return identities[key];
			}
		}

		public CredentialState GetAuthenticationStatus(string key)
		{
			IdentityInfo identityInfo = GetIdentityInfo(key);
			return identityInfo.AuthenticationStatus;
		}

		public void SetWindowsIdentity(Uri baseAddress, PersistOption persistOption = PersistOption.None)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			UserCredentials credentials = new UserCredentials
			{
				UserType = UserManagerTokenType.WindowsUser
			};
			SetIdentity(baseAddress.AbsoluteUri, credentials, persistOption);
			if (persistOption != 0 && isDefaultCache)
			{
				SaveIdentity(baseAddress.AbsoluteUri, persistOption);
			}
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.SetWindowsIdentity(baseAddress, (Sdl.Enterprise2.MultiTerm.Client.IdentityModel.PersistOption)persistOption);
		}

		public void SetWindowsIdentity(Uri baseAddress, string userName, string password, PersistOption persistOption = PersistOption.None)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}
			UserCredentials credentials = new UserCredentials
			{
				UserType = UserManagerTokenType.CustomWindowsUser,
				UserName = userName,
				Password = password
			};
			SetIdentity(baseAddress.AbsoluteUri, credentials, persistOption);
			if (persistOption != 0 && isDefaultCache)
			{
				SaveIdentity(baseAddress.AbsoluteUri, persistOption);
			}
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.SetWindowsIdentity(baseAddress, userName, password, (Sdl.Enterprise2.MultiTerm.Client.IdentityModel.PersistOption)persistOption);
		}

		public void SetCustomIdentity(Uri baseAddress, string userName, UserCredentials.SsoData ssoCredentials, PersistOption persistOption = PersistOption.None)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			UserCredentials credentials = new UserCredentials
			{
				UserName = userName,
				UserType = UserManagerTokenType.Saml2User,
				SsoCredentials = ssoCredentials
			};
			SetIdentity(baseAddress.AbsoluteUri, credentials, persistOption);
			if (persistOption != 0 && isDefaultCache)
			{
				SaveIdentity(baseAddress.AbsoluteUri, persistOption);
				SaveSsoIdentity(baseAddress.AbsoluteUri, persistOption);
			}
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.SetCustomIdentity(baseAddress, userName, new Sdl.Enterprise2.MultiTerm.Client.IdentityModel.UserCredentials.SsoData
			{
				AuthToken = ssoCredentials.AuthToken,
				ExpirationDate = ssoCredentials.ExpirationDate,
				SamlToken = ssoCredentials.SamlToken
			}, (Sdl.Enterprise2.MultiTerm.Client.IdentityModel.PersistOption)persistOption);
		}

		public void SetCustomIdentity(Uri baseAddress, string userName, string password, PersistOption persistOption = PersistOption.None)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.SetCustomIdentity(baseAddress, userName, password, (Sdl.Enterprise2.MultiTerm.Client.IdentityModel.PersistOption)persistOption);
			UserCredentials credentials = new UserCredentials
			{
				UserType = UserManagerTokenType.CustomUser,
				UserName = userName,
				Password = password
			};
			SetIdentity(baseAddress.AbsoluteUri, credentials, persistOption);
			if (persistOption != 0 && isDefaultCache)
			{
				SaveIdentity(baseAddress.AbsoluteUri, persistOption);
			}
		}

		public UserCredentials GetUserCredentials(string key)
		{
			IdentityInfo identityInfo = GetIdentityInfo(key);
			return identityInfo.Credentials;
		}

		public static string GetIdentityKey(Uri baseAddress)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (baseAddress.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
			{
				return baseAddress.AbsoluteUri;
			}
			return baseAddress.AbsoluteUri + "/";
		}

		public ConnectionInfo GetConnectionInfo(string key)
		{
			IdentityInfo identityInfo = GetIdentityInfo(key);
			return new ConnectionInfo
			{
				Credentials = identityInfo.Credentials,
				AuthenticationStatus = identityInfo.AuthenticationStatus,
				LastError = identityInfo.LastConnectionError,
				ConnectionStatus = identityInfo.ConnectionStatus
			};
		}

		public Uri GetBaseAddress(string key)
		{
			key = ValidateKey(key);
			return new Uri(key);
		}

		public bool RemoveIdentity(string key)
		{
			key = ValidateKey(key);
			if (isDefaultCache)
			{
				DeleteIdentity(key);
			}
			if (Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.ContainsKey(key))
			{
				Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.RemoveIdentity(key);
			}
			return Remove(key);
		}

		public bool RemoveUserCredentials(string key)
		{
			IdentityInfo identityInfo = GetIdentityInfo(key);
			IdentityInfo value = identityInfo.CloneWithNoCredentialInfo();
			if (isDefaultCache)
			{
				DeleteCredentials(key);
			}
			lock (syncLock)
			{
				Remove(key);
				Add(key, value);
			}
			if (Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.ContainsKey(key))
			{
				Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.RemoveUserCredentials(key);
			}
			return true;
		}

		public void ClearAllIdentities()
		{
			if (isDefaultCache)
			{
				DeleteAllIdentities();
			}
			string[] array = identities.Keys.ToArray();
			string[] array2 = array;
			foreach (string key in array2)
			{
				Remove(key);
			}
			Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoCache.Default.ClearAllIdentities();
		}

		public bool IsPersisted(string key)
		{
			key = ValidateKey(key);
			string path = Path.Combine(GetPath("SdlIdentityCredentials"), GenerateFileName(key));
			if (File.Exists(path))
			{
				return true;
			}
			return false;
		}

		public bool IsUserCredentialPersisted(string key)
		{
			key = ValidateKey(key);
			string fileName = Path.Combine(GetPath("SdlIdentityCredentials"), GenerateFileName(key));
			if (IsUserCredentialPersistedInFile(fileName))
			{
				return true;
			}
			string fileName2 = Path.Combine(GetPath("SsoIdentityCredentials"), GenerateFileName(key));
			if (IsUserCredentialPersistedInFile(fileName2))
			{
				return true;
			}
			return false;
		}

		private bool IsUserCredentialPersistedInFile(string fileName)
		{
			if (!File.Exists(fileName))
			{
				return false;
			}
			bool result = false;
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (Aes aes = new AesManaged())
				{
					using (CryptoStream input = new CryptoStream(stream, aes.CreateDecryptor(AesInfo.Key, AesInfo.IV), CryptoStreamMode.Read))
					{
						using (XmlTextReader xmlTextReader = new XmlTextReader(input))
						{
							while (xmlTextReader.Read())
							{
								XmlNodeType nodeType = xmlTextReader.NodeType;
								if (nodeType == XmlNodeType.Element && xmlTextReader.IsStartElement("UserType", "http://sdl.com/identity/2010"))
								{
									result = true;
								}
							}
							return result;
						}
					}
				}
			}
		}

		public bool HasPermissionInAnyResourceGroup(string permission)
		{
			ValidatePermissionFormat(permission, global: false);
			return CheckPermission(DefaultKey, null, null, permission, allResourceGroups: true, firstException: false);
		}

		public bool HasPermission(Guid resourceGroupId, string permission)
		{
			ValidatePermissionFormat(permission, global: false);
			return CheckPermission(DefaultKey, resourceGroupId, null, permission, allResourceGroups: false, firstException: false);
		}

		public bool HasPermission(Guid[] resourceGroupIds, string permission)
		{
			ValidatePermissionFormat(permission, global: false);
			return CheckPermission(DefaultKey, null, resourceGroupIds, permission, allResourceGroups: false, firstException: false);
		}

		public bool HasPermission(string permission)
		{
			ValidatePermissionFormat(permission, global: true);
			return CheckPermission(DefaultKey, null, null, permission, allResourceGroups: false, firstException: false);
		}

		public bool HasPermissionInAnyResourceGroup(string key, string permission)
		{
			ValidatePermissionFormat(permission, global: false);
			return CheckPermission(key, null, null, permission, allResourceGroups: true, firstException: false);
		}

		public bool HasPermission(string key, Guid resourceGroupId, string permission)
		{
			ValidatePermissionFormat(permission, global: false);
			return CheckPermission(key, resourceGroupId, null, permission, allResourceGroups: false, firstException: false);
		}

		public bool HasPermission(string key, Guid[] resourceGroupIds, string permission)
		{
			ValidatePermissionFormat(permission, global: false);
			return CheckPermission(key, null, resourceGroupIds, permission, allResourceGroups: false, firstException: false);
		}

		public bool HasPermission(string key, string permission)
		{
			ValidatePermissionFormat(permission, global: true);
			return CheckPermission(key, null, null, permission, allResourceGroups: false, firstException: false);
		}

		private bool CheckPermission(string key, Guid? resourceGroupId, IEnumerable<Guid> resourceGroupIds, string permission, bool allResourceGroups, bool firstException)
		{
			if (string.IsNullOrEmpty(permission))
			{
				throw new ArgumentNullException("permission");
			}
			IdentityInfo identityInfo = GetIdentityInfo(key);
			bool flag = false;
			try
			{
				Permission[] allResourceGroupAuthorizationData;
				if (allResourceGroups)
				{
					allResourceGroupAuthorizationData = identityInfo.GetAllResourceGroupAuthorizationData();
					return allResourceGroupAuthorizationData.Any((Permission item) => item.Name.Equals(permission, StringComparison.OrdinalIgnoreCase));
				}
				if (resourceGroupIds != null)
				{
					return CheckPermissionForMultipleResourceGroups(identityInfo, resourceGroupIds, permission);
				}
				allResourceGroupAuthorizationData = (resourceGroupId.HasValue ? identityInfo.GetAuthorizationData(resourceGroupId.Value) : identityInfo.GetAuthorizationData());
				return allResourceGroupAuthorizationData.Any((Permission item) => item.Name.Equals(permission, StringComparison.OrdinalIgnoreCase));
			}
			catch (InvalidOperationException)
			{
				if (identityInfo.ArePermissionsUpToDate || firstException)
				{
					throw;
				}
				AuthorizationHelper authorizationHelper = new AuthorizationHelper(identityInfo);
				authorizationHelper.GetAllAuthorizationData();
				return CheckPermission(key, resourceGroupId, resourceGroupIds, permission, allResourceGroups, firstException: true);
			}
		}

		private static bool CheckPermissionForMultipleResourceGroups(IdentityInfo idInfo, IEnumerable<Guid> resourceGroupIds, string permission)
		{
			return (from permissions in resourceGroupIds.Select(idInfo.GetAuthorizationData)
				select permissions.FirstOrDefault((Permission item) => item.Name.Equals(permission, StringComparison.OrdinalIgnoreCase))).Any((Permission p) => p != null);
		}

		private static void ValidatePermissionFormat(string permission, bool global)
		{
			if (string.IsNullOrEmpty(permission))
			{
				throw new ArgumentNullException("permission");
			}
			if (global)
			{
				if (permission.Split('.').Length != 1)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidGlobalPermissionFormat, permission));
				}
			}
			else if (permission.Split('.').Length != 2)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidResourcePermissionFormat, permission));
			}
		}

		private string ValidateKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (!key.EndsWith("/", StringComparison.Ordinal))
			{
				key += "/";
			}
			if (!identities.ContainsKey(key))
			{
				throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, "No identity information found for server '{0}'.", key));
			}
			return key;
		}

		private void SetIdentity(string key, UserCredentials credentials, PersistOption persistOption)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (!key.EndsWith("/", StringComparison.Ordinal))
			{
				key += "/";
			}
			IdentityInfo identityInfo = new IdentityInfo(key)
			{
				Credentials = credentials
			};
			lock (syncLock)
			{
				IdentityInfo value = null;
				if (identities.TryGetValue(key, out value))
				{
					if (value.Equals(identityInfo))
					{
						return;
					}
					value.AuthenticationFailed -= OnAuthenticationFailed;
					value.AuthenticationStatusChanged -= OnAuthenticationStatusChanged;
					value.ConnectionStatusChanged -= OnConnectionStatusChanged;
					identities.Remove(key);
				}
				identityInfo.AuthenticationFailed += OnAuthenticationFailed;
				identityInfo.AuthenticationStatusChanged += OnAuthenticationStatusChanged;
				identityInfo.ConnectionStatusChanged += OnConnectionStatusChanged;
				identities.Add(key, identityInfo);
			}
			if (this.AuthenticationStatusChanged != null)
			{
				this.AuthenticationStatusChanged(this, new IdentityInfoEventArgs(key));
			}
		}

		private void Add(string key, IdentityInfo value)
		{
			lock (syncLock)
			{
				if (!identities.ContainsKey(key))
				{
					value.AuthenticationFailed += OnAuthenticationFailed;
					value.AuthenticationStatusChanged += OnAuthenticationStatusChanged;
					value.ConnectionStatusChanged += OnConnectionStatusChanged;
					identities.Add(key, value);
					if (this.AuthenticationStatusChanged != null)
					{
						this.AuthenticationStatusChanged(this, new IdentityInfoEventArgs(key));
					}
				}
			}
		}

		private bool Remove(string key)
		{
			lock (syncLock)
			{
				if (identities.TryGetValue(key, out IdentityInfo value))
				{
					value.AuthenticationFailed -= OnAuthenticationFailed;
					value.AuthenticationStatusChanged -= OnAuthenticationStatusChanged;
					value.ConnectionStatusChanged -= OnConnectionStatusChanged;
					identities.Remove(key);
					return true;
				}
			}
			return false;
		}

		private void OnAuthenticationStatusChanged(object sender, EventArgs e)
		{
			if (this.AuthenticationStatusChanged != null)
			{
				IdentityInfo identityInfo = sender as IdentityInfo;
				if (identityInfo != null)
				{
					this.AuthenticationStatusChanged(this, new IdentityInfoEventArgs(identityInfo.CacheKey));
				}
			}
		}

		private void OnConnectionStatusChanged(object sender, EventArgs e)
		{
			if (this.ConnectionStatusChanged != null)
			{
				IdentityInfo identityInfo = sender as IdentityInfo;
				if (identityInfo != null)
				{
					this.ConnectionStatusChanged(this, new IdentityInfoEventArgs(identityInfo.CacheKey));
				}
			}
		}

		private void OnAuthenticationFailed(object sender, CancelEventArgs e)
		{
			if (this.AuthenticationFailed == null)
			{
				return;
			}
			IdentityInfo identityInfo = sender as IdentityInfo;
			if (identityInfo != null)
			{
				AuthenticationFailureEventArgs authenticationFailureEventArgs = new AuthenticationFailureEventArgs(identityInfo.CacheKey);
				this.AuthenticationFailed(this, authenticationFailureEventArgs);
				if (authenticationFailureEventArgs.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		private void SaveIdentity(string key, PersistOption persistOption)
		{
			string path = GetPath("SdlIdentityCredentials");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			using (FileStream stream = new FileStream(Path.Combine(path, GenerateFileName(key)), FileMode.Create))
			{
				using (Aes aes = new AesManaged())
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, aes.CreateEncryptor(AesInfo.Key, AesInfo.IV), CryptoStreamMode.Write))
					{
						using (XmlWriter writer = XmlWriter.Create(cryptoStream))
						{
							WriteIdentity(writer, key, persistOption);
						}
						cryptoStream.FlushFinalBlock();
					}
				}
			}
		}

		private void SaveSsoIdentity(string key, PersistOption persistOption)
		{
			string path = GetPath("SsoIdentityCredentials");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			using (FileStream stream = new FileStream(Path.Combine(path, GenerateFileName(key)), FileMode.Create))
			{
				using (Aes aes = new AesManaged())
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, aes.CreateEncryptor(AesInfo.Key, AesInfo.IV), CryptoStreamMode.Write))
					{
						using (XmlWriter writer = XmlWriter.Create(cryptoStream))
						{
							WriteSsoIdentity(writer, key, persistOption);
						}
						cryptoStream.FlushFinalBlock();
					}
				}
			}
		}

		private void LoadIdentities()
		{
			string path = GetPath("SdlIdentityCredentials");
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*");
				foreach (string path2 in files)
				{
					using (FileStream stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
					{
						using (Aes aes = new AesManaged())
						{
							using (CryptoStream input = new CryptoStream(stream, aes.CreateDecryptor(AesInfo.Key, AesInfo.IV), CryptoStreamMode.Read))
							{
								using (XmlTextReader reader = new XmlTextReader(input))
								{
									ReadIdentity(reader);
								}
							}
						}
					}
				}
			}
		}

		private void LoadSsoIdentities()
		{
			string path = GetPath("SsoIdentityCredentials");
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*");
				foreach (string path2 in files)
				{
					using (FileStream stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
					{
						using (Aes aes = new AesManaged())
						{
							using (CryptoStream input = new CryptoStream(stream, aes.CreateDecryptor(AesInfo.Key, AesInfo.IV), CryptoStreamMode.Read))
							{
								using (XmlTextReader reader = new XmlTextReader(input))
								{
									ReadSsoIdentity(reader);
								}
							}
						}
					}
				}
			}
		}

		private void DeleteCredentials(string key)
		{
			if (!key.EndsWith("/", StringComparison.Ordinal))
			{
				key += "/";
			}
			DeleteIdentity(key);
			SaveIdentity(key, PersistOption.ServerOnly);
		}

		private void DeleteIdentity(string key)
		{
			DeleteIdentityByFolder("SdlIdentityCredentials", key);
			DeleteIdentityByFolder("SsoIdentityCredentials", key);
		}

		private void DeleteIdentityByFolder(string folderName, string key)
		{
			string path = GetPath(folderName);
			if (Directory.Exists(path))
			{
				string path2 = path + GenerateFileName(key);
				if (File.Exists(path2))
				{
					File.Delete(path2);
				}
			}
		}

		private static void DeleteAllIdentities()
		{
			DeleteAllIdentitiesByFolder("SdlIdentityCredentials");
			DeleteAllIdentitiesByFolder("SsoIdentityCredentials");
		}

		private static void DeleteAllIdentitiesByFolder(string folderName)
		{
			string path = GetPath(folderName);
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*");
				foreach (string path2 in files)
				{
					File.Delete(path2);
				}
			}
		}

		private void LoadAllIdentities()
		{
			LoadIdentities();
			LoadSsoIdentities();
		}

		private void MtIdentityInfoCacheChanged(object sender, Sdl.Enterprise2.MultiTerm.Client.IdentityModel.IdentityInfoEventArgs e)
		{
			LoadAllIdentities();
		}

		private void MtIdentityInfoCacheChanged(object sender, Sdl.Enterprise2.MultiTerm.Client.IdentityModel.AuthenticationFailureEventArgs e)
		{
			LoadAllIdentities();
		}

		private static string GetPath(string cacheFolder)
		{
			string text = AppDomain.CurrentDomain.FriendlyName;
			int num = text.IndexOf(": ", StringComparison.Ordinal);
			if (num != -1)
			{
				text = text.Remove(0, num + 2);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}\\Sdl\\Platform\\Identity\\{1}\\{2}\\", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), text, cacheFolder);
		}

		private void WriteIdentity(XmlWriter writer, string key, PersistOption persistOption)
		{
			IdentityInfo identityInfo = GetIdentityInfo(key);
			writer.WriteStartDocument();
			writer.WriteStartElement("t", "Identity", "http://sdl.com/identity/2010");
			writer.WriteAttributeString("address", "http://sdl.com/identity/2010", key);
			if (identityInfo.Credentials != null && identityInfo.Credentials.UserType != UserManagerTokenType.Saml2User && persistOption == PersistOption.ServerAndUserCredentials)
			{
				writer.WriteStartElement("t", "Credentials", "http://sdl.com/identity/2010");
				writer.WriteElementString("UserType", "http://sdl.com/identity/2010", identityInfo.Credentials.UserType.ToString());
				if (!string.IsNullOrEmpty(identityInfo.Credentials.UserName))
				{
					writer.WriteElementString("UserName", "http://sdl.com/identity/2010", identityInfo.Credentials.UserName);
				}
				if (!string.IsNullOrEmpty(identityInfo.Credentials.Password))
				{
					writer.WriteElementString("Password", "http://sdl.com/identity/2010", identityInfo.Credentials.Password);
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private void WriteSsoIdentity(XmlWriter writer, string key, PersistOption persistOption)
		{
			IdentityInfo identityInfo = GetIdentityInfo(key);
			writer.WriteStartDocument();
			writer.WriteStartElement("t", "Identity", "http://sdl.com/identity/2010");
			writer.WriteAttributeString("address", "http://sdl.com/identity/2010", key);
			if (identityInfo.Credentials != null && persistOption == PersistOption.ServerAndUserCredentials)
			{
				writer.WriteStartElement("t", "Credentials", "http://sdl.com/identity/2010");
				writer.WriteElementString("UserType", "http://sdl.com/identity/2010", identityInfo.Credentials.UserType.ToString());
				if (!string.IsNullOrEmpty(identityInfo.Credentials.UserName))
				{
					writer.WriteElementString("UserName", "http://sdl.com/identity/2010", identityInfo.Credentials.UserName);
				}
				if (identityInfo.Credentials.SsoCredentials != null)
				{
					if (!string.IsNullOrEmpty(identityInfo.Credentials.SsoCredentials.AuthToken))
					{
						writer.WriteElementString("AuthToken", "http://sdl.com/identity/2010", identityInfo.Credentials.SsoCredentials.AuthToken);
					}
					if (!string.IsNullOrEmpty(identityInfo.Credentials.SsoCredentials.SamlToken))
					{
						writer.WriteElementString("SamlToken", "http://sdl.com/identity/2010", identityInfo.Credentials.SsoCredentials.SamlToken);
					}
					if (identityInfo.Credentials.SsoCredentials.ExpirationDate > DateTime.UtcNow)
					{
						writer.WriteElementString("Expiration", "http://sdl.com/identity/2010", identityInfo.Credentials.SsoCredentials.ExpirationDate.ToString(CultureInfo.CurrentCulture));
					}
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private void ReadIdentity(XmlReader reader)
		{
			UserManagerTokenType userType = UserManagerTokenType.CustomUser;
			UserCredentials userCredentials = null;
			bool flag = false;
			string text = string.Empty;
			string text2 = string.Empty;
			string key = string.Empty;
			while (reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					if (reader.IsStartElement("Identity", "http://sdl.com/identity/2010"))
					{
						key = reader.GetAttribute("address", "http://sdl.com/identity/2010");
					}
					else if (reader.IsStartElement("UserType", "http://sdl.com/identity/2010"))
					{
						flag = true;
						userType = (UserManagerTokenType)Enum.Parse(typeof(UserManagerTokenType), reader.ReadElementContentAsString(), ignoreCase: true);
					}
					if (reader.IsStartElement("UserName", "http://sdl.com/identity/2010"))
					{
						text = reader.ReadElementContentAsString();
					}
					if (reader.IsStartElement("Password", "http://sdl.com/identity/2010"))
					{
						text2 = reader.ReadElementContentAsString();
					}
				}
			}
			if (flag)
			{
				userCredentials = new UserCredentials
				{
					UserType = userType
				};
				if (!string.IsNullOrEmpty(text))
				{
					userCredentials.UserName = text;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					userCredentials.Password = text2;
				}
			}
			SetIdentity(key, userCredentials, flag ? PersistOption.ServerAndUserCredentials : PersistOption.None);
		}

		private void ReadSsoIdentity(XmlReader reader)
		{
			UserManagerTokenType userType = UserManagerTokenType.CustomUser;
			UserCredentials userCredentials = null;
			bool flag = false;
			string text = string.Empty;
			string text2 = string.Empty;
			string authToken = string.Empty;
			string samlToken = string.Empty;
			DateTime expirationDate = DateTime.UtcNow;
			while (reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					if (reader.IsStartElement("Identity", "http://sdl.com/identity/2010"))
					{
						text2 = reader.GetAttribute("address", "http://sdl.com/identity/2010");
					}
					else if (reader.IsStartElement("UserType", "http://sdl.com/identity/2010"))
					{
						flag = true;
						userType = (UserManagerTokenType)Enum.Parse(typeof(UserManagerTokenType), reader.ReadElementContentAsString(), ignoreCase: true);
					}
					if (reader.IsStartElement("UserName", "http://sdl.com/identity/2010"))
					{
						text = reader.ReadElementContentAsString();
					}
					if (reader.IsStartElement("AuthToken", "http://sdl.com/identity/2010"))
					{
						authToken = reader.ReadElementContentAsString();
					}
					if (reader.IsStartElement("SamlToken", "http://sdl.com/identity/2010"))
					{
						samlToken = reader.ReadElementContentAsString();
					}
					if (reader.IsStartElement("Expiration", "http://sdl.com/identity/2010"))
					{
						expirationDate = Convert.ToDateTime(reader.ReadElementContentAsString());
					}
				}
			}
			if (flag)
			{
				lock (syncLock)
				{
					if (!string.IsNullOrEmpty(text2) && !identities.ContainsKey(text2))
					{
						return;
					}
				}
				userCredentials = GetUserCredentials(text2);
				if (userCredentials != null)
				{
					return;
				}
				userCredentials = new UserCredentials
				{
					UserType = userType,
					SsoCredentials = new UserCredentials.SsoData
					{
						AuthToken = authToken,
						SamlToken = samlToken,
						ExpirationDate = expirationDate
					}
				};
				if (!string.IsNullOrEmpty(text))
				{
					userCredentials.UserName = text;
				}
			}
			SetIdentity(text2, userCredentials, flag ? PersistOption.ServerAndUserCredentials : PersistOption.None);
		}

		private string GenerateFileName(string key)
		{
			return GetBaseAddress(key).DnsSafeHost;
		}
	}
}
