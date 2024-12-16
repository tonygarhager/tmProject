using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Globalization;
using Sdl.Core.LanguageProcessing.Segmentation;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Globalization;
using System.Text;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class FileToBcmConverter
	{
		private readonly string _inputFilePath;

		private readonly BcmExtractor _bcmExtractor;

		private Lazy<IFileTypeManager> _fileTypeManager;

		private Segmentor _segmentor;

		private readonly CultureInfo _sourceCulture;

		private readonly CultureInfo _targetCulture;

		private readonly IResourceDataAccessor _accessor;

		private bool _sniffFile = true;

		private string _fileTypeDefinitionId;

		public IFileTypeManager FileTypeManager
		{
			get
			{
				return _fileTypeManager.Value;
			}
			set
			{
				_fileTypeManager = new Lazy<IFileTypeManager>(() => value);
			}
		}

		public bool PerformWordCount
		{
			get;
			set;
		}

		public int WordCountResult
		{
			get;
			private set;
		}

		public string DetectedEncoding
		{
			get;
			private set;
		}

		public string Encoding
		{
			get;
			set;
		}

		public Action<DependencyFile> DependencyFilesHandler
		{
			get
			{
				return _bcmExtractor?.DependencyFileHandler;
			}
			set
			{
				_bcmExtractor.DependencyFileHandler = value;
			}
		}

		public event EventHandler ParagraphUnitProcessing
		{
			add
			{
				_bcmExtractor.ParagraphUnitProcessing += value;
			}
			remove
			{
				_bcmExtractor.ParagraphUnitProcessing -= value;
			}
		}

		public event EventHandler<ParagraphUnitEventArgs> ParagraphUnitProcessed
		{
			add
			{
				_bcmExtractor.ParagraphUnitProcessed += value;
			}
			remove
			{
				_bcmExtractor.ParagraphUnitProcessed -= value;
			}
		}

		public event Action<IFileProperties> FilePropertiesSet
		{
			add
			{
				_bcmExtractor.FilePropertiesSet += value;
			}
			remove
			{
				_bcmExtractor.FilePropertiesSet -= value;
			}
		}

		public event EventHandler DependencyFilesAdded
		{
			add
			{
				_bcmExtractor.DependencyFilesAdded += value;
			}
			remove
			{
				_bcmExtractor.DependencyFilesAdded -= value;
			}
		}

		public FileToBcmConverter(string inputFilePath, CultureInfo sourceCulture, CultureInfo targetCulture = null)
		{
			_inputFilePath = inputFilePath;
			_sniffFile = true;
			_fileTypeManager = new Lazy<IFileTypeManager>(DefaultBuilder.GetDefaultFileTypeManager);
			_sourceCulture = sourceCulture;
			_targetCulture = targetCulture;
			_bcmExtractor = new BcmExtractor
			{
				BcmExtractionSettings = new BcmExtractionSettings
				{
					GenerateContextsDependencyFile = true
				}
			};
		}

		public FileToBcmConverter(string inputFilePath, IResourceDataAccessor accessor, CultureInfo sourceCulture, CultureInfo targetCulture = null)
			: this(inputFilePath, sourceCulture, targetCulture)
		{
			_accessor = accessor;
		}

		public string ConvertToJson()
		{
			Document value = ConvertToBcmDocument();
			return JsonConvert.SerializeObject(value);
		}

		public Document ConvertToBcmDocument(string fileTypeDefinitionId, bool sniffFile)
		{
			_sniffFile = sniffFile;
			_fileTypeDefinitionId = fileTypeDefinitionId;
			return ConvertToBcmDocument();
		}

		public Document ConvertToBcmDocumentAndUpdate(Document originalDocument)
		{
			if (!_inputFilePath.Trim().EndsWith(".sdlxliff", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new InvalidOperationException("Input file must be an SdlXliff and have the correct file extension");
			}
			Document document = ConvertToBcmDocument();
			if (!CheckMatch(originalDocument, document))
			{
				throw new InvalidOperationException("BCM Document Paragraph mismatch");
			}
			UpdateNewDocumentFromOriginal(originalDocument, document);
			return document;
		}

		public Document ConvertToBcmDocument()
		{
			WordCountResult = 0;
			IFileTypeManager value = _fileTypeManager.Value;
			value.SettingsBundle = (value.SettingsBundle ?? SettingsUtil.CreateSettingsBundle(null));
			IMultiFileConverter multiFileConverter = _sniffFile ? value.GetConverterToDefaultBilingual(new string[1]
			{
				_inputFilePath
			}, null, _sourceCulture, null, delegate
			{
			}) : value.GetConverterToDefaultBilingual(_fileTypeDefinitionId, _inputFilePath, null, _sourceCulture, null, delegate
			{
			});
			SetEncodingInExtractors(multiFileConverter, Encoding);
			multiFileConverter.BilingualDocumentGenerator = null;
			DetectLanguages(multiFileConverter);
			if (_sourceCulture != null)
			{
				multiFileConverter.DocumentInfo.SourceLanguage = ((multiFileConverter.DocumentInfo.SourceLanguage.CultureInfo == null) ? new Language(_sourceCulture) : multiFileConverter.DocumentInfo.SourceLanguage);
			}
			if (_targetCulture != null)
			{
				multiFileConverter.DocumentInfo.TargetLanguage = new Language(_targetCulture);
			}
			_segmentor = DefaultBuilder.GetDefaultSegmentor(_accessor);
			_segmentor.Settings.TargetSegmentCreationMode = TargetSegmentCreationMode.CreateEmptyTarget;
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(_segmentor));
			WordCountProcessor wordCountProcessor = null;
			if (PerformWordCount)
			{
				wordCountProcessor = new WordCountProcessor(new Language(_sourceCulture), GetTokenizer(_sourceCulture));
				multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(wordCountProcessor));
			}
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(_bcmExtractor));
			DetectEncodingFromExtractors(multiFileConverter);
			multiFileConverter.Parse();
			((MultiFileConverter)multiFileConverter).Dispose();
			if (PerformWordCount && wordCountProcessor != null)
			{
				WordCountResult = wordCountProcessor.WordCountResult;
			}
			return _bcmExtractor.OutputDocument;
		}

		private void DetectLanguages(IMultiFileConverter converter)
		{
			Language sourceLanguage = new Language(_sourceCulture);
			Language targetLanguage = new Language(_targetCulture);
			Pair<Language, DetectionLevel> detectedSourceLanguage = converter.DetectedSourceLanguage;
			if (detectedSourceLanguage.Second == DetectionLevel.Certain)
			{
				sourceLanguage = detectedSourceLanguage.First;
			}
			Pair<Language, DetectionLevel> detectedTargetLanguage = converter.DetectedTargetLanguage;
			if (detectedTargetLanguage.Second == DetectionLevel.Certain)
			{
				targetLanguage = detectedTargetLanguage.First;
			}
			_bcmExtractor.SourceLanguage = sourceLanguage;
			_bcmExtractor.TargetLanguage = targetLanguage;
		}

		private void DetectEncodingFromExtractors(IMultiFileConverter converter)
		{
			foreach (IFileExtractor extractor in converter.Extractors)
			{
				try
				{
					DetectedEncoding = "UTF-8";
					if (extractor.FileConversionProperties.FileSnifferInfo.DetectedEncoding.First.Encoding != null)
					{
						DetectedEncoding = extractor.FileConversionProperties.FileSnifferInfo.DetectedEncoding.First.Encoding.BodyName;
					}
				}
				catch
				{
					DetectedEncoding = "UTF-8";
				}
			}
		}

		private void SetEncodingInExtractors(IMultiFileConverter converter, string encoding)
		{
			if (!string.IsNullOrEmpty(encoding))
			{
				foreach (IFileExtractor extractor in converter.Extractors)
				{
					try
					{
						Codepage codepage = new Codepage(System.Text.Encoding.GetEncoding(encoding));
						IPersistentFileConversionProperties fileConversionProperties = extractor.FileConversionProperties;
						object obj;
						if (fileConversionProperties == null)
						{
							obj = null;
						}
						else
						{
							Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo fileSnifferInfo = fileConversionProperties.FileSnifferInfo;
							if (fileSnifferInfo == null)
							{
								obj = null;
							}
							else
							{
								Pair<Codepage, DetectionLevel> detectedEncoding = fileSnifferInfo.DetectedEncoding;
								obj = ((detectedEncoding == null) ? null : detectedEncoding.First?.Encoding);
							}
						}
						if (obj != null)
						{
							extractor.FileConversionProperties.FileSnifferInfo.DetectedEncoding = new Pair<Codepage, DetectionLevel>(codepage, DetectionLevel.Certain);
							extractor.FileConversionProperties.OriginalEncoding = codepage;
						}
						else if (extractor.FileConversionProperties?.FileSnifferInfo == null)
						{
							if (extractor.FileConversionProperties == null)
							{
								extractor.FileConversionProperties = new PersistentFileConversionProperties();
							}
							extractor.FileConversionProperties.FileSnifferInfo = new Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo
							{
								DetectedEncoding = new Pair<Codepage, DetectionLevel>(codepage, DetectionLevel.Certain)
							};
							extractor.FileConversionProperties.OriginalEncoding = codepage;
						}
					}
					catch
					{
					}
				}
			}
		}

		private bool CheckMatch(Document document, Document outputBcmDocument)
		{
			if (document.Files.Count != outputBcmDocument.Files.Count)
			{
				return false;
			}
			for (int i = 0; i < document.Files.Count; i++)
			{
				File file = document.Files[i];
				File file2 = outputBcmDocument.Files[i];
				if (file.ParagraphUnits.Count != file2.ParagraphUnits.Count)
				{
					return false;
				}
				if (file.GetMetadata("SDL:FileId") != file2.GetMetadata("SDL:FileId"))
				{
					return false;
				}
				for (int j = 0; j < file.ParagraphUnits.Count; j++)
				{
					ParagraphUnit paragraphUnit = file.ParagraphUnits[j];
					ParagraphUnit paragraphUnit2 = file2.ParagraphUnits[j];
					string metadata = paragraphUnit.GetMetadata("frameworkOriginalParagraphUnitId");
					string metadata2 = paragraphUnit2.GetMetadata("frameworkOriginalParagraphUnitId");
					if (metadata != metadata2)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static void UpdateNewDocumentFromOriginal(Document originalDocument, Document convertedDocument)
		{
			convertedDocument.Id = originalDocument.Id;
			for (int i = 0; i < convertedDocument.Files.Count; i++)
			{
				File file = originalDocument.Files[i];
				File file2 = convertedDocument.Files[i];
				file2.DependencyFiles = file.DependencyFiles;
			}
		}

		private static Tokenizer GetTokenizer(CultureInfo sourceLanguage)
		{
			TokenizerSetup tokenizerSetup = TokenizerSetupFactory.Create(sourceLanguage, BuiltinRecognizers.RecognizeAll);
			tokenizerSetup.CreateWhitespaceTokens = true;
			Tokenizer tokenizer = new Tokenizer(tokenizerSetup);
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Invalid tokenizer (null)");
			}
			return tokenizer;
		}
	}
}
