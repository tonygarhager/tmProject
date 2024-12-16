using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.FineGrainedAlignment
{
	public interface ITranslationModelManager
	{
		TranslationModel GetModel(TranslationModelId id);

		TranslationModelId AddModel(string name, List<AlignableCorpusId> corpora, CultureInfo sourceCulture, CultureInfo targetCulture, TranslationModelTypes modelType);

		List<TranslationModel> GetModels();

		void ClearModel(TranslationModelId id);

		void DeleteModel(TranslationModelId id);

		void UpdateModel(TranslationModelId id, string name, List<AlignableCorpusId> corpora);
	}
}
