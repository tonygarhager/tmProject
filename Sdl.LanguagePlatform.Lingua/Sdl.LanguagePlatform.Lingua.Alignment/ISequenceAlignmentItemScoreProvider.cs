namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public interface ISequenceAlignmentItemScoreProvider<T>
	{
		bool MaySkip
		{
			get;
		}

		int GetAlignScore(T a, T b);

		int GetSourceSkipScore(T a);

		int GetTargetSkipScore(T b);
	}
}
