using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class ContainerBasedTranslationModelDataService : ITranslationModelDataService, IDisposable
	{
		protected Container Container;

		protected TranslationModelDataServiceInner TmsData = new TranslationModelDataServiceInner();

		public ContainerBasedTranslationModelDataService(Container container)
		{
			Container = container;
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(TranslationModelId modelId, HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix)
		{
			return TmsData.LoadMatrixData(Container, modelId, sourceKeys, targetKeys, isReversedMatrix);
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(TranslationModelId modelId, ref int startAfter, int count, bool isReversedMatrix)
		{
			return TmsData.LoadMatrixData(Container, modelId, ref startAfter, count, isReversedMatrix);
		}

		public List<TranslationModelVocabEntry> LoadVocab(TranslationModelId modelId, bool target, IEnumerable<string> tokensToLoad)
		{
			return TmsData.LoadVocab(Container, modelId, target, tokensToLoad);
		}

		public TranslationModelDetails[] GetAllModelDetails()
		{
			return TmsData.GetAllModelDetails(Container);
		}

		public TranslationModelDetails GetModelDetails(TranslationModelId modelId)
		{
			return TmsData.GetModelDetails(Container, modelId);
		}

		public void Dispose()
		{
			if (TmsData != null)
			{
				TmsData.Dispose();
				TmsData = null;
			}
		}
	}
}
