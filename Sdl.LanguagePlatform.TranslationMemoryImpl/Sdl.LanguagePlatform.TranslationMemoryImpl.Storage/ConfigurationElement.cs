using System.Configuration;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class ConfigurationElement : System.Configuration.ConfigurationElement
	{
		[ConfigurationProperty("storageType", IsRequired = true)]
		public string StorageType
		{
			get
			{
				return (string)base["storageType"];
			}
			set
			{
				base["storageType"] = value;
			}
		}

		[ConfigurationProperty("connectionString", IsRequired = false)]
		public string ConnectionString
		{
			get
			{
				return (string)base["connectionString"];
			}
			set
			{
				base["connectionString"] = value;
			}
		}

		[ConfigurationProperty("connectionStringName", IsRequired = false)]
		public string ConnectionStringName
		{
			get
			{
				return (string)base["connectionStringName"];
			}
			set
			{
				base["connectionStringName"] = value;
			}
		}

		public ConfigurationElement()
		{
		}

		public ConfigurationElement(string storageType, string connectionString, string connectionStringName)
		{
			base["storageType"] = storageType;
			base["connectionString"] = connectionString;
			base["connectionStringName"] = connectionStringName;
		}
	}
}
