using System;

namespace Sdl.Core.PluginFramework
{
	public class PluginInitializationResultsEventArgs : EventArgs
	{
		public PluginInitializationResult[] Results
		{
			get;
		}

		public PluginInitializationResultsEventArgs(PluginInitializationResult[] results)
		{
			Results = results;
		}
	}
}
