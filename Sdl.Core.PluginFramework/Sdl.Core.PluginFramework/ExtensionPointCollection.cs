using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Core.PluginFramework
{
	public class ExtensionPointCollection : ReadOnlyCollection<IExtensionPoint>
	{
		internal ExtensionPointCollection(IList<IExtensionPoint> list)
			: base(list)
		{
		}
	}
}
