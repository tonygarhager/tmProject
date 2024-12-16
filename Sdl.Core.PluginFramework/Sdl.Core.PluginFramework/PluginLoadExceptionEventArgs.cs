using System;

namespace Sdl.Core.PluginFramework
{
	public class PluginLoadExceptionEventArgs : EventArgs
	{
		public IPluginDescriptor PluginDescriptor
		{
			get;
			private set;
		}

		public Exception Exception
		{
			get;
			private set;
		}

		public PluginLoadExceptionEventArgs(IPluginDescriptor pluginDescriptor, Exception exception)
		{
			PluginDescriptor = pluginDescriptor;
			Exception = exception;
		}
	}
}
