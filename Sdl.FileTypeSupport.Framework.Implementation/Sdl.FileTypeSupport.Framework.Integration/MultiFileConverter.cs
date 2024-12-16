using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class MultiFileConverter : AbstractBilingualProcessorContainer, IMultiFileConverter, IBilingualProcessorContainer, IDisposable
	{
		private List<IFileExtractor> _extractors = new List<IFileExtractor>();

		private NativeGeneratorProvider _nativeGeneratorProvider;

		private OutputPropertiesProvider _outputPropertiesProvider;

		private DependencyFileLocator _dependencyFileLocator;

		private BilingualVerifiersProvider _bilingualVerifiersProvider;

		private IBilingualDocumentOutputProperties _bilingualDocumentOutputProperties;

		private BilingualDocumentOutputPropertiesProvider _bilingualDocumentOutputPropertiesProvider;

		private IBilingualDocumentGenerator _bilingualDocumentGenerator;

		private BilingualMessageReporterToMessageEventAdapter _bilingualMessageAdapter;

		private IDocumentItemFactory _itemFactory;

		private IDocumentProperties _documentInfo;

		private ISettingsBundle _settingsBundle;

		private ISharedObjects _sharedObjects = new SharedObjects();

		private IFileExtractor _currentExtractor;

		private int _nextExtractorIndex;

		private IFileGenerator _currentGenerator;

		private bool _firstCall = true;

		private bool _initializeCalled;

		private bool _completeCalled;

		private EventHandler<ProgressEventArgs> _progressHandler;

		private bool _disposed;

		public virtual IDocumentItemFactory ItemFactory
		{
			get
			{
				if (_itemFactory == null)
				{
					_itemFactory = CreateItemFactory();
					if (_itemFactory.PropertiesFactory == null)
					{
						_itemFactory.PropertiesFactory = CreatePropertiesFactory();
					}
				}
				return _itemFactory;
			}
			set
			{
				if (_itemFactory != value)
				{
					_documentInfo = null;
					_itemFactory = value;
				}
			}
		}

		public IDocumentProperties DocumentInfo
		{
			get
			{
				return _documentInfo;
			}
			set
			{
				_documentInfo = value;
			}
		}

		public NativeGeneratorProvider NativeGeneratorProvider
		{
			get
			{
				return _nativeGeneratorProvider;
			}
			set
			{
				_nativeGeneratorProvider = value;
			}
		}

		public BilingualVerifiersProvider BilingualVerifiersProvider
		{
			get
			{
				return _bilingualVerifiersProvider;
			}
			set
			{
				_bilingualVerifiersProvider = value;
			}
		}

		public OutputPropertiesProvider OutputPropertiesProvider
		{
			get
			{
				return _outputPropertiesProvider;
			}
			set
			{
				_outputPropertiesProvider = value;
			}
		}

		public BilingualDocumentOutputPropertiesProvider BilingualDocumentOutputPropertiesProvider
		{
			get
			{
				return _bilingualDocumentOutputPropertiesProvider;
			}
			set
			{
				_bilingualDocumentOutputPropertiesProvider = value;
			}
		}

		public IBilingualDocumentGenerator BilingualDocumentGenerator
		{
			get
			{
				return _bilingualDocumentGenerator;
			}
			set
			{
				_bilingualDocumentGenerator = value;
				if (_bilingualDocumentGenerator != null)
				{
					Output = _bilingualDocumentGenerator.Input;
					ReconnectComponents();
				}
				else
				{
					Output = null;
					ReconnectComponents();
				}
			}
		}

		public virtual IEnumerable<IFileExtractor> Extractors => _extractors;

		public bool DetectedLanguagesCorrespondToDocumentProperties
		{
			get
			{
				Pair<Language, DetectionLevel> detectedSourceLanguage = DetectedSourceLanguage;
				Pair<Language, DetectionLevel> detectedTargetLanguage = DetectedTargetLanguage;
				if (detectedSourceLanguage == null || detectedTargetLanguage == null)
				{
					return false;
				}
				if (!detectedSourceLanguage.First.Equals(DocumentInfo.SourceLanguage))
				{
					return false;
				}
				if (!detectedTargetLanguage.First.Equals(DocumentInfo.TargetLanguage))
				{
					return false;
				}
				return true;
			}
		}

		public Pair<Language, DetectionLevel> DetectedSourceLanguage
		{
			get
			{
				Pair<Language, DetectionLevel> pair = new Pair<Language, DetectionLevel>(new Language(), DetectionLevel.Unknown);
				foreach (IFileExtractor extractor in _extractors)
				{
					if (extractor != null && extractor.FileConversionProperties != null)
					{
						SniffInfo fileSnifferInfo = extractor.FileConversionProperties.FileSnifferInfo;
						if (fileSnifferInfo != null && !CombineLanguageInfo(pair, fileSnifferInfo.DetectedSourceLanguage))
						{
							return null;
						}
					}
				}
				return pair;
			}
		}

		public Pair<Language, DetectionLevel> DetectedTargetLanguage
		{
			get
			{
				Pair<Language, DetectionLevel> pair = new Pair<Language, DetectionLevel>(new Language(), DetectionLevel.Unknown);
				foreach (IFileExtractor extractor in _extractors)
				{
					if (extractor != null && extractor.FileConversionProperties != null)
					{
						SniffInfo fileSnifferInfo = extractor.FileConversionProperties.FileSnifferInfo;
						if (fileSnifferInfo != null && !CombineLanguageInfo(pair, fileSnifferInfo.DetectedTargetLanguage))
						{
							return null;
						}
					}
				}
				return pair;
			}
		}

		public IPropertiesFactory PropertiesFactory
		{
			get
			{
				if (_itemFactory != null)
				{
					return _itemFactory.PropertiesFactory;
				}
				return null;
			}
		}

		public ISharedObjects SharedObjects => _sharedObjects;

		public IBilingualDocumentOutputProperties BilingualDocumentOutputProperties => _bilingualDocumentOutputProperties;

		public DependencyFileLocator DependencyFileLocator
		{
			get
			{
				return _dependencyFileLocator;
			}
			set
			{
				_dependencyFileLocator = value;
			}
		}

		public event EventHandler<MessageEventArgs> Message;

		public event EventHandler<BatchProgressEventArgs> Progress;

		public MultiFileConverter()
		{
			_progressHandler = _CurrentExtractor_Progress;
			_bilingualMessageAdapter = new BilingualMessageReporterToMessageEventAdapter(OnMessage);
		}

		public MultiFileConverter(ISettingsBundle settingsBundle)
		{
			_progressHandler = _CurrentExtractor_Progress;
			_bilingualMessageAdapter = new BilingualMessageReporterToMessageEventAdapter(OnMessage);
			_settingsBundle = settingsBundle;
		}

		protected virtual IPropertiesFactory CreatePropertiesFactory()
		{
			return new PropertiesFactory();
		}

		protected virtual IDocumentItemFactory CreateItemFactory()
		{
			IDocumentItemFactory documentItemFactory = new DocumentItemFactory();
			documentItemFactory.PropertiesFactory = CreatePropertiesFactory();
			return documentItemFactory;
		}

		public IEnumerable<object> GetCommonBilingualProcessingComponents()
		{
			foreach (IBilingualContentProcessor bilingualProcessor in base.BilingualProcessors)
			{
				yield return bilingualProcessor;
			}
			if (_bilingualDocumentGenerator != null)
			{
				foreach (object allComponent in _bilingualDocumentGenerator.AllComponents)
				{
					yield return allComponent;
				}
			}
		}

		protected virtual void SetFrameworkPropertiesForCommonComponents()
		{
			foreach (object commonBilingualProcessingComponent in GetCommonBilingualProcessingComponents())
			{
				IBilingualFileTypeComponent bilingualFileTypeComponent = commonBilingualProcessingComponent as IBilingualFileTypeComponent;
				if (bilingualFileTypeComponent != null)
				{
					bilingualFileTypeComponent.ItemFactory = ItemFactory;
					bilingualFileTypeComponent.MessageReporter = _bilingualMessageAdapter;
				}
				ISettingsAware settingsAware = commonBilingualProcessingComponent as ISettingsAware;
				if (_settingsBundle != null && settingsAware != null)
				{
					string configurationId = null;
					if (_bilingualDocumentGenerator != null && _bilingualDocumentGenerator.FileTypeDefinition != null && _bilingualDocumentGenerator.FileTypeDefinition.FileTypeInformation != null)
					{
						configurationId = _bilingualDocumentGenerator.FileTypeDefinition.FileTypeInformation.FileTypeDefinitionId.Id;
					}
					settingsAware.InitializeSettings(_settingsBundle, configurationId);
				}
			}
		}

		protected virtual void DetachFromBilingualFileTypeComponents()
		{
			foreach (object commonBilingualProcessingComponent in GetCommonBilingualProcessingComponents())
			{
				IBilingualFileTypeComponent bilingualFileTypeComponent = commonBilingualProcessingComponent as IBilingualFileTypeComponent;
				if (bilingualFileTypeComponent != null)
				{
					bilingualFileTypeComponent.ItemFactory = ItemFactory;
					bilingualFileTypeComponent.MessageReporter = null;
				}
			}
		}

		protected virtual IEnumerable<object> GetCurrentComponents()
		{
			if (_currentExtractor != null)
			{
				foreach (object allComponent in _currentExtractor.AllComponents)
				{
					yield return allComponent;
				}
			}
			foreach (IBilingualContentProcessor bilingualProcessor in GetBilingualProcessors())
			{
				yield return bilingualProcessor;
			}
			if (_bilingualDocumentGenerator != null)
			{
				foreach (object allComponent2 in _bilingualDocumentGenerator.AllComponents)
				{
					yield return allComponent2;
				}
			}
			else if (_currentGenerator != null)
			{
				foreach (object allComponent3 in _currentGenerator.AllComponents)
				{
					yield return allComponent3;
				}
			}
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			if (DocumentInfo == null)
			{
				DocumentInfo = documentInfo;
				SynchronizeDocumentProperties();
			}
			InitializeBilingualDocumentOutputProperties();
			CallSetSharedObjects(GetCommonBilingualProcessingComponents());
			if (!_initializeCalled)
			{
				CallInitialize();
			}
		}

		private void InitializeBilingualDocumentOutputProperties()
		{
			if (_bilingualDocumentGenerator == null)
			{
				return;
			}
			IBilingualDocumentFileWriter bilingualDocumentFileWriter = _bilingualDocumentGenerator.Writer as IBilingualDocumentFileWriter;
			if (bilingualDocumentFileWriter != null)
			{
				if (_bilingualDocumentOutputPropertiesProvider == null)
				{
					throw new FileTypeSupportException("BilingualDocumentOutputPropertiesProvider must be set when using a bilingual document generator.");
				}
				IOutputFileInfo proposedBilingualFileInfoFromComponents = GetProposedBilingualFileInfoFromComponents();
				BilingualDocumentOutputProperties bilingualDocumentOutputProperties = new BilingualDocumentOutputProperties();
				_bilingualDocumentOutputPropertiesProvider(bilingualDocumentOutputProperties, DocumentInfo, proposedBilingualFileInfoFromComponents);
				SetBilingualDocumentOutputProperties(bilingualDocumentOutputProperties);
			}
		}

		private void SetBilingualDocumentOutputProperties(IBilingualDocumentOutputProperties outputProperties)
		{
			foreach (object commonBilingualProcessingComponent in GetCommonBilingualProcessingComponents())
			{
				(commonBilingualProcessingComponent as IBilingualDocumentOutputPropertiesAware)?.SetOutputProperties(outputProperties);
			}
			_bilingualDocumentOutputProperties = outputProperties;
		}

		private IOutputFileInfo GetProposedBilingualFileInfoFromComponents()
		{
			IOutputFileInfo outputFileInfo = new OutputFileInfo(ContentRestriction.Bilingual);
			foreach (object commonBilingualProcessingComponent in GetCommonBilingualProcessingComponents())
			{
				(commonBilingualProcessingComponent as IBilingualDocumentOutputPropertiesAware)?.GetProposedFileInfo(DocumentInfo, outputFileInfo);
			}
			return outputFileInfo;
		}

		public override void Complete()
		{
			FinishGenerator();
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			if (_currentGenerator != null && ((FileGenerator)_currentGenerator).SubContentFinalized)
			{
				((FileGenerator)_currentGenerator).SetFileProperties(fileInfo);
				return;
			}
			if (_currentGenerator != null && fileInfo.FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM_LEVEL") != null && fileInfo.FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM_FLAG") == null)
			{
				((FileGenerator)_currentGenerator).SetFileProperties(fileInfo);
				return;
			}
			FinishGenerator();
			if (fileInfo.FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM_FLAG") != null)
			{
				fileInfo.FileConversionProperties.RemoveMetaData("SDL_SUBCONTENT_STREAM_FLAG");
			}
			UpdateDocumentLanguages(fileInfo);
			if (_bilingualDocumentGenerator == null)
			{
				NextGenerator(fileInfo);
				if (_currentGenerator != null)
				{
					((FileGenerator)_currentGenerator).SettingsBundle = _settingsBundle;
				}
			}
			_bilingualMessageAdapter.FilePath = fileInfo.FileConversionProperties.OriginalFilePath;
			base.SetFileProperties(fileInfo);
		}

		private void UpdateDocumentLanguages(IFileProperties fileInfo)
		{
			if (_documentInfo.SourceLanguage != null && _documentInfo.SourceLanguage.IsValid)
			{
				if (!_documentInfo.SourceLanguage.Equals(fileInfo.FileConversionProperties.SourceLanguage))
				{
					string message = string.Format(StringResources.MultiFileConverter_FileDocumentSourceLanguageError, fileInfo.FileConversionProperties.SourceLanguage.IsoAbbreviation, _documentInfo.SourceLanguage.IsoAbbreviation);
					ReportMessage(fileInfo.FileConversionProperties.OriginalFilePath, StringResources.FileProcessing, ErrorLevel.Warning, message, null);
				}
			}
			else
			{
				_documentInfo.SourceLanguage = fileInfo.FileConversionProperties.SourceLanguage;
			}
			if (_documentInfo.TargetLanguage != null && _documentInfo.TargetLanguage.IsValid)
			{
				if (!_documentInfo.TargetLanguage.Equals(fileInfo.FileConversionProperties.TargetLanguage))
				{
					string message2 = string.Format(StringResources.MultiFileConverter_FileDocumentTargetLanguageError, fileInfo.FileConversionProperties.TargetLanguage.IsoAbbreviation, _documentInfo.TargetLanguage.IsoAbbreviation);
					ReportMessage(fileInfo.FileConversionProperties.OriginalFilePath, StringResources.FileProcessing, ErrorLevel.Warning, message2, null);
				}
			}
			else
			{
				_documentInfo.TargetLanguage = fileInfo.FileConversionProperties.TargetLanguage;
			}
		}

		private void NextGenerator(IFileProperties fileInfo)
		{
			if (_nativeGeneratorProvider != null)
			{
				_currentGenerator = _nativeGeneratorProvider(fileInfo.FileConversionProperties);
				if (_currentGenerator == null)
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, StringResources.MultiFileConverter_FilterDefinitionNotFoundError, fileInfo.FileConversionProperties.FileTypeDefinitionId.ToString()));
				}
				_currentGenerator.ItemFactory = ItemFactory;
				_currentGenerator.Message += OnMessage;
				Output = _currentGenerator.Input;
				ReconnectComponents();
				CallSetSharedObjects(_currentGenerator.AllComponents);
				_currentGenerator.Input.Initialize(DocumentInfo);
				SetNativeOutputProperties(fileInfo);
				WhitespaceBetweenSegmentsBilingualProcessor whitespaceBetweenSegmentsBilingualProcessor = new WhitespaceBetweenSegmentsBilingualProcessor(fileInfo.FileConversionProperties);
				whitespaceBetweenSegmentsBilingualProcessor.Initialize(DocumentInfo);
				whitespaceBetweenSegmentsBilingualProcessor.ItemFactory = ItemFactory;
				_currentGenerator.InsertBilingualProcessor(0, whitespaceBetweenSegmentsBilingualProcessor);
				((FileGenerator)_currentGenerator).ReconnectComponents();
				CallSetSettingsOnObjects(_currentGenerator.AllComponents, fileInfo);
			}
		}

		public void CallSetSharedObjects(IEnumerable<object> objects)
		{
			foreach (object @object in objects)
			{
				(@object as ISharedObjectsAware)?.SetSharedObjects(_sharedObjects);
			}
		}

		public void CallSetSettingsOnObjects(IEnumerable<object> objects, IFileProperties fileInfo)
		{
			foreach (object @object in objects)
			{
				ISettingsAware settingsAware = @object as ISettingsAware;
				if (_settingsBundle != null && settingsAware != null)
				{
					string configurationId = null;
					if (fileInfo.FileConversionProperties != null)
					{
						configurationId = fileInfo.FileConversionProperties.FileTypeDefinitionId.Id;
					}
					settingsAware.InitializeSettings(_settingsBundle, configurationId);
				}
			}
		}

		private void SetNativeOutputProperties(IFileProperties fileInfo)
		{
			if (_outputPropertiesProvider == null)
			{
				throw new FileTypeSupportException(StringResources.MultiFileConverter_NoOutputPropertiesProviderError);
			}
			IOutputFileInfo proposedFileInfoFromComponents = GetProposedFileInfoFromComponents(fileInfo, _currentGenerator.NativeOutputProperties.ContentRestriction);
			_outputPropertiesProvider(_currentGenerator.NativeOutputProperties, fileInfo.FileConversionProperties, proposedFileInfoFromComponents);
		}

		private IOutputFileInfo GetProposedFileInfoFromComponents(IFileProperties fileInfo, ContentRestriction contentRestriction)
		{
			IOutputFileInfo outputFileInfo = new OutputFileInfo(contentRestriction);
			foreach (object currentComponent in GetCurrentComponents())
			{
				(currentComponent as INativeOutputSettingsAware)?.GetProposedOutputFileInfo(fileInfo.FileConversionProperties, outputFileInfo);
			}
			return outputFileInfo;
		}

		public override void FileComplete()
		{
			base.FileComplete();
		}

		private void FinishGenerator()
		{
			if (_currentGenerator != null)
			{
				_currentGenerator.Input.Complete();
				Output = null;
				ReconnectComponents();
				_currentGenerator.Message -= OnMessage;
				_currentGenerator = null;
			}
		}

		public void OnMessage(object source, string origin, string filePath, ErrorLevel level, string message, IMessageLocation fromLocation, IMessageLocation uptoLocation)
		{
			MessageEventArgs args = new MessageEventArgs(filePath, origin, level, message, fromLocation, uptoLocation);
			OnMessage(source, args);
		}

		public virtual void OnMessage(object source, MessageEventArgs args)
		{
			if (this.Message != null)
			{
				this.Message(source, args);
			}
		}

		public virtual void OnProgress(string filePath, int fileNumber, int totalFiles, byte filePercentComplete)
		{
			if (this.Progress != null)
			{
				this.Progress(this, new BatchProgressEventArgs(filePath, fileNumber, totalFiles, filePercentComplete));
			}
		}

		public virtual void Parse()
		{
			try
			{
				while (ParseNext())
				{
				}
			}
			catch (Exception)
			{
				DisposeParser();
				throw;
			}
		}

		public virtual bool ParseNext()
		{
			if (_firstCall)
			{
				if (_extractors.Count == 0)
				{
					throw new FileTypeSupportException(StringResources.MultiFileConverter_NoFileExtractorsError);
				}
				SetFrameworkPropertiesForCommonComponents();
				_firstCall = false;
			}
			if (_currentExtractor == null && !NextExtractor())
			{
				if (!_completeCalled)
				{
					CallComplete();
				}
				return false;
			}
			if (!_currentExtractor.ParseNext())
			{
				FinishExtractor();
			}
			return true;
		}

		private bool NextExtractor()
		{
			if (_nextExtractorIndex >= _extractors.Count)
			{
				return false;
			}
			_currentExtractor = _extractors[_nextExtractorIndex++];
			if (_bilingualVerifiersProvider != null)
			{
				IList<IBilingualVerifier> list = _bilingualVerifiersProvider(_currentExtractor.FileConversionProperties.FileTypeDefinitionId);
				if (list != null)
				{
					foreach (IBilingualVerifier item in list)
					{
						_currentExtractor.AddBilingualProcessor(new BilingualContentHandlerAdapter(item));
					}
				}
			}
			_currentExtractor.FileRestriction = base.FileRestriction;
			_currentExtractor.Output = this;
			_currentExtractor.ItemFactory = ItemFactory;
			_currentExtractor.Message += OnMessage;
			_currentExtractor.Progress += _progressHandler;
			if (_currentExtractor.BilingualParser != null)
			{
				IBilingualDocumentParser bilingualDocumentParser = _currentExtractor.BilingualParser as IBilingualDocumentParser;
				if (bilingualDocumentParser != null)
				{
					bilingualDocumentParser.DependencyFileLocator = _dependencyFileLocator;
					bilingualDocumentParser.FileRestriction = base.FileRestriction;
				}
				if (_currentExtractor.BilingualParser.DocumentProperties != null)
				{
					if (_documentInfo == null)
					{
						DocumentInfo = _currentExtractor.BilingualParser.DocumentProperties;
					}
					else if (!DocumentInfo.Equals(_currentExtractor.BilingualParser.DocumentProperties))
					{
						ReportMessage(_currentExtractor.FileConversionProperties.OriginalFilePath, StringResources.FileProcessing, ErrorLevel.Warning, string.Format(StringResources.MultiFileConverter_DifferentDocumentPropertiessError, DocumentInfo.ToString(), _currentExtractor.BilingualParser.DocumentProperties.ToString()), null);
					}
				}
			}
			_currentExtractor.DocumentInfo = DocumentInfo;
			CallSetSharedObjects(_currentExtractor.AllComponents);
			return true;
		}

		private void DisposeParser()
		{
			if (_currentExtractor != null)
			{
				if (_currentExtractor.NativeExtractor != null && _currentExtractor.NativeExtractor.Parser != null)
				{
					_currentExtractor.NativeExtractor.Parser.Dispose();
				}
				if (_currentExtractor.BilingualParser != null)
				{
					_currentExtractor.BilingualParser.Dispose();
				}
			}
			if (_currentGenerator != null)
			{
				if (_currentGenerator.NativeGenerator != null && _currentGenerator.NativeGenerator.Writer != null)
				{
					_currentGenerator.NativeGenerator.Writer.Dispose();
				}
				if (_currentGenerator.BilingualWriter != null)
				{
					_currentGenerator.BilingualWriter.Dispose();
				}
			}
		}

		private void _CurrentExtractor_Progress(object sender, ProgressEventArgs percent)
		{
			OnProgress(_currentExtractor.FileConversionProperties.OriginalFilePath, _nextExtractorIndex - 1, _extractors.Count, percent.ProgressValue);
		}

		private void FinishExtractor()
		{
			_currentExtractor.Progress -= _progressHandler;
			_currentExtractor.Message -= OnMessage;
			_currentExtractor.Output = null;
			_currentExtractor = null;
		}

		private void CallComplete()
		{
			base.Complete();
			_completeCalled = true;
			DetachFromBilingualFileTypeComponents();
		}

		private void CallInitialize()
		{
			base.Initialize(DocumentInfo);
			_initializeCalled = true;
		}

		public virtual void AddExtractor(IFileExtractor extractor)
		{
			extractor.SettingsBundle = _settingsBundle;
			_extractors.Add(extractor);
		}

		public virtual void InsertExtractor(int index, IFileExtractor extractor)
		{
			extractor.SettingsBundle = _settingsBundle;
			_extractors.Insert(index, extractor);
		}

		public virtual bool RemoveExtractor(IFileExtractor extractor)
		{
			return _extractors.Remove(extractor);
		}

		private static bool CombineLanguageInfo(Pair<Language, DetectionLevel> valueToUpdate, Pair<Language, DetectionLevel> valueToInclude)
		{
			if (valueToInclude.First == null || !valueToInclude.First.IsValid)
			{
				return true;
			}
			if (valueToUpdate.First == null || !valueToUpdate.First.IsValid)
			{
				valueToUpdate.First = valueToInclude.First;
				valueToUpdate.Second = valueToInclude.Second;
				return true;
			}
			if (valueToInclude.Second > valueToUpdate.Second)
			{
				valueToUpdate.First = valueToInclude.First;
				valueToUpdate.Second = valueToInclude.Second;
				return true;
			}
			if (!valueToUpdate.First.Equals(valueToInclude.First))
			{
				return valueToInclude.Second < DetectionLevel.Certain;
			}
			return true;
		}

		public void SetDocumentInfo(IDocumentProperties newDocumentInfo, bool applyToAllExtractors)
		{
			_documentInfo = newDocumentInfo;
			if (applyToAllExtractors)
			{
				ApplyDocumentPropertiesToExtractors();
			}
		}

		public void SynchronizeDocumentProperties()
		{
			if (_extractors.Count > 0)
			{
				IBilingualParser bilingualParser = _extractors[0].BilingualParser;
				if (bilingualParser != null && bilingualParser.DocumentProperties != null)
				{
					SetDocumentInfo(bilingualParser.DocumentProperties, applyToAllExtractors: true);
					return;
				}
			}
			UpdateDocumentPropertiesFromExtractors();
			if (DetectedLanguagesCorrespondToDocumentProperties)
			{
				ApplyDocumentPropertiesToExtractors();
			}
		}

		public bool UpdateDocumentPropertiesFromExtractors()
		{
			Pair<Language, DetectionLevel> detectedSourceLanguage = DetectedSourceLanguage;
			Pair<Language, DetectionLevel> detectedTargetLanguage = DetectedTargetLanguage;
			if (detectedSourceLanguage == null || detectedTargetLanguage == null)
			{
				return false;
			}
			if (_documentInfo == null)
			{
				_documentInfo = ItemFactory.CreateDocumentProperties();
			}
			DocumentInfo.SourceLanguage = detectedSourceLanguage.First;
			DocumentInfo.TargetLanguage = detectedTargetLanguage.First;
			return true;
		}

		public void ApplyDocumentPropertiesToExtractors()
		{
			foreach (IFileExtractor extractor in _extractors)
			{
				if (extractor != null)
				{
					extractor.DocumentInfo = DocumentInfo;
					if (extractor.FileConversionProperties != null)
					{
						extractor.FileConversionProperties.SourceLanguage = DocumentInfo.SourceLanguage;
						extractor.FileConversionProperties.TargetLanguage = DocumentInfo.TargetLanguage;
					}
				}
			}
		}

		public void ReportMessage(string filePath, string origin, ErrorLevel level, string message, string locationDescription)
		{
			BilingualMessageLocation bilingualMessageLocation = new BilingualMessageLocation();
			bilingualMessageLocation.LocationDescription = locationDescription;
			OnMessage(this, origin, filePath, level, message, bilingualMessageLocation, null);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}
			if (disposing)
			{
				List<IFileExtractor> list = Extractors.ToList();
				foreach (IFileExtractor item in list)
				{
					if (item.NativeExtractor != null && item.NativeExtractor.Parser != null)
					{
						item.NativeExtractor.Parser.Dispose();
					}
					if (item.BilingualParser != null)
					{
						item.BilingualParser.Dispose();
					}
					item.Message -= OnMessage;
					RemoveExtractor(item);
				}
				if (_currentGenerator != null)
				{
					if (_currentGenerator.NativeGenerator != null && _currentGenerator.NativeGenerator.Writer != null)
					{
						_currentGenerator.NativeGenerator.Writer.Dispose();
					}
					if (_currentGenerator.BilingualWriter != null)
					{
						_currentGenerator.BilingualWriter.Dispose();
					}
					_currentGenerator.Message -= OnMessage;
				}
			}
			_disposed = true;
		}
	}
}
