using log4net;
using Sdl.Enterprise2.MultiTerm.Client.IdentityModel;
using Sdl.Enterprise2.Platform.Contracts.Discovery;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Sdl.Enterprise2.MultiTerm.Client.Discovery
{
	internal class DiscoveryInfo
	{
		private readonly ServiceEndpointInfo[] endpoints;

		private readonly string preferredScheme;

		private readonly string discoveryScheme;

		private readonly UserManagerTokenType tokenType;

		protected readonly UserCredentials userCredentials;

		private readonly ILog _log = LogManager.GetLogger(typeof(DiscoveryInfo));

		public Uri IssuedTokenRouterEndpoint => GetServiceUri("router.svc");

		public Uri IdentityRouterEndpoint => GetServiceUri("identity.svc");

		private DiscoveryInfo(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			discoveryScheme = new Uri(key).Scheme;
			preferredScheme = Uri.UriSchemeNetTcp;
			string discoveryBaseAddress;
			using (DiscoveryServiceClient discoveryServiceClient = new DiscoveryServiceClient(key, userCredentials))
			{
				discoveryBaseAddress = discoveryServiceClient.BaseAddress.TrimEnd('/');
				endpoints = discoveryServiceClient.GetServiceEndpoints();
			}
			ServiceEndpointInfo[] array = endpoints;
			foreach (ServiceEndpointInfo serviceEndpointInfo in array)
			{
				serviceEndpointInfo.Address = ReplaceUriUsingDiscoveryAddress(serviceEndpointInfo.Address, discoveryBaseAddress);
			}
		}

		public DiscoveryInfo(string key, UserManagerTokenType tokenType)
			: this(key)
		{
			this.tokenType = tokenType;
		}

		public DiscoveryInfo(string key, UserManagerTokenType tokenType, UserCredentials userCredentials)
			: this(key, tokenType)
		{
			this.userCredentials = userCredentials;
		}

		private bool IsServiceAvaliable(ServiceEndpointInfo endpointInfo)
		{
			bool result = false;
			Uri address = endpointInfo.Address;
			Binding binding = null;
			EndpointAddress endpointAddress = null;
			try
			{
				if (address.Scheme.Equals(Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase))
				{
					SecurityMode securityMode = (SecurityMode)Enum.Parse(typeof(SecurityMode), endpointInfo.SecurityMode, ignoreCase: true);
					binding = new NetTcpBinding(securityMode);
					switch (securityMode)
					{
					case SecurityMode.Message:
					{
						MessageCredentialType clientCredentialType2 = (MessageCredentialType)Enum.Parse(typeof(MessageCredentialType), endpointInfo.MessageCredentialType, ignoreCase: true);
						(binding as NetTcpBinding).Security.Message.ClientCredentialType = clientCredentialType2;
						break;
					}
					case SecurityMode.Transport:
						endpointAddress = new EndpointAddress(address, EndpointIdentity.CreateSpnIdentity("identity.sdl.com"), new AddressHeaderCollection());
						break;
					case SecurityMode.None:
					{
						endpointAddress = new EndpointAddress(address, EndpointIdentity.CreateSpnIdentity("identity.sdl.com"), new AddressHeaderCollection());
						MessageCredentialType clientCredentialType = (MessageCredentialType)Enum.Parse(typeof(MessageCredentialType), endpointInfo.MessageCredentialType, ignoreCase: true);
						((NetTcpBinding)binding).Security.Message.ClientCredentialType = clientCredentialType;
						((NetTcpBinding)binding).Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
						break;
					}
					}
				}
				else if (address.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
				{
					BasicHttpSecurityMode securityMode2 = (BasicHttpSecurityMode)Enum.Parse(typeof(BasicHttpSecurityMode), endpointInfo.SecurityMode, ignoreCase: true);
					binding = new BasicHttpBinding(securityMode2);
				}
				else
				{
					if (!address.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
					{
						throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedUriScheme, address.Scheme));
					}
					binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
				}
				if (endpointAddress == null)
				{
					endpointAddress = new EndpointAddress(address);
				}
				ChannelFactory<IRouter> channelFactory = new ChannelFactory<IRouter>(binding, endpointAddress);
				if (address.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) && !channelFactory.Endpoint.Behaviors.Contains(typeof(BasicAuthEndpointBehavior)))
				{
					channelFactory.Endpoint.Behaviors.Add(new BasicAuthEndpointBehavior(userCredentials));
				}
				IRouter router = channelFactory.CreateChannel();
				ICommunicationObject communicationObject = router as ICommunicationObject;
				result = router.Ping();
				try
				{
					communicationObject.Close();
				}
				catch
				{
					try
					{
						communicationObject.Abort();
					}
					catch
					{
					}
				}
				try
				{
					channelFactory.Close();
					return result;
				}
				catch
				{
					try
					{
						channelFactory.Abort();
						return result;
					}
					catch
					{
						return result;
					}
				}
			}
			catch (Exception exception)
			{
				_log.Error($"{address} Ping Failed", exception);
				return result;
			}
		}

		private Uri GetServiceUri(string serviceName)
		{
			List<string> contractsOptions = new List<string>
			{
				"Sdl.Enterprise2.Platform.Client.ITokenManager",
				"Sdl.Enterprise2.Platform.ClientSecurity.ITokenManager"
			};
			_log.InfoFormat("GetServiceUri({0}), preferredScheme({1}), discoveryScheme({2}), tokenType({3})", serviceName, preferredScheme, discoveryScheme, tokenType);
			ServiceEndpointInfo serviceEndpointInfo;
			if (tokenType == UserManagerTokenType.WindowsUser && serviceName.Equals("identity.svc", StringComparison.Ordinal))
			{
				serviceEndpointInfo = endpoints.FirstOrDefault((ServiceEndpointInfo info) => !string.IsNullOrEmpty(info.Contract) && contractsOptions.Any((string contract) => info.Contract.Equals(contract, StringComparison.OrdinalIgnoreCase)) && string.Equals(info.Address.Scheme, preferredScheme, StringComparison.OrdinalIgnoreCase) && new string[2]
				{
					"SSO",
					"SSO_WindowsUser"
				}.Contains(info.Name));
				if (serviceEndpointInfo != null && IsServiceAvaliable(serviceEndpointInfo))
				{
					return serviceEndpointInfo.Address;
				}
				throw new EndpointNotFoundException(string.Format(CultureInfo.CurrentCulture, "Unable to discover an endpoint supporting windows authentication for service {0}.", serviceName));
			}
			serviceEndpointInfo = endpoints.FirstOrDefault((ServiceEndpointInfo info) => info.Address.AbsoluteUri.Contains(serviceName) && string.Equals(info.Address.Scheme, preferredScheme, StringComparison.OrdinalIgnoreCase));
			if (serviceEndpointInfo != null && IsServiceAvaliable(serviceEndpointInfo))
			{
				_log.InfoFormat("1/Returning {0}", serviceEndpointInfo.Address);
				return serviceEndpointInfo.Address;
			}
			serviceEndpointInfo = endpoints.FirstOrDefault((ServiceEndpointInfo info) => info.Address.AbsoluteUri.Contains(serviceName) && string.Equals(info.Address.Scheme, discoveryScheme, StringComparison.OrdinalIgnoreCase));
			if (serviceEndpointInfo != null && IsServiceAvaliable(serviceEndpointInfo))
			{
				_log.InfoFormat("2/Returning {0}", serviceEndpointInfo.Address);
				return serviceEndpointInfo.Address;
			}
			IEnumerable<ServiceEndpointInfo> source = endpoints.Where((ServiceEndpointInfo info) => info.Address.AbsoluteUri.Contains(serviceName) && !string.Equals(info.Address.Scheme, preferredScheme, StringComparison.OrdinalIgnoreCase) && !string.Equals(info.Address.Scheme, discoveryScheme, StringComparison.OrdinalIgnoreCase));
			using (IEnumerator<ServiceEndpointInfo> enumerator = source.Where(IsServiceAvaliable).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ServiceEndpointInfo current2 = enumerator.Current;
					return current2.Address;
				}
			}
			string arg = endpoints.Aggregate(string.Empty, (string current, ServiceEndpointInfo info) => current + " " + info.Address.AbsoluteUri);
			throw new EndpointNotFoundException(string.Format(CultureInfo.CurrentCulture, "Unable to discover an endpoint for service {0}. Endpoints: {1}", serviceName, arg));
		}

		private Uri ReplaceUriUsingDiscoveryAddress(Uri uri, string discoveryBaseAddress)
		{
			string text = uri.ToString();
			int num = text.IndexOf("/platform", StringComparison.Ordinal);
			if (num > 0)
			{
				return new Uri(discoveryBaseAddress + text.Substring(num, text.Length - num));
			}
			return uri;
		}
	}
}
