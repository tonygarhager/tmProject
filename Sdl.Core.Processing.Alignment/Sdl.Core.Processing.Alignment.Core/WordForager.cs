using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class WordForager
	{
		private CultureInfo _sourceCulture;

		private CultureInfo _targetCulture;

		public WordForager(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (sourceCulture == null)
			{
				throw new ArgumentNullException("sourceCulture");
			}
			if (targetCulture == null)
			{
				throw new ArgumentNullException("targetCulture");
			}
			_sourceCulture = sourceCulture;
			_targetCulture = targetCulture;
		}

		public DefaultBilingualDictionary CreateBilingualDictionary(IEnumerable<AlignedPair<AlignmentElement>> alignedPairs)
		{
			if (alignedPairs == null)
			{
				throw new ArgumentNullException("alignedPairs");
			}
			CorpusStatistics corpusStatistics = new CorpusStatistics(_sourceCulture, _targetCulture);
			foreach (AlignedPair<AlignmentElement> alignedPair in alignedPairs)
			{
				corpusStatistics.AddAlignedPair(alignedPair);
			}
			DefaultBilingualDictionary defaultBilingualDictionary = new DefaultBilingualDictionary(_sourceCulture, _targetCulture);
			foreach (StringPair sourceTargetWordPair in corpusStatistics.SourceTargetWordPairs)
			{
				int sourceTargetWordPairCount = corpusStatistics.GetSourceTargetWordPairCount(sourceTargetWordPair);
				if (sourceTargetWordPairCount >= 2)
				{
					string leftString = sourceTargetWordPair.LeftString;
					string rightString = sourceTargetWordPair.RightString;
					if (defaultBilingualDictionary.CanAddEntry(leftString, rightString))
					{
						int sourceWordCount = corpusStatistics.GetSourceWordCount(leftString);
						int targetWordCount = corpusStatistics.GetTargetWordCount(rightString);
						double sourceTargetScore = GetSourceTargetScore(sourceTargetWordPairCount, sourceWordCount, targetWordCount);
						if (sourceTargetScore >= 0.8)
						{
							defaultBilingualDictionary.AddEntry(leftString, rightString);
						}
					}
				}
			}
			return defaultBilingualDictionary;
		}

		private double GetSourceTargetScore(int sourceTargetWordCount, int sourceWordCount, int targetWordCount)
		{
			int num = Math.Min(sourceWordCount, targetWordCount);
			int num2 = Math.Max(sourceWordCount, targetWordCount);
			if (num >= 4 && num2 > 0)
			{
				return 1.0 * (double)sourceTargetWordCount / (double)num2;
			}
			return 0.0;
		}
	}
}
