using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Bilingual.SdlXliff;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi
{
	internal class BcmReader : AbstractBilingualFileTypeComponent, IBilingualParser, IParser, IDisposable, IBilingualFileTypeComponent, INativeContentCycleAware
	{
		private readonly Document _document;

		private BcmToBilingualConverter _bcmConverter;

		private IMultiFileConverter _converter;

		private readonly string _fileOutputPath;

		private File _currentFile;

		private readonly bool _saveAsSdlxliff;

		private CommonDelegates.GetGeneratorId _getGeneratorId;

		public ContentRestriction ContentRestriction
		{
			get;
			set;
		}

		public IDocumentProperties DocumentProperties
		{
			get;
			set;
		}

		public IBilingualContentHandler Output
		{
			get;
			set;
		}

		public Lazy<IFileTypeManager> FileTypeManager
		{
			get;
			set;
		}

		public string TargetEncoding
		{
			get;
			set;
		}

		public event EventHandler<ProgressEventArgs> Progress;

		public event EventHandler ParagraphUnitProcessing;

		public event EventHandler ParagraphUnitProcessed;

		public BcmReader(Document document, string fileOutputPath)
		{
			ContentRestriction = ContentRestriction.Target;
			_fileOutputPath = fileOutputPath;
			_document = document;
			string name = document.TargetLanguageCode ?? document.SourceLanguageCode;
			DocumentProperties = new DocumentProperties
			{
				SourceLanguage = new Language(new CultureInfo(document.SourceLanguageCode)),
				TargetLanguage = new Language(new CultureInfo(name))
			};
			FileTypeManager = new Lazy<IFileTypeManager>(DefaultBuilder.GetDefaultFileTypeManager);
			if (_fileOutputPath.Trim().EndsWith(".sdlxliff", StringComparison.InvariantCultureIgnoreCase))
			{
				_saveAsSdlxliff = true;
			}
		}

		public BcmReader(Document document, string fileOutputPath, CommonDelegates.GetGeneratorId getGeneratorIdCallback)
			: this(document, fileOutputPath)
		{
			_getGeneratorId = getGeneratorIdCallback;
		}

		private void OnProgress(ProgressEventArgs e)
		{
			this.Progress?.Invoke(this, e);
		}

		private void OnParagraphUnitProcessing()
		{
			this.ParagraphUnitProcessing?.Invoke(this, EventArgs.Empty);
		}

		private void OnParagraphUnitProcessed()
		{
			this.ParagraphUnitProcessed?.Invoke(this, EventArgs.Empty);
		}

		public void Convert()
		{
			OutputPropertiesProvider outputSettingsProvider = CreateOutputPropertiesProvider(_fileOutputPath);
			IFileTypeManager value = FileTypeManager.Value;
			value.SettingsBundle = (value.SettingsBundle ?? SettingsUtil.CreateSettingsBundle(null));
			_converter = (_saveAsSdlxliff ? value.GetConverterToDefaultBilingual(this, _fileOutputPath) : value.GetConverterToNative(this, outputSettingsProvider));
			if (_saveAsSdlxliff)
			{
				XliffFileWriter xliffFileWriter = _converter.BilingualDocumentGenerator.Writer as XliffFileWriter;
				if (xliffFileWriter != null)
				{
					xliffFileWriter.MaxEmbeddableFileSize = 314572800L;
				}
			}
			_converter.SetDocumentInfo(DocumentProperties, applyToAllExtractors: false);
			_converter.Parse();
		}

		public void SetFileProperties(IFileProperties properties)
		{
		}

		public bool ParseNext()
		{
			IFileTypeManager value = FileTypeManager.Value;
			Output?.Initialize(DocumentProperties);
			foreach (File file2 in _document.Files)
			{
				File file = _currentFile = file2;
				FileProperties fileProperties = BcmReaderUtil.CreateFileProperties(_currentFile, DocumentProperties);
				SetNativeGeneratorProvider(_converter, value, fileProperties.FileConversionProperties, _fileOutputPath);
				BcmReaderUtil.SetProperties(_currentFile, Output, DocumentProperties, fileProperties, TargetEncoding);
				_bcmConverter = new BcmToBilingualConverter(file, fileProperties);
				foreach (Sdl.Core.Bcm.BcmModel.ParagraphUnit paragraphUnit2 in _currentFile.ParagraphUnits)
				{
					IParagraphUnit paragraphUnit = ConvertParagraphUnit(paragraphUnit2);
					Output.ProcessParagraphUnit(paragraphUnit);
				}
				Output.FileComplete();
			}
			return false;
		}

		public void StartOfInput()
		{
		}

		public void EndOfInput()
		{
			Output.Complete();
			((MultiFileConverter)_converter).Dispose();
		}

		public void Dispose()
		{
		}

		private IParagraphUnit ConvertParagraphUnit(Sdl.Core.Bcm.BcmModel.ParagraphUnit pu)
		{
			OnParagraphUnitProcessing();
			IParagraphUnit result = _bcmConverter.Convert(pu);
			OnParagraphUnitProcessed();
			return result;
		}

		private void SetNativeGeneratorProvider(IMultiFileConverter converter, IFileTypeManager filterMgr, IPersistentFileConversionProperties fileConversionProperties, string outputPath)
		{
			converter.NativeGeneratorProvider = (NativeGeneratorProvider)Delegate.Combine(converter.NativeGeneratorProvider, (NativeGeneratorProvider)delegate(IPersistentFileConversionProperties properties)
			{
				properties.FileTypeDefinitionId = new FileTypeDefinitionId(_currentFile.FileTypeDefinitionId);
				IFileTypeDefinition fileTypeDefinition = filterMgr.FindFileTypeDefinition(properties.FileTypeDefinitionId);
				GeneratorId? generatorId = _getGeneratorId?.Invoke(fileTypeDefinition);
				IFileGenerator fileGenerator = (!generatorId.HasValue) ? fileTypeDefinition.BuildNativeGenerator() : (fileTypeDefinition.BuildGenerator(generatorId.Value) as IFileGenerator);
				fileGenerator?.BilingualWriter?.Initialize(null);
				INativeOutputSettingsAware nativeOutputSettingsAware = fileGenerator?.AllComponents.FirstOrDefault((object y) => y is INativeOutputSettingsAware) as INativeOutputSettingsAware;
				OutputFileInfo outputFileInfo = new OutputFileInfo(ContentRestriction);
				nativeOutputSettingsAware.GetProposedOutputFileInfo(fileConversionProperties, outputFileInfo);
				NativeOutputFileProperties outputProperties = new NativeOutputFileProperties();
				OutputPropertiesProvider outputPropertiesProvider = CreateOutputPropertiesProvider(outputPath);
				outputPropertiesProvider(outputProperties, fileConversionProperties, outputFileInfo);
				return fileGenerator;
			});
		}

		private OutputPropertiesProvider CreateOutputPropertiesProvider(string outputPath)
		{
			return PropertiesProvider;
			void PropertiesProvider(INativeOutputFileProperties properties, IPersistentFileConversionProperties conversionProperties, IOutputFileInfo info)
			{
				properties.OutputFilePath = outputPath;
				properties.Encoding = (conversionProperties.PreferredTargetEncoding ?? info.Encoding);
				properties.ContentRestriction = ContentRestriction;
			}
		}
	}
}
