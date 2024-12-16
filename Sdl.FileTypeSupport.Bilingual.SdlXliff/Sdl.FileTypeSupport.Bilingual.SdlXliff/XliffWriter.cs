using Oasis.Xliff12;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class XliffWriter : AbstractBilingualFileTypeComponent, IBilingualContentHandler, ISettingsAware, IDisposable
	{
		private long _maxEmbeddableFileSize = long.MaxValue;

		private string _xliffFilePath;

		private xliff _xliff;

		private readonly XmlDocument _xmlDoc = new XmlDocument();

		private List<file> _files;

		private readonly List<FileSkeleton> _fileHeaders = new List<FileSkeleton>();

		private DocSkeleton _docSkeleton;

		private readonly SdlXliffGeneralSettings _settings = new SdlXliffGeneralSettings();

		private XmlBuilder _segDefsElement;

		private HashSet<string> _segDefElementIDs;

		private Stack<List<object>> _openElementsStack = new Stack<List<object>>();

		private Stack<XmlBuilder> _openGroups = new Stack<XmlBuilder>();

		private XmlBuilder _transUnitBuilder;

		private Stack<transunit> _lockedContentTUs = new Stack<transunit>();

		private List<XmlElement> _dependencyFileElements = new List<XmlElement>();

		private List<ElemType_ExternalReference> _referenceElements = new List<ElemType_ExternalReference>();

		private XmlBuilder _referenceBuilder;

		private List<string> _bodyItemsFileList = new List<string>();

		private ParagraphDirectionCache _paragraphDirectionCache;

		private IContextProperties _currentContext;

		private IDocumentProperties _documentProperties;

		private DependencyFileHandling _dependencyHandling = DependencyFileHandling.ForceAutoEmbedOrLink;

		private IList<IDependencyFileProperties> _linkedDependencyFiles = new List<IDependencyFileProperties>();

		private int _nextLockedContentId = 1;

		private bool _writeXliffContextGroups;

		private FileSkeleton CurrentFileHeader => _fileHeaders[_fileHeaders.Count - 1];

		internal DocSkeleton DocSkeleton
		{
			get
			{
				return _docSkeleton;
			}
			set
			{
				_docSkeleton = value;
			}
		}

		internal Stack<transunit> LockedContentTUs
		{
			get
			{
				return _lockedContentTUs;
			}
			set
			{
				_lockedContentTUs = value;
			}
		}

		internal int NextLockedContentId
		{
			get
			{
				return _nextLockedContentId;
			}
			set
			{
				_nextLockedContentId = value;
			}
		}

		public bool WriteXliffContextGroups
		{
			get
			{
				return _writeXliffContextGroups;
			}
			set
			{
				_writeXliffContextGroups = value;
			}
		}

		public long MaxEmbeddableFileSize
		{
			get
			{
				return _maxEmbeddableFileSize;
			}
			set
			{
				_maxEmbeddableFileSize = value;
			}
		}

		public string OutputFilePath
		{
			get
			{
				return _xliffFilePath;
			}
			set
			{
				_xliffFilePath = value;
			}
		}

		public DependencyFileHandling DependencyHandling
		{
			get
			{
				return _dependencyHandling;
			}
			set
			{
				_dependencyHandling = value;
			}
		}

		public IList<IDependencyFileProperties> LinkedDependencyFiles
		{
			get
			{
				return _linkedDependencyFiles;
			}
			set
			{
				_linkedDependencyFiles = value;
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

		public xliff Xliff
		{
			get
			{
				return _xliff;
			}
			set
			{
				_xliff = value;
			}
		}

		public virtual void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = (documentInfo ?? throw new ArgumentNullException("documentInfo"));
			Xliff = new xliff
			{
				version = AttrType_Version.Item12
			};
			List<XmlAttribute> list = new List<XmlAttribute>();
			XmlAttribute xmlAttribute = CreateSdlPrefixAttribute("version");
			xmlAttribute.Value = SdlXliffVersions.CurrentVersionString;
			list.Add(xmlAttribute);
			Xliff.AnyAttr = list.ToArray();
			_files = new List<file>();
			_docSkeleton = new DocSkeleton();
		}

		public virtual void Complete()
		{
		}

		public virtual void WriteComplete(XliffFormattingXmlTextWriter writer)
		{
			XmlBuilder xmlBuilder = new XmlBuilder();
			xmlBuilder.AddDeclaration();
			xmlBuilder.StartElement("xliff");
			xmlBuilder.AddNamespace("sdl", "http://sdl.com/FileTypes/SdlXliff/1.0");
			xmlBuilder.AddNamespace("", "urn:oasis:names:tc:xliff:document:1.2");
			xmlBuilder.AddAttribute("version", "1.2");
			xmlBuilder.AddAttribute("sdl:version", "1.0");
			writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.OpeningTags));
			xmlBuilder.Clear();
			bool flag = false;
			if (_documentProperties.Repetitions.Count > 0 || _docSkeleton.HasCommentsItems || _docSkeleton.HasRevisions)
			{
				flag = true;
			}
			if (flag)
			{
				xmlBuilder.StartElement("", "doc-info", "http://sdl.com/FileTypes/SdlXliff/1.0");
			}
			if (_documentProperties.Repetitions.Count > 0)
			{
				BuildRepetitionDefinitionsElement(xmlBuilder);
			}
			if (_docSkeleton.HasRevisions)
			{
				BuildRevisionDefinitionsElement(xmlBuilder);
			}
			if (_docSkeleton.HasCommentsItems)
			{
				BuildCommentDefinitionsElement(xmlBuilder);
			}
			if (_docSkeleton.HasCommentsItems)
			{
				BuildCommentMetadataElement(xmlBuilder);
			}
			if (flag)
			{
				xmlBuilder.EndElement();
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
			}
		}

		public virtual void SetFileProperties(IFileProperties fileInfo)
		{
			_fileHeaders.Add(new FileSkeleton());
			CurrentFileHeader.PuStore.Add(new List<IParagraphUnit>());
			CurrentFileHeader.FileProperties = fileInfo;
			_paragraphDirectionCache = new ParagraphDirectionCache();
			file file = new file();
			file.header = new header();
			_files.Add(file);
			_openElementsStack = new Stack<List<object>>();
			_openElementsStack.Push(new List<object>());
			_currentContext = null;
			_bodyItemsFileList.Add(GetTempFileName());
		}

		public virtual void FileComplete()
		{
		}

		public void WriteFileComplete(FileSkeleton fileHeader, XliffFormattingXmlTextWriter writer)
		{
			XmlBuilder xmlBuilder = new XmlBuilder();
			xmlBuilder.StartElement("file");
			string text = _documentProperties.SourceLanguage.IsoAbbreviation;
			if (!_documentProperties.SourceLanguage.IsValid)
			{
				text = "en";
				if (MessageReporter != null)
				{
					MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, string.Format(StringResources.Writer_NoSourceLanguage, text), null);
				}
			}
			SetFileAttributes(fileHeader, xmlBuilder);
			if (!string.IsNullOrEmpty(text))
			{
				xmlBuilder.AddAttribute("source-language", text);
			}
			if (!string.IsNullOrEmpty(_documentProperties.TargetLanguage.IsoAbbreviation))
			{
				xmlBuilder.AddAttribute("target-language", _documentProperties.TargetLanguage.IsoAbbreviation);
			}
			xmlBuilder.StartElement("header");
			writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.OpeningTags));
			xmlBuilder.Clear();
			SetDependencyFiles(fileHeader, xmlBuilder);
			if (_referenceBuilder != null && _referenceBuilder.HasContent)
			{
				xmlBuilder.AddNodesToBuilder(_referenceBuilder);
				_referenceBuilder.Clear();
			}
			writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
			xmlBuilder.Clear();
			BuildFileInfoElement(fileHeader, xmlBuilder);
			writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
			xmlBuilder.Clear();
			BuildFilterInfoElement(xmlBuilder, fileHeader.FileProperties.FileConversionProperties.FileTypeDefinitionId.Id);
			writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
			xmlBuilder.Clear();
			if (fileHeader.HasFormattingDefinitions)
			{
				BuildFormattingDefinitionsElement(fileHeader, xmlBuilder);
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
				xmlBuilder.Clear();
			}
			if (fileHeader.HasContextDefinitions)
			{
				BuildContextDefinitionsElement(fileHeader, xmlBuilder);
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
				xmlBuilder.Clear();
			}
			if (fileHeader.HasStructureDefinitions)
			{
				BuildNodeDefinitionsElement(fileHeader, xmlBuilder);
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
				xmlBuilder.Clear();
			}
			if (fileHeader.HasTags)
			{
				BuildTagDefinitionsElement(fileHeader, xmlBuilder);
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
				xmlBuilder.Clear();
			}
			if (fileHeader.FileProperties.Comments != null && fileHeader.FileProperties.Comments.Count != 0)
			{
				WriteCommentsRefElement(xmlBuilder, fileHeader.FileProperties.Comments);
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
				xmlBuilder.Clear();
			}
			xmlBuilder.StartElement("header");
			writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.ClosingTags));
		}

		private void SetFileAttributes(FileSkeleton fileHeader, XmlBuilder fileBuilder)
		{
			fileBuilder.AddAttribute("original", (fileHeader.FileProperties.FileConversionProperties.OriginalFilePath != null) ? fileHeader.FileProperties.FileConversionProperties.OriginalFilePath : "");
			fileBuilder.AddAttribute("datatype", "x-sdlfilterframework2");
		}

		private void BuildFileInfoElement(FileSkeleton fileHeader, XmlBuilder builder)
		{
			BuildSdlElement(builder, "file-info");
			foreach (KeyValuePair<string, string> metaDatum in fileHeader.FileProperties.FileConversionProperties.MetaData)
			{
				switch (metaDatum.Key)
				{
				default:
					BuildKeyValuePairElement(builder, metaDatum.Key, metaDatum.Value, useSdlPrefix: false);
					break;
				case "SDL:SourceLanguage":
				case "SDL:TargetLanguage":
				case "SDL:FileTypeDefinitionId":
				case "ParagraphTextDirections":
					break;
				}
			}
			BuildKeyValuePairElement(builder, "ParagraphTextDirections", _paragraphDirectionCache.SerializeAsString(), useSdlPrefix: false);
			if (fileHeader.FileProperties.FileConversionProperties.FileSnifferInfo != null)
			{
				BuildSniffInfoElement(builder, fileHeader.FileProperties.FileConversionProperties.FileSnifferInfo);
			}
		}

		private void BuildKeyValuePairElement(XmlBuilder builder, string key, string value, bool useSdlPrefix)
		{
			if (useSdlPrefix)
			{
				BuildSdlPrefixElement(builder, "value");
			}
			else
			{
				builder.StartElement("value");
			}
			builder.SetAttribute("key", key);
			builder.AddText(value);
			builder.EndElement();
		}

		private void BuildSniffInfoElement(XmlBuilder builder, SniffInfo sniffInfo)
		{
			builder.StartElement("sniff-info");
			if (!sniffInfo.IsSupported)
			{
				builder.SetAttribute("is-supported", sniffInfo.IsSupported.ToString());
			}
			if (sniffInfo.DetectedEncoding.Second != 0)
			{
				builder.StartElement("detected-encoding");
				builder.SetAttribute("detection-level", sniffInfo.DetectedEncoding.Second.ToString());
				builder.SetAttribute("encoding", sniffInfo.DetectedEncoding.First.Name);
				builder.EndElement();
			}
			if (sniffInfo.DetectedSourceLanguage.Second != 0)
			{
				builder.StartElement("detected-source-lang");
				builder.SetAttribute("detection-level", sniffInfo.DetectedSourceLanguage.Second.ToString());
				builder.SetAttribute("lang", sniffInfo.DetectedSourceLanguage.First.IsoAbbreviation);
				builder.EndElement();
			}
			if (sniffInfo.DetectedTargetLanguage.Second != 0)
			{
				builder.StartElement("detected-target-lang");
				builder.SetAttribute("detection-level", sniffInfo.DetectedTargetLanguage.Second.ToString());
				builder.SetAttribute("lang", sniffInfo.DetectedTargetLanguage.First.IsoAbbreviation);
				builder.EndElement();
			}
			if (sniffInfo.SuggestedTargetEncoding != 0)
			{
				builder.StartElement("suggested-target-encoding");
				builder.SetAttribute("category", sniffInfo.SuggestedTargetEncoding.ToString());
				builder.EndElement();
			}
			if (sniffInfo.HasMetaData)
			{
				builder.StartElement("props");
				foreach (KeyValuePair<string, string> metaDatum in sniffInfo.MetaData)
				{
					BuildKeyValuePairElement(builder, metaDatum.Key, metaDatum.Value, useSdlPrefix: false);
				}
				builder.EndElement();
			}
		}

		private bool SegDefElementExists(string id)
		{
			if (_segDefElementIDs != null)
			{
				return _segDefElementIDs.Contains(id);
			}
			return false;
		}

		private void BuildFilterInfoElement(XmlBuilder builder, string filterId)
		{
			builder.StartElement("sdl", "filetype-info");
			builder.StartElement("sdl", "filetype-id");
			builder.AddText(filterId);
			builder.EndElement();
			builder.EndElement();
		}

		private void WriteCommentsRefElement(XmlBuilder builder, ICommentProperties comments)
		{
			builder.StartElement("sdl", "cmt");
			builder.SetAttribute("id", _docSkeleton.StoreComments(comments));
			builder.EndElement();
		}

		private void BuildNodeDefinitionsElement(FileSkeleton fileHeader, XmlBuilder builder)
		{
			BuildSdlElement(builder, "node-defs");
			foreach (KeyValuePair<string, IStructureInfo> item in fileHeader.StructureDefinitionsById)
			{
				BuildNodeDefinitionElement(fileHeader, builder, item.Value, item.Key);
			}
			builder.EndElement();
		}

		private void BuildNodeDefinitionElement(FileSkeleton fileHeader, XmlBuilder builder, IStructureInfo structureInfo, string id)
		{
			builder.StartElement("node-def");
			builder.SetAttribute("id", id);
			if (!structureInfo.MustUseDisplayName)
			{
				builder.SetAttribute("force-name", XliffBoolValue(structureInfo.MustUseDisplayName));
			}
			string structureId = fileHeader.GetStructureId(structureInfo.ParentStructure);
			if (!string.IsNullOrEmpty(structureId))
			{
				builder.SetAttribute("parent", structureId);
			}
			if (structureInfo.ContextInfo != null)
			{
				builder.StartElement("cxt");
				builder.SetAttribute("id", fileHeader.StoreContext(structureInfo.ContextInfo));
				builder.EndElement();
			}
			builder.EndElement();
		}

		private void BuildContextDefinitionsElement(FileSkeleton fileHeader, XmlBuilder builder)
		{
			BuildSdlElement(builder, "cxt-defs");
			foreach (KeyValuePair<string, IContextInfo> item in fileHeader.ContextDefinitionsById)
			{
				BuildContextDefinitionElement(fileHeader, builder, item.Value, item.Key);
			}
		}

		private void BuildContextDefinitionElement(FileSkeleton fileHeader, XmlBuilder builder, IContextInfo contextInfo, string id)
		{
			builder.StartElement("cxt-def");
			builder.SetAttribute("id", id);
			bool flag = false;
			if (!string.IsNullOrEmpty(contextInfo.ContextType))
			{
				builder.SetAttribute("type", contextInfo.ContextType);
				flag = StandardContextTypes.StandardContextData.ContainsKey(contextInfo.ContextType);
			}
			if (flag)
			{
				SetAttributesForStandardContext(builder, contextInfo);
			}
			else
			{
				SetAttributesForCustomContext(builder, contextInfo);
			}
			if (contextInfo.Purpose != 0)
			{
				builder.SetAttribute("purpose", contextInfo.Purpose.ToString());
			}
			if (contextInfo.DefaultFormatting != null)
			{
				builder.StartElement("fmt");
				builder.SetAttribute("id", fileHeader.StoreFormatting(contextInfo.DefaultFormatting));
				builder.EndElement();
			}
			if (contextInfo.MetaDataCount > 0)
			{
				BuildContextPropertiesElement(builder, contextInfo);
			}
			builder.EndElement();
		}

		private void SetAttributesForStandardContext(XmlBuilder builder, IContextInfo contextInfo)
		{
			StandardContextTypes.ContextData contextData = StandardContextTypes.StandardContextData[contextInfo.ContextType];
			if (!contextData.Code.Equals(contextInfo.DisplayCode))
			{
				builder.SetAttribute("code", contextInfo.DisplayCode);
			}
			if (!contextData.Name.Equals(contextInfo.DisplayName))
			{
				builder.SetAttribute("name", contextInfo.DisplayName);
			}
			if (!contextData.Description.Equals(contextInfo.Description))
			{
				builder.SetAttribute("descr", contextInfo.Description);
			}
			if (contextInfo.DisplayColor != contextData.Color)
			{
				builder.SetAttribute("color", TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, CultureInfo.InvariantCulture, contextInfo.DisplayColor));
			}
		}

		private void SetAttributesForCustomContext(XmlBuilder builder, IContextInfo contextInfo)
		{
			if (!string.IsNullOrEmpty(contextInfo.DisplayCode))
			{
				builder.SetAttribute("code", contextInfo.DisplayCode);
			}
			if (!string.IsNullOrEmpty(contextInfo.DisplayName))
			{
				builder.SetAttribute("name", contextInfo.DisplayName);
			}
			if (!string.IsNullOrEmpty(contextInfo.Description))
			{
				builder.SetAttribute("descr", contextInfo.Description);
			}
			if (contextInfo.DisplayColor != DefaultValues.ContextColor)
			{
				builder.SetAttribute("color", TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, CultureInfo.InvariantCulture, contextInfo.DisplayColor));
			}
		}

		private void BuildContextPropertiesElement(XmlBuilder builder, IMetaDataContainer metaData)
		{
			if (metaData != null)
			{
				builder.StartElement("props");
				foreach (KeyValuePair<string, string> metaDatum in metaData.MetaData)
				{
					builder.StartElement("value");
					builder.SetAttribute("key", metaDatum.Key);
					builder.AddText(metaDatum.Value);
					builder.EndElement();
				}
				builder.EndElement();
			}
		}

		private void BuildFormattingDefinitionsElement(FileSkeleton fileHeader, XmlBuilder builder)
		{
			BuildSdlElement(builder, "fmt-defs");
			foreach (KeyValuePair<string, IFormattingGroup> item in fileHeader.FormattingDefinitionsById)
			{
				BuildFormattingDefinitionElement(builder, item.Value, item.Key);
			}
			builder.EndElement();
		}

		private void BuildFormattingDefinitionElement(XmlBuilder builder, IFormattingGroup formatting, string id)
		{
			builder.StartElement("fmt-def");
			builder.SetAttribute("id", id);
			foreach (KeyValuePair<string, IFormattingItem> item in formatting)
			{
				BuildFormattingValueElement(builder, item.Value);
			}
			builder.EndElement();
		}

		private void BuildFormattingValueElement(XmlBuilder builder, IFormattingItem formattingItem)
		{
			builder.StartElement("value");
			builder.SetAttribute("key", formattingItem.FormattingName);
			builder.AddText(formattingItem.StringValue);
			builder.EndElement();
		}

		private void BuildRepetitionDefinitionsElement(XmlBuilder docInfo)
		{
			docInfo.StartElement("rep-defs");
			foreach (RepetitionId repetitionId in _documentProperties.Repetitions.RepetitionIds)
			{
				BuildRepetitionDefinitionElement(docInfo, repetitionId, _documentProperties.Repetitions.GetRepetitions(repetitionId));
			}
			docInfo.EndElement();
		}

		private void BuildRepetitionDefinitionElement(XmlBuilder repDef, RepetitionId id, IList<Pair<ParagraphUnitId, SegmentId>> repetitionList)
		{
			repDef.StartElement("rep-def");
			repDef.SetAttribute("id", id.ToString());
			foreach (Pair<ParagraphUnitId, SegmentId> repetition in repetitionList)
			{
				BuildRepetitionEntryElement(repDef, repetition.First, repetition.Second);
			}
			repDef.EndElement();
		}

		private void BuildRepetitionEntryElement(XmlBuilder repEntry, ParagraphUnitId puId, SegmentId segId)
		{
			repEntry.StartElement("entry");
			repEntry.SetAttribute("tu", puId.ToString());
			repEntry.SetAttribute("seg", segId.ToString());
			repEntry.EndElement();
		}

		private void BuildRevisionDefinitionsElement(XmlBuilder docInfo)
		{
			docInfo.StartElement("rev-defs");
			foreach (KeyValuePair<string, IRevisionProperties> item in _docSkeleton.RevisionsById)
			{
				BuildRevisionDefinitionElement(docInfo, item.Key, item.Value);
			}
			docInfo.EndElement();
		}

		private void BuildRevisionDefinitionElement(XmlBuilder revisionDef, string id, IRevisionProperties revision)
		{
			revisionDef.StartElement("rev-def");
			revisionDef.SetAttribute("id", id);
			if (revision.RevisionType != 0)
			{
				revisionDef.SetAttribute("type", revision.RevisionType.ToString());
			}
			if (!string.IsNullOrEmpty(revision.Author))
			{
				revisionDef.SetAttribute("author", revision.Author);
			}
			if (revision.Date.HasValue)
			{
				revisionDef.SetAttribute("date", revision.Date.Value.ToString(CultureInfo.InvariantCulture));
			}
			if (revision is FeedbackProperties)
			{
				FeedbackProperties feedbackProperties = revision as FeedbackProperties;
				if (!string.IsNullOrEmpty(feedbackProperties.DocumentCategory))
				{
					revisionDef.SetAttribute("docCategory", feedbackProperties.DocumentCategory);
				}
				if (!string.IsNullOrEmpty(feedbackProperties.FeedbackCategory))
				{
					revisionDef.SetAttribute("fbCategory", feedbackProperties.FeedbackCategory);
				}
				if (!string.IsNullOrEmpty(feedbackProperties.FeedbackSeverity))
				{
					revisionDef.SetAttribute("fbSeverity", feedbackProperties.FeedbackSeverity);
				}
				if (!string.IsNullOrEmpty(feedbackProperties.ReplacementId))
				{
					revisionDef.SetAttribute("fbReplacementId", feedbackProperties.ReplacementId);
				}
				if (feedbackProperties.FeedbackComment != null)
				{
					ICommentProperties commentProperties = base.PropertiesFactory.CreateCommentProperties();
					commentProperties.Add(feedbackProperties.FeedbackComment);
					WriteCommentsRefElement(revisionDef, commentProperties);
				}
			}
			foreach (KeyValuePair<string, string> metaDatum in revision.MetaData)
			{
				BuildKeyValuePairElement(revisionDef, metaDatum.Key, metaDatum.Value, useSdlPrefix: false);
			}
			revisionDef.EndElement();
		}

		private void BuildCommentDefinitionsElement(XmlBuilder docInfo)
		{
			docInfo.StartElement("cmt-defs");
			foreach (KeyValuePair<string, ICommentProperties> item in _docSkeleton.CommentsItemsById)
			{
				BuildCommentDefinitionElement(docInfo, item.Key, item.Value);
			}
			docInfo.EndElement();
		}

		private void BuildCommentDefinitionElement(XmlBuilder commentDefs, string id, ICommentProperties comments)
		{
			commentDefs.StartElement("cmt-def");
			commentDefs.SetAttribute("id", id);
			_ = comments.Xml;
			commentDefs.AddRawText(comments.Xml);
			commentDefs.EndElement();
		}

		private void BuildCommentMetadataElement(XmlBuilder docInfo)
		{
			if (HasMetadataInComments())
			{
				docInfo.StartElement("cmt-meta-defs");
				foreach (KeyValuePair<string, ICommentProperties> item in _docSkeleton.CommentsItemsById)
				{
					BuildCommentMetadataDefElement(docInfo, item.Key, item.Value);
				}
				docInfo.EndElement();
			}
		}

		private void BuildCommentMetadataDefElement(XmlBuilder commentDefs, string id, ICommentProperties properties)
		{
			if (HasMetadataInCommentProperties(properties))
			{
				commentDefs.StartElement("cmt-meta-def");
				commentDefs.AddAttribute("id", id);
				int num = 0;
				foreach (IComment comment in properties.Comments)
				{
					if (comment.HasMetaData)
					{
						commentDefs.StartElement("cmt-meta-data");
						commentDefs.AddAttribute("id", num.ToString());
						foreach (KeyValuePair<string, string> metaDatum in comment.MetaData)
						{
							BuildKeyValuePairElement(commentDefs, metaDatum.Key, metaDatum.Value, useSdlPrefix: false);
						}
						commentDefs.EndElement();
					}
					num++;
				}
				commentDefs.EndElement();
			}
		}

		private bool HasMetadataInComments()
		{
			foreach (KeyValuePair<string, ICommentProperties> item in _docSkeleton.CommentsItemsById)
			{
				foreach (IComment comment in item.Value.Comments)
				{
					if (comment.HasMetaData)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasMetadataInCommentProperties(ICommentProperties properties)
		{
			foreach (IComment comment in properties.Comments)
			{
				if (comment.HasMetaData)
				{
					return true;
				}
			}
			return false;
		}

		private void BuildTagDefinitionsElement(FileSkeleton fileHeader, XmlBuilder builder)
		{
			BuildSdlElement(builder, "tag-defs");
			foreach (TagId pairedTagId in fileHeader.PairedTagIds)
			{
				BuildTagDefinitionElement(fileHeader, builder, fileHeader.GetStartTagProperties(pairedTagId), fileHeader.GetEndTagProperties(pairedTagId), fileHeader.GetSubSegments(pairedTagId));
			}
			foreach (TagId placeholderTagId in fileHeader.PlaceholderTagIds)
			{
				BuildTagDefinitionElement(builder, fileHeader.GetPlaceholderTagProperties(placeholderTagId), fileHeader.GetSubSegments(placeholderTagId));
			}
			foreach (TagId structureTagId in fileHeader.StructureTagIds)
			{
				BuildTagDefinitionElement(builder, fileHeader.GetStructureTagProperties(structureTagId), fileHeader.GetSubSegments(structureTagId));
			}
			builder.EndElement();
		}

		private void BuildTagDefinitionElement(FileSkeleton fileHeader, XmlBuilder builder, IStartTagProperties startTagProperties, IEndTagProperties endTagProperties, IList<ISubSegmentReference> subSegments)
		{
			builder.StartElement("tag");
			builder.SetAttribute("id", startTagProperties.TagId.Id);
			builder.StartElement("bpt");
			ApplyStartTagProperties(builder, startTagProperties, subSegments);
			builder.EndElement();
			if (startTagProperties.HasMetaData)
			{
				builder.StartElement("bpt-props");
				ApplyCustomTagProperties(startTagProperties, builder);
				builder.EndElement();
			}
			builder.StartElement("ept");
			ApplyEndTagProperties(builder, endTagProperties);
			builder.EndElement();
			if (endTagProperties.HasMetaData)
			{
				builder.StartElement("ept-props");
				ApplyCustomTagProperties(endTagProperties, builder);
				builder.EndElement();
			}
			if (startTagProperties.Formatting != null)
			{
				builder.StartElement("fmt");
				builder.SetAttribute("id", fileHeader.StoreFormatting(startTagProperties.Formatting));
				builder.EndElement();
			}
			builder.EndElement();
		}

		private void ApplyBasicTagProperties(IAbstractBasicTagProperties tagProperties, XmlBuilder builder)
		{
			if (tagProperties.DisplayText != null)
			{
				builder.SetAttribute("name", tagProperties.DisplayText);
			}
		}

		private void ApplyCustomTagProperties(IMetaDataContainer metaDataContainer, XmlBuilder builder)
		{
			foreach (KeyValuePair<string, string> metaDatum in metaDataContainer.MetaData)
			{
				BuildKeyValuePairElement(builder, metaDatum.Key, metaDatum.Value, useSdlPrefix: false);
			}
		}

		private static void ApplyInlineTagProperties(IAbstractInlineTagProperties tagProperties, XmlBuilder builder)
		{
			if (tagProperties.CanHide)
			{
				builder.SetAttribute("can-hide", XliffBoolValue(tagProperties.CanHide));
			}
			if (!tagProperties.IsSoftBreak)
			{
				builder.SetAttribute("line-wrap", XliffBoolValue(tagProperties.IsSoftBreak));
			}
			if (!tagProperties.IsWordStop)
			{
				builder.SetAttribute("word-end", XliffBoolValue(tagProperties.IsWordStop));
			}
		}

		private void BuildTagDefinitionElement(XmlBuilder builder, IPlaceholderTagProperties placeholderTagProperties, IList<ISubSegmentReference> subSegments)
		{
			builder.StartElement("tag");
			builder.SetAttribute("id", placeholderTagProperties.TagId.Id);
			builder.StartElement("ph");
			ApplyPlaceholderTagProperties(builder, placeholderTagProperties, subSegments);
			builder.EndElement();
			if (placeholderTagProperties.HasMetaData)
			{
				builder.StartElement("props");
				ApplyCustomTagProperties(placeholderTagProperties, builder);
				builder.EndElement();
			}
			builder.EndElement();
		}

		private void ApplyPlaceholderTagProperties(XmlBuilder builder, IPlaceholderTagProperties tagProperties, IList<ISubSegmentReference> subSegments)
		{
			SetTagContent(builder, tagProperties.TagContent, subSegments);
			ApplyBasicTagProperties(tagProperties, builder);
			ApplyInlineTagProperties(tagProperties, builder);
			if (tagProperties.HasTextEquivalent)
			{
				builder.SetAttribute("equiv-text", tagProperties.TextEquivalent);
			}
			if (tagProperties.IsBreakableWhiteSpace)
			{
				builder.SetAttribute("is-whitespace", XliffBoolValue(tagProperties.IsBreakableWhiteSpace));
			}
			if (tagProperties.SegmentationHint != SegmentationHint.MayExclude)
			{
				builder.SetAttribute("seg-hint", tagProperties.SegmentationHint.ToString());
			}
		}

		internal static string XliffBoolValue(bool value)
		{
			if (!value)
			{
				return "false";
			}
			return "true";
		}

		private void BuildTagDefinitionElement(XmlBuilder builder, IStructureTagProperties structureTagProperties, IList<ISubSegmentReference> subSegments)
		{
			builder.StartElement("tag");
			builder.SetAttribute("id", structureTagProperties.TagId.Id);
			builder.StartElement("st");
			ApplyStructureTagProperties(builder, structureTagProperties, subSegments);
			builder.EndElement();
			if (structureTagProperties.HasMetaData)
			{
				builder.StartElement("props");
				ApplyCustomTagProperties(structureTagProperties, builder);
				builder.EndElement();
			}
			builder.EndElement();
		}

		private void ApplyStructureTagProperties(XmlBuilder builder, IStructureTagProperties tagProperties, IList<ISubSegmentReference> subSegments)
		{
			SetTagContent(builder, tagProperties.TagContent, subSegments);
			ApplyBasicTagProperties(tagProperties, builder);
		}

		private void SetDependencyFiles(FileSkeleton fileHeader, XmlBuilder builder)
		{
			if (fileHeader.FileProperties.FileConversionProperties.DependencyFiles.Count != 0 && _dependencyHandling != DependencyFileHandling.Ignore)
			{
				_referenceBuilder = new XmlBuilder();
				_referenceBuilder.StartElement("sdl", "ref-files");
				foreach (IDependencyFileProperties dependencyFile in fileHeader.FileProperties.FileConversionProperties.DependencyFiles)
				{
					AddDependencyFile(builder, dependencyFile);
				}
				_referenceElements.Clear();
			}
		}

		private void AddDependencyFile(XmlBuilder builder, IDependencyFileProperties file)
		{
			switch (file.PreferredLinkage)
			{
			case DependencyFileLinkOption.Ignore:
				break;
			case DependencyFileLinkOption.None:
				switch (_dependencyHandling)
				{
				case DependencyFileHandling.Ignore:
					break;
				case DependencyFileHandling.Embed:
					EmbedDependencyFile(builder, file);
					break;
				case DependencyFileHandling.Link:
					LinkDependencyFile(builder, file, forceAbsoluteReference: false);
					break;
				case DependencyFileHandling.AutoEmbedOrLink:
				case DependencyFileHandling.ForceAutoEmbedOrLink:
					AutoEmbedOrLinkDependencyFile(builder, file);
					break;
				}
				break;
			case DependencyFileLinkOption.ReferenceRelative:
				LinkDependencyFile(builder, file, forceAbsoluteReference: false);
				break;
			case DependencyFileLinkOption.ReferenceAbsolute:
				LinkDependencyFile(builder, file, forceAbsoluteReference: true);
				break;
			case DependencyFileLinkOption.Embed:
				if (_dependencyHandling == DependencyFileHandling.ForceAutoEmbedOrLink)
				{
					AutoEmbedOrLinkDependencyFile(builder, file);
				}
				else
				{
					EmbedDependencyFile(builder, file);
				}
				break;
			}
		}

		protected virtual void AutoEmbedOrLinkDependencyFile(XmlBuilder builder, IDependencyFileProperties file)
		{
			bool flag = true;
			if (File.Exists(file.CurrentFilePath))
			{
				if (new FileInfo(file.CurrentFilePath).Length > _maxEmbeddableFileSize)
				{
					flag = false;
					string tempPath = Path.GetTempPath();
					string fullPath = Path.GetFullPath(file.CurrentFilePath);
					if (fullPath.Length > tempPath.Length && string.Compare(fullPath.Substring(0, tempPath.Length), tempPath, ignoreCase: true) == 0)
					{
						flag = true;
					}
				}
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				EmbedDependencyFile(builder, file);
			}
			else
			{
				LinkDependencyFile(builder, file, forceAbsoluteReference: false);
			}
		}

		protected virtual void EmbedDependencyFile(XmlBuilder builder, IDependencyFileProperties file)
		{
			IXmlBuilderElement obj = builder.CurrentNode as IXmlBuilderElement;
			builder.StartElement("reference");
			builder.StartElement("internal-file");
			builder.AddAttribute("form", "base64");
			FileManager.WriteZippedFile(builder, file.ZippedCurrentFile.FilePath);
			builder.EndElement();
			builder.EndElement();
			string fileId = (obj.Count - 1).ToString();
			StoreDependencyFileProperties(file, fileId);
		}

		protected virtual void LinkDependencyFile(XmlBuilder builder, IDependencyFileProperties file, bool forceAbsoluteReference)
		{
			IDependencyFileProperties dependencyFileProperties = (IDependencyFileProperties)file.Clone();
			dependencyFileProperties.DisposableObject = null;
			string str = dependencyFileProperties.CurrentFilePath;
			if (!forceAbsoluteReference && !string.IsNullOrEmpty(file.PathRelativeToConverted))
			{
				str = dependencyFileProperties.PathRelativeToConverted;
			}
			else if (!forceAbsoluteReference)
			{
				string tempPath = Path.GetTempPath();
				string fullPath = Path.GetFullPath(OutputFilePath);
				if (fullPath.Length <= tempPath.Length || string.Compare(fullPath.Substring(0, tempPath.Length), tempPath, ignoreCase: true) != 0)
				{
					dependencyFileProperties.PathRelativeToConverted = PathUtil.RelativePathTo(Path.GetDirectoryName(Path.GetFullPath(OutputFilePath)), Path.GetFullPath(file.CurrentFilePath));
				}
			}
			int count = (builder.CurrentNode as IXmlBuilderElement).Count;
			builder.StartElement("reference");
			builder.StartElement("external-file");
			builder.AddAttribute("href", "file://" + str);
			string text = string.IsNullOrEmpty(file.Id) ? count.ToString(CultureInfo.InvariantCulture) : file.Id;
			text = text.Replace("/", "_x002F_");
			text = text.Replace(" ", "_x0020_");
			builder.AddAttribute("uid", text);
			builder.EndElement();
			builder.EndElement();
			_linkedDependencyFiles.Add(dependencyFileProperties);
			StoreDependencyFileProperties(dependencyFileProperties, text);
		}

		private void StoreDependencyFileProperties(IDependencyFileProperties file, string fileId)
		{
			_referenceBuilder.StartElement("sdl", "ref-file");
			_referenceBuilder.SetAttribute("uid", fileId);
			if (!string.IsNullOrEmpty(file.Id))
			{
				_referenceBuilder.SetAttribute("id", file.Id);
			}
			if (!string.IsNullOrEmpty(file.CurrentFilePath))
			{
				_referenceBuilder.SetAttribute("name", Path.GetFileName(file.CurrentFilePath));
			}
			if (!string.IsNullOrEmpty(file.OriginalFilePath))
			{
				_referenceBuilder.SetAttribute("o-path", file.OriginalFilePath);
				_referenceBuilder.SetAttribute("date", file.OriginalLastChangeDate.ToString(CultureInfo.InvariantCulture));
			}
			if (!string.IsNullOrEmpty(file.PathRelativeToConverted))
			{
				_referenceBuilder.SetAttribute("rel-path", file.PathRelativeToConverted);
			}
			if (!string.IsNullOrEmpty(file.Description))
			{
				_referenceBuilder.SetAttribute("descr", file.Description);
			}
			if (file.ExpectedUsage != 0)
			{
				_referenceBuilder.SetAttribute("expected-use", file.ExpectedUsage.ToString());
			}
			if (file.PreferredLinkage != 0)
			{
				_referenceBuilder.SetAttribute("pref-reftype", file.PreferredLinkage.ToString());
			}
			_referenceBuilder.EndElement();
		}

		private void CloseGroup(XliffFormattingXmlTextWriter writer)
		{
			XmlBuilder xmlBuilder = _openGroups.Pop();
			xmlBuilder.EndElement();
			_openElementsStack.Pop().ToArray();
			if (_openGroups.Count == 0)
			{
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.FullTree));
			}
			else
			{
				_openGroups.Peek().AddNodesToBuilder(xmlBuilder);
			}
		}

		private void ManageContextChange(IContextProperties paragraphUnitContext)
		{
			if (_currentContext != paragraphUnitContext)
			{
				if (paragraphUnitContext != null)
				{
					OpenNewContext(paragraphUnitContext);
				}
				_currentContext = paragraphUnitContext;
			}
		}

		private void OpenNewContext(IContextProperties contexts)
		{
			StoreSdlContextsElement(contexts);
		}

		private void StoreSdlContextsElement(IContextProperties contexts)
		{
			if (contexts.Contexts != null)
			{
				foreach (IContextInfo context in contexts.Contexts)
				{
					CurrentFileHeader.StoreContext(context);
				}
				if (contexts.StructureInfo != null)
				{
					CurrentFileHeader.StoreStructure(contexts.StructureInfo);
				}
			}
		}

		private void WriteContextChange(FileSkeleton fileHeader, XliffFormattingXmlTextWriter writer, IContextProperties paragraphUnitContext)
		{
			if (_currentContext != paragraphUnitContext)
			{
				if (_currentContext != null)
				{
					CloseGroup(writer);
				}
				if (paragraphUnitContext != null)
				{
					WriteNewContext(fileHeader, paragraphUnitContext);
				}
				_currentContext = paragraphUnitContext;
			}
		}

		private void WriteNewContext(FileSkeleton fileHeader, IContextProperties contexts)
		{
			XmlBuilder xmlBuilder = new XmlBuilder();
			xmlBuilder.StartElement("group");
			_openGroups.Push(xmlBuilder);
			_openElementsStack.Push(new List<object>());
			WriteSdlContextsElement(fileHeader, xmlBuilder, contexts);
		}

		private void WriteXliffContextGroupElement(XmlBuilder group, IContextProperties contexts)
		{
			if (contexts.Contexts.Count != 0)
			{
				group.StartElement("contextgroup");
				List<context> list = new List<context>();
				foreach (IContextInfo context in contexts.Contexts)
				{
					switch (context.Purpose)
					{
					case ContextPurpose.Information:
						WriteXliffContextElement(group, context);
						group.EndElement();
						break;
					case ContextPurpose.Match:
						WriteXliffContextElement(group, context);
						group.AddAttribute("matchmandatory", "yes");
						group.EndElement();
						break;
					}
				}
				if (list.Count != 0)
				{
					group.EndElement();
				}
			}
		}

		private void WriteXliffContextElement(XmlBuilder group, IContextInfo ctx)
		{
			group.StartElement("context");
			group.AddAttribute("contexttype", "x-" + ctx.ContextType);
			group.AddText(ctx.ContextType);
		}

		private void WriteSdlContextsElement(FileSkeleton fileHeader, XmlBuilder group, IContextProperties contexts)
		{
			if (contexts.Contexts != null)
			{
				group.StartElement("sdl", "cxts");
				foreach (IContextInfo context in contexts.Contexts)
				{
					group.StartElement("sdl", "cxt");
					group.AddAttribute("id", fileHeader.StoreContext(context));
					group.EndElement();
				}
				if (contexts.StructureInfo != null)
				{
					group.StartElement("sdl", "node");
					group.AddAttribute("id", CurrentFileHeader.StoreStructure(contexts.StructureInfo));
					group.EndElement();
				}
				group.EndElement();
			}
		}

		protected void WriteSdlXliff(XliffFormattingXmlTextWriter writer)
		{
			foreach (FileSkeleton fileHeader in _fileHeaders)
			{
				if (fileHeader.FileProperties.Comments != null && fileHeader.FileProperties.Comments.Count != 0)
				{
					_docSkeleton.StoreComments(fileHeader.FileProperties.Comments);
				}
			}
			WriteComplete(writer);
			XmlBuilder xmlBuilder = new XmlBuilder();
			foreach (FileSkeleton fileHeader2 in _fileHeaders)
			{
				_currentContext = null;
				WriteFileComplete(fileHeader2, writer);
				xmlBuilder.StartElement("body");
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.OpeningTags));
				foreach (IParagraphUnit item in fileHeader2.CurrentPuStore)
				{
					ProcessCachedParagraphUnit(fileHeader2, item, writer);
				}
				while (_openElementsStack.Count > 1)
				{
					CloseGroup(writer);
				}
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.ClosingTags));
				xmlBuilder.Clear();
				xmlBuilder.StartElement("file");
				writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.ClosingTags));
				xmlBuilder.Clear();
			}
			xmlBuilder.StartElement("xliff");
			writer.WriteRaw(xmlBuilder.BuildXmlString(TreeGeneration.ClosingTags));
		}

		public virtual void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			CurrentFileHeader.CurrentPuStore.Add(paragraphUnit);
			ManageContextChange(paragraphUnit.Properties.Contexts);
			TransUnitBuilder.BuildTransUnitContentForChildren(paragraphUnit.Source, this, segmentsAsMrk: false);
			if (!paragraphUnit.IsStructure)
			{
				_paragraphDirectionCache.StoreParagraphDirection(paragraphUnit.Source);
				if (TransUnitBuilder.BuildTransUnitContentForChildren(paragraphUnit.Target, this, segmentsAsMrk: false))
				{
					_paragraphDirectionCache.StoreParagraphDirection(paragraphUnit.Target);
				}
				if (paragraphUnit.Properties.Comments != null)
				{
					_docSkeleton.StoreComments(paragraphUnit.Properties.Comments);
				}
				_segDefsElement = null;
				_segDefElementIDs = null;
			}
		}

		public virtual void ProcessCachedParagraphUnit(FileSkeleton fileHeader, IParagraphUnit paragraphUnit, XliffFormattingXmlTextWriter writer)
		{
			_segDefsElement = null;
			_segDefElementIDs = null;
			WriteContextChange(fileHeader, writer, paragraphUnit.Properties.Contexts);
			XmlBuilder xmlBuilder = new XmlBuilder();
			_transUnitBuilder = ((_openGroups.Count > 0) ? _openGroups.Peek() : new XmlBuilder());
			xmlBuilder.StartElement("trans-unit");
			if (paragraphUnit.Properties.LockType == LockTypeFlags.Structure)
			{
				xmlBuilder.AddAttribute("translate", "no");
			}
			else
			{
				string lockString = GetLockString(paragraphUnit.Properties.LockType);
				if (lockString.Length > 0)
				{
					xmlBuilder.AddAttribute("sdl:locktype", lockString);
					xmlBuilder.AddAttribute("translate", "no");
				}
			}
			xmlBuilder.AddAttribute("id", paragraphUnit.Properties.ParagraphUnitId.Id);
			xmlBuilder.StartElement("source");
			TransUnitWriter transUnitWriter = new TransUnitWriter(this, xmlBuilder);
			transUnitWriter.WriteTransUnitContentForChildren(paragraphUnit.Source, segmentsAsMrk: false);
			xmlBuilder.EndElement();
			if (paragraphUnit.IsStructure)
			{
				_transUnitBuilder.AddNodesToBuilder(xmlBuilder);
				OutputTransUnits(writer);
				return;
			}
			xmlBuilder.StartElement("seg-source");
			TransUnitWriter transUnitWriter2 = new TransUnitWriter(this, xmlBuilder);
			transUnitWriter2.WriteTransUnitContentForChildren(paragraphUnit.Source, segmentsAsMrk: true);
			xmlBuilder.EndElement();
			_paragraphDirectionCache.StoreParagraphDirection(paragraphUnit.Source);
			xmlBuilder.StartElement("target");
			TransUnitWriter transUnitWriter3 = new TransUnitWriter(this, xmlBuilder);
			transUnitWriter3.WriteTransUnitContentForChildren(paragraphUnit.Target, segmentsAsMrk: true);
			xmlBuilder.EndElement();
			if (xmlBuilder.HasContent)
			{
				_paragraphDirectionCache.StoreParagraphDirection(paragraphUnit.Target);
			}
			if ((paragraphUnit.Properties.LockType & LockTypeFlags.Externalized) > LockTypeFlags.Unlocked || (paragraphUnit.Properties.LockType & LockTypeFlags.Manual) > LockTypeFlags.Unlocked)
			{
				xmlBuilder.SetAttribute("translate", "no");
			}
			AddLockedContentToBuilder(_transUnitBuilder, transUnitWriter3.LockedContentBuilders);
			AddLockedContentToBuilder(_transUnitBuilder, transUnitWriter2.LockedContentBuilders);
			AddLockedContentToBuilder(_transUnitBuilder, transUnitWriter.LockedContentBuilders);
			if (paragraphUnit.Properties.SourceCount != null)
			{
				xmlBuilder.StartElement("count-group");
				xmlBuilder.StartElement("count");
				SourceCount sourceCount = paragraphUnit.Properties.SourceCount;
				xmlBuilder.AddText(sourceCount.Value.ToString(CultureInfo.InvariantCulture));
				xmlBuilder.SetAttribute("unit", sourceCount.Unit.ToString());
				xmlBuilder.EndElement();
				xmlBuilder.AddAttribute("name", Guid.NewGuid().ToString());
				xmlBuilder.EndElement();
			}
			if (_segDefsElement != null && _segDefsElement.HasContent)
			{
				_segDefsElement.EndElement();
				xmlBuilder.AddNodesToBuilder(_segDefsElement);
			}
			_segDefsElement = null;
			_segDefElementIDs = null;
			if (paragraphUnit.Properties.Comments != null)
			{
				WriteCommentsRefElement(xmlBuilder, paragraphUnit.Properties.Comments);
			}
			_transUnitBuilder.AddNodesToBuilder(xmlBuilder);
			OutputTransUnits(writer);
		}

		public void AddLockedContentToBuilder(XmlBuilder target, List<XmlBuilder> builderList)
		{
			for (int num = builderList.Count - 1; num >= 0; num--)
			{
				target.AddNodesToBuilder(builderList[num]);
			}
		}

		public void OutputTransUnits(XliffFormattingXmlTextWriter writer)
		{
			if (_openGroups.Count == 0)
			{
				writer.WriteRaw(_transUnitBuilder.BuildXmlString(TreeGeneration.FullTree));
			}
		}

		internal void SetSegmentOriginProperties(ITranslationOrigin segmentOrigin, XmlBuilder segInfoElement)
		{
			if (!string.IsNullOrEmpty(segmentOrigin.OriginType))
			{
				segInfoElement.SetAttribute("origin", segmentOrigin.OriginType);
			}
			if (!string.IsNullOrEmpty(segmentOrigin.OriginSystem))
			{
				segInfoElement.SetAttribute("origin-system", segmentOrigin.OriginSystem);
			}
			if (segmentOrigin.MatchPercent != 0)
			{
				segInfoElement.SetAttribute("percent", segmentOrigin.MatchPercent.ToString("d", CultureInfo.InvariantCulture));
			}
			if (segmentOrigin.IsStructureContextMatch)
			{
				segInfoElement.SetAttribute("struct-match", XliffBoolValue(segmentOrigin.IsStructureContextMatch));
			}
			if (segmentOrigin.TextContextMatchLevel != 0)
			{
				segInfoElement.SetAttribute("text-match", segmentOrigin.TextContextMatchLevel.ToString());
			}
			if (segmentOrigin.IsRepeated)
			{
				segInfoElement.StartElement("sdl", "rep");
				segInfoElement.SetAttribute("id", segmentOrigin.RepetitionTableId.Id);
				segInfoElement.EndElement();
			}
			if (segmentOrigin.OriginBeforeAdaptation != null)
			{
				segInfoElement.StartElement("sdl", "prev-origin");
				SetSegmentOriginProperties(segmentOrigin.OriginBeforeAdaptation, segInfoElement);
				segInfoElement.EndElement();
			}
			foreach (KeyValuePair<string, string> metaDatum in segmentOrigin.MetaData)
			{
				GetOrCreateCustomElement(segInfoElement, "value", "key", metaDatum.Key).Add(new XmlBuilderText
				{
					Text = XmlBuilder.XmlEscape(metaDatum.Value)
				});
			}
		}

		private XmlElement GetOrCreateChildElement(XmlElement parentElement, string elementName)
		{
			XmlElement xmlElement = GetChildElement(parentElement, elementName);
			if (xmlElement == null)
			{
				xmlElement = CreateSdlPrefixElement(elementName);
				parentElement.AppendChild(xmlElement);
			}
			return xmlElement;
		}

		private IXmlBuilderElement GetOrCreateCustomElement(XmlBuilder parentElement, string elementName, string attributeName, string attributeValue)
		{
			IXmlBuilderElement xmlBuilderElement = null;
			IXmlBuilderElement xmlBuilderElement2 = null;
			XmlBuilder xmlBuilder = new XmlBuilder();
			xmlBuilder.AddNodesToBuilder(parentElement);
			xmlBuilderElement2 = xmlBuilder.GetCurrentElementFromRoot(string.Empty, elementName);
			if (xmlBuilderElement2 != null)
			{
				foreach (XmlBuilderAttribute item in xmlBuilderElement2.Attributes.Cast<XmlBuilderAttribute>())
				{
					if (item.Name == attributeName && item.Value == attributeValue)
					{
						xmlBuilderElement = xmlBuilderElement2;
					}
				}
			}
			if (xmlBuilderElement == null)
			{
				parentElement.StartElement("sdl", elementName);
				xmlBuilderElement = (parentElement.CurrentNode as IXmlBuilderElement);
				parentElement.AddAttribute(attributeName, attributeValue);
				parentElement.EndElement();
			}
			return xmlBuilderElement;
		}

		internal void SaveTagInfo(ITagPair tagPair)
		{
			TagId tagId = tagPair.StartTagProperties.TagId;
			IStartTagProperties startTagProperties = CurrentFileHeader.GetStartTagProperties(tagId);
			if (startTagProperties != null)
			{
				if (!CompareStartTagPropertiesWithoutAutoclonedMetadata(tagPair.StartTagProperties, startTagProperties))
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Internal error: Multiple tags with the same ID '{0}' have different start tag properties", tagId.Id));
				}
				IEndTagProperties endTagProperties = CurrentFileHeader.GetEndTagProperties(tagId);
				if (!tagPair.EndTagProperties.Equals(endTagProperties))
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Internal error: Multiple tags with the same ID '{0}' have different end tag properties", tagId.Id));
				}
				IList<ISubSegmentReference> subSegments = CurrentFileHeader.GetSubSegments(tagId);
				if (!SubSegmentsEqual(tagPair.SubSegments, subSegments))
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Internal error: Multiple tags with the same ID '{0}' have different sub-segment properties", tagId.Id));
				}
			}
			else
			{
				CurrentFileHeader.AddTagPair(tagPair);
			}
		}

		private static bool SubSegmentsEqual(IEnumerable<ISubSegmentReference> first, IEnumerable<ISubSegmentReference> second)
		{
			int num = first?.Count() ?? 0;
			int num2 = second?.Count() ?? 0;
			if (num != num2)
			{
				return false;
			}
			if (num == 0)
			{
				return true;
			}
			List<ISubSegmentReference> list = first.ToList();
			List<ISubSegmentReference> list2 = second.ToList();
			for (int i = 0; i < num; i++)
			{
				if (!list[i].Equals(list2[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal void SaveTagInfo(IPlaceholderTag placeholder)
		{
			TagId tagId = placeholder.Properties.TagId;
			IPlaceholderTagProperties placeholderTagProperties = CurrentFileHeader.GetPlaceholderTagProperties(tagId);
			if (placeholderTagProperties != null)
			{
				if (!placeholder.Properties.Equals(placeholderTagProperties))
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Internal error: Multiple placeholder tags with the same ID '{0}' have different tag properties", tagId.Id));
				}
				IList<ISubSegmentReference> subSegments = CurrentFileHeader.GetSubSegments(tagId);
				if (!SubSegmentsEqual(placeholder.SubSegments, subSegments))
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Internal error: Multiple tags with the same ID '{0}' have different sub-segment properties", tagId.Id));
				}
			}
			else
			{
				CurrentFileHeader.AddPlaceholderTag(placeholder);
			}
		}

		internal void SaveTagInfo(IStructureTag tag)
		{
			TagId tagId = tag.Properties.TagId;
			IStructureTagProperties structureTagProperties = CurrentFileHeader.GetStructureTagProperties(tagId);
			if (structureTagProperties != null)
			{
				if (!tag.Properties.Equals(structureTagProperties))
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Internal error: Multiple structure tags with the same ID '{0}' have different tag properties", tagId.Id));
				}
				IList<ISubSegmentReference> subSegments = CurrentFileHeader.GetSubSegments(tagId);
				if (!SubSegmentsEqual(tag.SubSegments, subSegments))
				{
					throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Internal error: Multiple tags with the same ID '{0}' have different sub-segment properties", tagId.Id));
				}
			}
			else
			{
				CurrentFileHeader.AddStructureTag(tag);
			}
		}

		private void ApplyStartTagProperties(XmlBuilder builder, IStartTagProperties tagProperties, IList<ISubSegmentReference> subSegments)
		{
			SetTagContent(builder, tagProperties.TagContent, subSegments);
			ApplyBasicTagProperties(tagProperties, builder);
			if (tagProperties.SegmentationHint != SegmentationHint.MayExclude)
			{
				builder.SetAttribute("seg-hint", tagProperties.SegmentationHint.ToString());
			}
			ApplyInlineTagProperties(tagProperties, builder);
		}

		private void ApplyEndTagProperties(XmlBuilder builder, IEndTagProperties tagProperties)
		{
			builder.AddText(tagProperties.TagContent);
			ApplyBasicTagProperties(tagProperties, builder);
			ApplyInlineTagProperties(tagProperties, builder);
		}

		private void SetTagContent(XmlBuilder builder, string tagContent, IList<ISubSegmentReference> subSegments)
		{
			int num = 0;
			if (subSegments != null)
			{
				foreach (ISubSegmentReference subSegment in subSegments)
				{
					if (subSegment.Properties.StartOffset < num)
					{
						throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, StringResources.SubSegmentPropertiesOverlapping, tagContent));
					}
					builder.AddText(tagContent.Substring(num, subSegment.Properties.StartOffset - num));
					builder.StartElement("sub");
					builder.AddText(tagContent.Substring(subSegment.Properties.StartOffset, subSegment.Properties.Length));
					builder.SetAttribute("xid", subSegment.ParagraphUnitId.Id);
					builder.EndElement();
					num = subSegment.Properties.StartOffset + subSegment.Properties.Length;
				}
			}
			builder.AddText(tagContent.Substring(num));
		}

		internal XmlElement CreateSdlPrefixElement(string elementName)
		{
			return _xmlDoc.CreateElement("sdl", elementName, "http://sdl.com/FileTypes/SdlXliff/1.0");
		}

		internal XmlElement CreateSdlElement(string elementName)
		{
			return _xmlDoc.CreateElement(null, elementName, "http://sdl.com/FileTypes/SdlXliff/1.0");
		}

		internal void BuildSdlPrefixElement(XmlBuilder builder, string elementName)
		{
			builder.StartElement("sdl", elementName, "http://sdl.com/FileTypes/SdlXliff/1.0");
		}

		internal void BuildSdlElement(XmlBuilder builder, string elementName)
		{
			builder.StartElement(string.Empty, elementName, "http://sdl.com/FileTypes/SdlXliff/1.0");
		}

		internal XmlAttribute CreateSdlPrefixAttribute(string attributeName)
		{
			return _xmlDoc.CreateAttribute("sdl", attributeName, "http://sdl.com/FileTypes/SdlXliff/1.0");
		}

		internal static string GetLockString(LockTypeFlags flags)
		{
			string text = "";
			if ((flags & LockTypeFlags.Structure) != 0)
			{
				if (text.Length > 0)
				{
					text += " ";
				}
				text += AttributeValues.Structure;
			}
			if ((flags & LockTypeFlags.Externalized) != 0)
			{
				if (text.Length > 0)
				{
					text += " ";
				}
				text += AttributeValues.Externalized;
			}
			if ((flags & LockTypeFlags.Manual) != 0)
			{
				if (text.Length > 0)
				{
					text += " ";
				}
				text += AttributeValues.Manual;
			}
			return text;
		}

		private static bool HasChildElement(XmlElement element, string childName)
		{
			foreach (XmlElement childNode in element.ChildNodes)
			{
				if (childNode.LocalName == childName)
				{
					return true;
				}
			}
			return false;
		}

		private static XmlElement GetChildElement(XmlElement element, string childName)
		{
			foreach (XmlElement childNode in element.ChildNodes)
			{
				if (childNode.LocalName == childName)
				{
					return childNode;
				}
			}
			return null;
		}

		internal void SaveSegmentInfo(ISegmentPairProperties segmentProperties)
		{
			if (!SegDefElementExists(segmentProperties.Id.Id))
			{
				if (_segDefsElement == null)
				{
					_segDefsElement = new XmlBuilder();
					_segDefsElement.StartElement("sdl", "seg-defs");
					_segDefElementIDs = new HashSet<string>();
				}
				_segDefsElement.StartElement("sdl", "seg");
				_segDefElementIDs.Add(segmentProperties.Id.Id);
				_segDefsElement.AddAttribute("id", segmentProperties.Id.Id);
				if (segmentProperties.IsLocked)
				{
					_segDefsElement.AddAttribute("locked", XliffBoolValue(segmentProperties.IsLocked));
				}
				if (segmentProperties.ConfirmationLevel != 0)
				{
					_segDefsElement.AddAttribute("conf", segmentProperties.ConfirmationLevel.ToString());
				}
				if (segmentProperties.TranslationOrigin != null)
				{
					SetSegmentOriginProperties(segmentProperties.TranslationOrigin, _segDefsElement);
				}
				_segDefsElement.EndElement();
			}
		}

		private bool CompareStartTagPropertiesWithoutAutoclonedMetadata(IStartTagProperties properties1, IStartTagProperties properties2)
		{
			List<KeyValuePair<string, string>> metaData = properties1.MetaData.Where((KeyValuePair<string, string> m) => m.Key != "SDL:AutoCloned").ToList();
			List<KeyValuePair<string, string>> metaData2 = properties2.MetaData.Where((KeyValuePair<string, string> m) => m.Key != "SDL:AutoCloned").ToList();
			if (!AreMetadataEqual(metaData, metaData2))
			{
				return false;
			}
			bool num = properties1.DisplayText != properties2.DisplayText || properties1.TagContent != properties2.TagContent;
			bool flag = properties1.IsSoftBreak != properties2.IsSoftBreak || properties1.IsWordStop != properties2.IsWordStop || properties1.CanHide != properties2.CanHide;
			if (num | flag)
			{
				return false;
			}
			if (!AreAbstractTagPropertiesEqual(properties1, properties2))
			{
				return false;
			}
			if (properties1.Formatting == null != (properties2.Formatting == null) || (properties1.Formatting != null && !properties1.Formatting.Equals(properties2.Formatting)) || properties1.SegmentationHint != properties2.SegmentationHint)
			{
				return false;
			}
			return true;
		}

		private bool AreMetadataEqual(List<KeyValuePair<string, string>> metaData1, List<KeyValuePair<string, string>> metaData2)
		{
			if (metaData1.Count != metaData2.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, string> pair in metaData1)
			{
				if (!metaData2.Contains(pair))
				{
					return false;
				}
				KeyValuePair<string, string>[] array = metaData2.Where((KeyValuePair<string, string> m) => m.Key == pair.Key).ToArray();
				if (array.Length == 0)
				{
					return false;
				}
				if (array[0].Value == null != (pair.Value == null))
				{
					return false;
				}
				if (array[0].Value != null && pair.Value != null && !array[0].Value.Equals(pair.Value))
				{
					return false;
				}
			}
			return true;
		}

		private bool AreAbstractTagPropertiesEqual(IAbstractTagProperties properties1, IAbstractTagProperties properties2)
		{
			if (properties1.HasLocalizableContent != properties2.HasLocalizableContent)
			{
				return false;
			}
			if (properties1.HasLocalizableContent)
			{
				ISubSegmentProperties[] array = properties1.LocalizableContent.ToArray();
				ISubSegmentProperties[] array2 = properties2.LocalizableContent.ToArray();
				if (array.Length != array2.Length)
				{
					return false;
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (!array[i].Equals(array2[i]))
					{
						return false;
					}
				}
			}
			if (properties1.TagId != properties2.TagId)
			{
				return false;
			}
			return true;
		}

		public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
		{
			_settings.PopulateFromSettingsBundle(settingsBundle, configurationId);
			_maxEmbeddableFileSize = _settings.GetMaximumFileEmbedSizeInBytes();
		}

		public void Dispose()
		{
			Dispose(isDisposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			_files = null;
		}

		protected string GetTempFileName()
		{
			string text = Path.Combine(Path.GetTempPath(), "SDLTempFileRegen");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return Path.Combine(text, Path.GetRandomFileName());
		}

		protected string ReadNextLine(TextReader r, out string terminator)
		{
			terminator = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			while (r.Peek() >= 0)
			{
				char c = (char)r.Read();
				switch (c)
				{
				case '\r':
				{
					terminator += c.ToString();
					int num = r.Peek();
					if (num == 10)
					{
						r.Read();
						terminator += ((char)num).ToString();
					}
					return stringBuilder.ToString();
				}
				case '\n':
					terminator += c.ToString();
					return stringBuilder.ToString();
				}
				stringBuilder.Append(c);
			}
			r.Read();
			if (stringBuilder.Length == 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}
	}
}
