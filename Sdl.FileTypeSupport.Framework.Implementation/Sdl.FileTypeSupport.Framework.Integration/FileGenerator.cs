using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class FileGenerator : AbstractBilingualProcessorContainer, IFileGenerator, IBilingualProcessorContainer, IAbstractGenerator, IFileTypeDefinitionAware, ILocationMarkerLocator
	{
		private IFileTypeManager _manager;

		private NativeWriterMessagesProxy _nativeWriterProxy;

		private GenerationBilingualContentLocator _bilingualContentLocator = new GenerationBilingualContentLocator();

		private INativeOutputFileProperties _nativeOutputProperties = new NativeOutputFileProperties();

		private List<IFilePostTweaker> _FileTweakers = new List<IFilePostTweaker>();

		private List<INativeFileVerifier> _NativeFileVerifiers = new List<INativeFileVerifier>();

		private IBilingualToNativeConverter _toNativeConverter = new BilingualToNativeConverter();

		private INativeGenerator _nativeGenerator;

		private IBilingualWriter _bilingualWriter;

		private InvalidEncodingRemovalVisitor _invalidEncodingRemovalVisitor = new InvalidEncodingRemovalVisitor();

		private ISettingsBundle _settingsBundle;

		private bool _frameworkPropertiesSet;

		private bool _hasSubcontent;

		private bool _isEndOfFile;

		private BilingualMessageReporterToMessageEventAdapter _bilingualMessageReporterAdapter;

		private List<ISubContentGenerator> _subContentGenerators = new List<ISubContentGenerator>();

		private StringBuilder _currentSubContent;

		private List<MemoryStream> _streamsToClose = new List<MemoryStream>();

		private IDocumentItemFactory _itemFactory;

		private IDocumentProperties _documentInfo;

		public List<IFilePostTweaker> FileTweakers => _FileTweakers;

		IEnumerable<IFilePostTweaker> IFileGenerator.FileTweakers => _FileTweakers;

		IEnumerable<INativeFileVerifier> IFileGenerator.NativeVerifiers => _NativeFileVerifiers;

		public ISettingsBundle SettingsBundle
		{
			set
			{
				_settingsBundle = value;
			}
		}

		protected override IBilingualContentHandler FirstHandler => _bilingualContentLocator;

		public bool SubContentFinalized
		{
			get;
			set;
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

		public INativeOutputFileProperties NativeOutputProperties
		{
			get
			{
				return _nativeOutputProperties;
			}
			set
			{
				_nativeOutputProperties = value;
			}
		}

		public INativeGenerator NativeGenerator
		{
			get
			{
				return _nativeGenerator;
			}
			set
			{
				_nativeGenerator = value;
				ReconnectComponents();
			}
		}

		public IBilingualToNativeConverter ToNativeConverter
		{
			get
			{
				return _toNativeConverter;
			}
			set
			{
				if (_toNativeConverter != null)
				{
					_toNativeConverter.Output = null;
				}
				_toNativeConverter = value;
				ReconnectComponents();
			}
		}

		public IBilingualContentHandler Input => this;

		public IEnumerable<object> AllComponents
		{
			get
			{
				if (_bilingualContentLocator != null)
				{
					yield return _bilingualContentLocator;
				}
				foreach (IBilingualContentProcessor publicBilingualProcessor in base.PublicBilingualProcessors)
				{
					yield return publicBilingualProcessor;
				}
				if (_bilingualWriter != null)
				{
					yield return _bilingualWriter;
				}
				else
				{
					foreach (object nativeComponent in GetNativeComponents())
					{
						yield return nativeComponent;
					}
				}
				foreach (IFilePostTweaker fileTweaker in FileTweakers)
				{
					yield return fileTweaker;
				}
				INativeFileVerifier[] nativeVerifiers = GetNativeVerifiers();
				for (int i = 0; i < nativeVerifiers.Length; i++)
				{
					yield return nativeVerifiers[i];
				}
			}
		}

		public IBilingualWriter BilingualWriter
		{
			get
			{
				return _bilingualWriter;
			}
			set
			{
				_bilingualWriter = value;
				ReconnectComponents();
			}
		}

		public event EventHandler<MessageEventArgs> Message;

		public FileGenerator()
		{
			_bilingualMessageReporterAdapter = new BilingualMessageReporterToMessageEventAdapter(OnMessage);
		}

		public FileGenerator(IFileTypeManager manager)
			: this()
		{
			_manager = manager;
		}

		public void AddFileTweaker(IFilePostTweaker tweaker)
		{
			_FileTweakers.Add(tweaker);
		}

		public IFilePostTweaker[] GetFileTweakers()
		{
			return _FileTweakers.ToArray();
		}

		public void RemoveFileTweaker(IFilePostTweaker tweaker)
		{
			_FileTweakers.Remove(tweaker);
		}

		public void AddNativeVerifier(INativeFileVerifier verifier)
		{
			_NativeFileVerifiers.Add(verifier);
		}

		public INativeFileVerifier[] GetNativeVerifiers()
		{
			return _NativeFileVerifiers.ToArray();
		}

		public override void ReconnectComponents()
		{
			if (_bilingualWriter != null)
			{
				Output = _bilingualWriter;
			}
			else
			{
				if (_nativeGenerator != null)
				{
					if (_toNativeConverter != null)
					{
						_toNativeConverter.Output = _nativeGenerator.Input;
					}
				}
				else if (_toNativeConverter != null)
				{
					_toNativeConverter.Output = null;
				}
				if (_toNativeConverter.Output != null)
				{
					Output = _toNativeConverter;
				}
			}
			base.ReconnectComponents();
			_bilingualContentLocator.Output = base.FirstHandler;
		}

		protected virtual void SetFrameworkPropertiesForComponents()
		{
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
			}
			_frameworkPropertiesSet = true;
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			if (!_frameworkPropertiesSet)
			{
				SetFrameworkPropertiesForComponents();
			}
			base.Initialize(documentInfo);
			_documentInfo = documentInfo;
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			if (fileInfo.FileConversionProperties.GetMetaData("SDL_SUBCONTENT_FILE_END") != null)
			{
				_isEndOfFile = true;
			}
			if (fileInfo.FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM") != null)
			{
				_hasSubcontent = true;
			}
			else
			{
				_hasSubcontent = false;
			}
			string metaData = fileInfo.FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM_LEVEL");
			if (metaData != null)
			{
				int num = int.Parse(metaData);
				if (num > _subContentGenerators.Count)
				{
					IFileTypeDefinition fileTypeDefinition = _manager.FindFileTypeDefinition(fileInfo.FileConversionProperties.FileTypeDefinitionId);
					if (fileTypeDefinition == null)
					{
						throw new SubContentProcessorDoesNotExistException(StringResources.SubContentProcessorIsMissing);
					}
					ISubContentGenerator subContentGenerator = fileTypeDefinition.BuildSubContentGenerator();
					if (subContentGenerator != null)
					{
						_subContentGenerators.Add(subContentGenerator);
						ISubContentWriter subContentWriter = null;
						if (subContentGenerator.BilingualWriter != null)
						{
							subContentWriter = (subContentGenerator.BilingualWriter as ISubContentWriter);
						}
						else if (subContentGenerator.NativeSubContentGenerator != null)
						{
							subContentWriter = (subContentGenerator.NativeSubContentGenerator.Writer as ISubContentWriter);
						}
						if (subContentWriter != null)
						{
							(subContentWriter as ISettingsAware)?.InitializeSettings(_settingsBundle, fileInfo.FileConversionProperties.FileTypeDefinitionId.Id);
							MemoryStream memoryStream = new MemoryStream();
							_streamsToClose.Add(memoryStream);
							TextWriter textWriter = new StreamWriter(memoryStream);
							textWriter.Write(_currentSubContent.ToString());
							textWriter.Flush();
							memoryStream.Seek(0L, SeekOrigin.Begin);
							subContentWriter.InitializeSubContentWriter(memoryStream);
						}
						((SubContentGenerator)subContentGenerator).ItemFactory = ItemFactory;
						((SubContentGenerator)subContentGenerator).Initialize(_documentInfo);
						((SubContentGenerator)subContentGenerator).NativeOutputProperties = NativeOutputProperties;
						((SubContentGenerator)subContentGenerator).SetFileProperties(fileInfo);
						WhitespaceBetweenSegmentsBilingualProcessor whitespaceBetweenSegmentsBilingualProcessor = new WhitespaceBetweenSegmentsBilingualProcessor(fileInfo.FileConversionProperties);
						whitespaceBetweenSegmentsBilingualProcessor.Initialize(_documentInfo);
						whitespaceBetweenSegmentsBilingualProcessor.ItemFactory = ItemFactory;
						((SubContentGenerator)subContentGenerator).InsertBilingualProcessor(0, whitespaceBetweenSegmentsBilingualProcessor);
						((SubContentGenerator)subContentGenerator).ReconnectComponents();
					}
				}
				else
				{
					((SubContentGenerator)_subContentGenerators[_subContentGenerators.Count - 1]).FileComplete();
					SubContentFinalized = true;
					ISubContentAware mainWriter = null;
					ISubContentGenerator subContentGenerator2 = _subContentGenerators[_subContentGenerators.Count - 2];
					if (subContentGenerator2.BilingualWriter != null)
					{
						mainWriter = (subContentGenerator2.BilingualWriter as ISubContentAware);
					}
					else if (subContentGenerator2.NativeSubContentGenerator != null)
					{
						NativeWriterMessagesProxy nativeWriterMessagesProxy = subContentGenerator2.NativeSubContentGenerator.Writer as NativeWriterMessagesProxy;
						mainWriter = (nativeWriterMessagesProxy.RealWriter as ISubContentAware);
					}
					ProcessSubContentWriting(mainWriter);
				}
				ReconnectSubContentComponents();
			}
			else if (_subContentGenerators.Count > 0)
			{
				((SubContentGenerator)_subContentGenerators[_subContentGenerators.Count - 1]).FileComplete();
				ISubContentAware mainWriter2 = null;
				if (BilingualWriter != null)
				{
					mainWriter2 = (BilingualWriter as ISubContentAware);
				}
				else if (NativeGenerator != null)
				{
					NativeWriterMessagesProxy nativeWriterMessagesProxy2 = NativeGenerator.Writer as NativeWriterMessagesProxy;
					mainWriter2 = (nativeWriterMessagesProxy2.RealWriter as ISubContentAware);
				}
				ProcessSubContentWriting(mainWriter2);
				SubContentFinalized = false;
				ReconnectComponents();
			}
			else
			{
				SubContentFinalized = false;
				if (_nativeGenerator != null && _nativeGenerator.Writer != null && !(_nativeGenerator.Writer is NativeWriterMessagesProxy))
				{
					_nativeWriterProxy = new NativeWriterMessagesProxy(_nativeGenerator.Writer, OnMessage, _bilingualContentLocator);
					_nativeGenerator.Writer = _nativeWriterProxy;
					_nativeWriterProxy.RealWriter.LocationTracker = _nativeWriterProxy.LocationTracker;
					PostGenerationMessageReporterAdapter messageReporter = new PostGenerationMessageReporterAdapter(OnMessage, _nativeWriterProxy.LocationTracker, _nativeWriterProxy.BilingualContentLocator);
					_nativeWriterProxy.MessageReporter = messageReporter;
				}
				CallSetOutputProperties();
				base.SetFileProperties(fileInfo);
				CallSetNativeFileProperties(fileInfo);
				CallStartOfInput();
			}
		}

		public void ProcessSubContentWriting(ISubContentAware mainWriter)
		{
			ISubContentGenerator subContentGenerator = _subContentGenerators[_subContentGenerators.Count - 1];
			((SubContentGenerator)subContentGenerator).Complete();
			ISubContentWriter subContentWriter = null;
			if (subContentGenerator.BilingualWriter != null)
			{
				subContentWriter = (subContentGenerator.BilingualWriter as ISubContentWriter);
			}
			else if (subContentGenerator.NativeSubContentGenerator != null)
			{
				NativeWriterMessagesProxy nativeWriterMessagesProxy = subContentGenerator.NativeSubContentGenerator.Writer as NativeWriterMessagesProxy;
				subContentWriter = (nativeWriterMessagesProxy.RealWriter as ISubContentWriter);
			}
			if (subContentWriter != null && mainWriter != null)
			{
				using (Stream stream = subContentWriter.GetSubContentStream())
				{
					stream.Seek(0L, SeekOrigin.Begin);
					mainWriter.AddSubContent(stream);
				}
			}
			_subContentGenerators.RemoveAt(_subContentGenerators.Count - 1);
		}

		protected void MarkStructureBoundary()
		{
			IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Structure);
			IStructureTag item = ItemFactory.CreateStructureTag(ItemFactory.PropertiesFactory.CreateStructureTagProperties("#SDL-SUBCONTENT-MARKER#"));
			paragraphUnit.Source.Add(item);
			ProcessParagraphUnit(paragraphUnit);
		}

		public override void FileComplete()
		{
			if (_subContentGenerators.Count > 0)
			{
				if (((SubContentGenerator)_subContentGenerators[_subContentGenerators.Count - 1]).BilingualWriter == null)
				{
					MarkStructureBoundary();
				}
				if (_subContentGenerators.Count == 1)
				{
					SubContentFinalized = true;
				}
				return;
			}
			if (_hasSubcontent && _isEndOfFile)
			{
				base.FileComplete();
			}
			else if (!_hasSubcontent)
			{
				base.FileComplete();
			}
			if (!_hasSubcontent)
			{
				CallEndOfInput();
				RunPostWritingOperations();
			}
		}

		public override void Complete()
		{
			if (_hasSubcontent)
			{
				CallEndOfInput();
				RunPostWritingOperations();
			}
			base.Complete();
			foreach (MemoryStream item in _streamsToClose)
			{
				item.Close();
			}
		}

		protected void ReconnectSubContentComponents()
		{
			Output = _subContentGenerators[_subContentGenerators.Count - 1].Input;
			base.ReconnectComponents();
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			_invalidEncodingRemovalVisitor.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure && paragraphUnit.Source.Count == 2 && paragraphUnit.Source[0] is IStructureTag)
			{
				IStructureTag structureTag = paragraphUnit.Source[0] as IStructureTag;
				if (structureTag.TagProperties.TagContent == "#SDL-SUBCONTENT-MARKER#")
				{
					IStructureTag structureTag2 = paragraphUnit.Source[1] as IStructureTag;
					if (structureTag2 != null)
					{
						_currentSubContent = new StringBuilder(structureTag2.TagProperties.TagContent);
					}
					if (_bilingualWriter == null)
					{
						IParagraphUnit paragraphUnit2 = paragraphUnit.Clone() as IParagraphUnit;
						paragraphUnit2.Source.RemoveAt(1);
						base.ProcessParagraphUnit(paragraphUnit2);
					}
					return;
				}
			}
			IParagraphUnit paragraphUnit3 = paragraphUnit.Clone() as IParagraphUnit;
			if (_subContentGenerators.Count > 0)
			{
				((SubContentGenerator)_subContentGenerators[_subContentGenerators.Count - 1]).ProcessParagraphUnit(paragraphUnit3);
			}
			else if (!(paragraphUnit3.Source.ToString() == "#SDL_SUBCONTENT_BOUNDARY#"))
			{
				base.ProcessParagraphUnit(paragraphUnit3);
			}
		}

		private void RunPostWritingOperations()
		{
			PostGenerationMessageReporterAdapter messageAdapter = null;
			if (_nativeGenerator != null)
			{
				messageAdapter = BuildMessageAdapter(_nativeWriterProxy.LocationTracker, _nativeWriterProxy.BilingualContentLocator);
			}
			ExecuteTweakers(messageAdapter);
			ExecuteFileVerifiers(messageAdapter);
		}

		private void ExecuteFileVerifiers(PostGenerationMessageReporterAdapter messageAdapter)
		{
			INativeFileVerifier[] nativeVerifiers = GetNativeVerifiers();
			INativeFileVerifier[] array = nativeVerifiers;
			foreach (INativeFileVerifier nativeFileVerifier in array)
			{
				nativeFileVerifier.MessageReporter = GetMessageReporter(nativeFileVerifier, messageAdapter);
				nativeFileVerifier.Verify();
				nativeFileVerifier.MessageReporter = null;
			}
		}

		private INativeTextLocationMessageReporter GetMessageReporter(INativeFileVerifier verifier, PostGenerationMessageReporterAdapter messageAdapter)
		{
			INativeTextLocationTrackerProvider nativeTextLocationTrackerProvider = verifier as INativeTextLocationTrackerProvider;
			if (nativeTextLocationTrackerProvider != null && nativeTextLocationTrackerProvider.LocationTracker != null)
			{
				return BuildMessageAdapter(nativeTextLocationTrackerProvider.LocationTracker, _bilingualContentLocator);
			}
			return messageAdapter;
		}

		private PostGenerationMessageReporterAdapter BuildMessageAdapter(INativeTextLocationInfoProvider locationTracker, INativeGenerationBilingualContentLocator contentLocator)
		{
			return new PostGenerationMessageReporterAdapter(OnMessage, locationTracker, contentLocator);
		}

		private void ExecuteTweakers(PostGenerationMessageReporterAdapter messageAdapter)
		{
			foreach (IFilePostTweaker fileTweaker in FileTweakers)
			{
				if (fileTweaker.Enabled)
				{
					fileTweaker.MessageReporter = messageAdapter;
					fileTweaker.TweakFilePostWriting(_nativeOutputProperties);
					fileTweaker.MessageReporter = null;
				}
			}
		}

		protected virtual void CallStartOfInput()
		{
			foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
			{
				(nonBilingualProcessorComponent as INativeContentCycleAware)?.StartOfInput();
			}
		}

		protected virtual void CallEndOfInput()
		{
			foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
			{
				(nonBilingualProcessorComponent as INativeContentCycleAware)?.EndOfInput();
			}
		}

		protected virtual void CallSetNativeFileProperties(IFileProperties fileProperties)
		{
			foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
			{
				(nonBilingualProcessorComponent as INativeContentCycleAware)?.SetFileProperties(fileProperties);
			}
		}

		protected virtual void CallSetOutputProperties()
		{
			foreach (object allComponent in AllComponents)
			{
				(allComponent as INativeOutputSettingsAware)?.SetOutputProperties(_nativeOutputProperties);
			}
		}

		protected virtual IEnumerable<object> GetNonBilingualProcessorComponents()
		{
			foreach (object nativeComponent in GetNativeComponents())
			{
				yield return nativeComponent;
			}
			INativeFileVerifier[] nativeVerifiers = GetNativeVerifiers();
			for (int i = 0; i < nativeVerifiers.Length; i++)
			{
				yield return nativeVerifiers[i];
			}
		}

		private IEnumerable<object> GetNativeComponents()
		{
			if (_toNativeConverter != null)
			{
				yield return _toNativeConverter;
			}
			if (_nativeGenerator != null)
			{
				foreach (INativeGenerationContentProcessor contentProcessor in _nativeGenerator.ContentProcessors)
				{
					yield return contentProcessor;
				}
				if (_nativeGenerator.Writer != null)
				{
					yield return _nativeGenerator.Writer;
				}
			}
		}

		public virtual void OnMessage(object sender, MessageEventArgs args)
		{
			if (this.Message != null)
			{
				if (args != null && string.IsNullOrEmpty(args.FilePath))
				{
					args.FilePath = _nativeOutputProperties.OutputFilePath;
				}
				this.Message(sender, args);
			}
		}

		public IMessageLocation GetLocation(LocationMarkerId marker)
		{
			NativeGenerationMessageLocation nativeGenerationMessageLocation = new NativeGenerationMessageLocation(marker);
			nativeGenerationMessageLocation.LocationInfoProvider = _nativeWriterProxy.LocationTracker;
			nativeGenerationMessageLocation.ContentLocator = _bilingualContentLocator;
			return nativeGenerationMessageLocation;
		}
	}
}
