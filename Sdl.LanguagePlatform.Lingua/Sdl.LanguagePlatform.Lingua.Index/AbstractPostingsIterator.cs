using System;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public abstract class AbstractPostingsIterator : IComparable<AbstractPostingsIterator>
	{
		public abstract bool AtEnd
		{
			get;
		}

		public abstract int Current
		{
			get;
		}

		public abstract int Count
		{
			get;
		}

		public abstract bool Next();

		public int CompareTo(AbstractPostingsIterator other)
		{
			return other.Count - Count;
		}
	}
}
