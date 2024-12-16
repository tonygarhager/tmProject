using System;
using System.Collections.Generic;

namespace Sdl.Core.PluginFramework
{
	public interface IPluginInitializer
	{
		event EventHandler<InitializePluginCompletedEventArgs> InitializePluginCompleted;

		event EventHandler<InitializePluginProgressChangedEventArgs> InitializePluginProgressChanged;

		void InitializePluginAsync(IEnumerable<IPlugin> plugin);

		void InitializePluginAsyncCancel(IEnumerable<IPlugin> plugin);
	}
}
