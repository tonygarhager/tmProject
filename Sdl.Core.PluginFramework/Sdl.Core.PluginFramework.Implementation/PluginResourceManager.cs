using System;
using System.Globalization;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class PluginResourceManager
	{
		private const string RESOURCE_PREFIX = "res:";

		private Plugin _plugin;

		public PluginResourceManager(Plugin plugin)
		{
			_plugin = plugin;
		}

		public string ParseString(string s)
		{
			if (s.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
			{
				string text = (string)_plugin.Descriptor.GetPluginResource(s.Substring("res:".Length));
				if (text != null)
				{
					return text;
				}
				return s;
			}
			return s;
		}

		public T GetPluginResource<T>(string resourceName) where T : class
		{
			string text = ParseResourceName(resourceName);
			object pluginResource = _plugin.Descriptor.GetPluginResource(text);
			if (pluginResource == null)
			{
				return null;
			}
			T val = pluginResource as T;
			if (val == null)
			{
				throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "The resource '{0}' is not of type '{1}'.", new object[2]
				{
					text,
					typeof(T).Name
				}));
			}
			return val;
		}

		private static string ParseResourceName(string resourceName)
		{
			if (!resourceName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
			{
				return resourceName;
			}
			return resourceName.Substring("res:".Length);
		}
	}
}
