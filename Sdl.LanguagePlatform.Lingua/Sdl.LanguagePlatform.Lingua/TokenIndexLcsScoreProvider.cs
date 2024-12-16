using Sdl.LanguagePlatform.Lingua.Alignment;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class TokenIndexLcsScoreProvider : ISequenceAlignmentItemScoreProvider<int>
	{
		private readonly SimilarityMatrix _simMatrix;

		private readonly double _threshold;

		public bool MaySkip
		{
			get;
		}

		public TokenIndexLcsScoreProvider(SimilarityMatrix simMatrix, double threshold, bool maySkip)
		{
			_simMatrix = simMatrix;
			_threshold = threshold;
			MaySkip = maySkip;
		}

		public int GetAlignScore(int a, int b)
		{
			double num = _simMatrix[a, b];
			if (!(num >= _threshold))
			{
				return -100000;
			}
			return 1;
		}

		public int GetSourceSkipScore(int a)
		{
			if (!MaySkip)
			{
				return -100000;
			}
			return -1;
		}

		public int GetTargetSkipScore(int a)
		{
			if (!MaySkip)
			{
				return -100000;
			}
			return -1;
		}
	}
}
