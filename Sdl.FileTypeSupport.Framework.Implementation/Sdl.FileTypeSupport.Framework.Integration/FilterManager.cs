using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class FilterManager : IFileTypeManager
	{
		private class FileTypeDefinitionComparer : IEqualityComparer<IFileTypeDefinition>
		{
			public bool Equals(IFileTypeDefinition x, IFileTypeDefinition y)
			{
				return x.FileTypeInformation.FileTypeDefinitionId.Equals(y.FileTypeInformation.FileTypeDefinitionId);
			}

			public int GetHashCode(IFileTypeDefinition obj)
			{
				return obj.GetHashCode();
			}
		}

		private List<IFileTypeDefinition> _FileTypeDefinitions = new List<IFileTypeDefinition>();

		private IFileTypeDefinition _DefaultBilingualFileTypeDefinition;

		private IFileTypeDefinitionFactory _definitionFactory;

		private string _configurationVersionInformation;

		public List<IFileTypeDefinition> FileTypeDefinitionsList => _FileTypeDefinitions;

		public ISettingsBundle SettingsBundle
		{
			get;
			set;
		}

		public IFileTypeDefinition[] FileTypeDefinitions => _FileTypeDefinitions.ToArray();

		public IFileTypeDefinition DefaultBilingualFileTypeDefinition
		{
			get
			{
				if (_DefaultBilingualFileTypeDefinition == null)
				{
					_DefaultBilingualFileTypeDefinition = _FileTypeDefinitions.Find((IFileTypeDefinition definition) => definition != null && definition.FileTypeInformation != null && definition.FileTypeInformation.Enabled && definition.FileTypeInformation.IsBilingualDocumentFileType);
				}
				return _DefaultBilingualFileTypeDefinition;
			}
			set
			{
				IFileTypeDefinition fileTypeDefinition = _FileTypeDefinitions.Find((IFileTypeDefinition item) => item == value);
				if (fileTypeDefinition == null)
				{
					throw new UnknownFileTypeDefinitionException(StringResources.FilterManager_MissingFilterDefinitionError);
				}
				if (!fileTypeDefinition.FileTypeInformation.IsBilingualDocumentFileType)
				{
					throw new NotBilingualFileTypeException(StringResources.FilterManager_NotBilingualFileTypeError);
				}
				_DefaultBilingualFileTypeDefinition = value;
			}
		}

		public string ConfigurationVersionInformation => _configurationVersionInformation;

		public IFileTypeDefinitionFactory FileTypeDefinitionFactory => _definitionFactory;

		public virtual List<string> AutoLoadedFileTypes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected static SniffInfo EvaluateFileTypeDefinitionForFile(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, IFileTypeDefinition filterDefinition, INativeTextLocationMessageReporter messageReporter, ISettingsBundle settingsBundle)
		{
			SniffInfo result = null;
			if (filterDefinition.ComponentBuilder is ISubContentComponentBuilder)
			{
				return result;
			}
			if (filterDefinition.IsSupportedFilename(nativeFilePath))
			{
				result = filterDefinition.SniffFile(nativeFilePath, suggestedSourceLanguage, suggestedCodepage, messageReporter, settingsBundle);
			}
			return result;
		}

		public Pair<IFileTypeDefinition, SniffInfo> GetBestMatchingFileTypeDefinition(string nativeFilePath, EventHandler<MessageEventArgs> messageHandler)
		{
			return GetBestMatchingFileTypeDefinition(nativeFilePath, new Language(), new Codepage(), messageHandler);
		}

		public IEnumerable<Pair<IFileTypeDefinition, SniffInfo>> GetAllMatchingFileTypeDefinitions(string nativeFilePath, EventHandler<MessageEventArgs> messageHandler)
		{
			return GetAllMatchingFileTypeDefinitions(nativeFilePath, new Language(), new Codepage(), messageHandler);
		}

		public IFileTypeDefinition FindFileTypeDefinition(FileTypeDefinitionId id)
		{
			Predicate<IFileTypeDefinition> match = (IFileTypeDefinition item) => item.FileTypeInformation.FileTypeDefinitionId == id;
			return _FileTypeDefinitions.Find(match);
		}

		protected virtual IMultiFileConverter CreateMultiFileConverter()
		{
			CheckForDuplicateFileTypes();
			return new MultiFileConverter(SettingsBundle);
		}

		private void CheckForDuplicateFileTypes()
		{
			List<FileTypeDefinitionId> list = new List<FileTypeDefinitionId>();
			foreach (IFileTypeDefinition fileTypeDefinition in _FileTypeDefinitions)
			{
				if (list.Contains(fileTypeDefinition.FileTypeInformation.FileTypeDefinitionId))
				{
					throw new Exception(string.Format(StringResources.DuplicateFilterIDsError, fileTypeDefinition.FileTypeInformation.FileTypeDefinitionId));
				}
				list.Add(fileTypeDefinition.FileTypeInformation.FileTypeDefinitionId);
			}
		}

		protected virtual IFileExtractor CreateExtractor()
		{
			return new FileExtractor(this);
		}

		public IMultiFileConverter GetConverter(string[] nativeFilePaths, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter multiFileConverter = CreateMultiFileConverter();
			AddExtractorsForConverter(multiFileConverter, nativeFilePaths, sourceLanguage, suggestedCodepage, messageHandler);
			multiFileConverter.SynchronizeDocumentProperties();
			if (messageHandler != null)
			{
				multiFileConverter.Message += messageHandler;
			}
			return multiFileConverter;
		}

		public IMultiFileConverter GetConverter(string fileTypeDefinitionId, string filePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter multiFileConverter = CreateMultiFileConverter();
			AddExtractorForConverter(multiFileConverter, fileTypeDefinitionId, filePath, sourceLanguage, suggestedCodepage, messageHandler);
			multiFileConverter.SynchronizeDocumentProperties();
			if (messageHandler != null)
			{
				multiFileConverter.Message += messageHandler;
			}
			return multiFileConverter;
		}

		protected void AddExtractorForConverter(IMultiFileConverter converter, string fileTypeDefinitionId, string filePath, CultureInfo suggestedSourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			PreExtractionMessageReporterAdapterForSniffers messageReporter = new PreExtractionMessageReporterAdapterForSniffers(messageHandler);
			List<FileNotSupportedException> list = new List<FileNotSupportedException>();
			try
			{
				IFileExtractor extractor = BuildExtractorNoSniffing(fileTypeDefinitionId, filePath, suggestedSourceLanguage, suggestedCodepage, messageReporter);
				converter.AddExtractor(extractor);
			}
			catch (FileNotSupportedException item)
			{
				list.Add(item);
				GenerateNotSupportedException(list);
			}
		}

		protected void AddExtractorsForConverter(IMultiFileConverter converter, string[] nativeFilePaths, CultureInfo suggestedSourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			PreExtractionMessageReporterAdapterForSniffers preExtractionMessageReporterAdapterForSniffers = new PreExtractionMessageReporterAdapterForSniffers(messageHandler);
			List<FileNotSupportedException> list = new List<FileNotSupportedException>();
			foreach (string filePath in nativeFilePaths)
			{
				try
				{
					preExtractionMessageReporterAdapterForSniffers.FilePath = filePath;
					IFileExtractor extractor = BuildExtractor(filePath, suggestedSourceLanguage, suggestedCodepage, preExtractionMessageReporterAdapterForSniffers);
					converter.AddExtractor(extractor);
				}
				catch (FileNotSupportedException item)
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				GenerateNotSupportedException(list);
			}
		}

		protected void GenerateNotSupportedException(List<FileNotSupportedException> fileNotSupportedExceptions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(StringResources.FilterManager_FileNotSupportedError);
			foreach (FileNotSupportedException fileNotSupportedException in fileNotSupportedExceptions)
			{
				stringBuilder.AppendFormat(" '{0}'", fileNotSupportedException.FilePath);
			}
			throw new OneOrMoreFilesNotSupportedException(stringBuilder.ToString(), fileNotSupportedExceptions);
		}

		public virtual IMultiFileConverter GetConverter(string bilingualFilePath, OutputPropertiesProvider outputSettingsProvider, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(new string[1]
			{
				bilingualFilePath
			}, new CultureInfo(""), new Codepage(), messageHandler);
			InitializeOutputSettings(converter, outputSettingsProvider);
			return converter;
		}

		private void InitializeOutputSettings(IMultiFileConverter converter, OutputPropertiesProvider outputSettingsProvider)
		{
			converter.OutputPropertiesProvider = outputSettingsProvider;
			if (outputSettingsProvider != null)
			{
				converter.NativeGeneratorProvider = BuildNativeGenerator;
			}
		}

		public virtual IFileExtractor BuildExtractorNoSniffing(string fileTypeDefinitionId, string filePath, CultureInfo suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter)
		{
			IFileTypeDefinition filterDefinitionToUse = _FileTypeDefinitions.Find((IFileTypeDefinition f) => f.FileTypeInformation.FileTypeDefinitionId.Id == fileTypeDefinitionId);
			SniffInfo info = null;
			Language suggestedLanguage = new Language(suggestedSourceLanguage);
			return BuildExtractorInternal(filterDefinitionToUse, filePath, info, suggestedLanguage, messageReporter);
		}

		public virtual IFileExtractor BuildExtractor(string filePath, CultureInfo suggestedSourceLanguage, Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter)
		{
			IFileTypeDefinition filterDefinitionToUse = null;
			SniffInfo sniffInfo = null;
			Language language = new Language(suggestedSourceLanguage);
			foreach (IFileTypeDefinition fileTypeDefinition in _FileTypeDefinitions)
			{
				if (fileTypeDefinition.FileTypeInformation.Enabled)
				{
					PreExtractionMessageReporterAdapterForSniffers preExtractionMessageReporterAdapterForSniffers = messageReporter as PreExtractionMessageReporterAdapterForSniffers;
					if (preExtractionMessageReporterAdapterForSniffers != null)
					{
						preExtractionMessageReporterAdapterForSniffers.CurrentFileTypeDefinitionId = fileTypeDefinition.FileTypeInformation.FileTypeDefinitionId;
					}
					sniffInfo = EvaluateFileTypeDefinitionForFile(filePath, language, suggestedCodepage, fileTypeDefinition, messageReporter, SettingsBundle);
					if (sniffInfo != null && sniffInfo.IsSupported)
					{
						filterDefinitionToUse = fileTypeDefinition;
						break;
					}
				}
			}
			return BuildExtractorInternal(filterDefinitionToUse, filePath, sniffInfo, language, messageReporter);
		}

		private IFileExtractor BuildExtractorInternal(IFileTypeDefinition filterDefinitionToUse, string filePath, SniffInfo info, Language suggestedLanguage, INativeTextLocationMessageReporter messageReporter)
		{
			if (filterDefinitionToUse == null)
			{
				if (!File.Exists(filePath))
				{
					throw new Exception(string.Format(StringResources.FilterManager_FileDoesNotExist, filePath));
				}
				string message = string.Format(StringResources.FilterManager_NoFileTypeSupportError, filePath);
				throw new FileNotSupportedException(message, filePath);
			}
			IFileExtractor fileExtractor = filterDefinitionToUse.BuildExtractor();
			fileExtractor.FileConversionProperties.FileTypeDefinitionId = filterDefinitionToUse.FileTypeInformation.FileTypeDefinitionId;
			fileExtractor.FileConversionProperties.FileSnifferInfo = info;
			fileExtractor.FileConversionProperties.OriginalFilePath = filePath;
			fileExtractor.FileConversionProperties.SourceLanguage = suggestedLanguage;
			if (info != null)
			{
				if (info.DetectedEncoding.Second >= DetectionLevel.Guess)
				{
					fileExtractor.FileConversionProperties.OriginalEncoding = info.DetectedEncoding.First;
				}
				if (info.DetectedSourceLanguage.Second >= DetectionLevel.Guess)
				{
					fileExtractor.FileConversionProperties.SourceLanguage = info.DetectedSourceLanguage.First;
				}
				else
				{
					info.DetectedSourceLanguage = new Pair<Language, DetectionLevel>(suggestedLanguage, DetectionLevel.Guess);
				}
				if (info.DetectedTargetLanguage.Second >= DetectionLevel.Guess)
				{
					fileExtractor.FileConversionProperties.TargetLanguage = info.DetectedTargetLanguage.First;
				}
			}
			PreExtractionMessageReporterAdapter preExtractionMessageReporterAdapter = messageReporter as PreExtractionMessageReporterAdapter;
			if (preExtractionMessageReporterAdapter != null)
			{
				INativeExtractionBilingualContentLocator bilingualContentLocator = null;
				FileExtractor fileExtractor2 = fileExtractor as FileExtractor;
				if (fileExtractor2 != null)
				{
					bilingualContentLocator = fileExtractor2.BilingualLocationMarkerLocator;
				}
				preExtractionMessageReporterAdapter.BilingualContentLocator = bilingualContentLocator;
				if (preExtractionMessageReporterAdapter.NativeLocations.Count > 0)
				{
					foreach (NativeExtractionMessageLocation messageLocation in preExtractionMessageReporterAdapter.MessageLocations)
					{
						messageLocation.BilingualContentLocator = bilingualContentLocator;
					}
				}
				if (fileExtractor.NativeExtractor != null && fileExtractor.NativeExtractor.Parser != null)
				{
					NativeExtractionMarkInserter nativeExtractionMarkInserter = new NativeExtractionMarkInserter();
					nativeExtractionMarkInserter.MarkLocations = preExtractionMessageReporterAdapter.NativeLocations;
					fileExtractor.NativeExtractor.InsertProcessor(0, nativeExtractionMarkInserter);
				}
				preExtractionMessageReporterAdapter.BilingualContentLocator = null;
			}
			foreach (IFilePreTweaker fileTweaker in fileExtractor.FileTweakers)
			{
				fileTweaker.MessageReporter = messageReporter;
			}
			return fileExtractor;
		}

		public void AddFileTypeDefinition(IFileTypeDefinition filterDefinition)
		{
			if (filterDefinition != null)
			{
				_FileTypeDefinitions.Add(filterDefinition);
			}
		}

		public void InsertFileTypeDefinition(int index, IFileTypeDefinition filterDefinition)
		{
			if (filterDefinition != null && index >= 0 && index <= _FileTypeDefinitions.Count)
			{
				_FileTypeDefinitions.Insert(index, filterDefinition);
			}
		}

		public void RemoveFileTypeDefinition(IFileTypeDefinition filterDefinition)
		{
			_FileTypeDefinitions.Remove(filterDefinition);
		}

		public void ClearFileTypeDefinitions()
		{
			_FileTypeDefinitions.Clear();
		}

		public IFileGenerator BuildNativeGenerator(IPersistentFileConversionProperties fileConversionProperties)
		{
			IFileTypeDefinition fileTypeDefinition = FindFileTypeDefinition(fileConversionProperties.FileTypeDefinitionId);
			if (fileTypeDefinition == null)
			{
				string message = string.Format(CultureInfo.CurrentCulture, StringResources.FilterManager_FileTypeDefinitionNotFoundError, fileConversionProperties.FileTypeDefinitionId.ToString());
				throw new FileTypeDefinitionNotFoundException(message, fileConversionProperties.FileTypeDefinitionId);
			}
			return fileTypeDefinition.BuildNativeGenerator();
		}

		public IFileGenerator BuildNativeGeneratorWithVerifiers(IPersistentFileConversionProperties fileConversionProperties)
		{
			IFileTypeDefinition fileTypeDefinition = FindFileTypeDefinition(fileConversionProperties.FileTypeDefinitionId);
			if (fileTypeDefinition == null)
			{
				string message = string.Format(CultureInfo.CurrentCulture, StringResources.FilterManager_FileTypeDefinitionNotFoundError, fileConversionProperties.FileTypeDefinitionId.ToString());
				throw new FileTypeDefinitionNotFoundException(message, fileConversionProperties.FileTypeDefinitionId);
			}
			IFileGenerator fileGenerator = fileTypeDefinition.BuildNativeGenerator();
			IVerifierCollection verifierCollection = fileTypeDefinition.BuildVerifierCollection();
			if (verifierCollection.NativeVerifiers != null)
			{
				foreach (INativeFileVerifier nativeVerifier in verifierCollection.NativeVerifiers)
				{
					fileGenerator.AddNativeVerifier(nativeVerifier);
				}
				return fileGenerator;
			}
			return fileGenerator;
		}

		public IMultiFileConverter GetConverterToBilingual(string[] nativeFilePaths, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(nativeFilePaths, sourceLanguage, suggestedCodepage, messageHandler);
			converter.BilingualDocumentGenerator = writer;
			converter.BilingualDocumentOutputPropertiesProvider = outputSettingsProvider;
			return converter;
		}

		public IMultiFileConverter GetConverterToDefaultBilingual(string[] nativeFilePaths, string bilingualOutputFilePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(nativeFilePaths, sourceLanguage, suggestedCodepage, messageHandler);
			converter.BilingualDocumentGenerator = BuildDefaultBilingualGenerator();
			SetBilingualOutputPropertiesProviderForFilename(converter, bilingualOutputFilePath);
			return converter;
		}

		public IMultiFileConverter GetConverterToDefaultBilingual(string fileTypeDefinitionId, string filePath, string bilingualOutputFilePath, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(fileTypeDefinitionId, filePath, sourceLanguage, suggestedCodepage, messageHandler);
			converter.BilingualDocumentGenerator = BuildDefaultBilingualGenerator();
			SetBilingualOutputPropertiesProviderForFilename(converter, bilingualOutputFilePath);
			return converter;
		}

		private void SetBilingualOutputPropertiesProviderForFilename(IMultiFileConverter converter, string bilingualOutputFilePath)
		{
			converter.BilingualDocumentOutputPropertiesProvider = delegate(IBilingualDocumentOutputProperties outputProperties, IDocumentProperties documentInfo, IOutputFileInfo suggestedOutputFileInfo)
			{
				outputProperties.OutputFilePath = bilingualOutputFilePath;
			};
		}

		public IMultiFileConverter GetConverterToNative(string[] nativeFilePaths, OutputPropertiesProvider outputSettingsProvider, CultureInfo sourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(nativeFilePaths, sourceLanguage, suggestedCodepage, messageHandler);
			InitializeOutputSettings(converter, outputSettingsProvider);
			return converter;
		}

		public IMultiFileConverter GetConverterToBilingual(string bilingualInputFilePath, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(bilingualInputFilePath, messageHandler);
			converter.BilingualDocumentGenerator = writer;
			converter.BilingualDocumentOutputPropertiesProvider = outputSettingsProvider;
			return converter;
		}

		public IMultiFileConverter GetConverterToDefaultBilingual(string bilingualInputFilePath, string bilingualOutputFilePath, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(bilingualInputFilePath, messageHandler);
			converter.BilingualDocumentGenerator = BuildDefaultBilingualGenerator();
			SetBilingualOutputPropertiesProviderForFilename(converter, bilingualOutputFilePath);
			return converter;
		}

		public IMultiFileConverter GetConverterToNative(string bilingualInputFilePath, OutputPropertiesProvider outputSettingsProvider, EventHandler<MessageEventArgs> messageHandler)
		{
			IMultiFileConverter converter = GetConverter(bilingualInputFilePath, messageHandler);
			InitializeOutputSettings(converter, outputSettingsProvider);
			return converter;
		}

		public IMultiFileConverter GetConverter(string bilingualInputFilePath, EventHandler<MessageEventArgs> messageHandler)
		{
			return GetConverter(bilingualInputFilePath, null, messageHandler);
		}

		public IMultiFileConverter GetConverterToBilingual(IBilingualParser bilingualParser, IBilingualDocumentGenerator writer, BilingualDocumentOutputPropertiesProvider outputSettingsProvider)
		{
			IMultiFileConverter converter = GetConverter(bilingualParser);
			converter.BilingualDocumentGenerator = writer;
			converter.BilingualDocumentOutputPropertiesProvider = outputSettingsProvider;
			return converter;
		}

		public IMultiFileConverter GetConverterToDefaultBilingual(IBilingualParser bilingualParser, string bilingualOutputFilePath)
		{
			IMultiFileConverter converter = GetConverter(bilingualParser);
			converter.BilingualDocumentGenerator = BuildDefaultBilingualGenerator();
			SetBilingualOutputPropertiesProviderForFilename(converter, bilingualOutputFilePath);
			return converter;
		}

		public IMultiFileConverter GetConverterToNative(IBilingualParser bilingualParser, OutputPropertiesProvider outputSettingsProvider)
		{
			IMultiFileConverter converter = GetConverter(bilingualParser);
			InitializeOutputSettings(converter, outputSettingsProvider);
			return converter;
		}

		public IMultiFileConverter GetConverter(IBilingualParser bilingualParser)
		{
			IFileExtractor fileExtractor = CreateExtractor();
			fileExtractor.BilingualParser = bilingualParser;
			IMultiFileConverter multiFileConverter = CreateMultiFileConverter();
			multiFileConverter.AddExtractor(fileExtractor);
			if (bilingualParser != null)
			{
				multiFileConverter.ItemFactory = bilingualParser.ItemFactory;
			}
			multiFileConverter.SynchronizeDocumentProperties();
			return multiFileConverter;
		}

		public IBilingualDocumentGenerator BuildDefaultBilingualGenerator()
		{
			if (DefaultBilingualFileTypeDefinition == null)
			{
				throw new NoDefaultBilingualFileTypeException(StringResources.FilterManager_NoDefaultBilingualFileTypeError);
			}
			return BuildBilingualGenerator(DefaultBilingualFileTypeDefinition);
		}

		public IBilingualDocumentGenerator BuildBilingualGenerator(IFileTypeDefinition filterDefinitionToUse)
		{
			if (filterDefinitionToUse == null)
			{
				throw new UnknownFileTypeDefinitionException(StringResources.FilterManager_NoFilterDefinitionSpecifiedError);
			}
			IBilingualDocumentGenerator bilingualDocumentGenerator = filterDefinitionToUse.BuildBilingualDocumentGenerator();
			bilingualDocumentGenerator.FileTypeDefinition = filterDefinitionToUse;
			return bilingualDocumentGenerator;
		}

		protected void SetFileTypeDefinitionFactory(IFileTypeDefinitionFactory factory)
		{
			_definitionFactory = factory;
		}

		protected void SetConfigurationVersionInformation(string info)
		{
			_configurationVersionInformation = info;
		}

		public IBilingualDocumentGenerator BuildBilingualGenerator(IBilingualDocumentWriter bilingualWriter)
		{
			BilingualDocumentGenerator bilingualDocumentGenerator = new BilingualDocumentGenerator();
			bilingualDocumentGenerator.Writer = bilingualWriter;
			return bilingualDocumentGenerator;
		}

		public IFileExtractor BuildFileExtractor(INativeExtractor nativeExtractor, IFileTypeComponentBuilder componentBuilder)
		{
			FileExtractor fileExtractor = new FileExtractor(this);
			fileExtractor.NativeExtractor = nativeExtractor;
			return fileExtractor;
		}

		public IFileExtractor BuildFileExtractor(IBilingualParser bilingualParser, IFileTypeComponentBuilder componentBuilder)
		{
			FileExtractor fileExtractor = new FileExtractor(this);
			fileExtractor.BilingualParser = bilingualParser;
			return fileExtractor;
		}

		public ISubContentExtractor BuildSubContentExtractor(IBilingualParser bilingualParser, IFileTypeComponentBuilder componentBuilder)
		{
			SubContentExtractor subContentExtractor = new SubContentExtractor();
			subContentExtractor.BilingualParser = bilingualParser;
			return subContentExtractor;
		}

		public ISubContentExtractor BuildSubContentExtractor(INativeSubContentExtractor nativeSubContentExtractor, IFileTypeComponentBuilder componentBuilder)
		{
			SubContentExtractor subContentExtractor = new SubContentExtractor();
			subContentExtractor.NativeSubContentExtractor = nativeSubContentExtractor;
			return subContentExtractor;
		}

		public INativeExtractor BuildNativeExtractor(INativeFileParser nativeFileParser)
		{
			NativeExtractor nativeExtractor = new NativeExtractor();
			nativeExtractor.Parser = nativeFileParser;
			return nativeExtractor;
		}

		public INativeSubContentExtractor BuildNativeSubContentExtractor(INativeFileParser nativeFileParser)
		{
			NativeSubContentExtractor nativeSubContentExtractor = new NativeSubContentExtractor();
			nativeSubContentExtractor.Parser = nativeFileParser;
			return nativeSubContentExtractor;
		}

		public IFileGenerator BuildFileGenerator(IBilingualWriter bilingualWriter)
		{
			FileGenerator fileGenerator = new FileGenerator(this);
			fileGenerator.BilingualWriter = bilingualWriter;
			return fileGenerator;
		}

		public IFileTypeInformation BuildFileTypeInformation()
		{
			return new FileTypeInformation();
		}

		public INativeGenerator BuildNativeGenerator(INativeFileWriter nativeFileWriter)
		{
			NativeGenerator nativeGenerator = new NativeGenerator();
			nativeGenerator.Writer = nativeFileWriter;
			return nativeGenerator;
		}

		public IFileGenerator BuildFileGenerator(INativeGenerator nativeGenerator)
		{
			FileGenerator fileGenerator = new FileGenerator(this);
			fileGenerator.NativeGenerator = nativeGenerator;
			return fileGenerator;
		}

		public ISubContentGenerator BuildSubContentGenerator(IBilingualWriter bilingualWriter)
		{
			SubContentGenerator subContentGenerator = new SubContentGenerator();
			subContentGenerator.BilingualWriter = bilingualWriter;
			return subContentGenerator;
		}

		public INativeSubContentGenerator BuildNativeSubContentGenerator(INativeFileWriter nativeFileWriter)
		{
			NativeSubContentGenerator nativeSubContentGenerator = new NativeSubContentGenerator();
			nativeSubContentGenerator.Writer = nativeFileWriter;
			return nativeSubContentGenerator;
		}

		public ISubContentGenerator BuildSubContentGenerator(INativeSubContentGenerator nativeSubContentGenerator)
		{
			SubContentGenerator subContentGenerator = new SubContentGenerator();
			subContentGenerator.NativeSubContentGenerator = nativeSubContentGenerator;
			return subContentGenerator;
		}

		public IPreviewSetsFactory BuildPreviewSetsFactory()
		{
			return new AgnosticPreviewSetsFactory();
		}

		public IQuickTagsFactory BuildQuickTagsFactory()
		{
			return new AgnosticQuickTagsFactory();
		}

		public IVerifierCollection BuildVerifierCollection()
		{
			return new VerifierCollection();
		}

		public IGeneratorInfo BuildGeneratorInfo()
		{
			return new GeneratorInfo();
		}

		public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(params IGeneratorInfo[] generators)
		{
			return new AdditionalGeneratorsInfo(generators);
		}

		public virtual IFileTypeDefinition CreateFileTypeDefinition(string fileTypeId, FileTypeProfile profileOverride)
		{
			throw new NotSupportedException("This FilterManager implementation does not support this method: SetStandardFiletypeDefinition. Use PocoFilterManager if needed.");
		}

		public virtual IFileTypeDefinition CreateFileTypeDefinition(IFileTypeComponentBuilder componentBuilder, FileTypeProfile profileOverride)
		{
			throw new NotSupportedException("This FilterManager implementation does not support this method: SetStandardFiletypeDefinition. Use PocoFilterManager if needed.");
		}

		public virtual IFileTypeDefinition CreateFileTypeDefinition(string fileTypeDefinitionId)
		{
			throw new NotSupportedException("This FilterManager implementation does not support this method: SetStandardFiletypeDefinition. Use PocoFilterManager if needed.");
		}

		public IEnumerable<Pair<IFileTypeDefinition, SniffInfo>> GetAllMatchingFileTypeDefinitions(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			PreExtractionMessageReporterAdapterForSniffers preExtractionMessageReporterAdapterForSniffers = new PreExtractionMessageReporterAdapterForSniffers(messageHandler);
			List<Pair<IFileTypeDefinition, SniffInfo>> list = new List<Pair<IFileTypeDefinition, SniffInfo>>();
			if (new FileInfo(nativeFilePath).Length == 0L)
			{
				preExtractionMessageReporterAdapterForSniffers.ReportMessage(this, StringResources.FileSniffingProcess, ErrorLevel.Error, StringResources.EmptyFileMessage, null);
				return list;
			}
			foreach (IFileTypeDefinition fileTypeDefinition in _FileTypeDefinitions)
			{
				if (fileTypeDefinition.FileTypeInformation.Enabled && !fileTypeDefinition.FileTypeInformation.Hidden)
				{
					preExtractionMessageReporterAdapterForSniffers.FilePath = nativeFilePath;
					preExtractionMessageReporterAdapterForSniffers.CurrentFileTypeDefinitionId = fileTypeDefinition.FileTypeInformation.FileTypeDefinitionId;
					SniffInfo sniffInfo = EvaluateFileTypeDefinitionForFile(nativeFilePath, suggestedSourceLanguage, suggestedCodepage, fileTypeDefinition, preExtractionMessageReporterAdapterForSniffers, SettingsBundle);
					if (sniffInfo != null && sniffInfo.IsSupported)
					{
						list.Add(new Pair<IFileTypeDefinition, SniffInfo>(fileTypeDefinition, sniffInfo));
					}
				}
			}
			return list;
		}

		public Pair<IFileTypeDefinition, SniffInfo> GetBestMatchingFileTypeDefinition(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage, EventHandler<MessageEventArgs> messageHandler)
		{
			PreExtractionMessageReporterAdapterForSniffers preExtractionMessageReporterAdapterForSniffers = new PreExtractionMessageReporterAdapterForSniffers(messageHandler);
			if (new FileInfo(nativeFilePath).Length == 0L)
			{
				preExtractionMessageReporterAdapterForSniffers.ReportMessage(this, StringResources.FileSniffingProcess, ErrorLevel.Error, StringResources.EmptyFileMessage, null);
				return null;
			}
			foreach (IFileTypeDefinition fileTypeDefinition in _FileTypeDefinitions)
			{
				if (fileTypeDefinition.FileTypeInformation.Enabled && !fileTypeDefinition.FileTypeInformation.Hidden)
				{
					preExtractionMessageReporterAdapterForSniffers.FilePath = nativeFilePath;
					preExtractionMessageReporterAdapterForSniffers.CurrentFileTypeDefinitionId = fileTypeDefinition.FileTypeInformation.FileTypeDefinitionId;
					SniffInfo sniffInfo = EvaluateFileTypeDefinitionForFile(nativeFilePath, suggestedSourceLanguage, suggestedCodepage, fileTypeDefinition, preExtractionMessageReporterAdapterForSniffers, SettingsBundle);
					if (sniffInfo != null && sniffInfo.IsSupported)
					{
						return new Pair<IFileTypeDefinition, SniffInfo>(fileTypeDefinition, sniffInfo);
					}
				}
			}
			return null;
		}
	}
}
