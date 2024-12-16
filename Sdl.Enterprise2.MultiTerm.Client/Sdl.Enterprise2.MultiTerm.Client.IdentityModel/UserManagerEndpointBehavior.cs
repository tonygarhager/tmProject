using System;
using System.Globalization;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	internal class UserManagerEndpointBehavior : IEndpointBehavior
	{
		private string _userName;

		private string _password;

		private ICryptoTransform _transform;

		public UserManagerEndpointBehavior()
		{
		}

		public UserManagerEndpointBehavior(byte[] key, byte[] iv)
		{
			if (key == null || key.Length == 0)
			{
				throw new ArgumentNullException("key");
			}
			if (iv == null || iv.Length == 0)
			{
				throw new ArgumentNullException("iv");
			}
			_transform = new AesManaged().CreateEncryptor(key, iv);
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
			ClientCredentials clientCredentials = bindingParameters.Find<ClientCredentials>();
			if (clientCredentials == null)
			{
				throw new InvalidOperationException("expected client credentials.");
			}
			UserManagerTokenType userManagerTokenType = (UserManagerTokenType)Enum.Parse(typeof(UserManagerTokenType), endpoint.ListenUri.Segments[endpoint.ListenUri.Segments.Length - 1], ignoreCase: true);
			if ((string.IsNullOrEmpty(clientCredentials.UserName.UserName) || string.IsNullOrEmpty(clientCredentials.UserName.Password)) && (userManagerTokenType == UserManagerTokenType.CustomUser || userManagerTokenType == UserManagerTokenType.CustomWindowsUser))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.MissingCredentials, userManagerTokenType));
			}
			_userName = clientCredentials.UserName.UserName;
			_password = clientCredentials.UserName.Password;
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			if (!string.IsNullOrEmpty(_userName) && !string.IsNullOrEmpty(_password))
			{
				if (_transform != null)
				{
					clientRuntime.MessageInspectors.Add(new UserManagerMessageInspector(_userName, _password, _transform, "http://www.w3.org/2001/04/xmlenc#aes256-cbc"));
				}
				else
				{
					clientRuntime.MessageInspectors.Add(new UserManagerMessageInspector(_userName, _password));
				}
			}
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			throw new NotSupportedException(Resources.ClientBehaviorOnly);
		}

		public void Validate(ServiceEndpoint endpoint)
		{
			if (!endpoint.Binding.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(Resources.HttpEndpointOnly);
			}
			if (!endpoint.Contract.Name.Equals("TokenManager", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(Resources.UserManagerOnly);
			}
		}
	}
}
