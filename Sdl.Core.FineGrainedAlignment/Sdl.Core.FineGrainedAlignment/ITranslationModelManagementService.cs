using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface ITranslationModelManagementService : ITranslationModelDataService, IDisposable
	{
		TranslationModelId AddModel(string name, List<AlignableCorpusId> corpora, CultureInfo sourceCulture, CultureInfo targetCulture, TranslationModelTypes modelType);

		bool CanBuildModel(TranslationModelId modelId);

		void UpdateModel(TranslationModelId id, string name, List<AlignableCorpusId> corpora);

		bool ShouldBuildModel(TranslationModelId modelId);

		void DeleteModel(TranslationModelId modelId);

		void ClearModel(TranslationModelId modelId);

		void BuildModel(TranslationModelId translationModelId);

		void CreateSchema();

		void DropSchema();
	}
}
