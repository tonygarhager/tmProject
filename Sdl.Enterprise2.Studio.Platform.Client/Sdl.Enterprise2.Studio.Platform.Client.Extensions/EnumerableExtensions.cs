using System.Collections.Generic;

namespace Sdl.Enterprise2.Studio.Platform.Client.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Select<T>(this IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				yield return item;
			}
		}
	}
}
