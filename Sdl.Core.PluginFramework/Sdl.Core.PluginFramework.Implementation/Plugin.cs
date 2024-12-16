using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class Plugin : IPlugin
	{
		private readonly IPluginCache _pluginCache;

		private string _name;

		private readonly List<IExtension> _extensions = new List<IExtension>(3);

		private ExtensionCollection _lazyReadOnlyExtensions;

		private PluginResourceManager _lazyResourceManager;

		private readonly object _syncObject = new object();

		public PluginId Id
		{
			get;
			private set;
		}

		public string Version
		{
			get;
			private set;
		}

		public string Name => GetPluginResource<string>(_name) ?? _name;

		public IPluginDescriptor Descriptor
		{
			get;
		}

		public bool Enabled
		{
			get;
			private set;
		}

		public bool CanEnable
		{
			get
			{
				if (PluginRegistry.PluginStateHandler != null)
				{
					return PluginRegistry.PluginStateHandler.CanEnable(this);
				}
				return true;
			}
		}

		public bool CanDisable
		{
			get
			{
				if (PluginRegistry.PluginStateHandler != null)
				{
					return PluginRegistry.PluginStateHandler.CanDisable(this);
				}
				return true;
			}
		}

		public bool IsDynamic
		{
			get
			{
				bool result = true;
				foreach (IExtension extension in _extensions)
				{
					if (!extension.ExtensionPoint.IsDynamic)
					{
						return false;
					}
				}
				return result;
			}
		}

		public ExtensionCollection Extensions
		{
			get
			{
				lock (_syncObject)
				{
					if (_lazyReadOnlyExtensions == null)
					{
						_lazyReadOnlyExtensions = new ExtensionCollection(_extensions);
					}
				}
				return _lazyReadOnlyExtensions;
			}
		}

		public PluginStatus Status
		{
			get;
			private set;
		}

		internal PluginResourceManager ResourceManager
		{
			get
			{
				lock (_syncObject)
				{
					if (_lazyResourceManager == null)
					{
						_lazyResourceManager = new PluginResourceManager(this);
					}
				}
				return _lazyResourceManager;
			}
		}

		internal IPluginRegistry PluginRegistry
		{
			get;
		}

		internal Plugin(IPluginRegistry pluginRegistry, IPluginDescriptor pluginDescriptor, IPluginCache pluginCache, IObjectResolver objectResolver)
		{
			PluginRegistry = pluginRegistry;
			Enabled = true;
			Descriptor = pluginDescriptor;
			_pluginCache = pluginCache;
			Load(pluginDescriptor, pluginCache, objectResolver);
		}

		private void Load(IPluginDescriptor pluginDescriptor, IPluginCache pluginCache, IObjectResolver objectResolver)
		{
			XDocument xDocument = null;
			try
			{
				xDocument = XDocument.Load(XmlReader.Create(pluginDescriptor.GetPluginManifestStream()));
			}
			catch (XmlException ex)
			{
				throw new PluginFrameworkException(string.Format(CultureInfo.InvariantCulture, StringResources.Plugin_FailedToLoadDefinition, new object[2]
				{
					pluginDescriptor.Name,
					ex.Message
				}), ex);
			}
			XElement root = xDocument.Root;
			if (root.Name != "plugin")
			{
				throw new PluginFrameworkException(string.Format(CultureInfo.InvariantCulture, StringResources.Plugin_FailedToLoadDefinition, new object[2]
				{
					pluginDescriptor.Name,
					string.Format(CultureInfo.InvariantCulture, StringResources.Plugin_InvalidDefinitionRoot, new object[1]
					{
						root.Name
					})
				}));
			}
			Id = new PluginId(PluginXmlUtils.GetRequiredAttribute(this, root, "id"));
			Version = PluginXmlUtils.GetRequiredAttribute(this, root, "version");
			_name = PluginXmlUtils.GetRequiredAttribute(this, root, "name");
			PluginState pluginState = null;
			if (pluginCache != null)
			{
				pluginState = pluginCache.GetPluginState(Id.Id);
				Enabled = pluginState.Enabled;
			}
			foreach (XElement item2 in root.Elements("extension"))
			{
				bool flag = true;
				if (PluginRegistry != null)
				{
					XElement xElement = item2.XPathSelectElement("extensionAttribute/properties/property[@name='Id']");
					flag = (PluginRegistry.PluginFilter == null || xElement == null || PluginRegistry.PluginFilter.ShouldLoadExtension(this, xElement.Value));
				}
				if (flag)
				{
					Extension item = new Extension(this, item2, pluginState?.Enabled ?? true, objectResolver);
					_extensions.Add(item);
				}
			}
			if (PluginRegistry?.PluginInitializer != null)
			{
				Status = PluginStatus.NotInitialized;
				PluginRegistry.PluginInitializer.InitializePluginCompleted += PluginInitializer_InitializePluginCompleted;
			}
			else
			{
				Status = PluginStatus.Initialized;
			}
		}

		public bool SetEnabled(bool enabled)
		{
			if (enabled == Enabled)
			{
				return false;
			}
			if (PluginRegistry.PluginStateHandler != null)
			{
				if (enabled && !PluginRegistry.PluginStateHandler.CanEnable(this))
				{
					throw new PluginFrameworkException("This plug-in cannot be enabled.");
				}
				if (!enabled && !PluginRegistry.PluginStateHandler.CanDisable(this))
				{
					throw new PluginFrameworkException("This plug-in cannot be disabled.");
				}
			}
			Enabled = enabled;
			bool flag = false;
			foreach (IExtension extension in _extensions)
			{
				if (extension.Enabled != enabled && !extension.SetEnabled(enabled))
				{
					flag = true;
				}
			}
			_pluginCache.StorePluginState(this);
			return !flag;
		}

		public T GetPluginResource<T>(string resourceName) where T : class
		{
			return ResourceManager.GetPluginResource<T>(resourceName);
		}

		public void Validate()
		{
			foreach (IExtension extension in Extensions)
			{
				extension.Validate();
			}
		}

		private void PluginInitializer_InitializePluginCompleted(object sender, InitializePluginCompletedEventArgs e)
		{
			if (e.Plugin == this)
			{
				if (e.Error != null)
				{
					Status = PluginStatus.InitializationFailed;
				}
				else if (!e.Cancelled)
				{
					Status = PluginStatus.Initialized;
					PluginRegistry.PluginInitializer.InitializePluginCompleted -= PluginInitializer_InitializePluginCompleted;
				}
			}
		}
	}
}
