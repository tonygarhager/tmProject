using System;

namespace Sdl.Core.PluginFramework
{
	public class PluginInitializationResult
	{
		public IPlugin Plugin
		{
			get;
		}

		public Exception Exception
		{
			get;
		}

		public PluginInitializationResult(IPlugin plugin, Exception exception)
		{
			Plugin = plugin;
			Exception = exception;
		}
	}
}
