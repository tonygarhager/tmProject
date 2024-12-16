using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	internal class BasicAuthEndpointBehavior : IEndpointBehavior
	{
		private readonly UserCredentials userCredentials;

		public BasicAuthEndpointBehavior(UserCredentials usrCredentials)
		{
			if (usrCredentials == null)
			{
				throw new ArgumentNullException("usrCredentials");
			}
			userCredentials = usrCredentials;
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			throw new NotSupportedException("this behavior is for client endpoints only.");
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new BasicAuthMessageInspector(userCredentials));
		}
	}
}
