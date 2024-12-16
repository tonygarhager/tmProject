using System;

namespace Sdl.Core.FineGrainedAlignment
{
	public class SimpleAlignerBroker : IAlignerBroker
	{
		private readonly ITranslationModelDataService _modelService;

		public SimpleAlignerBroker(ITranslationModelDataService modelService)
		{
			_modelService = modelService;
		}

		public IFineGrainedAligner GetAligner(AlignerDefinition definition)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			OnlineAlignerDefinition onlineAlignerDefinition = definition as OnlineAlignerDefinition;
			if (onlineAlignerDefinition != null)
			{
				throw new NotImplementedException();
			}
			ModelBasedAlignerDefinition modelBasedAlignerDefinition = definition as ModelBasedAlignerDefinition;
			if (modelBasedAlignerDefinition != null)
			{
				TrainedTranslationModelId trainedTranslationModelId = modelBasedAlignerDefinition.ModelId as TrainedTranslationModelId;
				if (trainedTranslationModelId != null)
				{
					TranslationModel model = GetModel(modelBasedAlignerDefinition.ModelId);
					TrainedTranslationModel trainedTranslationModel = model as TrainedTranslationModel;
					if (trainedTranslationModel == null)
					{
						throw new Exception("Unexpected model type: " + model.GetType().Name);
					}
					return new TrainedModelAligner(trainedTranslationModel);
				}
				ChiSquaredTranslationModelId chiSquaredTranslationModelId = modelBasedAlignerDefinition.ModelId as ChiSquaredTranslationModelId;
				if (chiSquaredTranslationModelId == null)
				{
					throw new Exception("Unsupported model id type: " + modelBasedAlignerDefinition.ModelId.GetType().Name);
				}
				TranslationModel model2 = GetModel(modelBasedAlignerDefinition.ModelId);
				ChiSquaredTranslationModel chiSquaredTranslationModel = model2 as ChiSquaredTranslationModel;
				if (chiSquaredTranslationModel == null)
				{
					throw new Exception("Unexpected model type: " + model2.GetType().Name);
				}
				return new ChiSquaredAligner(chiSquaredTranslationModel);
			}
			throw new Exception("Unsupported AlignerDefinition type: " + definition.GetType().Name);
		}

		public TranslationModel GetModel(TranslationModelId translationModelId)
		{
			ChiSquaredTranslationModelId chiSquaredTranslationModelId = translationModelId as ChiSquaredTranslationModelId;
			TrainedTranslationModelId trainedTranslationModelId = translationModelId as TrainedTranslationModelId;
			if (chiSquaredTranslationModelId == null && trainedTranslationModelId == null)
			{
				throw new Exception("Unknown model id type: " + translationModelId.GetType().Name);
			}
			TranslationModelDetails modelDetails = _modelService.GetModelDetails(translationModelId);
			if (trainedTranslationModelId != null)
			{
				return MakeTranslationModelObj(modelDetails, trainedTranslationModelId);
			}
			return MakeTranslationModelObj(modelDetails, chiSquaredTranslationModelId);
		}

		private TranslationModel MakeTranslationModelObj(TranslationModelDetails details, ChiSquaredTranslationModelId modelId)
		{
			ChiSquaredTranslationModelStoreReadOnly store = new ChiSquaredTranslationModelStoreReadOnly(_modelService, modelId);
			return new ChiSquaredTranslationModel(store, details.Name, modelId, details.CorpusIds, details.SourceCulture, details.TargetCulture, details.ModelDate, details.SampleCount, details.Version);
		}

		private TranslationModel MakeTranslationModelObj(TranslationModelDetails details, TrainedTranslationModelId modelId)
		{
			throw new NotImplementedException();
		}
	}
}
