using Sdl.LanguagePlatform.Core;
using System.Configuration;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class Configuration
	{
		public static readonly string Tm8ConfigurationSectionName = "sdl.languageplatform.translationmemoryimpl";

		public string ConnectionString
		{
			get;
			set;
		}

		public string StorageImplTypeName
		{
			get;
			set;
		}

		public string StorageImplAssemblyName
		{
			get;
			set;
		}

		public Configuration()
		{
			ConnectionString = string.Empty;
			StorageImplTypeName = string.Empty;
			StorageImplAssemblyName = string.Empty;
		}

		public static Configuration Load()
		{
			ConfigurationSection configurationSection = ConfigurationManager.GetSection(Tm8ConfigurationSectionName) as ConfigurationSection;
			if (configurationSection == null)
			{
				return new Configuration
				{
					ConnectionString = "dummy",
					StorageImplAssemblyName = typeof(Configuration).Assembly.FullName,
					StorageImplTypeName = typeof(InMemoryStorage).FullName
				};
			}
			Configuration configuration = new Configuration();
			if (!string.IsNullOrEmpty(configurationSection.Setup.ConnectionStringName))
			{
				ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[configurationSection.Setup.ConnectionStringName];
				if (connectionStringSettings != null)
				{
					configuration.ConnectionString = connectionStringSettings.ConnectionString;
				}
			}
			if (string.IsNullOrEmpty(configuration.ConnectionString))
			{
				configuration.ConnectionString = configurationSection.Setup.ConnectionString;
			}
			if (string.IsNullOrEmpty(configuration.ConnectionString))
			{
				throw new LanguagePlatformException(ErrorCode.ConfigurationConnectionStringNotFound, FaultStatus.Fatal);
			}
			int num = configurationSection.Setup.StorageType.IndexOf(',');
			if (num >= 0)
			{
				configuration.StorageImplTypeName = configurationSection.Setup.StorageType.Substring(0, num).Trim();
				configuration.StorageImplAssemblyName = configurationSection.Setup.StorageType.Substring(num + 1).Trim();
			}
			else
			{
				configuration.StorageImplTypeName = configurationSection.Setup.StorageType;
				configuration.StorageImplAssemblyName = string.Empty;
			}
			return configuration;
		}
	}
}
