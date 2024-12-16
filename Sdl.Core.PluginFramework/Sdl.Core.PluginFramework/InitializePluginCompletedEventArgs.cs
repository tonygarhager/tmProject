using System;
using System.ComponentModel;

namespace Sdl.Core.PluginFramework
{
	public class InitializePluginCompletedEventArgs : AsyncCompletedEventArgs
	{
		private IPlugin _plugin;

		public IPlugin Plugin => _plugin;

		public InitializePluginCompletedEventArgs(IPlugin plugin, Exception exception, bool canceled)
			: base(exception, canceled, null)
		{
			_plugin = plugin;
		}
	}
}
