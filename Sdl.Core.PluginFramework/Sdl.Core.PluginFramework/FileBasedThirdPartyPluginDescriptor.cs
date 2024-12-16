using Sdl.Core.PluginFramework.PackageSupport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace Sdl.Core.PluginFramework
{
	public class FileBasedThirdPartyPluginDescriptor : IThirdPartyPluginDescriptor, IPluginDescriptor
	{
		private string _thirdPartyManifestFilePath;

		private string _pluginManifestFilePath;

		private ResourceManager _lazyResourceManager;

		private List<InvalidSdlAssemblyReference> _invalidAssemblies;

		private PackageManifest _xmlPackageManifest;

		public string PluginManifestFilePath => _pluginManifestFilePath;

		public string Name => Path.GetFileName(_pluginManifestFilePath);

		private ResourceManager ResourceManager
		{
			get
			{
				if (_lazyResourceManager == null)
				{
					_lazyResourceManager = ResourceManager.CreateFileBasedResourceManager(PluginBasename, PluginDirectory, null);
				}
				return _lazyResourceManager;
			}
		}

		private string PluginDirectory => Path.GetDirectoryName(_pluginManifestFilePath);

		private string PluginBasename => Path.GetFileNameWithoutExtension(_pluginManifestFilePath);

		public string ThirdPartyManifestFilePath => _thirdPartyManifestFilePath;

		public string Author => _xmlPackageManifest.Author;

		public string Description => _xmlPackageManifest.Description;

		public string PlugInName => _xmlPackageManifest.PlugInName;

		public Version Version => _xmlPackageManifest.Version;

		public bool Validated => XmlPluginConfig.Current.IsValid(_xmlPackageManifest);

		[Obsolete("Use InvalidSdlAssemblyReferences to obtain list of invalid SDL API assembly references")]
		public List<string> InvalidAssemblies => _invalidAssemblies.ConvertAll((InvalidSdlAssemblyReference x) => x.AssemblyReference.FullName);

		public List<InvalidSdlAssemblyReference> InvalidSdlAssemblyReferences => _invalidAssemblies;

		public FileBasedThirdPartyPluginDescriptor(string pluginPackageManifestFilePath)
		{
			_pluginManifestFilePath = pluginPackageManifestFilePath;
			string text = _thirdPartyManifestFilePath = Path.Combine(Path.GetDirectoryName(pluginPackageManifestFilePath), "pluginpackage.manifest.xml");
			_xmlPackageManifest = new PackageManifest(_thirdPartyManifestFilePath);
			_invalidAssemblies = new List<InvalidSdlAssemblyReference>();
		}

		public Stream GetPluginManifestStream()
		{
			return File.OpenRead(_pluginManifestFilePath);
		}

		public object GetPluginResource(string name)
		{
			return ResourceManager.GetObject(name);
		}
	}
}
