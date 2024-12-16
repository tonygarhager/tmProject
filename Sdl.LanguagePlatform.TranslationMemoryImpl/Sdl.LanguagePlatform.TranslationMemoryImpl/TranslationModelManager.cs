using Sdl.Core.FineGrainedAlignment;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class TranslationModelManager : ITranslationModelManager
	{
		private ITranslationModelStorage ModelStorage
		{
			get;
		}

		static TranslationModelManager()
		{
			FGAInitializer.Initialize(null);
		}

		public TranslationModelManager(ITranslationModelStorage modelStorage)
		{
			ModelStorage = modelStorage;
		}

		private static int GetInternalId(TranslationModelId translationModelId)
		{
			return (translationModelId as TrainedTranslationModelId)?.InternalId ?? ((translationModelId as ChiSquaredTranslationModelId) ?? throw new Exception("Unexpected translation model id type: " + translationModelId.GetType().Name)).InternalId;
		}

		private TranslationModel MakeTranslationModelObj(string name, ChiSquaredTranslationModelId modelId, List<AlignableCorpusId> corpusIds, CultureInfo srcCulture, CultureInfo trgCulture, DateTime? modelDate, int sampleCount, int version)
		{
			return new ChiSquaredTranslationModel(new ChiSquaredTranslationModelStore(ModelStorage, modelId.InternalId), name, modelId, corpusIds, srcCulture, trgCulture, modelDate, sampleCount, version);
		}

		private TranslationModel MakeTranslationModelObj(string name, TrainedTranslationModelId modelId, List<AlignableCorpusId> corpusIds, CultureInfo srcCulture, CultureInfo trgCulture, DateTime? modelDate)
		{
			return new TrainedTranslationModel(new ChiSquaredTranslationModelStore(ModelStorage, modelId.InternalId), name, modelId, corpusIds, srcCulture, trgCulture, modelDate);
		}

		public TranslationModel GetModel(TranslationModelId translationModelId)
		{
			ChiSquaredTranslationModelId chiSquaredTranslationModelId = translationModelId as ChiSquaredTranslationModelId;
			TrainedTranslationModelId trainedTranslationModelId = translationModelId as TrainedTranslationModelId;
			if (chiSquaredTranslationModelId == null && trainedTranslationModelId == null)
			{
				throw new Exception("Unknown model id type: " + translationModelId.GetType().Name);
			}
			List<AlignableCorpusId> corpusIds = new List<AlignableCorpusId>();
			ModelStorage.GetTranslationModelDetails(GetInternalId(translationModelId), out string name, corpusIds, out CultureInfo sourceCulture, out CultureInfo targetCulture, out DateTime? modelDate, out int sampleCount, out int version);
			if (trainedTranslationModelId == null)
			{
				return MakeTranslationModelObj(name, chiSquaredTranslationModelId, corpusIds, sourceCulture, targetCulture, modelDate, sampleCount, version);
			}
			return MakeTranslationModelObj(name, trainedTranslationModelId, corpusIds, sourceCulture, targetCulture, modelDate);
		}

		public List<TranslationModel> GetModels()
		{
			List<string> list = new List<string>();
			List<List<AlignableCorpusId>> corpusIdLists = new List<List<AlignableCorpusId>>();
			List<int> modelIds = new List<int>();
			List<DateTime?> modelDateList = new List<DateTime?>();
			List<int> sampleCounts = new List<int>();
			List<CultureInfo> sourceCultures = new List<CultureInfo>();
			List<CultureInfo> targetCultures = new List<CultureInfo>();
			List<int> versions = new List<int>();
			ModelStorage.GetAllTranslationModelDetails(list, modelIds, corpusIdLists, sourceCultures, targetCultures, modelDateList, sampleCounts, versions);
			return list.Select((string name, int i) => MakeTranslationModelObj(name, new ChiSquaredTranslationModelId
			{
				InternalId = modelIds[i]
			}, corpusIdLists[i], sourceCultures[i], targetCultures[i], modelDateList[i], sampleCounts[i], versions[i])).ToList();
		}

		public TranslationModelId AddModel(string name, List<AlignableCorpusId> corpora, CultureInfo sourceCulture, CultureInfo targetCulture, TranslationModelTypes modelType)
		{
			int internalId = ModelStorage.AddTranslationModel(name, corpora, sourceCulture, targetCulture);
			switch (modelType)
			{
			case TranslationModelTypes.ChiSquared:
				return new ChiSquaredTranslationModelId
				{
					InternalId = internalId
				};
			case TranslationModelTypes.Trained:
				return new TrainedTranslationModelId
				{
					InternalId = internalId
				};
			default:
				throw new Exception("Unknown model type: " + modelType.ToString());
			}
		}

		public void ClearModel(TranslationModelId id)
		{
			ModelStorage.ClearTranslationModel(GetInternalId(id));
		}

		public void DeleteModel(TranslationModelId id)
		{
			ModelStorage.DeleteTranslationModel(GetInternalId(id));
		}

		public void UpdateModel(TranslationModelId id, string name, List<AlignableCorpusId> corpora)
		{
			ModelStorage.UpdateTranslationModel(GetInternalId(id), name, corpora);
		}
	}
}
