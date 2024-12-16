using System;
using System.ComponentModel;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	public class AuthenticationFailureEventArgs : CancelEventArgs
	{
		private readonly string address;

		public string ServerAddress => address;

		public AuthenticationFailureEventArgs(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			address = key;
		}
	}
}
