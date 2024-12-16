using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class SubContentExtractor : AbstractBilingualProcessorContainer, ISubContentExtractor, IBilingualProcessorContainer, IFileTypeDefinitionAware, ILocationMarkerLocator
	{
		private IPersistentFileConversionProperties _FileConversionProperties = new PersistentFileConversionProperties();

		private INativeSubContentExtractor _NativeSubContentExtractor;

		private INativeToBilingualConverter _ToBilingualConverter = new NativeToBilingualConverter();

		private IBilingualParser _BilingualParser;

		private LocationMarkerLocator _BilingualLocationMarkerLocator = new LocationMarkerLocator();

		private IDocumentProperties _DocumentInfo;

		private IFileProperties _FileInfo;

		private IDocumentItemFactory _itemFactory;

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

		public INativeSubContentExtractor NativeSubContentExtractor
		{
			get
			{
				return _NativeSubContentExtractor;
			}
			set
			{
				if (_NativeSubContentExtractor != null)
				{
					_NativeSubContentExtractor.Output = null;
				}
				_NativeSubContentExtractor = value;
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
				if (_NativeSubContentExtractor != null)
				{
					return _NativeSubContentExtractor.Parser != null;
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

		public event EventHandler<ProcessSubContentEventArgs> ProcessSubContent;

		public SubContentExtractor()
		{
			_bilingualMessageReporterAdapter = new BilingualMessageReporterToMessageEventAdapter(OnMessage);
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
			if (_NativeSubContentExtractor != null)
			{
				_NativeSubContentExtractor.Output = _ToBilingualConverter;
			}
			base.ReconnectComponents();
			_BilingualLocationMarkerLocator.Output = Output;
		}

		public virtual IParser GetParser()
		{
			if (_NativeSubContentExtractor != null && _NativeSubContentExtractor.Parser != null)
			{
				return _NativeSubContentExtractor.Parser;
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
			if (_NativeSubContentExtractor != null && _NativeSubContentExtractor.Parser != null)
			{
				list.Add(_NativeSubContentExtractor.Parser);
				foreach (INativeExtractionContentProcessor contentProcessor in _NativeSubContentExtractor.ContentProcessors)
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
				SetFrameworkPropertiesForComponents();
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
				CallEndOfInput();
				_EndOfInputCalled = true;
			}
			return flag;
		}

		private void PublisherProcessSubContent(object sender, ProcessSubContentEventArgs args)
		{
			if (this.ProcessSubContent != null)
			{
				this.ProcessSubContent(sender, args);
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
