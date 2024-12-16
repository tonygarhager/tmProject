using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Util
{
	public class SortedObjectRegistry<TSortableExtensionAttribute, TExtensionType> : ObjectRegistry<TSortableExtensionAttribute, TExtensionType> where TSortableExtensionAttribute : SortableExtensionAttribute where TExtensionType : class
	{
		private class SortableExtension : ITopologicalSortable
		{
			private IExtension _extension;

			private SortableExtensionAttribute _sortableExtensionAttribute;

			public IExtension Extension => _extension;

			public string Id => _sortableExtensionAttribute.Id;

			public string InsertBefore => _sortableExtensionAttribute.InsertBefore;

			public string InsertAfter => _sortableExtensionAttribute.InsertAfter;

			public int Priority => 0;

			public SortableExtension(IExtension extension)
			{
				_extension = extension;
				_sortableExtensionAttribute = (SortableExtensionAttribute)extension.ExtensionAttribute;
			}
		}

		public SortedObjectRegistry(IPluginRegistry pluginRegistry)
			: base(pluginRegistry)
		{
		}

		public override TExtensionType[] CreateObjects()
		{
			List<SortableExtension> list = new List<SortableExtension>();
			foreach (IExtension extension in base.ExtensionPoint.Extensions)
			{
				list.Add(new SortableExtension(extension));
			}
			TopologicalSort<SortableExtension>.Sort(list);
			TExtensionType[] array = new TExtensionType[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = (list[i].Extension.CreateInstance() as TExtensionType);
			}
			return array;
		}
	}
}
