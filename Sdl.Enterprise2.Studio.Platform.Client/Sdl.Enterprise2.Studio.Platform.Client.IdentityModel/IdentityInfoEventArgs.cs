using System;

namespace Sdl.Enterprise2.Studio.Platform.Client.IdentityModel
{
	public class IdentityInfoEventArgs : EventArgs
	{
		private readonly string address;

		public string ServerAddress => address;

		public IdentityInfoEventArgs(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			address = key;
		}
	}
}
