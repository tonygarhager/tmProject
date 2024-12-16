using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.PluginFramework.Util
{
	public static class TopologicalExtensions
	{
		public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> items) where T : ITopologicalSortable
		{
			return new TopologicalSorter<T>(items.ToList()).Execute();
		}
	}
}
