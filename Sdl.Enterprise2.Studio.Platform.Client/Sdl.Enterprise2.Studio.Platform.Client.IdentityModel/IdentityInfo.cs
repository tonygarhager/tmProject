using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.IdentityModel;
using Sdl.Enterprise2.Studio.Platform.Client.Discovery;
using Sdl.Enterprise2.Studio.Platform.Client.Extensions;
using Sdl.Enterprise2.Studio.Platform.Client.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	internal class IdentityInfo : FaultExceptionHandlerBase, IEquatable<IdentityInfo>
	{
		private static readonly byte[] _EncryptKey = new byte[32]
		{
			20,
			148,
			97,
			229,
			149,
			19,
			241,
			129,
			145,
			66,
			29,
			26,
			163,
			222,
			250,
			59,
			94,
			59,
			214,
			115,
			18,
			133,
			69,
			205,
			127,
			163,
			63,
			4,
			90,
			174,
			90,
			43
		};

		private static readonly byte[] _EncryptIv = new byte[16]
		{
			225,
			214,
			143,
			38,
			186,
			44,
			166,
			75,
			113,
			81,
			116,
			184,
			166,
			34,
			71,
			21
		};

		private readonly object synclock = new object();

		private readonly string cacheKey;

		private object securityToken;

		private UserCredentials credentials;

		private CredentialState authenticationStatus;

		private ConnectionState connectionStatus;

		private Exception lastConnectionError;

		private DateTime lastAuthenticated;

		private EndpointAddress tokenManagerAddress;

		private Uri issuedTokenAddress;

		private Permission[] globalPermissions;

		private Dictionary<Guid, Permission[]> resourceGroupPermissions;

		private DateTime? permissionsRetrievedAt;

		public DateTime LastAuthenticated
		{
			get
			{
				return lastAuthenticated;
			}
			private set
			{
				lastAuthenticated = value;
			}
		}

		public UserCredentials Credentials
		{
			get
			{
				return credentials;
			}
			set
			{
				credentials = value;
				AuthenticationStatus = ((credentials != null) ? CredentialState.NotValidated : CredentialState.Unavailable);
			}
		}

		public CredentialState AuthenticationStatus
		{
			get
			{
				return authenticationStatus;
			}
			private set
			{
				authenticationStatus = value;
			}
		}

		public ConnectionState ConnectionStatus
		{
			get
			{
				return connectionStatus;
			}
			private set
			{
				connectionStatus = value;
			}
		}

		public Exception LastConnectionError
		{
			get
			{
				return lastConnectionError;
			}
			private set
			{
				lastConnectionError = value;
			}
		}

		public EndpointAddress TokenManagerAddress
		{
			get
			{
				return tokenManagerAddress;
			}
			set
			{
				tokenManagerAddress = value;
			}
		}

		public Uri IssuedTokenAddress
		{
			get
			{
				if (issuedTokenAddress == null)
				{
					SetBaseAddresses();
				}
				return issuedTokenAddress;
			}
			private set
			{
				issuedTokenAddress = value;
			}
		}

		public bool ArePermissionsUpToDate
		{
			get
			{
				if (permissionsRetrievedAt.HasValue)
				{
					return permissionsRetrievedAt.Value.AddHours(1.0) > DateTime.UtcNow;
				}
				return false;
			}
		}

		public string CacheKey => cacheKey;

		public event EventHandler<CancelEventArgs> AuthenticationFailed;

		public event EventHandler<EventArgs> AuthenticationStatusChanged;

		public event EventHandler<EventArgs> ConnectionStatusChanged;

		public IdentityInfo(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			cacheKey = key;
		}

		private bool IsTokenExpired()
		{
			return (DateTime.UtcNow - LastAuthenticated).TotalMinutes > 55.0;
		}

		public object GetSecurityToken()
		{
			if (IsTokenExpired() || AuthenticationStatus == CredentialState.NotValidated)
			{
				int num = 0;
				while (true)
				{
					CredentialState credentialState = CredentialState.NotValidated;
					try
					{
						bool flag = false;
						lock (synclock)
						{
							if (AuthenticationStatus != CredentialState.Valid || IsTokenExpired())
							{
								credentialState = AuthenticationStatus;
								IssueToken();
								AuthenticationStatus = CredentialState.Valid;
								if (this.AuthenticationStatusChanged != null && credentialState != CredentialState.Valid)
								{
									flag = true;
								}
								LastAuthenticated = DateTime.UtcNow;
								goto IL_007c;
							}
						}
						goto end_IL_0018;
						IL_007c:
						if (flag)
						{
							this.AuthenticationStatusChanged(this, EventArgs.Empty);
						}
						end_IL_0018:;
					}
					catch (SecurityAccessDeniedException)
					{
						AuthenticationStatus = CredentialState.Invalid;
						if (this.AuthenticationStatusChanged != null && credentialState != CredentialState.Invalid)
						{
							this.AuthenticationStatusChanged(this, EventArgs.Empty);
						}
						if (this.AuthenticationFailed == null || num >= 3)
						{
							throw;
						}
						CancelEventArgs cancelEventArgs = new CancelEventArgs();
						this.AuthenticationFailed(this, cancelEventArgs);
						if (cancelEventArgs.Cancel)
						{
							throw;
						}
						num++;
						continue;
					}
					catch (FaultException<ServiceError> ex2)
					{
						throw ProcessServiceException(ex2);
					}
					break;
				}
			}
			return securityToken;
		}

		public void SetAllPermissions(Dictionary<Guid, Permission[]> allPermissions)
		{
			foreach (KeyValuePair<Guid, Permission[]> allPermission in allPermissions)
			{
				if (allPermission.Key.Equals(Guid.Empty))
				{
					SetGlobalPermissions(allPermission.Value);
				}
				else
				{
					SetResourceGroupPermissions(allPermission.Key, allPermission.Value);
				}
			}
			permissionsRetrievedAt = DateTime.UtcNow;
		}

		public IdentityInfo CloneWithNoCredentialInfo()
		{
			return new IdentityInfo(cacheKey)
			{
				TokenManagerAddress = TokenManagerAddress,
				IssuedTokenAddress = issuedTokenAddress,
				ConnectionStatus = ConnectionStatus,
				LastConnectionError = LastConnectionError
			};
		}

		public void SetConnectionError(Exception ex)
		{
			bool flag = false;
			lock (synclock)
			{
				LastConnectionError = ex;
				if (ConnectionStatus != ConnectionState.ConnectionFailed)
				{
					ConnectionStatus = ConnectionState.ConnectionFailed;
					AuthenticationStatus = CredentialState.NotValidated;
					if (this.ConnectionStatusChanged != null)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.ConnectionStatusChanged(this, EventArgs.Empty);
			}
		}

		public void SetConnectionSuccess()
		{
			bool flag = false;
			lock (synclock)
			{
				if (ConnectionStatus != ConnectionState.Connected)
				{
					ConnectionStatus = ConnectionState.Connected;
					LastConnectionError = null;
					if (this.ConnectionStatusChanged != null)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.ConnectionStatusChanged(this, EventArgs.Empty);
			}
		}

		public Permission[] GetAuthorizationData()
		{
			lock (synclock)
			{
				AssertPermissionsUptoDate();
				return globalPermissions ?? new Permission[0];
			}
		}

		public Permission[] GetAuthorizationData(Guid resourceGroupId)
		{
			lock (synclock)
			{
				AssertPermissionsUptoDate();
				return (resourceGroupPermissions != null && resourceGroupPermissions.ContainsKey(resourceGroupId)) ? resourceGroupPermissions[resourceGroupId] : new Permission[0];
			}
		}

		public Permission[] GetAllResourceGroupAuthorizationData()
		{
			lock (synclock)
			{
				AssertPermissionsUptoDate();
				return (resourceGroupPermissions != null) ? resourceGroupPermissions.Values.SelectMany((Permission[] permissions) => permissions.Select()).Distinct().ToArray() : new Permission[0];
			}
		}

		private void IssueToken()
		{
			SetBaseAddresses();
			Binding binding = null;
			if (TokenManagerAddress.Uri.Scheme.Equals(Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase))
			{
				binding = ((Credentials.UserType == UserManagerTokenType.Saml2User) ? new NetTcpBinding(SecurityMode.None) : ((Credentials.UserType != UserManagerTokenType.WindowsUser) ? new NetTcpBinding(SecurityMode.Message) : new NetTcpBinding(SecurityMode.Transport)));
				switch (Credentials.UserType)
				{
				case UserManagerTokenType.WindowsUser:
					((NetTcpBinding)binding).Security.Message.ClientCredentialType = MessageCredentialType.Windows;
					break;
				case UserManagerTokenType.Saml2User:
					((NetTcpBinding)binding).Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
					break;
				default:
					throw new InvalidOperationException(Resources.WindowsOnlyTcp);
				}
			}
			else if (TokenManagerAddress.Uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
			{
				switch (Credentials.UserType)
				{
				case UserManagerTokenType.WindowsUser:
					throw new InvalidOperationException(Resources.WindowsAuthNotHttp);
				case UserManagerTokenType.CustomUser:
				case UserManagerTokenType.CustomWindowsUser:
				case UserManagerTokenType.Saml2User:
					binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
					break;
				}
			}
			else
			{
				if (!TokenManagerAddress.Uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
				{
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedUriScheme, TokenManagerAddress.Uri.Scheme));
				}
				switch (Credentials.UserType)
				{
				case UserManagerTokenType.WindowsUser:
					throw new InvalidOperationException(Resources.WindowsAuthNotHttp);
				case UserManagerTokenType.CustomUser:
				case UserManagerTokenType.CustomWindowsUser:
				case UserManagerTokenType.Saml2User:
					binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
					break;
				}
			}
			ChannelFactory<ITokenManager> channelFactory = new ChannelFactory<ITokenManager>(binding, TokenManagerAddress);
			UserManagerTokenType userType = Credentials.UserType;
			if ((uint)userType <= 1u)
			{
				if (TokenManagerAddress.Uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
				{
					channelFactory.Endpoint.Behaviors.Add(new UserManagerEndpointBehavior());
				}
				else if (TokenManagerAddress.Uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
				{
					channelFactory.Endpoint.Behaviors.Add(new UserManagerEndpointBehavior(AesInfo.Key, AesInfo.IV));
				}
				channelFactory.Credentials.UserName.UserName = Credentials.UserName;
				channelFactory.Credentials.UserName.Password = Credentials.Password;
			}
			if (TokenManagerAddress.Uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) && !channelFactory.Endpoint.Behaviors.Contains(typeof(BasicAuthEndpointBehavior)))
			{
				channelFactory.Endpoint.Behaviors.Add(new BasicAuthEndpointBehavior(Credentials));
			}
			ITokenManager tokenManager = channelFactory.CreateChannel();
			if (Credentials.UserType == UserManagerTokenType.Saml2User)
			{
				string samlToken;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (AesManaged aesManaged = new AesManaged
					{
						Key = _EncryptKey,
						IV = _EncryptIv
					})
					{
						using (CryptoStream stream = new CryptoStream(memoryStream, aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV), CryptoStreamMode.Write))
						{
							using (StreamWriter streamWriter = new StreamWriter(stream))
							{
								byte[] bytes = Convert.FromBase64String(Credentials.SsoCredentials.SamlToken);
								string[] array = Encoding.ASCII.GetString(bytes).Split('=', '&');
								byte[] bytes2 = Convert.FromBase64String(Uri.UnescapeDataString(array[1]));
								streamWriter.Write(Encoding.UTF8.GetString(bytes2));
								aesManaged.Clear();
							}
						}
					}
					samlToken = Convert.ToBase64String(memoryStream.ToArray());
				}
				securityToken = tokenManager.IssueTokenFromSaml(samlToken);
			}
			else
			{
				securityToken = tokenManager.IssueToken();
			}
			try
			{
				channelFactory.Close();
			}
			catch
			{
				try
				{
					channelFactory.Abort();
				}
				catch
				{
				}
			}
		}

		private void SetBaseAddresses()
		{
			if (Credentials == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "No credentials set for server '{0}'.", cacheKey));
			}
			DiscoveryInfo discoveryInfo = new DiscoveryInfo(cacheKey, Credentials.UserType, Credentials);
			Uri identityRouterEndpoint = discoveryInfo.IdentityRouterEndpoint;
			EndpointAddress endpointAddress = (Credentials.UserType == UserManagerTokenType.WindowsUser) ? new EndpointAddress(identityRouterEndpoint, EndpointIdentity.CreateSpnIdentity("identity.sdl.com"), new AddressHeaderCollection()) : ((!identityRouterEndpoint.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)) ? new EndpointAddress(identityRouterEndpoint.AbsoluteUri + "/" + Credentials.UserType.ToString()) : new EndpointAddress(identityRouterEndpoint.AbsoluteUri + Credentials.UserType.ToString()));
			IssuedTokenAddress = discoveryInfo.IssuedTokenRouterEndpoint;
			TokenManagerAddress = endpointAddress;
		}

		private void SetResourceGroupPermissions(Guid resourceGroupId, Permission[] permissions)
		{
			lock (synclock)
			{
				if (resourceGroupPermissions == null)
				{
					resourceGroupPermissions = new Dictionary<Guid, Permission[]>();
				}
				if (resourceGroupPermissions.ContainsKey(resourceGroupId))
				{
					resourceGroupPermissions.Remove(resourceGroupId);
				}
				resourceGroupPermissions.Add(resourceGroupId, permissions);
			}
		}

		private void SetGlobalPermissions(Permission[] permissions)
		{
			lock (synclock)
			{
				globalPermissions = permissions;
			}
		}

		private void AssertPermissionsUptoDate()
		{
			if (!ArePermissionsUpToDate)
			{
				throw new InvalidOperationException("Authorization data must be initialized before calling this method.");
			}
		}

		public static bool Equals(IdentityInfo left, IdentityInfo right)
		{
			if (left == right)
			{
				return true;
			}
			return left?.Equals(right) ?? right.Equals(left);
		}

		public bool Equals(IdentityInfo other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			if (string.Equals(CacheKey, other.CacheKey))
			{
				return UserCredentials.Equals(Credentials, other.Credentials);
			}
			return false;
		}
	}
}
