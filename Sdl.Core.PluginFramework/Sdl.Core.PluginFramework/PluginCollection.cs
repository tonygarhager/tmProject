using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Core.PluginFramework
{
	public class PluginCollection : ReadOnlyCollection<IPlugin>
	{
		internal PluginCollection(IList<IPlugin> list)
			: base(list)
		{
		}
	}
}
