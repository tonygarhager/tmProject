using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IRepetitionsTable : ICloneable
	{
		IEnumerable<RepetitionId> RepetitionIds
		{
			get;
		}

		int Count
		{
			get;
		}

		IList<Pair<ParagraphUnitId, SegmentId>> GetRepetitions(RepetitionId repetitions);

		RepetitionId GetRepetitionId(ParagraphUnitId paragraphUnitId, SegmentId segmentId);

		bool Add(RepetitionId key, ParagraphUnitId pu, SegmentId newRepetition);

		bool Remove(ParagraphUnitId paragraphUnitId, SegmentId segmentId);

		void DeleteKey(RepetitionId repetitions);

		void Clear();
	}
}
