namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class SimpleLCSScoreProvider<T> : ISequenceAlignmentItemScoreProvider<T>
	{
		public bool MaySkip => false;

		public int GetAlignScore(T a, T b)
		{
			if (!a.Equals(b))
			{
				return -100000;
			}
			return 1;
		}

		public int GetSourceSkipScore(T a)
		{
			return -100000;
		}

		public int GetTargetSkipScore(T a)
		{
			return -100000;
		}
	}
}
