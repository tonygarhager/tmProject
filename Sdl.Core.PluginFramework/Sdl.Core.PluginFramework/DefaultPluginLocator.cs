using Sdl.Core.PluginFramework.PackageSupport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

namespace Sdl.Core.PluginFramework
{
	public sealed class DefaultPluginLocator : IPluginLocator, IDisposable
	{
		private const string PLUGINS_DIRECTORY = "plugins";

		private const string PLUGIN_EXTENSION = "plugin.xml";

		private const string PLUGIN_PACKAGE_EXTENSION = ".sdlplugin";

		private readonly List<string> _thirdPartyPluginsDirectories;

		private readonly List<string> _thirdPartyPluginsPackagesDirectories;

		public bool LoadThirdPartyPlugins
		{
			get;
			set;
		} = true;


		public string SystemPluginsDirectory
		{
			get;
		}

		public string ThirdPartyPluginsDirectory
		{
			get
			{
				if (_thirdPartyPluginsDirectories.Count != 0)
				{
					return _thirdPartyPluginsDirectories[0];
				}
				return null;
			}
		}

		public string ThirdPartyPluginsPackagesDirectory
		{
			get
			{
				if (_thirdPartyPluginsPackagesDirectories.Count != 0)
				{
					return _thirdPartyPluginsPackagesDirectories[0];
				}
				return null;
			}
		}

		public DefaultPluginLocator()
		{
			string text = null;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if (executingAssembly != null)
			{
				string text2 = Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "plugins");
				if (Directory.Exists(text2))
				{
					text = text2;
				}
			}
			List<string> plugins = new List<string>();
			List<string> packages = new List<string>();
			_thirdPartyPluginsDirectories = new List<string>();
			_thirdPartyPluginsPackagesDirectories = new List<string>();
			GetThirdPartyPluginsDirectories(plugins, packages);
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			if (text == null && callingAssembly != null)
			{
				string text3 = Path.Combine(Path.GetDirectoryName(callingAssembly.Location), "plugins");
				if (Directory.Exists(text3))
				{
					text = text3;
				}
			}
			if (text == null && HttpContext.Current != null)
			{
				string text4 = HttpContext.Current.Server.MapPath("/bin/Plugins");
				if (Directory.Exists(text4))
				{
					text = text4;
				}
			}
			if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
			{
				SystemPluginsDirectory = text;
				CreateThirdPartyPluginsDirectories(plugins, packages);
				return;
			}
			throw new PluginFrameworkException("No Application specific plug-in directory found.");
		}

		public DefaultPluginLocator(string systemPluginsDirectory, string thirdPartyPluginsDirectory, string thirdPartyPluginsPackagesDirectory)
		{
			_thirdPartyPluginsDirectories = new List<string>();
			_thirdPartyPluginsPackagesDirectories = new List<string>();
			SystemPluginsDirectory = systemPluginsDirectory;
			if (thirdPartyPluginsDirectory != null)
			{
				_thirdPartyPluginsDirectories.Add(thirdPartyPluginsDirectory);
			}
			if (thirdPartyPluginsPackagesDirectory != null)
			{
				_thirdPartyPluginsPackagesDirectories.Add(thirdPartyPluginsPackagesDirectory);
			}
		}

		public IPluginDescriptor[] GetPluginDescriptors()
		{
			IPluginDescriptor[] systemPluginDescriptors = GetSystemPluginDescriptors();
			if (_thirdPartyPluginsDirectories.Count == 0)
			{
				return systemPluginDescriptors;
			}
			if (LoadThirdPartyPlugins)
			{
				SyncPlugInPackages();
			}
			IPluginDescriptor[] thirdPartyPluginDescriptors = GetThirdPartyPluginDescriptors();
			IPluginDescriptor[] array = new IPluginDescriptor[systemPluginDescriptors.Length + thirdPartyPluginDescriptors.Length];
			systemPluginDescriptors.CopyTo(array, 0);
			thirdPartyPluginDescriptors.CopyTo(array, systemPluginDescriptors.Length);
			return array;
		}

		private void SyncPlugInPackages()
		{
			for (int i = 0; i < _thirdPartyPluginsDirectories.Count; i++)
			{
				SyncPlugInPackages(_thirdPartyPluginsDirectories[i], _thirdPartyPluginsPackagesDirectories[i]);
			}
		}

		private void SyncPlugInPackages(string thirdPartyPluginsDirectory, string thirdPartyPluginsPackagesDirectory)
		{
			string[] files = Directory.GetFiles(thirdPartyPluginsPackagesDirectory, "*.sdlplugin");
			Dictionary<string, Version> dictionary = new Dictionary<string, Version>();
			string[] array = files;
			foreach (string text in array)
			{
				using (PluginPackage pluginPackage = new PluginPackage(text, FileAccess.Read))
				{
					dictionary.Add(Path.GetFileNameWithoutExtension(text), XmlPluginConfig.Current.IsValid(pluginPackage.PackageManifest) ? pluginPackage.PackageManifest.Version : null);
				}
			}
			array = Directory.GetDirectories(thirdPartyPluginsDirectory);
			foreach (string text2 in array)
			{
				string fileName = Path.GetFileName(text2);
				PackageManifest packageManifest = new PackageManifest(Path.Combine(text2, "pluginpackage.manifest.xml"));
				Version value = null;
				bool flag = dictionary.TryGetValue(fileName, out value);
				if (flag && value != null)
				{
					if (packageManifest.LoadedSucessfully && (!XmlPluginConfig.Current.IsValid(packageManifest) || packageManifest.Version < value))
					{
						using (PluginPackage pluginPackage2 = new PluginPackage(Path.Combine(thirdPartyPluginsPackagesDirectory, fileName + ".sdlplugin"), FileAccess.Read))
						{
							pluginPackage2.Extract(Path.Combine(thirdPartyPluginsDirectory, fileName));
						}
					}
					dictionary.Remove(fileName);
				}
				else if (!flag)
				{
					Directory.Delete(text2, recursive: true);
				}
			}
			foreach (KeyValuePair<string, Version> item in dictionary)
			{
				if (item.Value != null)
				{
					using (PluginPackage pluginPackage3 = new PluginPackage(Path.Combine(thirdPartyPluginsPackagesDirectory, item.Key + ".sdlplugin"), FileAccess.Read))
					{
						pluginPackage3.Extract(Path.Combine(thirdPartyPluginsDirectory, item.Key));
					}
				}
			}
		}

		private IPluginDescriptor[] GetThirdPartyPluginDescriptors()
		{
			List<IPluginDescriptor> list = new List<IPluginDescriptor>();
			foreach (string thirdPartyPluginsDirectory in _thirdPartyPluginsDirectories)
			{
				list.AddRange(GetThirdPartyPluginDescriptors(thirdPartyPluginsDirectory));
			}
			return list.ToArray();
		}

		private List<IPluginDescriptor> GetThirdPartyPluginDescriptors(string thirdPartyPluginsDirectory)
		{
			List<IPluginDescriptor> list = new List<IPluginDescriptor>();
			if (LoadThirdPartyPlugins)
			{
				string[] directories = Directory.GetDirectories(thirdPartyPluginsDirectory);
				for (int i = 0; i < directories.Length; i++)
				{
					string[] files = Directory.GetFiles(directories[i], "*.plugin.xml");
					for (int j = 0; j < files.Length; j++)
					{
						FileBasedThirdPartyPluginDescriptor fileBasedThirdPartyPluginDescriptor = new FileBasedThirdPartyPluginDescriptor(files[j]);
						if (fileBasedThirdPartyPluginDescriptor.Validated)
						{
							list.Add(fileBasedThirdPartyPluginDescriptor);
						}
					}
				}
			}
			return list;
		}

		private IPluginDescriptor[] GetSystemPluginDescriptors()
		{
			string[] files = Directory.GetFiles(SystemPluginsDirectory, "*.plugin.xml");
			IPluginDescriptor[] array = new IPluginDescriptor[files.Length];
			for (int i = 0; i < files.Length; i++)
			{
				array[i] = new FileBasedPluginDescriptor(files[i]);
			}
			return array;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		private void CreateThirdPartyPluginsDirectories(List<string> plugins, List<string> packages)
		{
			_thirdPartyPluginsDirectories.Clear();
			foreach (string plugin in plugins)
			{
				try
				{
					if (!Directory.Exists(plugin))
					{
						Directory.CreateDirectory(plugin);
					}
					_thirdPartyPluginsDirectories.Add(plugin);
				}
				catch
				{
				}
			}
			_thirdPartyPluginsPackagesDirectories.Clear();
			foreach (string package in packages)
			{
				try
				{
					if (!Directory.Exists(package))
					{
						Directory.CreateDirectory(package);
					}
					_thirdPartyPluginsPackagesDirectories.Add(package);
				}
				catch
				{
				}
			}
		}

		private void GetThirdPartyPluginsDirectories(List<string> plugins, List<string> packages)
		{
			if (XmlPluginConfig.Current.ThirdPartyPluginsEnabled && !string.IsNullOrEmpty(XmlPluginConfig.Current.ThirdPartyPluginsRelativePath) && !string.IsNullOrEmpty(XmlPluginConfig.Current.ThirdPartyPluginPackagesRelativePath))
			{
				foreach (string item in new List<string>
				{
					Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
				})
				{
					plugins.Add(Path.Combine(item, XmlPluginConfig.Current.ThirdPartyPluginsRelativePath));
					packages.Add(Path.Combine(item, XmlPluginConfig.Current.ThirdPartyPluginPackagesRelativePath));
				}
			}
		}
	}
}
