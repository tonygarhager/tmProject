using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Util
{
	public static class TopologicalSort<T> where T : ITopologicalSortable
	{
		public static void Sort(IList<T> items)
		{
			List<T> list = new TopologicalSorter<T>(items).Execute();
			items.Clear();
			foreach (T item in list)
			{
				items.Add(item);
			}
		}
	}
}
