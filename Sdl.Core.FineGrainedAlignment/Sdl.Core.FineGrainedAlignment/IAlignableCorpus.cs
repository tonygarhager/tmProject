using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface IAlignableCorpus
	{
		AlignableCorpusId Id
		{
			get;
		}

		[Obsolete("replaced by UnalignedCount")]
		int UnalignedContentPairCount
		{
			get;
		}

		int PairCount
		{
			get;
		}

		CultureInfo SourceCulture
		{
			get;
		}

		CultureInfo TargetCulture
		{
			get;
		}

		int GetPostdatedContentPairCount(DateTime modelDate);

		IEnumerable<IAlignableContentPair> Pairs();

		AlignerDefinition GetAlignerDefinition();

		int GetAlignedPredatedContentPairCount(DateTime modelDate);

		[Obsolete("replaced by UnalignedCount")]
		int UnalignedUnscheduledContentPairCount(int schedule_delta, DateTime modelDate);

		int UnalignedCount(DateTime? modelDate);
	}
}
