using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public class ChiSquaredTranslationModelStoreReadOnly : IChiSquaredTranslationModelStoreReadOnly
	{
		private readonly ChiSquaredTranslationModelId _modelId;

		private readonly ITranslationModelDataService _modelService;

		private readonly TranslationModelDetails _modelDetails;

		public int SampleCount => _modelDetails.SampleCount;

		public DateTime? TranslationModelDate => _modelDetails.ModelDate;

		public ChiSquaredTranslationModelStoreReadOnly(ITranslationModelDataService service, ChiSquaredTranslationModelId modelId)
		{
			_modelId = modelId;
			_modelService = service;
			_modelDetails = _modelService.GetModelDetails(modelId);
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(ref int startAfter, int count, bool isReversedMatrix)
		{
			return _modelService.LoadMatrixData(_modelId, ref startAfter, count, isReversedMatrix);
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix)
		{
			return _modelService.LoadMatrixData(_modelId, sourceKeys, targetKeys, isReversedMatrix);
		}

		public List<TranslationModelVocabEntry> LoadVocab(bool target, IEnumerable<string> tokensToLoad)
		{
			return _modelService.LoadVocab(_modelId, target, tokensToLoad);
		}

		public int TotalVocabSize(bool target)
		{
			throw new NotImplementedException();
		}
	}
}
