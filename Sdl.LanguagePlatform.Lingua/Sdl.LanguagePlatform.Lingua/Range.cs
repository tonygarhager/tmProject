using System;

namespace Sdl.LanguagePlatform.Lingua
{
	public class Range<T> where T : IComparable<T>
	{
		public T Start
		{
			get;
			set;
		}

		public T End
		{
			get;
			set;
		}

		public Range(T start, T end)
		{
			Start = start;
			End = end;
		}
	}
}
