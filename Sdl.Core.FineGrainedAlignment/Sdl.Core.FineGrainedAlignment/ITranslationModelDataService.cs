using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface ITranslationModelDataService : IDisposable
	{
		List<TranslationModelMatrixEntry> LoadMatrixData(TranslationModelId modelId, ref int startAfter, int count, bool isReversedMatrix);

		List<TranslationModelMatrixEntry> LoadMatrixData(TranslationModelId modelId, HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix);

		List<TranslationModelVocabEntry> LoadVocab(TranslationModelId modelId, bool target, IEnumerable<string> tokensToLoad);

		TranslationModelDetails GetModelDetails(TranslationModelId modelId);

		TranslationModelDetails[] GetAllModelDetails();
	}
}
