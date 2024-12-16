using Sdl.Enterprise2.Platform.Contracts.ApplicationDiscoveryService;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Sdl.Enterprise2.MultiTerm.Client.ApplicationDiscoveryService
{
	public class ApplicationDiscoveryServiceClient
	{
		private IApplicationDiscoveryService proxy;

		private string baseAddress;

		public ApplicationDiscoveryServiceClient(string baseAddress)
		{
			this.baseAddress = baseAddress + "ApplicationDiscoveryService2014";
		}

		public List<ApplicationServiceEndpointInfo> GetEndPoints()
		{
			try
			{
				proxy = GetProxy();
				return proxy.GetEndPoints();
			}
			finally
			{
				CleanupChannel(proxy as ICommunicationObject);
			}
		}

		protected virtual void CleanupChannel(ICommunicationObject co)
		{
			if (co != null)
			{
				try
				{
					co.Close();
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

		private IApplicationDiscoveryService GetProxy()
		{
			if (proxy != null)
			{
				return proxy;
			}
			proxy = CreateChannelFactory<IApplicationDiscoveryService>(baseAddress).CreateChannel();
			return proxy;
		}

		private ChannelFactory<T> CreateChannelFactory<T>(string servicePath)
		{
			Uri uri = new Uri(servicePath);
			Binding binding;
			if (uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
			{
				BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
				binding = basicHttpBinding;
			}
			else if (uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
			{
				BasicHttpBinding basicHttpBinding2 = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
				binding = basicHttpBinding2;
			}
			else
			{
				if (!uri.Scheme.Equals(Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase))
				{
					throw new NotSupportedException($"Unsupported uri scheme {uri.Scheme}. only http, https  and tcp connections are supported.");
				}
				NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
				binding = netTcpBinding;
			}
			return new ChannelFactory<T>(binding, servicePath);
		}
	}
}
