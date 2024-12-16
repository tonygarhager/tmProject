using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Xml;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	internal class IssuedTokenMessageHeader : MessageHeader
	{
		private readonly object token;

		public override bool MustUnderstand => true;

		public override string Name => "Security";

		public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

		public IssuedTokenMessageHeader(IdentityInfo idInfo)
		{
			if (idInfo == null)
			{
				throw new ArgumentNullException("idInfo");
			}
			token = idInfo.GetSecurityToken();
		}

		protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
		{
			DateTime utcNow = DateTime.UtcNow;
			string value = XmlConvert.ToString(utcNow, "yyyy-MM-ddTHH:mm:ss.fffZ");
			string value2 = XmlConvert.ToString(utcNow.AddMinutes(5.0), "yyyy-MM-ddTHH:mm:ss.fffZ");
			writer.WriteStartElement("u", "Timestamp", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
			writer.WriteAttributeString("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", "_0");
			writer.WriteElementString("Created", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", value);
			writer.WriteElementString("Expires", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", value2);
			writer.WriteEndElement();
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreWhitespace = true
			};
			writer.WriteNode(XmlReader.Create(new StringReader(token.ToString()), settings), defattr: false);
		}
	}
}
