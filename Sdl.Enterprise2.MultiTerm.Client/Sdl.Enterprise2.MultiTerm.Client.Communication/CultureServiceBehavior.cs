using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	public class CultureServiceBehavior : Attribute, IServiceBehavior
	{
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (EndpointDispatcher item in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>().SelectMany((ChannelDispatcher channelDispatcher) => channelDispatcher.Endpoints))
			{
				item.DispatchRuntime.MessageInspectors.Add(new CultureBehavior());
			}
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}
	}
}
