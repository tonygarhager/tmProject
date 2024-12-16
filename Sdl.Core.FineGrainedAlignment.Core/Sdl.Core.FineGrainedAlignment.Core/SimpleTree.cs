using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment.Core
{
	public class SimpleTree<T>
	{
		private readonly KeyValuePair<T, List<SimpleTree<T>>> _kvp;

		public T Key => _kvp.Key;

		public List<SimpleTree<T>> Value => _kvp.Value;

		public SimpleTree(T key, List<SimpleTree<T>> value)
		{
			_kvp = new KeyValuePair<T, List<SimpleTree<T>>>(key, value);
		}
	}
}
