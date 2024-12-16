using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[DataContract]
	public class DatabaseContainer : Container
	{
		[DataMember]
		public bool UseIntegratedSecurity
		{
			get;
			set;
		}

		[DataMember]
		public string UserId
		{
			get;
			set;
		}

		[DataMember]
		public string Password
		{
			get;
			set;
		}

		[DataMember]
		public string Server
		{
			get;
			set;
		}

		[DataMember]
		public string Database
		{
			get;
			set;
		}

		[DataMember]
		public string ProviderId
		{
			get;
			set;
		}

		[DataMember]
		public string ConnectionString
		{
			get;
			set;
		}

		public DatabaseContainer()
		{
		}

		public DatabaseContainer(string providerId, string server, string database, string userId, string password)
		{
			UserId = userId;
			Password = password;
			Server = server;
			Database = database;
			ProviderId = providerId;
			UseIntegratedSecurity = false;
			ConnectionString = null;
		}

		public DatabaseContainer(string providerId, string server, string database)
		{
			UserId = null;
			Password = null;
			Server = server;
			Database = database;
			ProviderId = providerId;
			UseIntegratedSecurity = true;
			ConnectionString = null;
		}

		public DatabaseContainer(string providerId, string connectionString)
		{
			ProviderId = providerId;
			ConnectionString = connectionString;
		}
	}
}
