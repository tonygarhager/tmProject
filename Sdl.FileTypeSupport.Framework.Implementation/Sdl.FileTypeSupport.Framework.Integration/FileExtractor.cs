using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class FileExtractor : AbstractBilingualProcessorContainer, IFileExtractor, IBilingualProcessorContainer, IFileTypeDefinitionAware, ILocationMarkerLocator
	{
		internal class SubContentExtractorData
		{
			public ISubContentExtractor SubContentExtractor
			{
				get;
				set;
			}

			public IFileProperties FileProperties
			{
				get;
				set;
			}
		}

		private IFileTypeManager _manager;

		private IPersistentFileConversionProperties _FileConversionProperties = new PersistentFileConversionProperties();

		private INativeExtractor _NativeExtractor;

		private INativeToBilingualConverter _ToBilingualConverter = new NativeToBilingualConverter();

		private IBilingualParser _BilingualParser;

		private List<IFilePreTweaker> _FileTweakers = new List<IFilePreTweaker>();

		private List<SubContentExtractorData> _subContentExtractorsDataList = new List<SubContentExtractorData>();

		private bool _startFileSection = true;

		private IPersistentFileConversionProperties _originalFileProperties;

		private IPersistentFileConversionProperties _lastFileProperties;

		private InvalidEncodingRemovalVisitor _invalidEncodingRemovalVisitor = new InvalidEncodingRemovalVisitor();

		private LocationMarkerLocator _BilingualLocationMarkerLocator = new LocationMarkerLocator();

		private IDocumentProperties _DocumentInfo;

		private IFileProperties _FileInfo;

		private IFileProperties _LastFileInfo;

		private IDocumentItemFactory _itemFactory;

		private IPropertiesFactory _propertiesFactory = new PropertiesFactory();

		private bool _IsFirstCall = true;

		private bool _EndOfInputCalled;

		private EventHandler<ProgressEventArgs> _ProgressHandler;

		private BilingualMessageReporterToMessageEventAdapter _bilingualMessageReporterAdapter;

		private ISettingsBundle _settingsBundle;

		public LocationMarkerLocator BilingualLocationMarkerLocator
		{
			get
			{
				return _BilingualLocationMarkerLocator;
			}
			set
			{
				_BilingualLocationMarkerLocator = value;
			}
		}

		public List<IFilePreTweaker> FileTweakers => _FileTweakers;

		public IPersistentFileConversionProperties LastFileProperties => _lastFileProperties;

		public IPersistentFileConversionProperties NextFileProperties
		{
			get
			{
				_lastFileProperties = (IPersistentFileConversionProperties)_originalFileProperties.Clone();
				_lastFileProperties.DependencyFiles.Clear();
				return _lastFileProperties;
			}
		}

		IEnumerable<IFilePreTweaker> IFileExtractor.FileTweakers => _FileTweakers;

		protected override IBilingualContentHandler OutputHandler => _BilingualLocationMarkerLocator;

		public IDocumentProperties DocumentInfo
		{
			get
			{
				return _DocumentInfo;
			}
			set
			{
				_DocumentInfo = value;
			}
		}

		public IPersistentFileConversionProperties FileConversionProperties
		{
			get
			{
				return _FileConversionProperties;
			}
			set
			{
				_FileConversionProperties = value;
			}
		}

		public IBilingualParser BilingualParser
		{
			get
			{
				return _BilingualParser;
			}
			set
			{
				if (_BilingualParser != null)
				{
					_BilingualParser.Output = null;
				}
				_BilingualParser = value;
				ReconnectComponents();
			}
		}

		public INativeExtractor NativeExtractor
		{
			get
			{
				return _NativeExtractor;
			}
			set
			{
				if (_NativeExtractor != null)
				{
					_NativeExtractor.Output = null;
				}
				_NativeExtractor = value;
				ReconnectComponents();
			}
		}

		public IDocumentItemFactory ItemFactory
		{
			get
			{
				return _itemFactory;
			}
			set
			{
				_itemFactory = value;
			}
		}

		public INativeToBilingualConverter ToBilingualConverter
		{
			get
			{
				return _ToBilingualConverter;
			}
			set
			{
				if (_ToBilingualConverter != null)
				{
					_ToBilingualConverter.Output = null;
				}
				_ToBilingualConverter = value;
				ReconnectComponents();
			}
		}

		public IEnumerable<object> AllComponents
		{
			get
			{
				foreach (IFilePreTweaker fileTweaker in FileTweakers)
				{
					yield return fileTweaker;
				}
				if (UsesNativeParser)
				{
					foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
					{
						yield return nonBilingualProcessorComponent;
					}
				}
				else if (_BilingualParser != null)
				{
					yield return _BilingualParser;
				}
				foreach (IBilingualContentProcessor publicBilingualProcessor in base.PublicBilingualProcessors)
				{
					yield return publicBilingualProcessor;
				}
				if (_BilingualLocationMarkerLocator != null)
				{
					yield return _BilingualLocationMarkerLocator;
				}
			}
		}

		public bool UsesNativeParser
		{
			get
			{
				if (_NativeExtractor != null)
				{
					return _NativeExtractor.Parser != null;
				}
				return false;
			}
		}

		public ISettingsBundle SettingsBundle
		{
			get
			{
				return _settingsBundle;
			}
			set
			{
				_settingsBundle = value;
			}
		}

		public event EventHandler<MessageEventArgs> Message;

		public event EventHandler<ProgressEventArgs> Progress;

		public FileExtractor()
		{
			_bilingualMessageReporterAdapter = new BilingualMessageReporterToMessageEventAdapter(OnMessage);
		}

		public FileExtractor(IFileTypeManager manager)
			: this()
		{
			_manager = manager;
		}

		public void AddFileTweaker(IFilePreTweaker tweaker)
		{
			if (tweaker == null)
			{
				throw new ArgumentNullException("tweaker");
			}
			_FileTweakers.Add(tweaker);
		}

		public IFilePreTweaker[] GetFileTweakers()
		{
			return _FileTweakers.ToArray();
		}

		public void RemoveFileTweaker(IFilePreTweaker tweaker)
		{
			_FileTweakers.Remove(tweaker);
		}

		public virtual void OnProgress(object sender, ProgressEventArgs args)
		{
			if (this.Progress != null)
			{
				this.Progress(sender, args);
			}
		}

		public override void ReconnectComponents()
		{
			if (_BilingualParser != null)
			{
				_BilingualParser.Output = this;
			}
			if (_ToBilingualConverter != null)
			{
				_ToBilingualConverter.Output = this;
			}
			if (_NativeExtractor != null)
			{
				_NativeExtractor.Output = _ToBilingualConverter;
			}
			base.ReconnectComponents();
			_BilingualLocationMarkerLocator.Output = Output;
		}

		protected virtual IParser GetParser()
		{
			if (_NativeExtractor != null && _NativeExtractor.Parser != null)
			{
				return _NativeExtractor.Parser;
			}
			return _BilingualParser;
		}

		protected virtual void SetFrameworkPropertiesForComponents()
		{
			if (_BilingualParser != null)
			{
				_BilingualParser.DocumentProperties = _DocumentInfo;
			}
			if (_ToBilingualConverter != null)
			{
				_ToBilingualConverter.DocumentInfo = _DocumentInfo;
			}
			NativeMessageReporterToMessageEventAdapter messageReporter = new NativeMessageReporterToMessageEventAdapter(this, OnMessage);
			foreach (object allComponent in AllComponents)
			{
				INativeFileTypeComponent nativeFileTypeComponent = allComponent as INativeFileTypeComponent;
				if (nativeFileTypeComponent != null)
				{
					nativeFileTypeComponent.PropertiesFactory = ItemFactory.PropertiesFactory;
					nativeFileTypeComponent.MessageReporter = messageReporter;
				}
				IBilingualFileTypeComponent bilingualFileTypeComponent = allComponent as IBilingualFileTypeComponent;
				if (bilingualFileTypeComponent != null)
				{
					bilingualFileTypeComponent.ItemFactory = ItemFactory;
					bilingualFileTypeComponent.MessageReporter = _bilingualMessageReporterAdapter;
				}
				ISettingsAware settingsAware = allComponent as ISettingsAware;
				if (_settingsBundle != null)
				{
					settingsAware?.InitializeSettings(_settingsBundle, _FileConversionProperties.FileTypeDefinitionId.Id);
				}
			}
		}

		protected virtual List<object> GetNonBilingualProcessorComponents()
		{
			List<object> list = new List<object>();
			if (_NativeExtractor != null && _NativeExtractor.Parser != null)
			{
				list.Add(_NativeExtractor.Parser);
				foreach (INativeExtractionContentProcessor contentProcessor in _NativeExtractor.ContentProcessors)
				{
					list.Add(contentProcessor);
				}
				if (_ToBilingualConverter != null)
				{
					list.Add(_ToBilingualConverter);
				}
			}
			else if (_BilingualParser != null)
			{
				list.Add(_BilingualParser);
			}
			return list;
		}

		protected virtual void CallSetNativeFileProperties()
		{
			if (DocumentInfo != null)
			{
				_FileConversionProperties.SourceLanguage = DocumentInfo.SourceLanguage;
				_FileConversionProperties.TargetLanguage = DocumentInfo.TargetLanguage;
			}
			foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
			{
				CallSetNativeFileProperties(nonBilingualProcessorComponent as INativeContentCycleAware);
			}
		}

		protected void CallSetNativeFileProperties(INativeContentCycleAware aware)
		{
			if (aware != null)
			{
				if (_FileInfo == null)
				{
					_FileInfo = ItemFactory.CreateFileProperties();
					_FileInfo.FileConversionProperties = _FileConversionProperties;
				}
				aware.SetFileProperties(_FileInfo);
			}
		}

		protected void SetQuickInsertMetadata(IPersistentFileConversionProperties fileConversionProperties)
		{
			if (fileConversionProperties == null)
			{
				return;
			}
			_ = fileConversionProperties.FileTypeDefinitionId;
			if (_manager == null)
			{
				return;
			}
			IFileTypeDefinition fileTypeDefinition = _manager.FindFileTypeDefinition(fileConversionProperties.FileTypeDefinitionId);
			if (fileTypeDefinition != null)
			{
				List<QuickInsertIds> list = fileTypeDefinition.BuildQuickInsertIdsList();
				if (list.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (QuickInsertIds item in list)
					{
						stringBuilder.Append(item.ToString() + ";");
					}
					string value = stringBuilder.ToString().TrimEnd(';');
					fileConversionProperties.SetMetaData("SDL:QuickInsertsList", value);
				}
			}
		}

		protected virtual void CallInitialize()
		{
			base.Initialize(_DocumentInfo);
		}

		protected virtual void CallSetFileProperties()
		{
			if (_FileInfo == null)
			{
				_FileInfo = ItemFactory.CreateFileProperties();
				_FileInfo.FileConversionProperties = _FileConversionProperties;
			}
			base.SetFileProperties(_FileInfo);
		}

		protected virtual void CallFileComplete()
		{
			base.FileComplete();
		}

		protected virtual void CallComplete()
		{
			base.Complete();
		}

		protected virtual void CallStartOfInput()
		{
			foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
			{
				CallStartOfInput(nonBilingualProcessorComponent as INativeContentCycleAware);
			}
		}

		protected void CallStartOfInput(INativeContentCycleAware aware)
		{
			aware?.StartOfInput();
		}

		protected virtual void CallEndOfInput()
		{
			foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
			{
				CallEndOfInput(nonBilingualProcessorComponent as INativeContentCycleAware);
			}
		}

		protected void CallEndOfInput(INativeContentCycleAware aware)
		{
			aware?.EndOfInput();
		}

		public virtual void OnMessage(object sender, MessageEventArgs args)
		{
			if (this.Message != null)
			{
				if (args != null && string.IsNullOrEmpty(args.FilePath) && _FileConversionProperties != null)
				{
					args.FilePath = _FileConversionProperties.OriginalFilePath;
				}
				this.Message(sender, args);
			}
		}

		public void Parse()
		{
			while (ParseNext())
			{
			}
		}

		public bool ParseNext()
		{
			IParser parser = GetParser();
			if (parser == null)
			{
				throw new FileTypeSupportException(StringResources.FileExtractor_NoFileError);
			}
			if (_IsFirstCall)
			{
				SetQuickInsertMetadata(FileConversionProperties);
				SetFrameworkPropertiesForComponents();
				RunFileTweakersPreParsing(_FileConversionProperties);
				CallSetNativeFileProperties();
				CallStartOfInput();
				_ProgressHandler = OnProgress;
				parser.Progress += _ProgressHandler;
				OnProgress(this, new ProgressEventArgs(0));
				ISubContentPublisher subContentPublisher = parser as ISubContentPublisher;
				if (subContentPublisher != null)
				{
					subContentPublisher.ProcessSubContent += PublisherProcessSubContent;
				}
				_IsFirstCall = false;
			}
			bool flag = parser.ParseNext();
			if (!flag && _ProgressHandler != null)
			{
				OnProgress(this, new ProgressEventArgs(100));
				parser.Progress -= _ProgressHandler;
				_ProgressHandler = null;
			}
			if (!flag && !_EndOfInputCalled)
			{
				ISubContentPublisher subContentPublisher2 = parser as ISubContentPublisher;
				if (subContentPublisher2 != null)
				{
					subContentPublisher2.ProcessSubContent -= PublisherProcessSubContent;
				}
				if (_LastFileInfo != null)
				{
					_LastFileInfo.FileConversionProperties.SetMetaData("SDL_SUBCONTENT_FILE_END", "true");
				}
				CallEndOfInput();
				_EndOfInputCalled = true;
			}
			return flag;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			_invalidEncodingRemovalVisitor.ProcessParagraphUnit(paragraphUnit);
			base.ProcessParagraphUnit(paragraphUnit);
		}

		private void PublisherProcessSubContent(object sender, ProcessSubContentEventArgs args)
		{
			MarkSubContentBoundary(args.SubContentStream);
			IFileTypeDefinition fileTypeDefinition = _manager.FindFileTypeDefinition(new FileTypeDefinitionId(args.FileTypeDefinitionId));
			if (fileTypeDefinition == null)
			{
				throw new SubContentProcessorDoesNotExistException(StringResources.SubContentProcessorIsMissing);
			}
			ISubContentExtractor subContentExtractor = fileTypeDefinition.BuildSubContentExtractor();
			if (subContentExtractor != null)
			{
				if (_startFileSection)
				{
					FileConversionProperties.SetMetaData("SDL_SUBCONTENT_STREAM", "true");
					_originalFileProperties = (IPersistentFileConversionProperties)FileConversionProperties.Clone();
					FileConversionProperties.SetMetaData("SDL_SUBCONTENT_FILE_START", "true");
					_startFileSection = false;
				}
				subContentExtractor.Output = this;
				Output.FileComplete();
				IFileProperties fileProperties = _itemFactory.CreateFileProperties();
				_subContentExtractorsDataList.Add(new SubContentExtractorData
				{
					FileProperties = fileProperties,
					SubContentExtractor = subContentExtractor
				});
				subContentExtractor.ProcessSubContent += PublisherProcessSubContent;
				subContentExtractor.FileConversionProperties = NextFileProperties;
				_subContentExtractorsDataList[_subContentExtractorsDataList.Count - 1].FileProperties.FileConversionProperties = subContentExtractor.FileConversionProperties;
				subContentExtractor.FileConversionProperties.SetMetaData("SDL_SUBCONTENT_STREAM", "true");
				subContentExtractor.FileConversionProperties.SetMetaData("SDL_SUBCONTENT_STREAM_LEVEL", _subContentExtractorsDataList.Count.ToString());
				subContentExtractor.FileConversionProperties.SetMetaData("SDL_SUBCONTENT_STREAM_FLAG", "PARSING");
				subContentExtractor.FileConversionProperties.FileTypeDefinitionId = new FileTypeDefinitionId(args.FileTypeDefinitionId);
				SetQuickInsertMetadata(subContentExtractor.FileConversionProperties);
				subContentExtractor.ItemFactory = ItemFactory;
				subContentExtractor.DocumentInfo = DocumentInfo;
				subContentExtractor.SettingsBundle = SettingsBundle;
				ISubContentParser subContentParser = subContentExtractor.GetParser() as ISubContentParser;
				args.SubContentStream.Seek(0L, SeekOrigin.Begin);
				subContentParser.InitializeSubContentParser(args.SubContentStream);
				while (subContentExtractor.ParseNext())
				{
				}
				subContentExtractor.ProcessSubContent -= PublisherProcessSubContent;
				_subContentExtractorsDataList.RemoveAt(_subContentExtractorsDataList.Count - 1);
				if (_subContentExtractorsDataList.Count == 0)
				{
					_LastFileInfo = _itemFactory.CreateFileProperties();
					_LastFileInfo.FileConversionProperties = NextFileProperties;
					Output.SetFileProperties(_LastFileInfo);
				}
				else
				{
					Output.SetFileProperties(_subContentExtractorsDataList[_subContentExtractorsDataList.Count - 1].FileProperties);
				}
				MarkSubContentBoundary(null);
				_isInitialized = false;
			}
		}

		private void RunFileTweakersPreParsing(IPersistentFileConversionProperties properties)
		{
			IFilePreTweaker[] fileTweakers = GetFileTweakers();
			IFilePreTweaker[] array = fileTweakers;
			foreach (IFilePreTweaker filePreTweaker in array)
			{
				if (filePreTweaker.Enabled)
				{
					filePreTweaker.TweakFilePreParsing(properties, ItemFactory.PropertiesFactory);
				}
			}
		}

		protected void MarkSubContentBoundary(Stream subContentStream)
		{
			if (_subContentExtractorsDataList.Count > 0)
			{
				INativeSubContentExtractor nativeSubContentExtractor = _subContentExtractorsDataList[_subContentExtractorsDataList.Count - 1].SubContentExtractor.NativeSubContentExtractor;
				if (nativeSubContentExtractor != null)
				{
					IStructureTagProperties tagInfo = ItemFactory.PropertiesFactory.CreateStructureTagProperties("#SDL_SUBCONTENT_BOUNDARY#");
					nativeSubContentExtractor.Parser.Output.StructureTag(tagInfo);
				}
			}
			else if ((_NativeExtractor != null || _BilingualParser != null) && _ToBilingualConverter != null)
			{
				IStructureTagProperties tagInfo2 = ItemFactory.PropertiesFactory.CreateStructureTagProperties("#SDL_SUBCONTENT_BOUNDARY#");
				if (_NativeExtractor != null)
				{
					_NativeExtractor.Parser.Output.StructureTag(tagInfo2);
				}
				else
				{
					IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Structure);
					IStructureTag item = ItemFactory.CreateStructureTag(ItemFactory.PropertiesFactory.CreateStructureTagProperties("#SDL_SUBCONTENT_BOUNDARY#"));
					paragraphUnit.Source.Add(item);
					Output.ProcessParagraphUnit(paragraphUnit);
				}
			}
			if (subContentStream != null)
			{
				subContentStream.Seek(0L, SeekOrigin.Begin);
				StringBuilder stringBuilder = new StringBuilder();
				TextReader textReader = new StreamReader(subContentStream);
				stringBuilder.Append(textReader.ReadToEnd());
				IParagraphUnit paragraphUnit2 = ItemFactory.CreateParagraphUnit(LockTypeFlags.Structure);
				IStructureTag item2 = ItemFactory.CreateStructureTag(ItemFactory.PropertiesFactory.CreateStructureTagProperties("#SDL-SUBCONTENT-MARKER#"));
				IStructureTag item3 = ItemFactory.CreateStructureTag(ItemFactory.PropertiesFactory.CreateStructureTagProperties(stringBuilder.ToString()));
				paragraphUnit2.Source.Add(item2);
				paragraphUnit2.Source.Add(item3);
				Output.ProcessParagraphUnit(paragraphUnit2);
			}
		}

		public IMessageLocation GetLocation(LocationMarkerId marker)
		{
			NativeExtractionMessageLocation nativeExtractionMessageLocation = new NativeExtractionMessageLocation(marker);
			nativeExtractionMessageLocation.BilingualContentLocator = _BilingualLocationMarkerLocator;
			return nativeExtractionMessageLocation;
		}
	}
}
