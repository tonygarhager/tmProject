using Sdl.Enterprise2.Studio.Platform.Client.Communication;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	public abstract class IssuedTokenClientBase<T> : FaultExceptionHandlerBase, IDisposable where T : class
	{
		protected readonly IdentityInfoCache identityCache;

		private readonly string servicePath;

		protected readonly string baseAddress;

		private readonly bool useCompression;

		protected readonly UserCredentials credentials;

		private IdentityInfo identityInfo;

		private T proxy;

		public IdentityInfoCache IdentityCache => identityCache;

		protected ChannelFactory<T> ChannelFactory
		{
			get;
			set;
		}

		protected string ServicePath => servicePath;

		public string BaseAddress => baseAddress;

		protected IssuedTokenClientBase(IdentityInfoCache identityCache, string servicePath, string baseAddress, UserCredentials credentials = null, bool useCompression = false)
		{
			if (identityCache == null)
			{
				throw new ArgumentNullException("identityCache");
			}
			if (string.IsNullOrEmpty(servicePath))
			{
				throw new ArgumentNullException("servicePath");
			}
			this.identityCache = identityCache;
			this.servicePath = servicePath;
			this.baseAddress = baseAddress;
			this.credentials = credentials;
			this.useCompression = useCompression;
		}

		public T GetProxy()
		{
			if (this.identityInfo != null)
			{
				IdentityInfo identityInfo = identityCache.GetIdentityInfo(this.identityInfo.CacheKey);
				if (!identityInfo.Equals(this.identityInfo))
				{
					this.identityInfo = identityInfo;
					CleanupChannelFactory();
					CleanupChannel(proxy as ICommunicationObject);
					proxy = null;
				}
			}
			if (proxy != null)
			{
				return proxy;
			}
			if (ChannelFactory == null)
			{
				CreateChannelFactory();
			}
			return ChannelFactory.CreateChannel();
		}

		protected virtual void CreateChannelFactory()
		{
			if (credentials != null)
			{
				if (identityCache.ContainsKey(baseAddress))
				{
					if (!identityCache.GetUserCredentials(baseAddress).Equals(credentials))
					{
						SetCacheCredentials();
					}
				}
				else
				{
					SetCacheCredentials();
				}
			}
			else if (!identityCache.ContainsKey(baseAddress))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "No credentials specified for server '{0}'.", baseAddress));
			}
			identityInfo = identityCache.GetIdentityInfo(baseAddress);
			IssuedTokenConnectionHelper issuedTokenConnectionHelper = new IssuedTokenConnectionHelper(identityInfo, useCompression);
			ChannelFactory = issuedTokenConnectionHelper.CreateChannelFactory<T>(servicePath);
			Uri uri = new Uri(BaseAddress);
			if (uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) && !ChannelFactory.Endpoint.Behaviors.Contains(typeof(BasicAuthEndpointBehavior)))
			{
				ChannelFactory.Endpoint.Behaviors.Add(new BasicAuthEndpointBehavior(identityInfo.Credentials));
			}
		}

		private void SetCacheCredentials()
		{
			Uri uri = new Uri(baseAddress);
			switch (credentials.UserType)
			{
			case UserManagerTokenType.CustomUser:
				identityCache.SetCustomIdentity(uri, credentials.UserName, credentials.Password);
				break;
			case UserManagerTokenType.WindowsUser:
				identityCache.SetWindowsIdentity(uri);
				break;
			case UserManagerTokenType.CustomWindowsUser:
				identityCache.SetWindowsIdentity(uri, credentials.UserName, credentials.Password);
				break;
			case UserManagerTokenType.Saml2User:
				identityCache.SetCustomIdentity(uri, credentials.UserName, credentials.SsoCredentials);
				break;
			}
		}

		public void Dispose()
		{
			CleanupChannelFactory();
		}

		private void CleanupChannelFactory()
		{
			if (ChannelFactory != null)
			{
				try
				{
					ChannelFactory.Close();
				}
				catch
				{
					try
					{
						ChannelFactory.Abort();
					}
					catch
					{
					}
				}
				ChannelFactory = null;
			}
		}

		protected virtual void CleanupChannel(ICommunicationObject co, bool connectionError = false)
		{
			if (co != null)
			{
				try
				{
					if (connectionError)
					{
						co.Abort();
					}
					else
					{
						co.Close();
					}
					if (!connectionError && identityInfo != null)
					{
						identityInfo.SetConnectionSuccess();
					}
				}
				catch
				{
					try
					{
						co.Abort();
					}
					catch
					{
					}
				}
			}
		}

		protected override Exception ProcessServiceException(FaultException ex)
		{
			if (ex.Code.Namespace == "http://sdl.com/enterprise/platform/2010")
			{
				string name = ex.Code.Name;
				if (name == "ServiceNotInstalled")
				{
					return CreateServiceNotAvailableException(ex);
				}
				if (name == "ApplicationServerNotReachable")
				{
					return new ApplicationServerNotReachableException(Resources.ApplicationServerNotReachable, ex);
				}
			}
			return base.ProcessServiceException(ex);
		}

		protected bool ProcessException(Exception ex)
		{
			if (ex is SecurityAccessDeniedException)
			{
				return false;
			}
			if (ex is CommunicationException)
			{
				if (identityInfo != null)
				{
					identityInfo.SetConnectionError(ex);
				}
				return true;
			}
			if (ex is TimeoutException)
			{
				if (identityInfo != null)
				{
					identityInfo.SetConnectionError(ex);
				}
				return true;
			}
			if (ex is ApplicationServerNotReachableException)
			{
				if (identityInfo != null)
				{
					identityInfo.SetConnectionError(ex);
				}
				return true;
			}
			if (ex is COMException)
			{
				if (identityInfo != null)
				{
					identityInfo.SetConnectionError(ex);
				}
				return true;
			}
			return false;
		}

		protected virtual ServiceNotAvailableException CreateServiceNotAvailableException(FaultException innerException)
		{
			return new ServiceNotAvailableException(string.Format(Resources.ServiceNotAvailable, new Uri(BaseAddress).Host, ServicePath), innerException);
		}
	}
}
