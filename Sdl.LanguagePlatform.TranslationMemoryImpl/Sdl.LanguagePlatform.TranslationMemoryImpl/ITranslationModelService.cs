using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[ServiceContract]
	public interface ITranslationModelService
	{
		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool CreateTranslationModelContainerSchema(Container container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool DropTranslationModelContainerSchema(Container container);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationModelId AddModel(Container container, string name, List<PersistentObjectToken> corpora, CultureInfo sourceCulture, CultureInfo targetCulture);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool CanBuildModel(Container container, TranslationModelId modelId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationModelDetails GetModelDetails(Container container, TranslationModelId modelId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		bool ShouldBuildModel(Container container, TranslationModelId modelId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void DeleteModel(Container container, TranslationModelId modelId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void ClearModel(Container container, TranslationModelId modelId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		void BuildModel(Container container, TranslationModelId translationModelId);

		[OperationContract]
		[FaultContract(typeof(FaultDescription))]
		TranslationModelDetails[] GetAllModelDetails(Container container);
	}
}
