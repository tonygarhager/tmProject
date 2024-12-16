using System;

namespace Sdl.LanguagePlatform.Stat
{
	public class PhrasePair : IComparable<PhrasePair>
	{
		public int ID;

		public int SourcePhraseKey;

		public int TargetPhraseKey;

		public int Count;

		public int CompareTo(PhrasePair other)
		{
			int num = SourcePhraseKey - other.SourcePhraseKey;
			if (num == 0)
			{
				num = TargetPhraseKey - other.TargetPhraseKey;
			}
			return num;
		}
	}
}
