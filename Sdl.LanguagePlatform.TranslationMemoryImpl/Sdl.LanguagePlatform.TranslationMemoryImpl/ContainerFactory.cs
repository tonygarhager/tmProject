using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class ContainerFactory
	{
		public static Container GetContainerFromOptions(Dictionary<string, string> options)
		{
			if (options == null || options.Count == 0)
			{
				return null;
			}
			foreach (KeyValuePair<string, string> option in options)
			{
				string text = option.Key.Trim().ToLowerInvariant();
				if (text.Equals("inmemorystorage", StringComparison.Ordinal) || text.Equals("inmemory", StringComparison.Ordinal) || text.Equals("mem", StringComparison.Ordinal))
				{
					return new InMemoryContainer();
				}
				if (text.Equals("datafile", StringComparison.Ordinal) || text.Equals("file", StringComparison.Ordinal) || text.Equals("sqlite", StringComparison.Ordinal))
				{
					return GetFileContainerFromOptions(options);
				}
				if (text.Equals("cs", StringComparison.Ordinal) || text.Equals("csname", StringComparison.Ordinal) || text.Equals("connectionstring", StringComparison.Ordinal) || text.Equals("connectionstringname", StringComparison.Ordinal))
				{
					return GetContainerFromNamedConnectionString(options);
				}
				if (text.Equals("db_host", StringComparison.Ordinal) || text.Equals("db_catalogue", StringComparison.Ordinal) || text.Equals("db_provider", StringComparison.Ordinal))
				{
					return GetDatabaseContainerFromOptions(options);
				}
			}
			return null;
		}

		public static FileContainer GetFileContainerFromOptions(Dictionary<string, string> options)
		{
			FileContainer fileContainer = new FileContainer();
			foreach (KeyValuePair<string, string> option in options)
			{
				switch (option.Key.ToLowerInvariant())
				{
				case "datafile":
				case "file":
				case "sqlite":
					if (fileContainer.Path == null)
					{
						fileContainer.Path = option.Value;
					}
					break;
				case "create":
					fileContainer.CreateIfNotExists = true;
					break;
				case "password":
					fileContainer.Password = option.Value;
					break;
				}
			}
			if (string.IsNullOrEmpty(fileContainer.Path))
			{
				throw new ArgumentException("Data file not specified");
			}
			return fileContainer;
		}

		public static Container GetContainerFromNamedConnectionString(string csName)
		{
			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[csName];
			if (connectionStringSettings == null)
			{
				throw new LanguagePlatformException(ErrorCode.ConfigurationConnectionStringNotFound);
			}
			if (string.IsNullOrEmpty(connectionStringSettings.ProviderName) || connectionStringSettings.ProviderName.Equals("system.data.sqlclient", StringComparison.OrdinalIgnoreCase) || connectionStringSettings.ProviderName.Equals("oracle.dataaccess.client", StringComparison.OrdinalIgnoreCase) || connectionStringSettings.ProviderName.Equals("system.data.sqlite", StringComparison.OrdinalIgnoreCase))
			{
				return new DatabaseContainer(connectionStringSettings.ProviderName, connectionStringSettings.ConnectionString);
			}
			if (connectionStringSettings.ProviderName.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
			{
				return new InMemoryContainer();
			}
			if (connectionStringSettings.ProviderName.Equals("File", StringComparison.OrdinalIgnoreCase))
			{
				return new FileContainer(connectionStringSettings.ConnectionString);
			}
			throw new LanguagePlatformException(ErrorCode.ConfigurationUnknownProviderType, FaultStatus.Fatal, connectionStringSettings.ProviderName);
		}

		public static Container GetContainerFromNamedConnectionString(Dictionary<string, string> options)
		{
			foreach (KeyValuePair<string, string> option in options)
			{
				if (option.Key != null && option.Value != null)
				{
					string text = option.Key.ToLowerInvariant();
					if (text.Equals("cs") || text.Equals("csname") || text.Equals("connectionstring") || text.Equals("connectionstringname"))
					{
						return GetContainerFromNamedConnectionString(option.Value);
					}
				}
			}
			throw new Exception("Unexpected: the CS key is supposed to be in the options collection");
		}

		public static DatabaseContainer GetDatabaseContainerFromOptions(Dictionary<string, string> options)
		{
			DatabaseContainer databaseContainer = new DatabaseContainer();
			foreach (KeyValuePair<string, string> option in options)
			{
				if (option.Key != null && option.Value != null)
				{
					string text = option.Key.Trim().ToLowerInvariant();
					string text2 = option.Value.Trim();
					switch (text)
					{
					case "db_host":
						databaseContainer.Server = text2;
						break;
					case "db_catalogue":
						databaseContainer.Database = text2;
						break;
					case "db_username":
						databaseContainer.UserId = text2;
						break;
					case "db_password":
						databaseContainer.Password = text2;
						break;
					case "db_integrated_security":
						databaseContainer.UseIntegratedSecurity = (text2.Equals("yes", StringComparison.InvariantCultureIgnoreCase) || text2.Equals("true", StringComparison.InvariantCultureIgnoreCase) || text2.Equals("1", StringComparison.InvariantCultureIgnoreCase));
						break;
					case "db_provider":
						databaseContainer.ProviderId = text2;
						break;
					}
				}
			}
			if (databaseContainer.UseIntegratedSecurity)
			{
				databaseContainer.UserId = null;
				databaseContainer.Password = null;
			}
			if (string.IsNullOrEmpty(databaseContainer.ProviderId))
			{
				throw new ArgumentException("Invalid container: no provider specified");
			}
			return databaseContainer;
		}
	}
}
