using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	public class CultureBehavior : IEndpointBehavior, IDispatchMessageInspector
	{
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			throw new NotSupportedException();
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
		}
	}
}
