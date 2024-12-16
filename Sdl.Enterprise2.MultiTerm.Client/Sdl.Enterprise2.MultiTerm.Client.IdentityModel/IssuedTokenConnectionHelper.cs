using Sdl.Enterprise2.MultiTerm.Client.Communication;
using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	internal class IssuedTokenConnectionHelper
	{
		private readonly int maxReceivedMessageSize = 314572800;

		private readonly int maxStringContentLength = 10240000;

		private readonly int maxArrayLength = 1024000;

		private readonly int maxItemsInObjectGraph = 3145728;

		private readonly int maxDepth = 1000;

		private readonly IdentityInfo identityInfo;

		private readonly bool useCompression;

		public IssuedTokenConnectionHelper(IdentityInfo idInfo)
		{
			if (idInfo == null)
			{
				throw new ArgumentNullException("idInfo");
			}
			identityInfo = idInfo;
			useCompression = false;
		}

		public IssuedTokenConnectionHelper(IdentityInfo idInfo, bool useCompression)
			: this(idInfo)
		{
			this.useCompression = useCompression;
		}

		public IssuedTokenConnectionHelper(IdentityInfo idInfo, int maxReceivedMessageSize, int maxStringContentLength, int maxArrayLength, int maxItemsInObjectGraph, int maxDepth)
			: this(idInfo)
		{
			this.maxReceivedMessageSize = maxReceivedMessageSize;
			this.maxStringContentLength = maxStringContentLength;
			this.maxArrayLength = maxArrayLength;
			this.maxItemsInObjectGraph = maxItemsInObjectGraph;
			this.maxDepth = maxDepth;
		}

		public ChannelFactory<T> CreateChannelFactory<T>(string servicePath)
		{
			Uri issuedTokenAddress = identityInfo.IssuedTokenAddress;
			Binding binding;
			if (issuedTokenAddress.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
			{
				BasicHttpBinding basicHttpBinding = new BasicHttpBinding
				{
					MaxReceivedMessageSize = maxReceivedMessageSize,
					ReaderQuotas = 
					{
						MaxStringContentLength = maxStringContentLength,
						MaxArrayLength = maxArrayLength,
						MaxDepth = maxDepth
					}
				};
				binding = basicHttpBinding;
			}
			else if (issuedTokenAddress.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
			{
				BasicHttpBinding basicHttpBinding2 = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
				{
					MaxReceivedMessageSize = maxReceivedMessageSize,
					ReaderQuotas = 
					{
						MaxStringContentLength = maxStringContentLength,
						MaxArrayLength = maxArrayLength,
						MaxDepth = maxDepth
					}
				};
				binding = basicHttpBinding2;
			}
			else
			{
				if (!issuedTokenAddress.Scheme.Equals(Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase))
				{
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Unsupported uri scheme {0}. only http, https  and tcp connections are supported.", issuedTokenAddress.Scheme));
				}
				NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None)
				{
					MaxReceivedMessageSize = maxReceivedMessageSize,
					ReaderQuotas = 
					{
						MaxStringContentLength = maxStringContentLength,
						MaxArrayLength = maxArrayLength,
						MaxDepth = maxDepth
					}
				};
				binding = netTcpBinding;
			}
			ChannelFactory<T> channelFactory = new ChannelFactory<T>(binding, ConstructServerAddress(issuedTokenAddress, servicePath));
			channelFactory.Endpoint.Behaviors.Add(new IssuedTokenEndpointBehavior(identityInfo));
			channelFactory.Endpoint.Behaviors.Add(new CompressionEndpointBehavior(WSCompressionMode.Deflate, useCompression));
			foreach (DataContractSerializerOperationBehavior item in from op in channelFactory.Endpoint.Contract.Operations
				select op.Behaviors.Find<DataContractSerializerOperationBehavior>() into dataContractBehavior
				where dataContractBehavior != null
				select dataContractBehavior)
			{
				item.MaxItemsInObjectGraph = maxItemsInObjectGraph;
			}
			if (issuedTokenAddress.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) && !channelFactory.Endpoint.Behaviors.Contains(typeof(BasicAuthEndpointBehavior)))
			{
				channelFactory.Endpoint.Behaviors.Add(new BasicAuthEndpointBehavior(identityInfo.Credentials));
			}
			return channelFactory;
		}

		private static EndpointAddress ConstructServerAddress(Uri baseAddress, string servicePath)
		{
			string text = servicePath.StartsWith("/", StringComparison.Ordinal) ? servicePath.Substring(1) : servicePath;
			Uri uri = (!baseAddress.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)) ? new Uri(baseAddress.AbsoluteUri + "/" + text) : new Uri(baseAddress.AbsoluteUri + text);
			return new EndpointAddress(uri);
		}
	}
}
