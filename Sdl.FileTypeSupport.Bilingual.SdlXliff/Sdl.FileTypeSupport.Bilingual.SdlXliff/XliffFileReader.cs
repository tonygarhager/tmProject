#define TRACE
using Oasis.Xliff12;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Bilingual.SdlXliff.Util;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public sealed class XliffFileReader : AbstractBilingualFileTypeComponent, IBilingualDocumentParser, IBilingualParser, IParser, IDisposable, IBilingualFileTypeComponent, INativeContentCycleAware, ISettingsAware, ISdlXliffStreamContentHandler, ISharedObjectsAware
	{
		private enum ParseStage
		{
			NotStarted,
			BeforeProcessingFile,
			ProcessingFileHeader,
			BeforeProcessingFileBody,
			ProcessingFileBody,
			Done
		}

		private const long ProgressReportTreshold = 20000L;

		private string _xliffOrigFilePath;

		private string _xliffInputFilePath;

		private readonly ParagraphUnitBuffer _outputBuffer = new ParagraphUnitBuffer();

		private IDocumentProperties _documentProperties;

		private IFileProperties _fileProperties;

		private DependencyFileLocator _dependencyFileLocator;

		private Predicate<IPersistentFileConversionProperties> _fileRestriction;

		private FileSkeleton _fileSkeleton;

		private DocSkeleton _docSkeleton;

		private SdlXliffGeneralSettings _settings = new SdlXliffGeneralSettings();

		private long _processedTransUnits;

		private ISharedObjects _sharedObjects;

		private ParseStage _stage;

		private SdlXliffFeeder _sdlXliffFeeder;

		private string _currentFileTypeDefinitionId;

		private long _fileSize;

		private byte _lastReportedPercent;

		private bool _isOutputInitializedOnDocInfo;

		private List<object> _referencedFiles;

		private readonly XmlSerializer _groupSerializer = new XmlSerializer(typeof(group));

		private readonly XmlSerializer _transUnitSerializer = new XmlSerializer(typeof(transunit));

		private readonly XmlSerializer _internalFileSerializer = new XmlSerializer(typeof(internalfile));

		private readonly XmlSerializer _externalFileSerializer = new XmlSerializer(typeof(externalfile));

		private readonly List<group> _currentGroups = new List<group>();

		private IContextProperties _currentContext;

		private readonly List<ISubSegmentReference> _incompleteSubsegments = new List<ISubSegmentReference>();

		private readonly TransUnitIterator _transUnitIterator = new TransUnitIterator();

		private readonly ParagraphBuilder _paragraphBuilder = new ParagraphBuilder();

		private readonly Dictionary<string, ILockedContent> _lockedContent = new Dictionary<string, ILockedContent>();

		private ParagraphDirectionCache _paragraphDirectionCache;

		private bool _validateXliff;

		private List<IDependencyFileProperties> _dependencyFiles;

		public bool ValidateXliff
		{
			get
			{
				return _validateXliff;
			}
			set
			{
				_validateXliff = value;
			}
		}

		public string XliffFilePath
		{
			get
			{
				return _xliffOrigFilePath;
			}
			set
			{
				_xliffOrigFilePath = value;
			}
		}

		public IDocumentProperties DocumentProperties
		{
			get
			{
				return _documentProperties;
			}
			set
			{
				_documentProperties = value;
			}
		}

		public IBilingualContentHandler Output
		{
			get
			{
				return _outputBuffer.Output;
			}
			set
			{
				_outputBuffer.Output = value;
			}
		}

		public Predicate<IPersistentFileConversionProperties> FileRestriction
		{
			get
			{
				return _fileRestriction;
			}
			set
			{
				_fileRestriction = value;
			}
		}

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

		public event EventHandler<ProgressEventArgs> Progress;

		public XliffFileReader()
		{
			_validateXliff = false;
		}

		public XliffFileReader(string filePath)
			: this()
		{
			_xliffOrigFilePath = filePath;
			_xliffInputFilePath = filePath;
		}

		public FileTypeDefinitionId[] GetFilterDefinitionIds()
		{
			ValidateInputXml();
			return new FilterDefinitionReader
			{
				MessageReporter = MessageReporter
			}.GetFilterDefinitionIds(encryptionKey: (_sharedObjects == null) ? null : _sharedObjects.GetSharedObject<string>("SDL-ENC:EncryptionKey"), xliffInputFilePath: _xliffInputFilePath);
		}

		private void OnProgress(byte percent)
		{
			this.Progress?.Invoke(this, new ProgressEventArgs(percent));
		}

		private void ReportProgress()
		{
			if (_fileSize <= 20000)
			{
				OnProgress(100);
				return;
			}
			byte b = Math.Min((byte)(100 * _sdlXliffFeeder.EstimatedCharsRead / _fileSize), (byte)100);
			if (_lastReportedPercent != b)
			{
				_lastReportedPercent = b;
				OnProgress(b);
			}
		}

		public bool ParseNext()
		{
			if (_stage == ParseStage.Done)
			{
				return false;
			}
			if (_stage == ParseStage.NotStarted)
			{
				BeginParsing();
			}
			while (ContinueParsing())
			{
				switch (_stage)
				{
				case ParseStage.ProcessingFileBody:
					return true;
				default:
					throw new FileTypeSupportException("Internal error: Unrecognized parsing stage!");
				case ParseStage.BeforeProcessingFile:
				case ParseStage.ProcessingFileHeader:
				case ParseStage.BeforeProcessingFileBody:
					break;
				}
			}
			_stage = ParseStage.Done;
			return true;
		}

		private void BeginParsing()
		{
			if (Output == null)
			{
				throw new FileTypeSupportException("Internal error: Output has not been set.");
			}
			string text = (_sharedObjects == null) ? null : _sharedObjects.GetSharedObject<string>("SDL-ENC:EncryptionKey");
			if (!string.IsNullOrEmpty(text))
			{
				byte[] decodedKey = CryptographicHelper.GetDecodedKey(text);
				AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider
				{
					Mode = CipherMode.CBC,
					Padding = PaddingMode.PKCS7,
					Key = decodedKey
				};
				using (FileStream fileStream = File.OpenRead(_xliffInputFilePath))
				{
					if (!CryptographicHelper.ReadHeader(fileStream, out byte[] IV))
					{
						throw new Exception(StringResources.Encryption_AttemptingToReadInvalidEncryptedFile);
					}
					aesCryptoServiceProvider.IV = IV;
					using (CryptoStream stream = new CryptoStream(fileStream, aesCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read))
					{
						_sdlXliffFeeder = new SdlXliffFeeder(new StringReader(new StreamReader(stream).ReadToEnd()));
					}
				}
			}
			else
			{
				if (_fileProperties.FileConversionProperties.FileSnifferInfo?.GetMetaData("SDL:SDLXLIFF-HEADER") == "true")
				{
					throw new XliffEncryptedException(StringResources.Encryption_AttemptingToReadEncryptedFile);
				}
				ValidateInputXml();
				using (FileStream stream2 = File.OpenRead(_xliffInputFilePath))
				{
					_sdlXliffFeeder = new SdlXliffFeeder(new StringReader(new StreamReader(stream2).ReadToEnd()));
				}
			}
			_sdlXliffFeeder.RegisterSubscriber(this);
			_fileSize = new FileInfo(_xliffInputFilePath).Length;
			_lastReportedPercent = 0;
			InitializeDocumentProperties(text);
		}

		private void InitializeDocumentProperties(string encryptionKey)
		{
			_docSkeleton = new DocSkeleton();
			if (_documentProperties == null)
			{
				_documentProperties = ItemFactory.CreateDocumentProperties();
			}
			_documentProperties.LastOpenedAsPath = _xliffOrigFilePath;
			LanguageReader languageReader = new LanguageReader
			{
				XliffOrignalFilePath = _xliffOrigFilePath
			};
			languageReader.ProcessStream(_xliffInputFilePath, encryptionKey);
			if (languageReader.SourceLanguage != null)
			{
				_documentProperties.SourceLanguage = languageReader.SourceLanguage;
			}
			if (languageReader.TargetLanguage != null)
			{
				_documentProperties.TargetLanguage = languageReader.TargetLanguage;
			}
		}

		private bool ContinueParsing()
		{
			if (!_sdlXliffFeeder.ContinueScanning())
			{
				Output.Complete();
				return false;
			}
			ReportProgress();
			return true;
		}

		private void ParseDependencyFilesElement(XmlElement dependencyFilesElement)
		{
			_dependencyFiles = new List<IDependencyFileProperties>();
			foreach (XmlElement item in dependencyFilesElement)
			{
				string attribute = item.GetAttribute("name");
				IDependencyFileProperties dependencyFileProperties = base.PropertiesFactory.CreateDependencyFileProperties(attribute);
				dependencyFileProperties.Id = item.GetAttribute("id");
				if (item.HasAttribute("o-path"))
				{
					dependencyFileProperties.OriginalFilePath = item.GetAttribute("o-path");
					dependencyFileProperties.OriginalLastChangeDate = DateTime.Parse(item.GetAttribute("date"), CultureInfo.InvariantCulture);
				}
				dependencyFileProperties.PathRelativeToConverted = item.GetAttribute("rel-path");
				dependencyFileProperties.Description = item.GetAttribute("descr");
				if (item.HasAttribute("expected-use"))
				{
					dependencyFileProperties.ExpectedUsage = (DependencyFileUsage)Enum.Parse(typeof(DependencyFileUsage), item.GetAttribute("expected-use"));
				}
				if (item.HasAttribute("pref-reftype"))
				{
					dependencyFileProperties.PreferredLinkage = (DependencyFileLinkOption)Enum.Parse(typeof(DependencyFileLinkOption), item.GetAttribute("pref-reftype"));
				}
				_dependencyFiles.Add(dependencyFileProperties);
			}
		}

		private void AddEmbeddedFile(internalfile internalFileElement, IDependencyFileProperties file)
		{
			if (internalFileElement.Value == null || !internalFileElement.Value.StartsWith("file://"))
			{
				throw new FileTypeSupportException("Internal error: Unrecognized internalFileElement zip file name!");
			}
			DeserializeFileInfo deserializeFileInfo = FileSerializer.UnzipExternalFile(internalFileElement.Value.Substring("file://".Length));
			if (deserializeFileInfo.IsFileCreated)
			{
				file.CurrentFilePath = deserializeFileInfo.FullPath;
				FileJanitor fileJanitor = (FileJanitor)(file.DisposableObject = new FileJanitor(deserializeFileInfo.FullPath)
				{
					DeleteDirectoryIfEmpty = deserializeFileInfo.IsDirectoryCreated
				});
				_fileProperties.FileConversionProperties.DependencyFiles.Add(file);
			}
		}

		private void AddExternalFilePath(externalfile externalFileElement, IDependencyFileProperties file)
		{
			string text = externalFileElement.href;
			if (text.StartsWith("file://"))
			{
				text = text.Remove(0, "file://".Length);
			}
			string text2 = null;
			if (!Path.IsPathRooted(text) && !string.IsNullOrEmpty(file.PathRelativeToConverted))
			{
				string text3 = Path.Combine(Path.GetDirectoryName(_xliffOrigFilePath), file.PathRelativeToConverted);
				if (File.Exists(text3))
				{
					text2 = text3;
				}
			}
			if (string.IsNullOrEmpty(text2) && File.Exists(text))
			{
				text2 = Path.GetFullPath(text);
			}
			file.CurrentFilePath = (string.IsNullOrEmpty(text2) ? text : text2);
			if (!file.FileExists && _dependencyFileLocator != null)
			{
				text2 = _dependencyFileLocator(file);
				if (!string.IsNullOrEmpty(text2))
				{
					file.CurrentFilePath = text2;
				}
				else
				{
					string message = string.Format(StringResources.MissingDependencyFile, file.CurrentFilePath, file.Description, file.ExpectedUsage, file.OriginalFilePath, file.PathRelativeToConverted);
					ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, message, null);
				}
			}
			_fileProperties.FileConversionProperties.DependencyFiles.Add(file);
		}

		private void ParseTransUnit(transunit transunit)
		{
			_processedTransUnits++;
			_paragraphBuilder.ItemFactory = ItemFactory;
			_paragraphBuilder.FileSkeleton = _fileSkeleton;
			_paragraphBuilder.DocSkeleton = _docSkeleton;
			_paragraphBuilder.KnownSegmentIds.Clear();
			_paragraphBuilder.CurrentTransunit = transunit;
			_paragraphBuilder.MessageReporter = MessageReporter;
			_transUnitIterator.TransUnit = transunit;
			_transUnitIterator.Output = _paragraphBuilder;
			LockTypeFlags lockTypeFlags = LockTypeFlags.Unlocked;
			if (transunit.AnyAttr != null)
			{
				XmlAttribute[] anyAttr = transunit.AnyAttr;
				foreach (XmlAttribute xmlAttribute in anyAttr)
				{
					if (xmlAttribute.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0")
					{
						string localName = xmlAttribute.LocalName;
						if (localName != null && localName == "locktype")
						{
							lockTypeFlags = GetLockedContentFlags(xmlAttribute.Value);
						}
					}
				}
			}
			if (transunit.id.StartsWith("lockTU_"))
			{
				ParseLockedContent(transunit, lockTypeFlags);
			}
			else if (transunit.translate == AttrType_YesNo.no && (lockTypeFlags == LockTypeFlags.Unlocked || (lockTypeFlags & LockTypeFlags.Structure) == LockTypeFlags.Structure))
			{
				ParseStructureParagraphUnit(transunit, lockTypeFlags);
			}
			else
			{
				ParseLocalizableParagraphUnit(transunit, lockTypeFlags);
			}
		}

		private void ParseLockedContent(transunit transunit, LockTypeFlags lockFlags)
		{
			_paragraphBuilder.IsStructure = false;
			_paragraphBuilder.LockedContent = _lockedContent;
			ILockedContentProperties properties = base.PropertiesFactory.CreateLockedContentProperties(lockFlags);
			ILockedContent lockedContent = ItemFactory.CreateLockedContent(properties);
			_paragraphBuilder.Content = lockedContent.Content;
			_transUnitIterator.Parse(source: true);
			_lockedContent.Add(transunit.id, lockedContent);
		}

		private void ParseStructureParagraphUnit(transunit transunit, LockTypeFlags lockFlags)
		{
			_paragraphBuilder.IsStructure = true;
			if (lockFlags == LockTypeFlags.Unlocked)
			{
				lockFlags = LockTypeFlags.Structure;
			}
			IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(lockFlags);
			SetStructurePuProperties(paragraphUnit, transunit);
			_paragraphBuilder.Content = paragraphUnit.Source;
			_transUnitIterator.Parse(source: true);
			SetParagraphDirectionality(paragraphUnit.Source);
			SetParagraphDirectionality(paragraphUnit.Target);
			OutputParagraphUnit(paragraphUnit);
		}

		private void OutputParagraphUnit(IParagraphUnit pu)
		{
			if (_incompleteSubsegments.Count == 0)
			{
				_outputBuffer.Release();
			}
			else
			{
				_outputBuffer.Hold();
			}
			_outputBuffer.ProcessParagraphUnit(pu);
		}

		private void ParseLocalizableParagraphUnit(transunit transunit, LockTypeFlags lockFlags)
		{
			_paragraphBuilder.IsStructure = false;
			IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(lockFlags);
			SetParagraphProperties(paragraphUnit.Properties, transunit);
			_paragraphBuilder.Content = paragraphUnit.Source;
			_transUnitIterator.Parse(source: true);
			SetParagraphDirectionality(paragraphUnit.Source);
			_paragraphBuilder.Content = paragraphUnit.Target;
			_transUnitIterator.Parse(source: false);
			SetParagraphDirectionality(paragraphUnit.Target);
			foreach (ISubSegmentReference incompleteSubsegment in _incompleteSubsegments)
			{
				if (incompleteSubsegment.ParagraphUnitId == paragraphUnit.Properties.ParagraphUnitId)
				{
					incompleteSubsegment.Properties.Contexts = paragraphUnit.Properties.Contexts;
					_incompleteSubsegments.Remove(incompleteSubsegment);
					break;
				}
			}
			OutputParagraphUnit(paragraphUnit);
		}

		private void SetParagraphDirectionality(IParagraph paragraph)
		{
			paragraph.TextDirection = _paragraphDirectionCache.GetDirection(paragraph);
		}

		private void ParseGroup(group group)
		{
			_currentGroups.Add(group);
			UpdateContext();
			object[] items = group.Items;
			foreach (object obj in items)
			{
				transunit transunit = obj as transunit;
				if (transunit != null)
				{
					ParseTransUnit(transunit);
					continue;
				}
				group group2 = obj as group;
				if (group2 != null)
				{
					ParseGroup(group2);
				}
				else if (obj is binunit)
				{
					throw new XliffParseException(StringResources.CorruptFile_BinUnitNotSupported);
				}
			}
			_currentGroups.Remove(group);
			UpdateContext();
		}

		private void UpdateContext()
		{
			if (_currentGroups.Count == 0)
			{
				_currentContext = null;
				return;
			}
			_currentContext = null;
			foreach (group currentGroup in _currentGroups)
			{
				IContextProperties contextProperties = ParseContextsInGroup(currentGroup);
				if (_currentContext == null)
				{
					_currentContext = contextProperties;
				}
				else
				{
					if (contextProperties.StructureInfo != null)
					{
						_currentContext.StructureInfo = contextProperties.StructureInfo;
					}
					for (int i = 0; i < contextProperties.Contexts.Count; i++)
					{
						_currentContext.Contexts.Insert(i, contextProperties.Contexts[i]);
					}
				}
			}
		}

		private IContextProperties ParseContextsInGroup(group groupElement)
		{
			if (groupElement.Any == null)
			{
				return null;
			}
			IContextProperties contextProperties = base.PropertiesFactory.CreateContextProperties();
			XmlElement[] any = groupElement.Any;
			foreach (XmlElement xmlElement in any)
			{
				if (xmlElement.NamespaceURI.Equals("http://sdl.com/FileTypes/SdlXliff/1.0"))
				{
					string localName = xmlElement.LocalName;
					if (localName != null && localName == "cxts")
					{
						ParseSdlContextsElement(xmlElement, contextProperties);
					}
					else
					{
						ReportUnrecognizedElement(xmlElement);
					}
				}
				else
				{
					ReportUnrecognizedElement(xmlElement);
				}
			}
			return contextProperties;
		}

		private bool ParseSdlContextsElement(XmlElement cxtsElement, IContextProperties appendToContext)
		{
			bool result = false;
			foreach (XmlElement childNode in cxtsElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode))
				{
					switch (childNode.LocalName)
					{
					case "cxt":
						appendToContext.Contexts.Add(_fileSkeleton.GetContext(childNode.GetAttribute("id")));
						result = true;
						break;
					case "node":
						appendToContext.StructureInfo = _fileSkeleton.GetStructure(childNode.GetAttribute("id"));
						result = true;
						break;
					default:
						ReportUnrecognizedElement(childNode);
						break;
					}
				}
			}
			return result;
		}

		private void ReportUnrecognizedElement(XmlElement element)
		{
			string message = string.Format(StringResources.UnrecognizedElement, element.OuterXml);
			MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, message, null);
		}

		private void SetStructurePuProperties(IParagraphUnit pu, transunit transunit)
		{
			pu.Properties.ParagraphUnitId = new ParagraphUnitId(transunit.id);
			pu.Properties.Contexts = _currentContext;
		}

		private void ParsePersistentFileProperties(XmlElement element)
		{
			foreach (XmlElement childNode in element.ChildNodes)
			{
				if (childNode != null)
				{
					switch (childNode.LocalName)
					{
					case "value":
						ParseValueElement(childNode);
						break;
					case "sniff-info":
						ParseSniffInfoElement(childNode);
						break;
					default:
						ReportUnrecognizedElement(childNode);
						break;
					}
				}
			}
		}

		private void ParseSniffInfoElement(XmlElement sniffInfoElement)
		{
			SniffInfo sniffInfo = new SniffInfo();
			sniffInfo.IsSupported = true;
			sniffInfo.SuggestedTargetEncoding = EncodingCategory.NotSpecified;
			if (sniffInfoElement.HasAttribute("is-supported"))
			{
				sniffInfo.IsSupported = bool.Parse(sniffInfoElement.GetAttribute("is-supported"));
			}
			foreach (XmlElement childNode in sniffInfoElement.ChildNodes)
			{
				if (childNode != null)
				{
					switch (childNode.LocalName)
					{
					case "detected-encoding":
						sniffInfo.DetectedEncoding.Second = ParseDetectionLevelAttribute(childNode);
						sniffInfo.DetectedEncoding.First = new Codepage(childNode.GetAttribute("encoding"));
						break;
					case "detected-source-lang":
						sniffInfo.DetectedSourceLanguage.Second = ParseDetectionLevelAttribute(childNode);
						sniffInfo.DetectedSourceLanguage.First = new Language(childNode.GetAttribute("lang"));
						break;
					case "detected-target-lang":
						sniffInfo.DetectedTargetLanguage.Second = ParseDetectionLevelAttribute(childNode);
						sniffInfo.DetectedTargetLanguage.First = new Language(childNode.GetAttribute("lang"));
						break;
					case "suggested-target-encoding":
						sniffInfo.SuggestedTargetEncoding = (EncodingCategory)Enum.Parse(typeof(EncodingCategory), childNode.GetAttribute("category"));
						break;
					case "props":
						ParseSniffInfoValuesElement(sniffInfo, childNode);
						break;
					default:
						ReportUnrecognizedElement(childNode);
						break;
					}
				}
			}
			_fileProperties.FileConversionProperties.FileSnifferInfo = sniffInfo;
		}

		private static DetectionLevel ParseDetectionLevelAttribute(XmlElement element)
		{
			if (element.HasAttribute("detection-level"))
			{
				return (DetectionLevel)Enum.Parse(typeof(DetectionLevel), element.GetAttribute("detection-level"));
			}
			return DetectionLevel.Unknown;
		}

		private static void ParseSniffInfoValuesElement(SniffInfo sniffInfo, XmlElement valuesElement)
		{
			foreach (XmlElement item in valuesElement)
			{
				if (!(item.LocalName != "value"))
				{
					string attribute = item.GetAttribute("key");
					if (string.IsNullOrEmpty(attribute))
					{
						throw new XliffParseException(string.Format(StringResources.CorruptFile_ValueWithoutKey, item.OuterXml));
					}
					sniffInfo.SetMetaData(attribute, item.InnerText);
				}
			}
		}

		private static void ParseRevisionValuesElement(IRevisionProperties revisionProperties, XmlElement valuesElement)
		{
			foreach (XmlElement item in valuesElement)
			{
				if (!(item.LocalName != "value"))
				{
					string attribute = item.GetAttribute("key");
					if (string.IsNullOrEmpty(attribute))
					{
						throw new XliffParseException(string.Format(StringResources.CorruptFile_ValueWithoutKey, item.OuterXml));
					}
					revisionProperties.SetMetaData(attribute, item.InnerText);
				}
			}
		}

		private static void ParseCommentValuesElement(IComment comment, XmlElement valuesElement)
		{
			foreach (XmlElement item in valuesElement)
			{
				if (!(item.LocalName != "value"))
				{
					string attribute = item.GetAttribute("key");
					if (string.IsNullOrEmpty(attribute))
					{
						throw new XliffParseException(string.Format(StringResources.CorruptFile_ValueWithoutKey, item.OuterXml));
					}
					comment.SetMetaData(attribute, item.InnerText);
				}
			}
		}

		private void ParseFeedbackComment(IRevisionProperties revisionProperties, XmlElement revisionElement)
		{
			foreach (XmlElement item in revisionElement)
			{
				if (!(item.LocalName != "cmt"))
				{
					(revisionProperties as FeedbackProperties).FeedbackComment = _docSkeleton.GetComments(item.GetAttribute("id")).Comments.First();
					break;
				}
			}
		}

		private void ParseValueElement(XmlElement valueElement)
		{
			string attribute = valueElement.GetAttribute("key");
			if (string.IsNullOrEmpty(attribute))
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_ValueWithoutKey, valueElement.OuterXml));
			}
			if (attribute == "ParagraphTextDirections")
			{
				_paragraphDirectionCache.InitializeFromString(valueElement.InnerText);
			}
			_fileProperties.FileConversionProperties.SetMetaData(attribute, valueElement.InnerText);
		}

		private void ParseContextDefinitionElement(XmlElement cxtDefElement)
		{
			string attribute = cxtDefElement.GetAttribute("id");
			if (_fileSkeleton.HasContextDefinitionWithId(attribute))
			{
				throw new XliffParseException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_ContextAlreadyDefined, attribute));
			}
			bool flag = false;
			IContextInfo contextInfo;
			if (cxtDefElement.HasAttribute("type"))
			{
				contextInfo = base.PropertiesFactory.CreateContextInfo(cxtDefElement.GetAttribute("type"));
				flag = StandardContextTypes.StandardContextData.ContainsKey(contextInfo.ContextType);
			}
			else
			{
				contextInfo = base.PropertiesFactory.CreateContextInfo(null);
			}
			if (cxtDefElement.HasAttribute("code"))
			{
				contextInfo.DisplayCode = cxtDefElement.GetAttribute("code");
			}
			if (cxtDefElement.HasAttribute("name"))
			{
				contextInfo.DisplayName = cxtDefElement.GetAttribute("name");
			}
			if (cxtDefElement.HasAttribute("descr"))
			{
				contextInfo.Description = cxtDefElement.GetAttribute("descr");
			}
			if (cxtDefElement.HasAttribute("color"))
			{
				contextInfo.DisplayColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(null, CultureInfo.InvariantCulture, cxtDefElement.GetAttribute("color"));
			}
			else if (!flag)
			{
				contextInfo.DisplayColor = DefaultValues.ContextColor;
			}
			contextInfo.Purpose = ContextPurpose.Information;
			if (cxtDefElement.HasAttribute("purpose"))
			{
				contextInfo.Purpose = (ContextPurpose)Enum.Parse(typeof(ContextPurpose), cxtDefElement.GetAttribute("purpose"));
			}
			foreach (XmlElement childNode in cxtDefElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode))
				{
					switch (childNode.LocalName)
					{
					case "fmt":
						contextInfo.DefaultFormatting = _fileSkeleton.GetFormatting(childNode.GetAttribute("id"));
						break;
					case "props":
						ParseContextPropertiesElement(childNode, contextInfo);
						break;
					default:
						ReportUnrecognizedElement(childNode);
						break;
					}
				}
			}
			_fileSkeleton.AddContextDefinition(attribute, contextInfo);
		}

		private void ParseContextPropertiesElement(XmlElement propertiesElement, IMetaDataContainer toPopulate)
		{
			toPopulate.ClearMetaData();
			foreach (XmlElement item in propertiesElement)
			{
				if (CheckForExpectedSdlElement(item, "value") && item.HasAttribute("key"))
				{
					toPopulate.SetMetaData(item.GetAttribute("key"), item.InnerText);
				}
			}
		}

		private void ParseNodeDefinitionElement(XmlElement nodeDefElement)
		{
			string attribute = nodeDefElement.GetAttribute("id");
			IStructureInfo structureInfo = null;
			if (_fileSkeleton.HasStructureDefinitionWithId(attribute))
			{
				structureInfo = _fileSkeleton.GetStructure(attribute);
			}
			else
			{
				structureInfo = base.PropertiesFactory.CreateStructureInfo();
				structureInfo.MustUseDisplayName = true;
			}
			if (nodeDefElement.HasAttribute("force-name"))
			{
				structureInfo.MustUseDisplayName = bool.Parse(nodeDefElement.GetAttribute("force-name"));
			}
			string attribute2 = nodeDefElement.GetAttribute("parent");
			if (attribute2 != null && _fileSkeleton.HasStructureDefinitionWithId(attribute2))
			{
				structureInfo.ParentStructure = _fileSkeleton.GetStructure(attribute2);
			}
			else if (!string.IsNullOrEmpty(attribute2))
			{
				IStructureInfo structureInfo2 = base.PropertiesFactory.CreateStructureInfo();
				structureInfo2.MustUseDisplayName = true;
				structureInfo.ParentStructure = structureInfo2;
				_fileSkeleton.AddStructureDefinition(attribute2, structureInfo2);
			}
			foreach (XmlElement item in nodeDefElement)
			{
				if (CheckForExpectedSdlElement(item, "cxt"))
				{
					structureInfo.ContextInfo = _fileSkeleton.GetContext(item.GetAttribute("id"));
				}
			}
			if (!_fileSkeleton.HasStructureDefinitionWithId(attribute))
			{
				_fileSkeleton.AddStructureDefinition(attribute, structureInfo);
			}
		}

		private bool CheckForExpectedSdlElement(XmlElement element, string expectedName)
		{
			if (!CheckForExpectedSdlElement(element))
			{
				return false;
			}
			if (!element.LocalName.Equals(expectedName))
			{
				ReportUnrecognizedElement(element);
				return false;
			}
			return true;
		}

		private bool CheckForExpectedSdlElement(XmlElement element)
		{
			if (element == null)
			{
				return false;
			}
			if (!element.NamespaceURI.Equals("http://sdl.com/FileTypes/SdlXliff/1.0"))
			{
				ReportUnrecognizedElement(element);
				return false;
			}
			return true;
		}

		private void ParseFormattingDefinitionElement(XmlElement fmtDefElement)
		{
			string attribute = fmtDefElement.GetAttribute("id");
			IFormattingGroup formattingGroup = new FormattingGroup();
			if (_fileSkeleton.HasFormattingDefinitionWithId(attribute))
			{
				throw new XliffParseException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_ConflictingFormatDefinitions, attribute));
			}
			foreach (XmlElement item in fmtDefElement)
			{
				if (item != null && item.LocalName == "value")
				{
					string attribute2 = item.GetAttribute("key");
					string innerText = item.InnerText;
					formattingGroup.Add(base.PropertiesFactory.FormattingItemFactory.CreateFormattingItem(attribute2, innerText));
				}
			}
			_fileSkeleton.AddFormattingDefinition(attribute, formattingGroup);
		}

		private void ParseDocInfoElement(XmlElement docInfoElement)
		{
			foreach (XmlElement childNode in docInfoElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode))
				{
					switch (childNode.LocalName)
					{
					case "cmt-defs":
						ParseCommentDefinitionsElement(childNode);
						break;
					case "cmt-meta-defs":
						ParseCommentMetadataElement(childNode);
						break;
					default:
						ReportUnrecognizedElement(childNode);
						break;
					case "rep-defs":
					case "rev-defs":
						break;
					}
				}
			}
			foreach (XmlElement childNode2 in docInfoElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode2))
				{
					switch (childNode2.LocalName)
					{
					case "rep-defs":
						ParseRepetitionDefinitionsElement(childNode2);
						break;
					case "rev-defs":
						ParseRevisionDefinitionsElement(childNode2);
						break;
					default:
						ReportUnrecognizedElement(childNode2);
						break;
					case "cmt-defs":
					case "cmt-meta-defs":
						break;
					}
				}
			}
		}

		private void ParseRevisionDefinitionsElement(XmlElement revDefsElement)
		{
			foreach (XmlElement childNode in revDefsElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode, "rev-def"))
				{
					string attribute = childNode.GetAttribute("id");
					ParseRevisionDefinitionElement(attribute, childNode);
				}
			}
		}

		private void ParseRevisionDefinitionElement(string id, XmlElement revDefElement)
		{
			RevisionType revisionType = RevisionType.Insert;
			if (revDefElement.HasAttribute("type"))
			{
				revisionType = (RevisionType)Enum.Parse(typeof(RevisionType), revDefElement.GetAttribute("type"));
			}
			bool num = revisionType == RevisionType.FeedbackComment || revisionType == RevisionType.FeedbackAdded || revisionType == RevisionType.FeedbackDeleted;
			IRevisionProperties revisionProperties = num ? ItemFactory.PropertiesFactory.CreateFeedbackProperties(revisionType) : ItemFactory.PropertiesFactory.CreateRevisionProperties(revisionType);
			if (revDefElement.HasAttribute("author"))
			{
				revisionProperties.Author = revDefElement.GetAttribute("author");
			}
			if (revDefElement.HasAttribute("date"))
			{
				revisionProperties.Date = DateTime.Parse(revDefElement.GetAttribute("date"), CultureInfo.InvariantCulture);
			}
			if (num)
			{
				FeedbackProperties feedbackProperties = (FeedbackProperties)revisionProperties;
				if (revDefElement.HasAttribute("docCategory"))
				{
					feedbackProperties.DocumentCategory = revDefElement.GetAttribute("docCategory");
				}
				if (revDefElement.HasAttribute("fbCategory"))
				{
					feedbackProperties.FeedbackCategory = revDefElement.GetAttribute("fbCategory");
				}
				if (revDefElement.HasAttribute("fbSeverity"))
				{
					feedbackProperties.FeedbackSeverity = revDefElement.GetAttribute("fbSeverity");
				}
				if (revDefElement.HasAttribute("fbReplacementId"))
				{
					feedbackProperties.ReplacementId = revDefElement.GetAttribute("fbReplacementId");
				}
			}
			ParseFeedbackComment(revisionProperties, revDefElement);
			ParseRevisionValuesElement(revisionProperties, revDefElement);
			_docSkeleton.AddRevision(id, revisionProperties);
		}

		private void ParseCommentDefinitionsElement(XmlElement cmtDefsElement)
		{
			foreach (XmlElement childNode in cmtDefsElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode, "cmt-def"))
				{
					ICommentProperties commentProperties = base.PropertiesFactory.CreateCommentProperties();
					string oldValue = " xmlns=\"http://sdl.com/FileTypes/SdlXliff/1.0\"";
					string innerXml = childNode.InnerXml;
					innerXml = (commentProperties.Xml = innerXml.Replace(oldValue, ""));
					_docSkeleton.AddCommentsItem(childNode.GetAttribute("id"), commentProperties);
				}
			}
		}

		private void ParseCommentMetadataElement(XmlElement cmtMetadataElement)
		{
			foreach (XmlElement childNode in cmtMetadataElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode, "cmt-meta-def"))
				{
					string attribute = childNode.GetAttribute("id");
					ICommentProperties comments = _docSkeleton.GetComments(attribute);
					foreach (XmlElement childNode2 in childNode.ChildNodes)
					{
						if (CheckForExpectedSdlElement(childNode2, "cmt-meta-data"))
						{
							int index = int.Parse(childNode2.GetAttribute("id"));
							ParseCommentValuesElement(comments.GetItem(index), childNode2);
						}
					}
				}
			}
		}

		private void ParseRepetitionDefinitionsElement(XmlElement repDefsElement)
		{
			foreach (XmlElement childNode in repDefsElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode, "rep-def"))
				{
					string attribute = childNode.GetAttribute("id");
					ParseRepetitionDefinitionElement(attribute, childNode);
				}
			}
		}

		private void ParseRepetitionDefinitionElement(string repId, XmlElement repDefElement)
		{
			foreach (XmlElement item in repDefElement)
			{
				if (CheckForExpectedSdlElement(item, "entry"))
				{
					RepetitionId key = new RepetitionId(repId);
					ParagraphUnitId pu = new ParagraphUnitId(item.GetAttribute("tu"));
					SegmentId newRepetition = new SegmentId(item.GetAttribute("seg"));
					_documentProperties.Repetitions.Add(key, pu, newRepetition);
				}
			}
		}

		private void ParseTagElement(XmlElement tagElement)
		{
			TagId id = new TagId(tagElement.GetAttribute("id"));
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(tagElement.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("sdl", "http://sdl.com/FileTypes/SdlXliff/1.0");
			XmlElement xmlElement = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("bpt", xmlNamespaceManager), xmlNamespaceManager) as XmlElement;
			if (xmlElement != null)
			{
				ParseTagAsTagPair(xmlNamespaceManager, id, tagElement, xmlElement);
				return;
			}
			XmlElement xmlElement2 = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("ph", xmlNamespaceManager), xmlNamespaceManager) as XmlElement;
			if (xmlElement2 != null)
			{
				ParseTagAsPlaceholder(xmlNamespaceManager, id, tagElement, xmlElement2);
				return;
			}
			XmlElement xmlElement3 = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("st", xmlNamespaceManager), xmlNamespaceManager) as XmlElement;
			if (xmlElement3 != null)
			{
				ParseTagAsStructureElement(xmlNamespaceManager, id, tagElement, xmlElement3);
			}
			else
			{
				ReportUnrecognizedElement(tagElement);
			}
		}

		private void ParseTagAsTagPair(XmlNamespaceManager nsManager, TagId id, XmlElement tagElement, XmlElement bptElement)
		{
			if (_fileSkeleton.HasTagWithId(id, FileSkeleton.TagIdSearchType.SearchStartTags))
			{
				throw new XliffParseException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_TagDefinedMultipleTimes, id));
			}
			IStartTagProperties startTagProperties = base.PropertiesFactory.CreateStartTagProperties(bptElement.InnerText);
			startTagProperties.TagId = id;
			SetStartTagProperties(startTagProperties, bptElement.Attributes);
			XmlElement xmlElement = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("bpt-props", nsManager), nsManager) as XmlElement;
			if (xmlElement != null)
			{
				ParseCustomTagProperties(startTagProperties, xmlElement);
			}
			XmlElement xmlElement2 = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("ept", nsManager), nsManager) as XmlElement;
			if (xmlElement2 == null)
			{
				throw new XliffParseException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_MissingEndTagDefinition, tagElement.ToString()));
			}
			IEndTagProperties endTagProperties = base.PropertiesFactory.CreateEndTagProperties(xmlElement2.InnerText);
			SetEndTagProperties(endTagProperties, xmlElement2.Attributes);
			XmlElement xmlElement3 = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("ept-props", nsManager), nsManager) as XmlElement;
			if (xmlElement3 != null)
			{
				ParseCustomTagProperties(endTagProperties, xmlElement3);
			}
			ITagPair tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);
			ParseSubSegments(bptElement, tagPair);
			string text = (tagElement.SelectSingleNode(SdlXliffNames.Prefixed("fmt", nsManager), nsManager) as XmlElement)?.GetAttribute("id");
			if (!string.IsNullOrEmpty(text))
			{
				IFormattingGroup formatting = _fileSkeleton.GetFormatting(text);
				if (formatting == null)
				{
					throw new XliffParseException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_MissingFormattingDefinition, text));
				}
				if (startTagProperties.Formatting == null)
				{
					startTagProperties.Formatting = formatting;
				}
			}
			_fileSkeleton.AddTagPair(tagPair);
		}

		private void ParseTagAsPlaceholder(XmlNamespaceManager nsManager, TagId id, XmlElement tagElement, XmlElement phElement)
		{
			if (_fileSkeleton.HasTagWithId(id, FileSkeleton.TagIdSearchType.SearchPlaceholderTags))
			{
				throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_TagDefinedMultipleTimes, id));
			}
			IPlaceholderTagProperties placeholderTagProperties = base.PropertiesFactory.CreatePlaceholderTagProperties(phElement.InnerText);
			placeholderTagProperties.TagId = id;
			SetPlaceholderTagProperties(placeholderTagProperties, phElement.Attributes);
			XmlElement xmlElement = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("props", nsManager), nsManager) as XmlElement;
			if (xmlElement != null)
			{
				ParseCustomTagProperties(placeholderTagProperties, xmlElement);
			}
			IPlaceholderTag placeholderTag = ItemFactory.CreatePlaceholderTag(placeholderTagProperties);
			ParseSubSegments(phElement, placeholderTag);
			_fileSkeleton.AddPlaceholderTag(placeholderTag);
		}

		private void ParseTagAsStructureElement(XmlNamespaceManager nsManager, TagId id, XmlElement tagElement, XmlElement stElement)
		{
			if (_fileSkeleton.HasTagWithId(id, FileSkeleton.TagIdSearchType.SearchStructureTags))
			{
				throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_TagDefinedMultipleTimes, id));
			}
			IStructureTagProperties structureTagProperties = base.PropertiesFactory.CreateStructureTagProperties(stElement.InnerText);
			structureTagProperties.TagId = id;
			SetStructureTagProperties(structureTagProperties, stElement.Attributes);
			XmlElement xmlElement = tagElement.SelectSingleNode(SdlXliffNames.Prefixed("props", nsManager), nsManager) as XmlElement;
			if (xmlElement != null)
			{
				ParseCustomTagProperties(structureTagProperties, xmlElement);
			}
			IStructureTag tag = ItemFactory.CreateStructureTag(structureTagProperties);
			ParseSubSegments(stElement, tag);
			_fileSkeleton.AddStructureTag(tag);
		}

		private void SetStartTagProperties(IStartTagProperties properties, XmlAttributeCollection attrs)
		{
			SetBasicTagProperties(properties, attrs);
			SetInlineTagProperties(properties, attrs);
			foreach (XmlAttribute attr in attrs)
			{
				string localName = attr.LocalName;
				if (localName != null && localName == "seg-hint")
				{
					properties.SegmentationHint = (SegmentationHint)Enum.Parse(typeof(SegmentationHint), attr.Value);
				}
			}
		}

		private void SetEndTagProperties(IEndTagProperties properties, XmlAttributeCollection attrs)
		{
			SetBasicTagProperties(properties, attrs);
			SetInlineTagProperties(properties, attrs);
		}

		private void SetStructureTagProperties(IStructureTagProperties properties, XmlAttributeCollection attrs)
		{
			SetBasicTagProperties(properties, attrs);
		}

		private void SetPlaceholderTagProperties(IPlaceholderTagProperties properties, XmlAttributeCollection attrs)
		{
			SetBasicTagProperties(properties, attrs);
			SetInlineTagProperties(properties, attrs);
			properties.SegmentationHint = SegmentationHint.MayExclude;
			properties.IsBreakableWhiteSpace = false;
			foreach (XmlAttribute attr in attrs)
			{
				switch (attr.LocalName)
				{
				case "equiv-text":
					properties.TextEquivalent = attr.Value;
					break;
				case "is-whitespace":
					properties.IsBreakableWhiteSpace = ParseBoolValue(attr.Value);
					break;
				case "seg-hint":
					properties.SegmentationHint = (SegmentationHint)Enum.Parse(typeof(SegmentationHint), attr.Value);
					break;
				}
			}
		}

		private static void SetBasicTagProperties(IAbstractBasicTagProperties properties, XmlAttributeCollection attrs)
		{
			foreach (XmlAttribute attr in attrs)
			{
				string localName = attr.LocalName;
				if (localName != null && localName == "name")
				{
					properties.DisplayText = attr.Value;
				}
			}
		}

		private void ParseCustomTagProperties(IAbstractBasicTagProperties properties, XmlElement customPropertiesElement)
		{
			foreach (XmlElement childNode in customPropertiesElement.ChildNodes)
			{
				if (CheckForExpectedSdlElement(childNode, "value"))
				{
					properties.SetMetaData(childNode.GetAttribute("key"), childNode.InnerText);
				}
			}
		}

		private static void SetInlineTagProperties(IAbstractInlineTagProperties properties, XmlAttributeCollection attributes)
		{
			properties.CanHide = false;
			properties.IsSoftBreak = true;
			properties.IsWordStop = true;
			foreach (XmlAttribute attribute in attributes)
			{
				switch (attribute.LocalName)
				{
				case "can-hide":
					properties.CanHide = ParseBoolValue(attribute.Value);
					break;
				case "line-wrap":
					properties.IsSoftBreak = ParseBoolValue(attribute.Value);
					break;
				case "word-end":
					properties.IsWordStop = ParseBoolValue(attribute.Value);
					break;
				}
			}
		}

		internal static bool ParseBoolValue(string xsdBoolean)
		{
			if (xsdBoolean != null)
			{
				if (xsdBoolean == "true")
				{
					return true;
				}
				if (xsdBoolean == "false")
				{
					return false;
				}
			}
			throw new XliffParseException(string.Format(StringResources.CorruptFile_NotABooleanValue, xsdBoolean, "true", "false"));
		}

		private void ParseSubSegments(XmlElement element, IAbstractTag tag)
		{
			int num = 0;
			foreach (XmlNode childNode in element.ChildNodes)
			{
				XmlText xmlText = childNode as XmlText;
				if (xmlText != null)
				{
					num += xmlText.Value.Length;
				}
				else
				{
					XmlElement xmlElement = childNode as XmlElement;
					if (xmlElement != null && xmlElement.LocalName == "sub")
					{
						int offset = num;
						int length = xmlElement.InnerText.Length;
						num += length;
						ISubSegmentReference subSegmentReference = ItemFactory.CreateSubSegmentReference(base.PropertiesFactory.CreateSubSegmentProperties(offset, length), new ParagraphUnitId(xmlElement.GetAttribute("xid")));
						tag.AddSubSegmentReference(subSegmentReference);
						tag.TagProperties.AddSubSegment(subSegmentReference.Properties);
						IList<ISubSegmentReference> subSegments = _fileSkeleton.GetSubSegments(tag.TagProperties.TagId);
						if (subSegments == null || !subSegments.Contains(subSegmentReference))
						{
							_incompleteSubsegments.Add(subSegmentReference);
						}
					}
					else
					{
						num += childNode.InnerText.Length;
						string message = string.Format(StringResources.CorruptFile_UnexpectedTagContent, childNode.ToString(), element.OuterXml);
						ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Error, message, new TextLocation(tag), null);
					}
				}
			}
		}

		private void SetParagraphProperties(IParagraphUnitProperties puProperties, transunit transunit)
		{
			puProperties.ParagraphUnitId = new ParagraphUnitId(transunit.id);
			puProperties.Contexts = _currentContext;
			if (transunit.Any != null)
			{
				XmlElement[] any = transunit.Any;
				foreach (XmlElement xmlElement in any)
				{
					if (xmlElement.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0" && xmlElement.LocalName == "cmt")
					{
						puProperties.Comments = _docSkeleton.GetComments(xmlElement.GetAttribute("id"));
					}
				}
			}
			if (transunit.Items == null)
			{
				return;
			}
			object[] items = transunit.Items;
			for (int i = 0; i < items.Length; i++)
			{
				countgroup countgroup = items[i] as countgroup;
				if (countgroup != null)
				{
					Trace.Assert(1 == countgroup.count.GetLength(0));
					puProperties.SourceCount = new SourceCount
					{
						Value = long.Parse(countgroup.count[0].Value, CultureInfo.InvariantCulture)
					};
					switch (countgroup.count[0].unit)
					{
					case "word":
						puProperties.SourceCount.Unit = SourceCount.CountUnit.word;
						continue;
					case "character":
						puProperties.SourceCount.Unit = SourceCount.CountUnit.character;
						continue;
					}
					string message = string.Format(StringResources.UnrecognizedCountUnit, countgroup.count[0].unit);
					string locationDescription = transunit.ToString();
					MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, message, locationDescription);
				}
			}
		}

		private static LockTypeFlags GetLockedContentFlags(string flags)
		{
			LockTypeFlags lockTypeFlags = LockTypeFlags.Unlocked;
			if (flags.Contains(LockTypeFlags.Structure.ToString()))
			{
				lockTypeFlags = LockTypeFlags.Structure;
			}
			if (flags.Contains(LockTypeFlags.Externalized.ToString()))
			{
				lockTypeFlags |= LockTypeFlags.Externalized;
			}
			if (flags.Contains(LockTypeFlags.Manual.ToString()))
			{
				lockTypeFlags |= LockTypeFlags.Manual;
			}
			return lockTypeFlags;
		}

		public void SetFileProperties(IFileProperties properties)
		{
			IPersistentFileConversionProperties fileConversionProperties = properties.FileConversionProperties;
			if (fileConversionProperties == null)
			{
				throw new ArgumentNullException("properties", "FileConversionProperties is null inside.");
			}
			if (!string.IsNullOrEmpty(fileConversionProperties.OriginalFilePath))
			{
				_xliffOrigFilePath = fileConversionProperties.OriginalFilePath;
				_xliffInputFilePath = fileConversionProperties.InputFilePath;
			}
			if (_fileProperties == null)
			{
				_fileProperties = ItemFactory.CreateFileProperties();
			}
			_fileProperties.FileConversionProperties = fileConversionProperties;
		}

		public void StartOfInput()
		{
			if (_xliffInputFilePath == null)
			{
				throw new XliffParseException("Internal error: The XLIFF file path has not been set.");
			}
		}

		private void ValidateInputXml()
		{
			if (_validateXliff)
			{
				SdlXliffValidator sdlXliffValidator = new SdlXliffValidator(this.Progress);
				sdlXliffValidator.ValidationIssue += delegate(object o, SdlXliffValidationEventArgs args)
				{
					string message;
					string locationDescription;
					if (args.Line >= 0 && args.Offset >= 0)
					{
						message = string.Format(StringResources.XLIFF_ReaderValidationMessage, args.Message, args.Line.ToString(CultureInfo.CurrentCulture), args.Offset.ToString(CultureInfo.CurrentCulture));
						locationDescription = string.Format(StringResources.LineOffsetLocationInFile, args.Line.ToString(CultureInfo.CurrentCulture), args.Offset.ToString(CultureInfo.CurrentCulture), args.FilePath);
					}
					else
					{
						message = string.Format(StringResources.XLIFF_ReaderValidationMessageNoLineOffset, args.Message);
						locationDescription = string.Format(StringResources.LocationIsFileNameOnly, args.FilePath);
					}
					ReportMessage(this, StringResources.XliffFilterName, (args.Severity == XmlSeverityType.Warning) ? ErrorLevel.Warning : ErrorLevel.Error, message, locationDescription);
				};
				sdlXliffValidator.Validate(_xliffInputFilePath);
			}
		}

		public void EndOfInput()
		{
			Close();
		}

		public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
		{
			_settings.PopulateFromSettingsBundle(settingsBundle, configurationId);
			_validateXliff = _settings.ValidateXliff;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sdlXliffFeeder != null)
				{
					_sdlXliffFeeder.UnregisterSubscriber(this);
					_sdlXliffFeeder.Dispose();
					_sdlXliffFeeder = null;
				}
				Close();
			}
		}

		private void Close()
		{
			_sdlXliffFeeder?.Close();
		}

		~XliffFileReader()
		{
			Dispose(disposing: false);
		}

		public void OnDocInfo(XmlElement docInfo)
		{
			if (docInfo.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0")
			{
				ParseDocInfoElement(docInfo);
				_stage = ParseStage.BeforeProcessingFile;
			}
		}

		public bool OnStartFile(List<XmlAttribute> fileAttributes)
		{
			if (!_isOutputInitializedOnDocInfo)
			{
				Output.Initialize(_documentProperties);
				_isOutputInitializedOnDocInfo = true;
			}
			if (_fileProperties == null)
			{
				_fileProperties = ItemFactory.CreateFileProperties();
			}
			_paragraphDirectionCache = new ParagraphDirectionCache();
			if (_fileRestriction != null && !_fileRestriction(_fileProperties.FileConversionProperties))
			{
				_fileProperties = null;
				_fileSkeleton = null;
				return false;
			}
			_fileSkeleton = new FileSkeleton();
			XmlAttribute xmlAttribute = fileAttributes.Find((XmlAttribute attr) => attr.LocalName == "o-path");
			if (xmlAttribute != null)
			{
				_fileProperties.FileConversionProperties.OriginalFilePath = xmlAttribute.Value;
			}
			else
			{
				_fileProperties.FileConversionProperties.OriginalFilePath = string.Empty;
			}
			_fileProperties.FileConversionProperties.SourceLanguage = _documentProperties.SourceLanguage;
			_fileProperties.FileConversionProperties.TargetLanguage = _documentProperties.TargetLanguage;
			if (_referencedFiles == null)
			{
				_referencedFiles = new List<object>();
			}
			else
			{
				_referencedFiles.Clear();
			}
			_stage = ParseStage.BeforeProcessingFile;
			return true;
		}

		public void OnFileInfo(XmlElement fileInfo)
		{
			ParsePersistentFileProperties(fileInfo);
			_stage = ParseStage.ProcessingFileHeader;
		}

		public void OnTagDefinition(XmlElement tagDefinition)
		{
			if (tagDefinition.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0")
			{
				ParseTagElement(tagDefinition);
				_stage = ParseStage.ProcessingFileHeader;
			}
		}

		public void OnContextDefinition(XmlElement contextDefinition)
		{
			if (contextDefinition.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0")
			{
				ParseContextDefinitionElement(contextDefinition);
				_stage = ParseStage.ProcessingFileHeader;
			}
		}

		public void OnNodeDefinition(XmlElement nodeDefinition)
		{
			if (nodeDefinition.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0")
			{
				ParseNodeDefinitionElement(nodeDefinition);
				_stage = ParseStage.ProcessingFileHeader;
			}
		}

		public void OnFormattingDefinition(XmlElement formattingDefintion)
		{
			if (formattingDefintion.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0")
			{
				ParseFormattingDefinitionElement(formattingDefintion);
				_stage = ParseStage.ProcessingFileHeader;
			}
		}

		public void OnFileTypeInfo(XmlElement fileTypeInfo)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(fileTypeInfo.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("sdl", "http://sdl.com/FileTypes/SdlXliff/1.0");
			XmlElement xmlElement = fileTypeInfo.SelectSingleNode(SdlXliffNames.Prefixed("filetype-id", xmlNamespaceManager), xmlNamespaceManager) as XmlElement;
			if (xmlElement != null && _currentFileTypeDefinitionId == null)
			{
				_currentFileTypeDefinitionId = xmlElement.InnerText;
			}
			_stage = ParseStage.ProcessingFileHeader;
		}

		public void OnCommentReference(XmlElement commentReference)
		{
			string attribute = commentReference.GetAttribute("id");
			_fileProperties.Comments = _docSkeleton.GetComments(attribute);
			_stage = ParseStage.ProcessingFileHeader;
		}

		public void OnExternalFile(XmlElement externalFile)
		{
			externalfile externalfile = _externalFileSerializer.Deserialize(new WhitespacePreservingXmlTextReader(new StringReader(externalFile.OuterXml))) as externalfile;
			if (externalfile != null)
			{
				_referencedFiles.Add(externalfile);
			}
			else
			{
				string message = string.Format(StringResources.UnrecognizedContentInReferenceElement, externalFile.LocalName);
				MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, message, null);
			}
			_stage = ParseStage.ProcessingFileHeader;
		}

		public void OnInternalFile(XmlElement internalFile)
		{
			internalfile internalfile = _internalFileSerializer.Deserialize(new WhitespacePreservingXmlTextReader(new StringReader(internalFile.OuterXml))) as internalfile;
			if (internalfile != null)
			{
				_referencedFiles.Add(internalfile);
			}
			else
			{
				string message = string.Format(StringResources.UnrecognizedContentInReferenceElement, internalFile.LocalName);
				MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, message, null);
			}
			_stage = ParseStage.ProcessingFileHeader;
		}

		public void OnDependencyFiles(XmlElement dependencyFiles)
		{
			ParseDependencyFilesElement(dependencyFiles);
			int num = 0;
			foreach (object referencedFile in _referencedFiles)
			{
				if (_dependencyFiles.Count <= num)
				{
					throw new XliffParseException(string.Format(StringResources.CorruptFile_MissingReferenceFileDefinition, num));
				}
				IDependencyFileProperties dependencyFileProperties = _dependencyFiles[num];
				num++;
				externalfile externalfile = referencedFile as externalfile;
				if (externalfile != null)
				{
					if (!externalfile.uid.Equals(dependencyFileProperties.Id, StringComparison.InvariantCulture))
					{
						throw new XliffParseException(string.Format(StringResources.CorruptFile_FileIdMismatch, externalfile.uid, dependencyFileProperties.Id));
					}
					AddExternalFilePath(externalfile, dependencyFileProperties);
				}
				else
				{
					internalfile internalfile = referencedFile as internalfile;
					if (internalfile != null)
					{
						AddEmbeddedFile(internalfile, dependencyFileProperties);
					}
				}
			}
			_stage = ParseStage.ProcessingFileHeader;
		}

		public void OnGroup(XmlElement group)
		{
			group group2 = _groupSerializer.Deserialize(new WhitespacePreservingXmlTextReader(new StringReader(group.OuterXml))) as group;
			ParseGroup(group2);
			_stage = ParseStage.ProcessingFileBody;
		}

		public void OnTranslationUnit(XmlElement translationUnit)
		{
			transunit transunit = _transUnitSerializer.Deserialize(new WhitespacePreservingXmlTextReader(new StringReader(translationUnit.OuterXml))) as transunit;
			ParseTransUnit(transunit);
			_stage = ParseStage.ProcessingFileBody;
		}

		public void OnBinaryUnit(XmlElement binaryUnit)
		{
			throw new XliffParseException(StringResources.CorruptFile_BinUnitNotSupported);
		}

		public void OnStartFileHeader()
		{
			_currentFileTypeDefinitionId = null;
			_stage = ParseStage.ProcessingFileHeader;
		}

		public void OnEndFileHeader()
		{
			if (_currentFileTypeDefinitionId == null)
			{
				MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Error, StringResources.CorruptFile_FileTypeIdNotFound, null);
				_currentFileTypeDefinitionId = "Unknown File Type ID";
			}
			_fileProperties.FileConversionProperties.FileTypeDefinitionId = new FileTypeDefinitionId(_currentFileTypeDefinitionId);
			Output.SetFileProperties(_fileProperties);
			_stage = ParseStage.BeforeProcessingFileBody;
		}

		public void OnEndFile()
		{
			if (_incompleteSubsegments.Count > 0)
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_MissingSubsegmentParagraphUnit, _incompleteSubsegments[0].ParagraphUnitId.ToString()));
			}
			if (_outputBuffer.IsHolding)
			{
				_outputBuffer.Release();
			}
			Output.FileComplete();
			_fileProperties = null;
			_fileSkeleton = null;
			_stage = ParseStage.BeforeProcessingFile;
		}

		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			_sharedObjects = sharedObjects;
		}
	}
}
