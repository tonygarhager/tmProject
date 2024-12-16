using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.Discovery;
using Sdl.Enterprise2.Studio.Platform.Client.IdentityModel;
using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Sdl.Enterprise2.Studio.Platform.Client.Discovery
{
	public class DiscoveryServiceClient : IssuedTokenClientBase<IDiscoveryService>
	{
		private const string SERVICE_PATH = "/platform/sdl/discovery.svc";

		private UserCredentials usr;

		public ServiceEndpointInfo[] GetServiceEndpoints()
		{
			IDiscoveryService discoveryService = null;
			bool connectionError = false;
			try
			{
				discoveryService = GetProxy();
				return discoveryService.GetServiceEndpoints();
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (Exception ex3)
			{
				connectionError = ProcessException(ex3);
				throw;
			}
			finally
			{
				CleanupChannel(discoveryService as ICommunicationObject, connectionError);
			}
		}

		public DiscoveryServiceClient()
			: base(IdentityInfoCache.Default, "/platform/sdl/discovery.svc", (IdentityInfoCache.Default.Count > 0) ? IdentityInfoCache.Default.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
			if (IdentityInfoCache.Default.Count == 0)
			{
				throw new NotSupportedException(Resources.InlineInitializationNotSupported);
			}
		}

		public DiscoveryServiceClient(string baseAddress)
			: base(IdentityInfoCache.Default, "/platform/sdl/discovery.svc", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public DiscoveryServiceClient(string baseAddress, UserCredentials UserCredentials)
			: this(baseAddress)
		{
			usr = UserCredentials;
		}

		public DiscoveryServiceClient(IdentityInfoCache identityCache)
			: base(identityCache, "/platform/sdl/discovery.svc", (identityCache.Count > 0) ? identityCache.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
			if (IdentityInfoCache.Default.Count == 0)
			{
				throw new NotSupportedException(Resources.InlineInitializationNotSupported);
			}
		}

		public DiscoveryServiceClient(string baseAddress, IdentityInfoCache identityCache)
			: base(identityCache, "/platform/sdl/discovery.svc", baseAddress, (UserCredentials)null, useCompression: false)
		{
			if (string.IsNullOrEmpty(baseAddress))
			{
				throw new ArgumentNullException("baseAddress");
			}
		}

		public DiscoveryServiceClient(string baseAddress, IdentityInfoCache identityCache, UserCredentials UserCredentials)
			: base(identityCache, "/platform/sdl/discovery.svc", baseAddress, UserCredentials, useCompression: false)
		{
			if (string.IsNullOrEmpty(baseAddress))
			{
				throw new ArgumentNullException("baseAddress");
			}
		}

		protected override void CreateChannelFactory()
		{
			Uri uri = new Uri(base.BaseAddress);
			Binding binding = null;
			if (uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
			{
				binding = new BasicHttpBinding();
			}
			else
			{
				if (!uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
				{
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedUriScheme, uri.Scheme));
				}
				binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
			}
			base.ChannelFactory = new ChannelFactory<IDiscoveryService>(binding, ConstructServerAddress(uri, base.ServicePath));
			if (uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) && identityCache.ContainsKey(baseAddress))
			{
				UserCredentials credentials = identityCache.GetIdentityInfo(baseAddress).Credentials;
				if (!base.ChannelFactory.Endpoint.Behaviors.Contains(typeof(BasicAuthEndpointBehavior)))
				{
					base.ChannelFactory.Endpoint.Behaviors.Add(new BasicAuthEndpointBehavior(credentials));
				}
			}
		}

		private static EndpointAddress ConstructServerAddress(Uri baseAddress, string servicePath)
		{
			string text = servicePath.StartsWith("/", StringComparison.Ordinal) ? servicePath.Substring(1) : servicePath;
			Uri uri = (!baseAddress.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)) ? new Uri(baseAddress.AbsoluteUri + "/" + text) : new Uri(baseAddress.AbsoluteUri + text);
			return new EndpointAddress(uri);
		}
	}
}
