using Sdl.Core.Globalization;
using Sdl.Core.PluginFramework;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public abstract class DynamicFilterComponentBuilder : IFileTypeComponentBuilder, IFileTypeDefinitionAware, IDefaultFileTypeSettingsProvider, IQuickInsertsBuilder
	{
		private class LimitedFileTypeManager : IFileTypeManager
		{
			public string ConfigurationVersionInformation
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IFileTypeDefinitionFactory FileTypeDefinitionFactory
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IFileTypeDefinition[] FileTypeDefinitions
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public ISettingsBundle SettingsBundle
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public IFileTypeDefinition DefaultBilingualFileTypeDefinition
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public List<string> AutoLoadedFileTypes
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public IFileTypeDefinition FindFileTypeDefinition(FileTypeDefinitionId id)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<Pair<IFileTypeDefinition, SniffInfo>> GetAllMatchingFileTypeDefinitions(string nativeFilePath, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public Pair<IFileTypeDefinition, SniffInfo> GetBestMatchingFileTypeDefinition(string nativeFilePath, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public void AddFileTypeDefinition(IFileTypeDefinition fileTypeDefinition)
			{
				throw new NotImplementedException();
			}

			public IFileTypeDefinition CreateFileTypeDefinition(string fileTypeDefinitionId, FileTypeProfile profileOverride)
			{
				throw new NotImplementedException();
			}

			public IFileTypeDefinition CreateFileTypeDefinition(IFileTypeComponentBuilder componentBuilder, FileTypeProfile profileOverride)
			{
				throw new NotImplementedException();
			}

			public IFileTypeDefinition CreateFileTypeDefinition(string fileTypeDefinitionId)
			{
				throw new NotImplementedException();
			}

			public void InsertFileTypeDefinition(int index, IFileTypeDefinition filterDefinition)
			{
				throw new NotImplementedException();
			}

			public void RemoveFileTypeDefinition(IFileTypeDefinition filterDefinition)
			{
				throw new NotImplementedException();
			}

			public void ClearFileTypeDefinitions()
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToBilingual(string[] nativeFilePaths, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToDefaultBilingual(string[] nativeFilePaths, string bilingualOutputFilePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToDefaultBilingual(string fileTypeDefinitionId, string filePath, string bilingualOutputFilePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToNative(string[] nativeFilePaths, OutputPropertiesProvider outputSettingsProvider, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverter(string[] nativeFilePaths, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverter(string fileTypeDefinitionId, string filePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToBilingual(string bilingualInputFilePath, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToDefaultBilingual(string bilingualInputFilePath, string bilingualOutputFilePath, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToNative(string bilingualInputFilePath, OutputPropertiesProvider outputSettingsProvider, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverter(string bilingualInputFilePath, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToBilingual(IBilingualParser bilingualParser, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToDefaultBilingual(IBilingualParser bilingualParser, string bilingualOutputFilePath)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverterToNative(IBilingualParser bilingualParser, OutputPropertiesProvider outputSettingsProvider)
			{
				throw new NotImplementedException();
			}

			public IMultiFileConverter GetConverter(IBilingualParser bilingualParser)
			{
				throw new NotImplementedException();
			}

			public IFileExtractor BuildExtractor(string filePath, CultureInfo cultureInfo, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter)
			{
				throw new NotImplementedException();
			}

			public IFileExtractor BuildExtractorNoSniffing(string fileTypeDefinitionId, string filePath, CultureInfo suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter)
			{
				throw new NotImplementedException();
			}

			public IFileGenerator BuildNativeGenerator(IPersistentFileConversionProperties fileConversionProperties)
			{
				throw new NotImplementedException();
			}

			public IFileGenerator BuildNativeGeneratorWithVerifiers(IPersistentFileConversionProperties fileConversionProperties)
			{
				throw new NotImplementedException();
			}

			public IBilingualDocumentGenerator BuildBilingualGenerator(IFileTypeDefinition filterDefinitionToUse)
			{
				throw new NotImplementedException();
			}

			public IBilingualDocumentGenerator BuildDefaultBilingualGenerator()
			{
				throw new NotImplementedException();
			}

			public IBilingualDocumentGenerator BuildBilingualGenerator(IBilingualDocumentWriter bilingualWriter)
			{
				throw new NotImplementedException();
			}

			public IFileExtractor BuildFileExtractor(IBilingualParser fileReader, IFileTypeComponentBuilder componentBuilder)
			{
				throw new NotImplementedException();
			}

			public IFileExtractor BuildFileExtractor(INativeExtractor fileReader, IFileTypeComponentBuilder componentBuilder)
			{
				throw new NotImplementedException();
			}

			public INativeExtractor BuildNativeExtractor(INativeFileParser nativeFileParser)
			{
				throw new NotImplementedException();
			}

			public IFileTypeInformation BuildFileTypeInformation()
			{
				return new LimitedFileTypeInformation();
			}

			public IFileGenerator BuildFileGenerator(IBilingualWriter bilingualWriter)
			{
				throw new NotImplementedException();
			}

			public INativeGenerator BuildNativeGenerator(INativeFileWriter nativeFileWriter)
			{
				throw new NotImplementedException();
			}

			public IFileGenerator BuildFileGenerator(INativeGenerator nativeGenerator)
			{
				throw new NotImplementedException();
			}

			public IPreviewSetsFactory BuildPreviewSetsFactory()
			{
				throw new NotImplementedException();
			}

			public IQuickTagsFactory BuildQuickTagsFactory()
			{
				throw new NotImplementedException();
			}

			public IVerifierCollection BuildVerifierCollection()
			{
				throw new NotImplementedException();
			}

			public IGeneratorInfo BuildGeneratorInfo()
			{
				throw new NotImplementedException();
			}

			public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(params IGeneratorInfo[] generators)
			{
				throw new NotImplementedException();
			}

			public ISubContentExtractor BuildSubContentExtractor(IBilingualParser fileReader, IFileTypeComponentBuilder componentBuilder)
			{
				throw new NotImplementedException();
			}

			public INativeSubContentExtractor BuildNativeSubContentExtractor(INativeFileParser nativeFileParser)
			{
				throw new NotImplementedException();
			}

			public ISubContentExtractor BuildSubContentExtractor(INativeSubContentExtractor subContentReader, IFileTypeComponentBuilder componentBuilder)
			{
				throw new NotImplementedException();
			}

			public ISubContentGenerator BuildSubContentGenerator(IBilingualWriter bilingualWriter)
			{
				throw new NotImplementedException();
			}

			public INativeSubContentGenerator BuildNativeSubContentGenerator(INativeFileWriter nativeFileWriter)
			{
				throw new NotImplementedException();
			}

			public ISubContentGenerator BuildSubContentGenerator(INativeSubContentGenerator nativeSubContentGenerator)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<Pair<IFileTypeDefinition, SniffInfo>> GetAllMatchingFileTypeDefinitions(string nativeFilePath, Language language, Codepage codepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}

			public Pair<IFileTypeDefinition, SniffInfo> GetBestMatchingFileTypeDefinition(string nativeFilePath, Language language, Codepage codepage, EventHandler<MessageEventArgs> messageHandler)
			{
				throw new NotImplementedException();
			}
		}

		private class LimitedFileTypeInformation : IFileTypeInformation, IFileTypeDefinitionAware, IMetaDataContainer
		{
			public FileTypeDefinitionId FileTypeDefinitionId
			{
				get;
				set;
			}

			public LocalizableString FileTypeName
			{
				get;
				set;
			}

			public LocalizableString FileTypeDocumentName
			{
				get;
				set;
			}

			public LocalizableString FileTypeDocumentsName
			{
				get;
				set;
			}

			public string FileDialogWildcardExpression
			{
				get;
				set;
			}

			public Regex Expression => new Regex(".*");

			public string DefaultFileExtension
			{
				get;
				set;
			}

			public LocalizableString Description
			{
				get;
				set;
			}

			public bool IsBilingualDocumentFileType
			{
				get;
				set;
			}

			public bool Enabled
			{
				get;
				set;
			}

			public bool Hidden
			{
				get;
				set;
			}

			public IconDescriptor Icon
			{
				get;
				set;
			}

			public Version FileTypeFrameworkVersion
			{
				get;
				set;
			}

			public string[] SilverlightSettingsPageIds
			{
				get;
				set;
			}

			public string[] WinFormSettingsPageIds
			{
				get;
				set;
			}

			public IFileTypeDefinition FileTypeDefinition
			{
				get;
				set;
			}

			public IEnumerable<KeyValuePair<string, string>> MetaData => new List<KeyValuePair<string, string>>();

			public bool HasMetaData => false;

			public int MetaDataCount => 0;

			public bool Removed
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public bool MetaDataContainsKey(string key)
			{
				return false;
			}

			public string GetMetaData(string key)
			{
				return string.Empty;
			}

			public void SetMetaData(string key, string value)
			{
			}

			public bool RemoveMetaData(string key)
			{
				return false;
			}

			public void ClearMetaData()
			{
			}
		}

		private IFileTypeComponentBuilder _baseFileTypeComponentBuilder;

		private string _fileTypeDefinitionId;

		private string _fileTypeName;

		public virtual IFileTypeManager FileTypeManager
		{
			get
			{
				return _baseFileTypeComponentBuilder.FileTypeManager;
			}
			set
			{
				_baseFileTypeComponentBuilder.FileTypeManager = value;
			}
		}

		public virtual IFileTypeDefinition FileTypeDefinition
		{
			get
			{
				return _baseFileTypeComponentBuilder.FileTypeDefinition;
			}
			set
			{
				_baseFileTypeComponentBuilder.FileTypeDefinition = value;
			}
		}

		protected DynamicFilterComponentBuilder(string baseFileTypeDefinitionId, string fileTypeDefinitionId, string fileTypeName)
		{
			if (baseFileTypeDefinitionId == null)
			{
				throw new ArgumentNullException("baseFileTypeDefinitionId");
			}
			if (fileTypeDefinitionId == null)
			{
				throw new ArgumentNullException("fileTypeDefinitionId");
			}
			if (fileTypeName == null)
			{
				throw new ArgumentNullException("fileTypeName");
			}
			_baseFileTypeComponentBuilder = GetFileTypeComponentBuilder(baseFileTypeDefinitionId);
			if (_baseFileTypeComponentBuilder == null)
			{
				throw new ArgumentException($"The base file type definition id '{baseFileTypeDefinitionId}' could not be found.", "baseFileTypeDefinitionId");
			}
			_fileTypeDefinitionId = fileTypeDefinitionId;
			_fileTypeName = fileTypeName;
		}

		private IFileTypeComponentBuilder GetFileTypeComponentBuilder(string fileTypeDefinitionId)
		{
			IExtensionPoint extensionPoint = PluginManager.DefaultPluginRegistry.GetExtensionPoint<FileTypeComponentBuilderAttribute>();
			IFileTypeManager fileTypeManager = new LimitedFileTypeManager();
			foreach (IExtension extension in extensionPoint.Extensions)
			{
				IFileTypeComponentBuilder fileTypeComponentBuilder = (IFileTypeComponentBuilder)extension.CreateInstance();
				fileTypeComponentBuilder.FileTypeManager = fileTypeManager;
				IFileTypeInformation fileTypeInformation = fileTypeComponentBuilder.BuildFileTypeInformation(string.Empty);
				string id = fileTypeInformation.FileTypeDefinitionId.Id;
				if (object.Equals(id, fileTypeDefinitionId))
				{
					return fileTypeComponentBuilder;
				}
			}
			return null;
		}

		public virtual IFileTypeInformation BuildFileTypeInformation(string name)
		{
			IFileTypeInformation fileTypeInformation = _baseFileTypeComponentBuilder.BuildFileTypeInformation(name);
			if (_fileTypeDefinitionId != null)
			{
				fileTypeInformation.FileTypeDefinitionId = new FileTypeDefinitionId(_fileTypeDefinitionId);
			}
			if (_fileTypeName != null)
			{
				fileTypeInformation.FileTypeName = new LocalizableString(_fileTypeName);
			}
			fileTypeInformation.Enabled = true;
			fileTypeInformation.Hidden = false;
			return fileTypeInformation;
		}

		public virtual INativeFileSniffer BuildFileSniffer(string name)
		{
			return _baseFileTypeComponentBuilder.BuildFileSniffer(name);
		}

		public virtual IFileExtractor BuildFileExtractor(string name)
		{
			return _baseFileTypeComponentBuilder.BuildFileExtractor(name);
		}

		public virtual IFileGenerator BuildFileGenerator(string name)
		{
			return _baseFileTypeComponentBuilder.BuildFileGenerator(name);
		}

		public virtual IQuickTagsFactory BuildQuickTagsFactory(string name)
		{
			return _baseFileTypeComponentBuilder.BuildQuickTagsFactory(name);
		}

		public virtual IPreviewSetsFactory BuildPreviewSetsFactory(string name)
		{
			return _baseFileTypeComponentBuilder.BuildPreviewSetsFactory(name);
		}

		public virtual IVerifierCollection BuildVerifierCollection(string name)
		{
			return _baseFileTypeComponentBuilder.BuildVerifierCollection(name);
		}

		public virtual IAbstractPreviewApplication BuildPreviewApplication(string name)
		{
			return _baseFileTypeComponentBuilder.BuildPreviewApplication(name);
		}

		public virtual IAbstractPreviewControl BuildPreviewControl(string name)
		{
			return _baseFileTypeComponentBuilder.BuildPreviewControl(name);
		}

		public virtual IAbstractGenerator BuildAbstractGenerator(string name)
		{
			return _baseFileTypeComponentBuilder.BuildAbstractGenerator(name);
		}

		public virtual IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
		{
			return _baseFileTypeComponentBuilder.BuildAdditionalGeneratorsInfo(name);
		}

		public virtual IBilingualDocumentGenerator BuildBilingualGenerator(string name)
		{
			return _baseFileTypeComponentBuilder.BuildBilingualGenerator(name);
		}

		public void PopulateDefaultSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
		}

		public List<QuickInsertIds> BuildQuickInsertIdList()
		{
			return null;
		}
	}
}
