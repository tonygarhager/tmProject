using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IFileTypeManager
	{
		string ConfigurationVersionInformation
		{
			get;
		}

		IFileTypeDefinitionFactory FileTypeDefinitionFactory
		{
			get;
		}

		IFileTypeDefinition[] FileTypeDefinitions
		{
			get;
		}

		List<string> AutoLoadedFileTypes
		{
			get;
		}

		ISettingsBundle SettingsBundle
		{
			get;
			set;
		}

		IFileTypeDefinition DefaultBilingualFileTypeDefinition
		{
			get;
			set;
		}

		IFileTypeDefinition FindFileTypeDefinition(FileTypeDefinitionId id);

		IEnumerable<Pair<IFileTypeDefinition, SniffInfo>> GetAllMatchingFileTypeDefinitions(string nativeFilePath, EventHandler<MessageEventArgs> messageHandler);

		IEnumerable<Pair<IFileTypeDefinition, SniffInfo>> GetAllMatchingFileTypeDefinitions(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		Pair<IFileTypeDefinition, SniffInfo> GetBestMatchingFileTypeDefinition(string nativeFilePath, EventHandler<MessageEventArgs> messageHandler);

		Pair<IFileTypeDefinition, SniffInfo> GetBestMatchingFileTypeDefinition(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		void AddFileTypeDefinition(IFileTypeDefinition fileTypeDefinition);

		IFileTypeDefinition CreateFileTypeDefinition(string fileTypeDefinitionId, FileTypeProfile profileOverride);

		IFileTypeDefinition CreateFileTypeDefinition(IFileTypeComponentBuilder componentBuilder, FileTypeProfile profileOverride);

		IFileTypeDefinition CreateFileTypeDefinition(string fileTypeDefinitionId);

		void InsertFileTypeDefinition(int index, IFileTypeDefinition filterDefinition);

		void RemoveFileTypeDefinition(IFileTypeDefinition filterDefinition);

		void ClearFileTypeDefinitions();

		IMultiFileConverter GetConverterToBilingual(string[] nativeFilePaths, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverterToDefaultBilingual(string[] nativeFilePaths, string bilingualOutputFilePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverterToDefaultBilingual(string fileTypeDefinitionId, string filePath, string bilingualOutputFilePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverterToNative(string[] nativeFilePaths, OutputPropertiesProvider outputSettingsProvider, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverter(string[] nativeFilePaths, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverter(string fileTypeDefinitionId, string filePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverterToBilingual(string bilingualInputFilePath, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverterToDefaultBilingual(string bilingualInputFilePath, string bilingualOutputFilePath, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverterToNative(string bilingualInputFilePath, OutputPropertiesProvider outputSettingsProvider, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverter(string bilingualInputFilePath, EventHandler<MessageEventArgs> messageHandler);

		IMultiFileConverter GetConverterToBilingual(IBilingualParser bilingualParser, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider);

		IMultiFileConverter GetConverterToDefaultBilingual(IBilingualParser bilingualParser, string bilingualOutputFilePath);

		IMultiFileConverter GetConverterToNative(IBilingualParser bilingualParser, OutputPropertiesProvider outputSettingsProvider);

		IMultiFileConverter GetConverter(IBilingualParser bilingualParser);

		IFileExtractor BuildExtractor(string filePath, CultureInfo cultureInfo, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter);

		IFileExtractor BuildExtractorNoSniffing(string fileTypeDefinitionId, string filePath, CultureInfo suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter);

		IFileGenerator BuildNativeGenerator(IPersistentFileConversionProperties fileConversionProperties);

		IFileGenerator BuildNativeGeneratorWithVerifiers(IPersistentFileConversionProperties fileConversionProperties);

		IBilingualDocumentGenerator BuildBilingualGenerator(IFileTypeDefinition filterDefinitionToUse);

		IBilingualDocumentGenerator BuildDefaultBilingualGenerator();

		IBilingualDocumentGenerator BuildBilingualGenerator(IBilingualDocumentWriter bilingualWriter);

		IFileExtractor BuildFileExtractor(IBilingualParser fileReader, IFileTypeComponentBuilder componentBuilder);

		ISubContentExtractor BuildSubContentExtractor(IBilingualParser fileReader, IFileTypeComponentBuilder componentBuilder);

		ISubContentExtractor BuildSubContentExtractor(INativeSubContentExtractor subContentReader, IFileTypeComponentBuilder componentBuilder);

		IFileExtractor BuildFileExtractor(INativeExtractor fileReader, IFileTypeComponentBuilder componentBuilder);

		INativeExtractor BuildNativeExtractor(INativeFileParser nativeFileParser);

		INativeSubContentExtractor BuildNativeSubContentExtractor(INativeFileParser nativeFileParser);

		IFileTypeInformation BuildFileTypeInformation();

		IFileGenerator BuildFileGenerator(IBilingualWriter bilingualWriter);

		INativeGenerator BuildNativeGenerator(INativeFileWriter nativeFileWriter);

		IFileGenerator BuildFileGenerator(INativeGenerator nativeGenerator);

		ISubContentGenerator BuildSubContentGenerator(IBilingualWriter bilingualWriter);

		INativeSubContentGenerator BuildNativeSubContentGenerator(INativeFileWriter nativeFileWriter);

		ISubContentGenerator BuildSubContentGenerator(INativeSubContentGenerator nativeSubContentGenerator);

		IPreviewSetsFactory BuildPreviewSetsFactory();

		IQuickTagsFactory BuildQuickTagsFactory();

		IVerifierCollection BuildVerifierCollection();

		IGeneratorInfo BuildGeneratorInfo();

		IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(params IGeneratorInfo[] generators);
	}
}
