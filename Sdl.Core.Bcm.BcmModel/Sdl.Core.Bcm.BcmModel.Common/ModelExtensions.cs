using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Common
{
	public static class ModelExtensions
	{
		public static bool IsSequenceEqual<TSource>(this IList<TSource> first, IList<TSource> second)
		{
			if (object.Equals(first, second))
			{
				return true;
			}
			if (first == null && second != null)
			{
				return false;
			}
			if (first != null && second == null)
			{
				return false;
			}
			if (first.Count != second.Count)
			{
				return false;
			}
			return first.SequenceEqual(second);
		}
	}
}
