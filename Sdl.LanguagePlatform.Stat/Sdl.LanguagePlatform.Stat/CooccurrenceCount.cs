using System;

namespace Sdl.LanguagePlatform.Stat
{
	public struct CooccurrenceCount : IComparable<CooccurrenceCount>, ICountable
	{
		public int First;

		public int Second;

		public int Count
		{
			get;
			set;
		}

		public CooccurrenceCount(int s, int t)
		{
			First = s;
			Second = t;
			Count = 1;
		}

		public CooccurrenceCount(int s, int t, int c)
		{
			First = s;
			Second = t;
			Count = c;
		}

		public int CompareTo(CooccurrenceCount other)
		{
			int num = First - other.First;
			if (num == 0)
			{
				num = Second - other.Second;
			}
			return num;
		}
	}
}
