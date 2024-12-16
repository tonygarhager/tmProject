using Sdl.Core.PluginFramework.PackageSupport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Sdl.Core.PluginFramework
{
	internal class XmlPluginConfig
	{
		private const string PLUGIN_CONFIG_FILENAME = "pluginconfig.xml";

		private const string PLUGINCONFIG_ELEMENT = "PluginConfig";

		private const string THIRDPARTYPLUGIN_ELEMENT = "ThirdPartyPlugIn";

		private const string PLUGINSROOT_ATTRIBUTE = "pluginsroot";

		private const string PACKAGESFOLDER_ATTRIBUTE = "packagesroot";

		private const string PLUGINSENABLED_ATTRIBUTE = "enabled";

		private const string PRODUCT_ELEMENT = "Product";

		private const string PRODUCTNAME_ATTRIBUTE = "name";

		private const string PUBLICASSEMBLY_ELEMENT = "PublicAssembly";

		private const string ASSEMBLYNAME_ATTRIBUTE = "name";

		private const string VERSION_ATTRIBUTE = "version";

		public static XmlPluginConfig Current
		{
			get;
		} = new XmlPluginConfig();


		public string PluginConfigFilePath
		{
			get;
		}

		public bool ThirdPartyPluginsEnabled
		{
			get;
			set;
		} = true;


		public string ThirdPartyPluginsRelativePath
		{
			get;
			set;
		}

		public string ThirdPartyPluginPackagesRelativePath
		{
			get;
			set;
		}

		public Dictionary<string, Version> ProductVersions
		{
			get;
		} = new Dictionary<string, Version>();


		public Dictionary<string, Version> ApiVersions
		{
			get;
		} = new Dictionary<string, Version>();


		private XmlPluginConfig()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			PluginConfigFilePath = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "pluginconfig.xml");
			Load();
		}

		public bool IsValid(PackageManifest packageManifest)
		{
			if (!packageManifest.LoadedSucessfully)
			{
				return false;
			}
			Version value = null;
			if (packageManifest.RequiredProductName == null)
			{
				return false;
			}
			if (!ProductVersions.TryGetValue(packageManifest.RequiredProductName, out value))
			{
				return false;
			}
			if (packageManifest.MinRequiredProductVersion == null || packageManifest.MinRequiredProductVersion.Major > value.Major || (packageManifest.MinRequiredProductVersion.Major == value.Major && packageManifest.MinRequiredProductVersion.Minor > value.Minor))
			{
				return false;
			}
			if (packageManifest.MaxRequiredProductVersion != null && (packageManifest.MaxRequiredProductVersion.Major < value.Major || (packageManifest.MaxRequiredProductVersion.Major == value.Major && packageManifest.MaxRequiredProductVersion.Minor < value.Minor)))
			{
				return false;
			}
			return true;
		}

		private void Load()
		{
			ThirdPartyPluginsRelativePath = null;
			if (File.Exists(PluginConfigFilePath))
			{
				using (XmlReader xmlReader = XmlReader.Create(PluginConfigFilePath))
				{
					while (xmlReader.Read())
					{
						if (xmlReader.Name == "ThirdPartyPlugIn")
						{
							string attribute = xmlReader.GetAttribute("enabled");
							ThirdPartyPluginsEnabled = Convert.ToBoolean(attribute, CultureInfo.InvariantCulture);
							ThirdPartyPluginsRelativePath = xmlReader.GetAttribute("pluginsroot");
							ThirdPartyPluginPackagesRelativePath = xmlReader.GetAttribute("packagesroot");
						}
						else if (xmlReader.Name == "Product")
						{
							string attribute2 = xmlReader.GetAttribute("name");
							ProductVersions.Add(attribute2, new Version(xmlReader.GetAttribute("version")));
						}
						else if (xmlReader.Name == "PublicAssembly")
						{
							string attribute3 = xmlReader.GetAttribute("name");
							ApiVersions.Add(attribute3, new Version(xmlReader.GetAttribute("version")));
						}
					}
				}
			}
		}
	}
}
