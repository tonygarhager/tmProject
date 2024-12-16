using System.Configuration;

namespace Sdl.Core.LanguageProcessing.Resources
{
	internal class ResourceDataAccessorConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)base["name"];
			}
			set
			{
				base["name"] = value;
			}
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get
			{
				return (string)base["type"];
			}
			set
			{
				base["type"] = value;
			}
		}

		[ConfigurationProperty("parameter", IsRequired = false)]
		public string Parameter
		{
			get
			{
				return (string)base["parameter"];
			}
			set
			{
				base["parameter"] = value;
			}
		}

		public ResourceDataAccessorConfigurationElement()
		{
		}

		public ResourceDataAccessorConfigurationElement(string name, string type, string parameter)
		{
			Name = name;
			Type = type;
			Parameter = parameter;
		}

		public ResourceDataAccessorConfigurationElement(string name)
		{
			Name = name;
		}
	}
}
