namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class StorageFactoryBase
	{
		public const string PROVIDER_TYPE_SQL_SERVER = "system.data.sqlclient";

		public const string PROVIDER_TYPE_ORACLE = "oracle.dataaccess.client";

		public const string PROVIDER_TYPE_SQLITE = "system.data.sqlite";

		public const string IN_MEMORY_CONTAINER_ID = "memory";

		public const string STORAGE_OPTIONS_DB_HOSTNAME_KEY = "db_host";

		public const string STORAGE_OPTIONS_DB_CATALOGUE_KEY = "db_catalogue";

		public const string STORAGE_OPTIONS_DB_USERNAME_KEY = "db_username";

		public const string STORAGE_OPTIONS_DB_PASSWORD_KEY = "db_password";

		public const string STORAGE_OPTIONS_DB_INTEGRATED_SECURITY_KEY = "db_integrated_security";

		public const string STORAGE_OPTIONS_DB_PROVIDER_KEY = "db_provider";
	}
}
