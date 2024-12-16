using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	internal class BasicAuthMessageInspector : IClientMessageInspector
	{
		private readonly UserCredentials userCredentials;

		public BasicAuthMessageInspector(UserCredentials usrCredentials)
		{
			if (usrCredentials == null)
			{
				throw new ArgumentNullException("usrCredentials");
			}
			userCredentials = usrCredentials;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			HttpRequestMessageProperty httpRequestMessageProperty;
			if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
			{
				httpRequestMessageProperty = (request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty);
				if (httpRequestMessageProperty == null)
				{
					httpRequestMessageProperty = new HttpRequestMessageProperty();
					request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessageProperty);
				}
			}
			else
			{
				httpRequestMessageProperty = new HttpRequestMessageProperty();
				request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessageProperty);
			}
			httpRequestMessageProperty.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userCredentials.UserName + ":" + userCredentials.Password)));
			return null;
		}
	}
}
