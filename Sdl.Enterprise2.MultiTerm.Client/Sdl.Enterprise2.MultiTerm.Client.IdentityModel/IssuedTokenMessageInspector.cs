using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	internal class IssuedTokenMessageInspector : IClientMessageInspector
	{
		private readonly IdentityInfo identityInfo;

		public IssuedTokenMessageInspector(IdentityInfo idInfo)
		{
			if (idInfo == null)
			{
				throw new ArgumentNullException("idInfo");
			}
			identityInfo = idInfo;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			request.Headers.Add(new IssuedTokenMessageHeader(identityInfo));
			request.Headers.Add(new ClientIdMessageHeader());
			return null;
		}
	}
}
