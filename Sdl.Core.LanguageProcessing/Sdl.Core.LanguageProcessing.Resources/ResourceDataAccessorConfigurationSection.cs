using System.Configuration;

namespace Sdl.Core.LanguageProcessing.Resources
{
	internal class ResourceDataAccessorConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("accessors")]
		public ResourceDataAccessorConfigurationElementCollection ResourceDataAccessors => (ResourceDataAccessorConfigurationElementCollection)base["accessors"];
	}
}
