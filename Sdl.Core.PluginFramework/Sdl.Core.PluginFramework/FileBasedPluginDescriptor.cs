using System.IO;
using System.Resources;

namespace Sdl.Core.PluginFramework
{
	public class FileBasedPluginDescriptor : IPluginDescriptor
	{
		private string _pluginManifestFilePath;

		private ResourceManager _lazyResourceManager;

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

		public FileBasedPluginDescriptor(string pluginManifestFilePath)
		{
			_pluginManifestFilePath = pluginManifestFilePath;
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
