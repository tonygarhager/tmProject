using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	internal class UserManagerMessageInspector : IClientMessageInspector
	{
		private UserManagerMessageHeader _header;

		public UserManagerMessageInspector(string userName, string password)
		{
			_header = new UserManagerMessageHeader(userName, password);
		}

		public UserManagerMessageInspector(string userName, string password, ICryptoTransform encryptor, string xmlEncryptionAlgorithmUrl)
		{
			_header = new UserManagerMessageHeader(userName, password, encryptor, xmlEncryptionAlgorithmUrl);
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			request.Headers.Add(_header);
			return null;
		}
	}
}
