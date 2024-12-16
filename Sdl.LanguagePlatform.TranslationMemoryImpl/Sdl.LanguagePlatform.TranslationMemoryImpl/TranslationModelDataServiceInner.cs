using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class TranslationModelDataServiceInner : IDisposable
	{
		internal static int GetInternalId(TranslationModelId translationModelId)
		{
			return (translationModelId as TrainedTranslationModelId)?.InternalId ?? ((translationModelId as ChiSquaredTranslationModelId) ?? throw new Exception("Unexpected translation model id type: " + translationModelId.GetType().Name)).InternalId;
		}

		protected TranslationModelCallContext CreateTranslationModelCallContext(Container container, string methodName)
		{
			return new TranslationModelCallContext(container, null);
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(Container container, TranslationModelId modelId, ref int startAfter, int count, bool isReversedMatrix)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "LoadMatrixData"))
			{
				return translationModelCallContext.TranslationModelStorage.ReadTranslationModelData(GetInternalId(modelId), ref startAfter, count, isReversedMatrix);
			}
		}

		public List<TranslationModelMatrixEntry> LoadMatrixData(Container container, TranslationModelId modelId, HashSet<int> sourceKeys, HashSet<int> targetKeys, bool isReversedMatrix)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "LoadMatrixData"))
			{
				return translationModelCallContext.TranslationModelStorage.ReadTranslationModelData(GetInternalId(modelId), sourceKeys, targetKeys, isReversedMatrix);
			}
		}

		public List<TranslationModelVocabEntry> LoadVocab(Container container, TranslationModelId modelId, bool target, IEnumerable<string> tokensToLoad)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "LoadVocab"))
			{
				return translationModelCallContext.TranslationModelStorage.LoadTranslationModelVocab(GetInternalId(modelId), target, tokensToLoad);
			}
		}

		public TranslationModelDetails GetModelDetails(Container container, TranslationModelId modelId)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "GetModelDetails"))
			{
				return DetailsFromModel(translationModelCallContext.TranslationModelManager.GetModel(modelId));
			}
		}

		public TranslationModelDetails[] GetAllModelDetails(Container container)
		{
			using (TranslationModelCallContext translationModelCallContext = CreateTranslationModelCallContext(container, "GetAllModelDetails"))
			{
				List<TranslationModel> models = translationModelCallContext.TranslationModelManager.GetModels();
				List<TranslationModelDetails> list = new List<TranslationModelDetails>();
				foreach (TranslationModel item in models)
				{
					list.Add(DetailsFromModel(item));
				}
				return list.ToArray();
			}
		}

		private static TranslationModelDetails DetailsFromModel(TranslationModel model)
		{
			TranslationModelDetails translationModelDetails = new TranslationModelDetails
			{
				Id = model.Id,
				ModelDate = model.TranslationModelDate,
				Name = model.Name,
				SourceCulture = model.SourceCulture,
				TargetCulture = model.TargetCulture,
				CorpusIds = new List<AlignableCorpusId>(model.CorpusIds),
				ModelType = model.ModelType
			};
			ChiSquaredTranslationModel chiSquaredTranslationModel = model as ChiSquaredTranslationModel;
			if (chiSquaredTranslationModel == null)
			{
				return translationModelDetails;
			}
			translationModelDetails.SampleCount = chiSquaredTranslationModel.SampleCount;
			translationModelDetails.Version = chiSquaredTranslationModel.Version;
			return translationModelDetails;
		}

		public void Dispose()
		{
		}
	}
}
