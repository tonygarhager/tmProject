using System.ComponentModel;

namespace Sdl.Core.PluginFramework
{
	public class InitializePluginProgressChangedEventArgs : ProgressChangedEventArgs
	{
		private IPlugin _plugin;

		public IPlugin Plugin => _plugin;

		public InitializePluginProgressChangedEventArgs(IPlugin plugin, int progressPercentage)
			: base(progressPercentage, null)
		{
			_plugin = plugin;
		}
	}
}
