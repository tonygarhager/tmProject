using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface IChiSquaredTranslationModelStoreReadOnly
	{
		DateTime? TranslationModelDate
		{
			get;
		}

		int SampleCount
		{
			get;
		}

		List<TranslationModelVocabEntry> LoadVocab(bool target, IEnumerable<string> tokensToLoad);

		int TotalVocabSize(bool target);

		List<TranslationModelMatrixEntry> LoadMatrixData(HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix);

		List<TranslationModelMatrixEntry> LoadMatrixData(ref int startAfter, int count, bool isReversedMatrix);
	}
}
