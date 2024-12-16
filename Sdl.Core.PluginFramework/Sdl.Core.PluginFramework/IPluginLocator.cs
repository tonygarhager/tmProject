using System;

namespace Sdl.Core.PluginFramework
{
	public interface IPluginLocator : IDisposable
	{
		IPluginDescriptor[] GetPluginDescriptors();
	}
}
