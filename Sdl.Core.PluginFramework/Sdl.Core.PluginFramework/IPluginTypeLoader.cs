using System;

namespace Sdl.Core.PluginFramework
{
	public interface IPluginTypeLoader
	{
		Type LoadType(IPlugin plugin, string typeName);
	}
}
