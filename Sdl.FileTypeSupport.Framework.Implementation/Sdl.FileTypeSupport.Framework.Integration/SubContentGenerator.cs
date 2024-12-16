using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class SubContentGenerator : AbstractBilingualProcessorContainer, ISubContentGenerator, IBilingualProcessorContainer, IAbstractGenerator, IFileTypeDefinitionAware, ILocationMarkerLocator
	{
		private NativeWriterMessagesProxy _nativeWriterProxy;

		private GenerationBilingualContentLocator _bilingualContentLocator = new GenerationBilingualContentLocator();

		private INativeOutputFileProperties _nativeOutputProperties = new NativeOutputFileProperties();

		private List<IFilePostTweaker> _FileTweakers = new List<IFilePostTweaker>();

		private List<INativeFileVerifier> _NativeFileVerifiers = new List<INativeFileVerifier>();

		private IBilingualToNativeConverter _toNativeConverter = new BilingualToNativeConverter();

		private INativeSubContentGenerator _nativeSubContentGenerator;

		private IBilingualWriter _bilingualWriter;

		private bool _frameworkPropertiesSet;

		private BilingualMessageReporterToMessageEventAdapter _bilingualMessageReporterAdapter;

		private IDocumentItemFactory _itemFactory;

		public List<IFilePostTweaker> FileTweakers => _FileTweakers;

		IEnumerable<IFilePostTweaker> ISubContentGenerator.FileTweakers => _FileTweakers;

		IEnumerable<INativeFileVerifier> ISubContentGenerator.NativeVerifiers => _NativeFileVerifiers;

		protected override IBilingualContentHandler FirstHandler => _bilingualContentLocator;

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

		public INativeSubContentGenerator NativeSubContentGenerator
		{
			get
			{
				return _nativeSubContentGenerator;
			}
			set
			{
				_nativeSubContentGenerator = value;
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
					foreach (object nonBilingualProcessorComponent in GetNonBilingualProcessorComponents())
					{
						yield return nonBilingualProcessorComponent;
					}
				}
				foreach (IFilePostTweaker fileTweaker in FileTweakers)
				{
					yield return fileTweaker;
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

		public SubContentGenerator()
		{
			_bilingualMessageReporterAdapter = new BilingualMessageReporterToMessageEventAdapter(OnMessage);
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
				if (_nativeSubContentGenerator != null)
				{
					if (_toNativeConverter != null)
					{
						_toNativeConverter.Output = _nativeSubContentGenerator.Input;
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
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			if (_nativeSubContentGenerator != null && _nativeSubContentGenerator.Writer != null && !(_nativeSubContentGenerator.Writer is NativeWriterMessagesProxy))
			{
				_nativeWriterProxy = new NativeWriterMessagesProxy(_nativeSubContentGenerator.Writer, OnMessage, _bilingualContentLocator);
				_nativeSubContentGenerator.Writer = _nativeWriterProxy;
				_nativeWriterProxy.RealWriter.LocationTracker = _nativeWriterProxy.LocationTracker;
				PostGenerationMessageReporterAdapter messageReporter = new PostGenerationMessageReporterAdapter(OnMessage, _nativeWriterProxy.LocationTracker, _nativeWriterProxy.BilingualContentLocator);
				_nativeWriterProxy.MessageReporter = messageReporter;
			}
			CallSetOutputProperties();
			base.SetFileProperties(fileInfo);
			CallSetNativeFileProperties(fileInfo);
			CallStartOfInput();
		}

		public override void FileComplete()
		{
			base.FileComplete();
			CallEndOfInput();
			RunPostWritingOperations();
		}

		private void RunPostWritingOperations()
		{
			PostGenerationMessageReporterAdapter messageAdapter = null;
			if (_nativeSubContentGenerator != null)
			{
				messageAdapter = new PostGenerationMessageReporterAdapter(OnMessage, _nativeWriterProxy.LocationTracker, _nativeWriterProxy.BilingualContentLocator);
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
				nativeFileVerifier.MessageReporter = messageAdapter;
				nativeFileVerifier.Verify();
				nativeFileVerifier.MessageReporter = null;
			}
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
			if (_toNativeConverter != null)
			{
				yield return _toNativeConverter;
			}
			if (_nativeSubContentGenerator != null)
			{
				foreach (INativeGenerationContentProcessor contentProcessor in _nativeSubContentGenerator.ContentProcessors)
				{
					yield return contentProcessor;
				}
				if (_nativeSubContentGenerator.Writer != null)
				{
					yield return _nativeSubContentGenerator.Writer;
				}
			}
			INativeFileVerifier[] nativeVerifiers = GetNativeVerifiers();
			for (int i = 0; i < nativeVerifiers.Length; i++)
			{
				yield return nativeVerifiers[i];
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
