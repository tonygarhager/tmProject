using System;
using System.Data.SqlClient;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class SqlStorageUtils
	{
		public static string BuildConnectionString(DatabaseContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException();
			}
			if (container.ProviderId == null)
			{
				throw new ArgumentException("container.ProviderId is null");
			}
			if (!container.ProviderId.Trim().ToLowerInvariant().Equals("system.data.sqlclient", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new ArgumentException("container.ProviderId is invalid for this storage implementation");
			}
			if (container.ConnectionString != null)
			{
				return container.ConnectionString;
			}
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder
			{
				DataSource = container.Server,
				InitialCatalog = container.Database
			};
			if (container.UseIntegratedSecurity)
			{
				sqlConnectionStringBuilder.IntegratedSecurity = true;
			}
			else
			{
				sqlConnectionStringBuilder.UserID = container.UserId;
				sqlConnectionStringBuilder.Password = container.Password;
			}
			sqlConnectionStringBuilder.Enlist = false;
			sqlConnectionStringBuilder.MaxPoolSize = 500;
			return sqlConnectionStringBuilder.ConnectionString;
		}
	}
}
