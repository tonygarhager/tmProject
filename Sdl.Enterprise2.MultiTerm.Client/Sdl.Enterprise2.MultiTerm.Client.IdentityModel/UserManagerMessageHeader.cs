using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Xml;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	internal class UserManagerMessageHeader : MessageHeader
	{
		private readonly string _userName;

		private readonly string _password;

		private readonly bool _useEncryption;

		private readonly string _encryptedDataId;

		private readonly ICryptoTransform _transform;

		private readonly string _xmlEncAlgorithmUrl;

		private const string _xmlEncryptionNamespace = "http://www.w3.org/2001/04/xmlenc#";

		private const string _xmlElementEncryption = "http://www.w3.org/2001/04/xmlenc#Element";

		public override bool MustUnderstand => true;

		public override string Name => "Security";

		public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

		public UserManagerMessageHeader(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}
			_userName = userName;
			_password = password;
		}

		public UserManagerMessageHeader(string userName, string password, ICryptoTransform encryptor, string xmlEncryptionAlgorithmUrl)
			: this(userName, password)
		{
			if (encryptor == null)
			{
				throw new ArgumentNullException("encryptor");
			}
			if (string.IsNullOrEmpty(xmlEncryptionAlgorithmUrl))
			{
				throw new ArgumentNullException("xmlEncryptionAlgorithmUrl");
			}
			_transform = encryptor;
			_xmlEncAlgorithmUrl = xmlEncryptionAlgorithmUrl;
			_encryptedDataId = "r1";
			_useEncryption = true;
		}

		protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
		{
			WriteTimestamp(writer);
			if (_useEncryption)
			{
				WriteEncryptedUserNameToken(writer, _encryptedDataId);
			}
			else
			{
				WriteUserNameToken(writer);
			}
		}

		private static void WriteTimestamp(XmlWriter writer)
		{
			DateTime utcNow = DateTime.UtcNow;
			string value = XmlConvert.ToString(utcNow, "yyyy-MM-ddTHH:mm:ss.fffZ");
			string value2 = XmlConvert.ToString(utcNow.AddMinutes(5.0), "yyyy-MM-ddTHH:mm:ss.fffZ");
			writer.WriteStartElement("u", "Timestamp", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
			writer.WriteAttributeString("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", "_0");
			writer.WriteElementString("Created", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", value);
			writer.WriteElementString("Expires", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", value2);
			writer.WriteEndElement();
		}

		private void WriteUserNameToken(XmlWriter writer)
		{
			writer.WriteStartElement("o", "UsernameToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
			writer.WriteAttributeString("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", string.Format(CultureInfo.InvariantCulture, "uuid_{0}", Guid.NewGuid()));
			writer.WriteElementString("Username", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", _userName);
			writer.WriteStartElement("Password", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
			writer.WriteAttributeString("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText");
			writer.WriteString(_password);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		private void WriteEncryptedUserNameToken(XmlWriter writer, string id)
		{
			writer.WriteStartElement("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
			if (!string.IsNullOrEmpty(id))
			{
				writer.WriteAttributeString("Id", id);
			}
			writer.WriteAttributeString("Type", "http://www.w3.org/2001/04/xmlenc#Element");
			writer.WriteStartElement("EncryptionMethod", "http://www.w3.org/2001/04/xmlenc#");
			writer.WriteAttributeString("Algorithm", _xmlEncAlgorithmUrl);
			writer.WriteEndElement();
			writer.WriteStartElement("CipherData", "http://www.w3.org/2001/04/xmlenc#");
			writer.WriteElementString("CipherValue", "http://www.w3.org/2001/04/xmlenc#", EncryptUserNameToken());
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		private string EncryptUserNameToken()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream output = new CryptoStream(memoryStream, _transform, CryptoStreamMode.Write))
				{
					using (XmlWriter writer = XmlWriter.Create(output))
					{
						WriteUserNameToken(writer);
					}
				}
				memoryStream.Flush();
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}
	}
}
