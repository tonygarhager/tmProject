using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing
{
	public abstract class TrieIterator<T, V> : IEnumerable<KeyValuePair<IList<T>, V>>, IEnumerable
	{
		public abstract bool IsValid
		{
			get;
		}

		public abstract V Value
		{
			get;
		}

		public abstract bool IsFinal
		{
			get;
		}

		public abstract IList<T> Path
		{
			get;
		}

		public abstract bool Traverse(T key);

		public abstract IEnumerator<KeyValuePair<IList<T>, V>> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
