using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeDefinition
	{
		IFileTypeInformation FileTypeInformation
		{
			get;
		}

		IFileTypeComponentBuilder ComponentBuilder
		{
			get;
			set;
		}

		FileTypeDefinitionCustomizationLevel CustomizationLevel
		{
			get;
			set;
		}

		IQuickTagsFactory BuildQuickTagsFactory();

		IQuickTags GetQuickInserts(ISettingsBundle settingsBundle, IFileProperties fileProperties);

		List<QuickInsertIds> BuildQuickInsertIdsList();

		IPreviewSetsFactory BuildPreviewSetsFactory();

		IFileExtractor BuildExtractor();

		ISubContentExtractor BuildSubContentExtractor();

		ISubContentGenerator BuildSubContentGenerator();

		IFileGenerator BuildNativeGenerator();

		IBilingualDocumentGenerator BuildBilingualDocumentGenerator();

		IAbstractGenerator BuildGenerator(GeneratorId generatorId);

		IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo();

		IAbstractPreviewControl BuildPreviewControl(PreviewControlId previewControlId);

		IAbstractPreviewApplication BuildPreviewApplication(PreviewApplicationId previewApplicationId);

		IVerifierCollection BuildVerifierCollection();

		bool IsSupportedFilename(string nativeFilePath);

		SniffInfo SniffFile(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter, ISettingsBundle settingsBundle);
	}
}
