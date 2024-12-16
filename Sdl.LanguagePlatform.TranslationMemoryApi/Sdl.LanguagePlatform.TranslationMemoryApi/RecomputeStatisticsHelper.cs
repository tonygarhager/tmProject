namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class RecomputeStatisticsHelper
	{
		private const int minSize = 50000;

		private const double minGrowth = 0.1;

		private const int maxSize = 1000000;

		public static bool ShouldRecomputeFuzzyIndexStatistics(int? lastRecomputeSize, int tuCount)
		{
			if (lastRecomputeSize.HasValue && lastRecomputeSize.Value > 1000000)
			{
				return false;
			}
			if (tuCount < 50000)
			{
				return false;
			}
			if (!lastRecomputeSize.HasValue)
			{
				return true;
			}
			double num = (double)(tuCount - lastRecomputeSize.Value) / (double)tuCount;
			return num > 0.1;
		}
	}
}
