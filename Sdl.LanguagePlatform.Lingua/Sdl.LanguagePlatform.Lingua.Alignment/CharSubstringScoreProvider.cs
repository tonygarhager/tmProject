namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class CharSubstringScoreProvider : ISequenceAlignmentItemScoreProvider<char>
	{
		public bool MaySkip => true;

		public int GetAlignScore(char a, char b)
		{
			if (a == b)
			{
				return 3;
			}
			return -100;
		}

		public int GetSourceSkipScore(char a)
		{
			return -100;
		}

		public int GetTargetSkipScore(char a)
		{
			return -100;
		}
	}
}
