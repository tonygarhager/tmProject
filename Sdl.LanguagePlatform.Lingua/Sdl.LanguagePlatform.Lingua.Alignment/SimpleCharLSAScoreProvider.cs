namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class SimpleCharLSAScoreProvider : ISequenceAlignmentItemScoreProvider<char>
	{
		public bool MaySkip => true;

		public int GetAlignScore(char a, char b)
		{
			if (a != b)
			{
				return -1;
			}
			return 2;
		}

		public int GetSourceSkipScore(char a)
		{
			return -1;
		}

		public int GetTargetSkipScore(char a)
		{
			return -1;
		}
	}
}
