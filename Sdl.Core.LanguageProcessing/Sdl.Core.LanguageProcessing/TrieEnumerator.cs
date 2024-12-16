using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing
{
	public abstract class TrieEnumerator<T, V> : IEnumerator<KeyValuePair<IList<T>, V>>, IDisposable, IEnumerator
	{
		public abstract KeyValuePair<IList<T>, V> Current
		{
			get;
		}

		object IEnumerator.Current => Current;

		public abstract void Dispose();

		public abstract bool MoveNext();

		public abstract void Reset();
	}
}
