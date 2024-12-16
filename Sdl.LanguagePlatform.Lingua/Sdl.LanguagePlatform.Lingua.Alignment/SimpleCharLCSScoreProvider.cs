namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class SimpleCharLCSScoreProvider : ISequenceAlignmentItemScoreProvider<char>
	{
		public bool MaySkip => false;

		public int GetAlignScore(char a, char b)
		{
			if (a != b)
			{
				return -100000;
			}
			return 1;
		}

		public int GetSourceSkipScore(char a)
		{
			return -100000;
		}

		public int GetTargetSkipScore(char a)
		{
			return -100000;
		}
	}
}
