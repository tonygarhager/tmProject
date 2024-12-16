using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface IChiSquaredTranslationModelStore : IChiSquaredTranslationModelStoreReadOnly
	{
		void ClearTranslationModel();

		void SetTranslationModelDate(DateTime? date);

		void SetSampleCount(int count);

		void SaveVocab(bool target, IEnumerable<TranslationModelVocabEntry> vocab);

		void WriteMatrixData(IEnumerable<TranslationModelMatrixEntry> entries, bool isReversedMatrix);
	}
}
