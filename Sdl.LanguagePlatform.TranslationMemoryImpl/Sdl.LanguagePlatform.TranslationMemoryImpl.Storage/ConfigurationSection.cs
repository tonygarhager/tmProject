using System.Configuration;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class ConfigurationSection : System.Configuration.ConfigurationSection
	{
		[ConfigurationProperty("setup")]
		public ConfigurationElement Setup => (ConfigurationElement)base["setup"];
	}
}
