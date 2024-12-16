using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	public class ClientIdMessageHeader : MessageHeader
	{
		private static string _clientId;

		private static string ClientId => _clientId ?? (_clientId = Guid.NewGuid().ToString());

		public override bool MustUnderstand => false;

		public override string Name => "ClientToken";

		public override string Namespace => "http://sdl.com/sso/2010";

		protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
		{
			writer.WriteString(ClientId);
		}
	}
}
