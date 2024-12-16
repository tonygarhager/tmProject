using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.Core.PluginFramework
{
	public class ExtensionCollection : ReadOnlyCollection<IExtension>
	{
		public IExtension this[string extensionId]
		{
			get
			{
				if (string.IsNullOrEmpty(extensionId))
				{
					throw new ArgumentNullException("extensionId");
				}
				return base.Items.FirstOrDefault((IExtension e) => e.ExtensionAttribute.Id.Equals(extensionId, StringComparison.OrdinalIgnoreCase));
			}
		}

		public ExtensionCollection(IList<IExtension> list)
			: base(list)
		{
		}
	}
}
