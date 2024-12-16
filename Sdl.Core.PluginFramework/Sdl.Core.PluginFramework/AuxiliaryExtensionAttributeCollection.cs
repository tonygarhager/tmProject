using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Core.PluginFramework
{
	public class AuxiliaryExtensionAttributeCollection : ReadOnlyCollection<AuxiliaryExtensionAttribute>
	{
		internal AuxiliaryExtensionAttributeCollection(IList<AuxiliaryExtensionAttribute> list)
			: base(list)
		{
		}
	}
}
