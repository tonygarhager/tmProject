using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	internal class IssuedTokenEndpointBehavior : IEndpointBehavior
	{
		private readonly IdentityInfo identityInfo;

		public IssuedTokenEndpointBehavior(IdentityInfo idInfo)
		{
			if (idInfo == null)
			{
				throw new ArgumentNullException("idInfo");
			}
			identityInfo = idInfo;
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new IssuedTokenMessageInspector(identityInfo));
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			throw new NotSupportedException("this behavior is for client endpoints only.");
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}
