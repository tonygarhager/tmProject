using System;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	public class ConnectionInfo
	{
		public ConnectionState ConnectionStatus
		{
			get;
			internal set;
		}

		public Exception LastError
		{
			get;
			internal set;
		}

		public UserCredentials Credentials
		{
			get;
			internal set;
		}

		public CredentialState AuthenticationStatus
		{
			get;
			internal set;
		}
	}
}
