using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class PluginRegistry : IPluginRegistry, IDisposable
	{
		private class PluginInitializationWorker
		{
			private readonly IEnumerable<IPlugin> _plugins;

			private readonly EventHandler<PluginInitializationResultsEventArgs> _pluginsInitialized;

			private List<PluginInitializationResult> _results;

			private readonly IPluginInitializer _initializer;

			public PluginInitializationWorker(IPluginInitializer initializer, IEnumerable<IPlugin> plugins, EventHandler<PluginInitializationResultsEventArgs> pluginsInitialized)
			{
				_initializer = initializer;
				_plugins = plugins;
				_pluginsInitialized = pluginsInitialized;
			}

			public void InitializePlugins()
			{
				_results = new List<PluginInitializationResult>();
				_initializer.InitializePluginCompleted += plugin_InitializePluginCompleted;
				_initializer.InitializePluginAsync(_plugins);
			}

			private void plugin_InitializePluginCompleted(object sender, InitializePluginCompletedEventArgs e)
			{
				if (_plugins.Contains(e.Plugin))
				{
					_results.Add(new PluginInitializationResult(e.Plugin, e.Error));
					if (_results.Count == _plugins.Count())
					{
						_initializer.InitializePluginCompleted -= plugin_InitializePluginCompleted;
						PluginInitializationResultsEventArgs e2 = new PluginInitializationResultsEventArgs(_results.ToArray());
						_pluginsInitialized(this, e2);
					}
				}
			}
		}

		private readonly bool _validate;

		private List<IPlugin> _plugins;

		private PluginCollection _lazyReadOnlyPlugins;

		private Dictionary<string, ExtensionPoint> _extensionPointsDictionary;

		private readonly IObjectResolver _objectResolver;

		private readonly object _syncObject = new object();

		public IPluginCache PluginCache
		{
			get;
		}

		public IPluginLocator PluginLocator
		{
			get;
		}

		public IPluginInitializer PluginInitializer
		{
			get;
		}

		public IPluginTypeLoader PluginTypeLoader
		{
			get;
		}

		public IPluginFilter PluginFilter
		{
			get;
			set;
		}

		public PluginCollection Plugins
		{
			get
			{
				lock (_syncObject)
				{
					if (_lazyReadOnlyPlugins == null)
					{
						_lazyReadOnlyPlugins = new PluginCollection(_plugins);
					}
				}
				return _lazyReadOnlyPlugins;
			}
		}

		public IPluginStateHandler PluginStateHandler
		{
			get;
			set;
		}

		internal event EventHandler<PluginLoadExceptionEventArgs> PluginLoadException;

		internal PluginRegistry(IPluginLocator pluginLocator, IPluginInitializer pluginInitializer, IPluginTypeLoader pluginTypeLoader, IPluginCache pluginCache, bool validate, IObjectResolver objectResolver)
		{
			PluginLocator = pluginLocator;
			PluginInitializer = pluginInitializer;
			PluginTypeLoader = pluginTypeLoader;
			PluginCache = pluginCache;
			_validate = validate;
			_objectResolver = objectResolver;
		}

		public IExtensionPoint GetExtensionPoint<T>() where T : ExtensionAttribute
		{
			if (_extensionPointsDictionary.TryGetValue(typeof(T).AssemblyQualifiedName, out ExtensionPoint value))
			{
				return value;
			}
			string versionlessName = typeof(T).FullName + ",";
			string[] array = _extensionPointsDictionary.Keys.Where((string x) => x.StartsWith(versionlessName, StringComparison.InvariantCulture)).ToArray();
			if (array.Length == 0)
			{
				return new ExtensionPoint(typeof(T).AssemblyQualifiedName);
			}
			return _extensionPointsDictionary[array[0]];
		}

		public void EnsurePluginsInitialized(IEnumerable<IExtension> extensions, EventHandler<PluginInitializationResultsEventArgs> pluginsInitializedCallback)
		{
			List<IPlugin> list = new List<IPlugin>();
			foreach (IExtension extension in extensions)
			{
				if (extension.Plugin.Status != PluginStatus.Initialized && !list.Contains(extension.Plugin))
				{
					list.Add(extension.Plugin);
				}
			}
			if (list.Count > 0)
			{
				new PluginInitializationWorker(PluginInitializer, list, pluginsInitializedCallback).InitializePlugins();
			}
			else
			{
				pluginsInitializedCallback(this, new PluginInitializationResultsEventArgs(new PluginInitializationResult[0]));
			}
		}

		public void Dispose()
		{
			PluginLocator.Dispose();
			PluginCache.Save();
			GC.SuppressFinalize(this);
		}

		internal void Load()
		{
			_plugins = new List<IPlugin>();
			IPluginDescriptor[] pluginDescriptors = PluginLocator.GetPluginDescriptors();
			foreach (IPluginDescriptor pluginDescriptor in pluginDescriptors)
			{
				try
				{
					if (PluginFilter == null || PluginFilter.ShouldLoadPlugin(pluginDescriptor.Name))
					{
						_plugins.Add(new Plugin(this, pluginDescriptor, PluginCache, _objectResolver));
					}
				}
				catch (Exception ex)
				{
					if (!OnPluginLoadException(pluginDescriptor, ex))
					{
						throw;
					}
				}
			}
			_extensionPointsDictionary = new Dictionary<string, ExtensionPoint>();
			foreach (Plugin plugin in _plugins)
			{
				foreach (Extension extension in plugin.Extensions)
				{
					string text = GetExtentionAttributeTypeName(extension) ?? extension.ExtensionAttributeTypeName;
					if (!_extensionPointsDictionary.TryGetValue(text, out ExtensionPoint value))
					{
						value = new ExtensionPoint(text);
						_extensionPointsDictionary[text] = value;
					}
					extension.RegisterExtensionPoint(value);
					value.RegisterExtension(extension);
				}
			}
			if (_validate)
			{
				foreach (IPlugin plugin2 in _plugins)
				{
					plugin2.Validate();
				}
			}
		}

		private static string GetExtentionAttributeTypeName(Extension extension)
		{
			try
			{
				if (extension.Plugin.Descriptor is IThirdPartyPluginDescriptor)
				{
					return extension.ExtensionAttribute.GetType().AssemblyQualifiedName;
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		private bool OnPluginLoadException(IPluginDescriptor pluginDescriptor, Exception ex)
		{
			if (this.PluginLoadException != null)
			{
				this.PluginLoadException(this, new PluginLoadExceptionEventArgs(pluginDescriptor, ex));
				return true;
			}
			return false;
		}
	}
}
