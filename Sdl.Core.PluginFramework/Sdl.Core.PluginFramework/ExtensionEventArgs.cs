using System;

namespace Sdl.Core.PluginFramework
{
	public class ExtensionEventArgs : EventArgs
	{
		private IExtension _extension;

		public IExtension Extension => _extension;

		public ExtensionEventArgs(IExtension extension)
		{
			_extension = extension;
		}
	}
}
