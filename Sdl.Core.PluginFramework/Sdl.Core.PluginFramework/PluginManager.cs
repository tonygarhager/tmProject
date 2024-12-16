using Sdl.Core.PluginFramework.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Sdl.Core.PluginFramework
{
	public static class PluginManager
	{
		private const string PLUGIN_CACHE_FILENAME = "plugincache.xml";

		private static IPluginRegistry _defaultPluginRegistry;

		public static IPluginRegistry DefaultPluginRegistry
		{
			get
			{
				return _defaultPluginRegistry ?? (_defaultPluginRegistry = CreatePluginRegistry(new DefaultPluginLocator(), null, null, new DefaultPluginTypeLoader(), CreateDefaultPluginCache(), validate: false));
			}
			set
			{
				_defaultPluginRegistry = value;
			}
		}

		public static event EventHandler<PluginLoadExceptionEventArgs> PluginLoadException;

		public static IPluginCache CreateDefaultPluginCache()
		{
			return new XmlPluginCache(Path.Combine(Application.UserAppDataPath, "plugincache.xml"));
		}

		public static string ResolvePluginAssemblyReference(string pluginAssemblyReference)
		{
			if (pluginAssemblyReference.IndexOf("PublicKeyToken=c28cdb26c445c888") == -1)
			{
				return null;
			}
			KeyValuePair<string, Version> keyValuePair = XmlPluginConfig.Current.ApiVersions.FirstOrDefault((KeyValuePair<string, Version> v) => pluginAssemblyReference.IndexOf(v.Key + ",") != -1);
			if (keyValuePair.Value == null)
			{
				return null;
			}
			AssemblyName assemblyName = new AssemblyName(pluginAssemblyReference);
			if (assemblyName.Version.Major == keyValuePair.Value.Major && assemblyName.Version.Minor < keyValuePair.Value.Minor)
			{
				return $"{keyValuePair.Key}, Version={keyValuePair.Value}, Culture=neutral, PublicKeyToken=c28cdb26c445c888";
			}
			return null;
		}

		public static IPluginRegistry CreatePluginRegistry(IPluginLocator pluginLocator, IPluginStateHandler pluginStateHandler, IPluginInitializer pluginInitializer, IPluginTypeLoader pluginTypeLoader, IPluginCache pluginCache, IPluginFilter filter, bool validate, IObjectResolver objectResolver = null)
		{
			try
			{
				if (objectResolver == null)
				{
					objectResolver = new DefaultObjectResolver();
				}
				PluginRegistry pluginRegistry = new PluginRegistry(pluginLocator, pluginInitializer, pluginTypeLoader, pluginCache, validate, objectResolver);
				pluginRegistry.PluginLoadException += PluginRegistry_PluginLoadException;
				pluginRegistry.PluginStateHandler = pluginStateHandler;
				pluginRegistry.PluginFilter = filter;
				pluginRegistry.Load();
				return pluginRegistry;
			}
			catch (Exception innerException)
			{
				throw new PluginFrameworkException("Failed to create plugin registry.", innerException);
			}
		}

		public static IPluginRegistry CreatePluginRegistry(IPluginLocator pluginLocator, IPluginStateHandler pluginStateHandler, IPluginInitializer pluginInitializer, IPluginTypeLoader pluginTypeLoader, IPluginCache pluginCache, bool validate)
		{
			return CreatePluginRegistry(pluginLocator, pluginStateHandler, pluginInitializer, pluginTypeLoader, pluginCache, null, validate);
		}

		private static void PluginRegistry_PluginLoadException(object sender, PluginLoadExceptionEventArgs e)
		{
			PluginManager.PluginLoadException?.Invoke(sender, e);
		}
	}
}
