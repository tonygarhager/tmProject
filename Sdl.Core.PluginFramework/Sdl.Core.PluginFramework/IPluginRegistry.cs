using System;
using System.Collections.Generic;

namespace Sdl.Core.PluginFramework
{
	public interface IPluginRegistry : IDisposable
	{
		IPluginCache PluginCache
		{
			get;
		}

		IPluginLocator PluginLocator
		{
			get;
		}

		IPluginTypeLoader PluginTypeLoader
		{
			get;
		}

		IPluginFilter PluginFilter
		{
			get;
		}

		PluginCollection Plugins
		{
			get;
		}

		IPluginStateHandler PluginStateHandler
		{
			get;
			set;
		}

		IPluginInitializer PluginInitializer
		{
			get;
		}

		IExtensionPoint GetExtensionPoint<T>() where T : ExtensionAttribute;

		void EnsurePluginsInitialized(IEnumerable<IExtension> extensions, EventHandler<PluginInitializationResultsEventArgs> pluginsInitializedCallback);
	}
}
