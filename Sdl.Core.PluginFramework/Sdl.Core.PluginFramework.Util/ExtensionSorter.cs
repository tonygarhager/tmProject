using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.PluginFramework.Util
{
	public static class ExtensionSorter
	{
		private class SortableExtensionWrapper : ITopologicalSortable
		{
			private IExtension _extension;

			private SortableExtensionAttribute _attribute;

			public IExtension Extension => _extension;

			public string Id => _attribute.Id;

			public string InsertBefore => _attribute.InsertBefore;

			public string InsertAfter => _attribute.InsertAfter;

			public int Priority => 1;

			public SortableExtensionWrapper(IExtension extension)
			{
				_extension = extension;
				_attribute = (_extension.ExtensionAttribute as SortableExtensionAttribute);
				if (_attribute == null)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Extension '{0}' does not belong to a sortable extension point.", new object[1]
					{
						_extension.ExtensionAttribute.Id
					}));
				}
			}
		}

		public static IList<IExtension> Sort(IEnumerable<IExtension> extensions)
		{
			List<ITopologicalSortable> list = new List<ITopologicalSortable>();
			foreach (IExtension extension in extensions)
			{
				list.Add(new SortableExtensionWrapper(extension));
			}
			TopologicalSort<ITopologicalSortable>.Sort(list);
			List<IExtension> list2 = new List<IExtension>();
			foreach (SortableExtensionWrapper item in list)
			{
				list2.Add(item.Extension);
			}
			return list2;
		}
	}
}
