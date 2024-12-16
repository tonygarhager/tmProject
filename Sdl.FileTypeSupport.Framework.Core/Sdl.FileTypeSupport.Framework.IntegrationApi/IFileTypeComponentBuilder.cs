using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeComponentBuilder : IFileTypeDefinitionAware
	{
		IFileTypeManager FileTypeManager
		{
			get;
			set;
		}

		IFileTypeInformation BuildFileTypeInformation(string name);

		IQuickTagsFactory BuildQuickTagsFactory(string name);

		INativeFileSniffer BuildFileSniffer(string name);

		IFileExtractor BuildFileExtractor(string name);

		IFileGenerator BuildFileGenerator(string name);

		IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name);

		IAbstractGenerator BuildAbstractGenerator(string name);

		IPreviewSetsFactory BuildPreviewSetsFactory(string name);

		IAbstractPreviewControl BuildPreviewControl(string name);

		IAbstractPreviewApplication BuildPreviewApplication(string name);

		IBilingualDocumentGenerator BuildBilingualGenerator(string name);

		IVerifierCollection BuildVerifierCollection(string name);
	}
}
