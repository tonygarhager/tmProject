using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.Preview
{
	public sealed class XliffPreviewXmlWriter : IBilingualDocumentFileWriter, IBilingualDocumentWriter, IBilingualWriter, IBilingualContentHandler, IDisposable, IBilingualDocumentOutputPropertiesAware, IMarkupDataVisitor
	{
		private IDocumentProperties _DocumentInfo;

		private IFileProperties _FileInfo;

		private XmlWriter _XmlWriter;

		private StringWriter _StringWriter;

		private IBilingualDocumentOutputProperties _outputProperties;

		private string _TransformPath;

		private string _CssPath;

		private string _JSPath;

		private bool _IsExternal = true;

		private int _commentCount;

		private const string COMMENT_ID_NAME = "comment";

		private const string DOCUMENT_ELEMENT = "document";

		private const string FILE_ELEMENT = "file";

		private const string SOURCE_ELEMENT = "source";

		private const string TARGET_ELEMENT = "target";

		private const string PARAGRAPH_ELEMENT = "paragraph";

		private const string SEGMENT_ELEMENT = "segment";

		private const string FORMATTING_ELEMENT = "formatting";

		private const string LOCKED_ELEMENT = "locked";

		private const string COMMENT_ELEMENT = "comment";

		private const string TAGPAIR_ELEMENT = "tagpair";

		private const string PLACEHOLDER_ELEMENT = "placeholder";

		private const string EXTERNAL_ELEMENT = "external";

		private const string CONTEXT_ELEMENT = "context";

		private const string REVISION_ELEMENT = "revision";

		private const string TEXT_ELEMENT = "text";

		private const string NAME_ATT = "name";

		private const string SEVERITY_ATT = "severity";

		private const string TEXT_ATT = "text";

		private const string TRANS_ORIGIN_ATT = "translationOrigin";

		private const string CAN_HIDE_ATT = "canHide";

		private const string RIGHT_ALIGN_ATT = "rightAlign";

		private const string TEXT_EQUIV_ATT = "textEquiv";

		private const string COMMENT_ID_ATT = "commentId";

		private const string CONTEXT_COLOR_ATT = "contextColor";

		private const string DELETED_ATT = "deleted";

		private const string CONTEXT_MATCH = "context-match";

		public string Transform
		{
			set
			{
				_TransformPath = value;
			}
		}

		public string StyleSheet
		{
			set
			{
				_CssPath = value;
			}
		}

		public string JavaScript
		{
			set
			{
				_JSPath = value;
			}
		}

		public XliffPreviewXmlWriter()
		{
			InitXmlPreviewWriter();
		}

		private void InitXmlPreviewWriter()
		{
			if (_XmlWriter == null)
			{
				_StringWriter = new StringWriter();
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.CheckCharacters = false;
				xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
				_XmlWriter = XmlWriter.Create(_StringWriter, xmlWriterSettings);
				_XmlWriter.WriteStartDocument();
				_XmlWriter.WriteStartElement("document");
			}
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			_DocumentInfo = documentInfo;
		}

		public void Complete()
		{
			_XmlWriter.WriteEndElement();
			_XmlWriter.WriteEndDocument();
			_XmlWriter.Flush();
			XPathDocument input = new XPathDocument(new StringReader(_StringWriter.ToString()));
			TextReader textReader = CreateCssReader();
			TextReader textReader2 = CreateJSReader();
			XsltArgumentList xsltArgumentList = new XsltArgumentList();
			xsltArgumentList.AddParam("css", "", textReader.ReadToEnd());
			xsltArgumentList.AddParam("js", "", textReader2.ReadToEnd());
			XliffPreviewResourceManager extension = new XliffPreviewResourceManager();
			xsltArgumentList.AddExtensionObject("urn:resources", extension);
			XslCompiledTransform xslCompiledTransform = CreateTransform();
			StringWriter stringWriter = new StringWriter();
			XmlWriterSettings xmlWriterSettings = xslCompiledTransform.OutputSettings.Clone();
			xmlWriterSettings.CheckCharacters = false;
			XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings);
			xslCompiledTransform.Transform(input, xsltArgumentList, xmlWriter);
			xmlWriter.Flush();
			SaveToFile(stringWriter);
			xmlWriter.Close();
			Close();
		}

		private void SaveToFile(StringWriter writer)
		{
			try
			{
				StreamWriter streamWriter = new StreamWriter(_outputProperties.OutputFilePath, append: false, Encoding.UTF8);
				streamWriter.Write(writer.ToString());
				streamWriter.Close();
			}
			catch (Exception innerException)
			{
				throw new Exception(StringResources.Preview_FileException, innerException);
			}
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_FileInfo = fileInfo;
			InitXmlPreviewWriter();
			_XmlWriter.WriteStartElement("file");
		}

		public void FileComplete()
		{
			_XmlWriter.WriteEndElement();
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (!paragraphUnit.SegmentPairs.GetEnumerator().MoveNext())
			{
				return;
			}
			IFormattingGroup formatting = null;
			if (paragraphUnit.Properties.Contexts != null)
			{
				formatting = paragraphUnit.Properties.Contexts.EffectiveDefaultFormatting;
			}
			_XmlWriter.WriteStartElement("paragraph");
			_XmlWriter.WriteStartElement("source");
			ProcessParagraph(paragraphUnit.Source, formatting);
			_XmlWriter.WriteEndElement();
			_XmlWriter.WriteStartElement("target");
			ProcessParagraph(paragraphUnit.Target, formatting);
			_XmlWriter.WriteEndElement();
			if (!paragraphUnit.IsStructure && paragraphUnit.Properties.Contexts != null && paragraphUnit.Properties.Contexts.Contexts != null && paragraphUnit.Properties.Contexts.Contexts.Count > 0)
			{
				IContextInfo contextInfo = paragraphUnit.Properties.Contexts.Contexts[0];
				_XmlWriter.WriteStartElement("context");
				if (contextInfo.DisplayColor != Color.Empty)
				{
					_XmlWriter.WriteAttributeString("contextColor", ColorTranslator.ToHtml(contextInfo.DisplayColor));
				}
				_XmlWriter.WriteString(contextInfo.DisplayCode);
				_XmlWriter.WriteEndElement();
			}
			_XmlWriter.WriteEndElement();
		}

		private void ProcessParagraph(IParagraph paragraph, IFormattingGroup formatting)
		{
			Language language = new Language();
			language = ((!paragraph.IsSource) ? _DocumentInfo.TargetLanguage : _DocumentInfo.SourceLanguage);
			if (language != null && language.CultureInfo != null)
			{
				_XmlWriter.WriteAttributeString("rightAlign", language.CultureInfo.TextInfo.IsRightToLeft.ToString());
			}
			if (formatting != null)
			{
				XliffXmlFormattingVisitor visitor = new XliffXmlFormattingVisitor(_XmlWriter);
				_XmlWriter.WriteStartElement("formatting");
				foreach (IFormattingItem value in formatting.Values)
				{
					value.AcceptVisitor(visitor);
				}
			}
			foreach (IAbstractMarkupData item in paragraph)
			{
				if (!(item is IStructureTag))
				{
					item.AcceptVisitor(this);
				}
			}
			if (formatting != null)
			{
				_XmlWriter.WriteEndElement();
			}
		}

		private XslCompiledTransform CreateTransform()
		{
			XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
			if (_TransformPath == null)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Sdl.FileTypeSupport.Bilingual.SdlXliff.Preview.BilingualTransform.xslt");
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(manifestResourceStream);
				xslCompiledTransform.Load(xmlDocument);
			}
			else
			{
				xslCompiledTransform.Load(_TransformPath);
			}
			return xslCompiledTransform;
		}

		private TextReader CreateCssReader()
		{
			if (_CssPath == null)
			{
				return GetResourceTextReader("Sdl.FileTypeSupport.Bilingual.SdlXliff.Preview.PreviewStylesheet.css");
			}
			return new StreamReader(_CssPath);
		}

		private TextReader CreateJSReader()
		{
			if (_JSPath == null)
			{
				return GetResourceTextReader("Sdl.FileTypeSupport.Bilingual.SdlXliff.Preview.XliffPreviewHelper.js");
			}
			return new StreamReader(_JSPath);
		}

		private static TextReader GetResourceTextReader(string assemblyPath)
		{
			return new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyPath));
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			_XmlWriter.WriteStartElement("tagpair");
			_XmlWriter.WriteAttributeString("name", tagPair.StartTagProperties.DisplayText);
			_XmlWriter.WriteAttributeString("canHide", tagPair.StartTagProperties.CanHide.ToString());
			if (tagPair.StartTagProperties.Formatting != null)
			{
				_XmlWriter.WriteStartElement("formatting");
				XliffXmlFormattingVisitor visitor = new XliffXmlFormattingVisitor(_XmlWriter);
				foreach (IFormattingItem value in tagPair.StartTagProperties.Formatting.Values)
				{
					value.AcceptVisitor(visitor);
				}
			}
			foreach (IAbstractMarkupData item in tagPair)
			{
				item.AcceptVisitor(this);
			}
			if (tagPair.StartTagProperties.Formatting != null)
			{
				_XmlWriter.WriteEndElement();
			}
			_XmlWriter.WriteEndElement();
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (_IsExternal)
			{
				_XmlWriter.WriteStartElement("external");
			}
			_XmlWriter.WriteStartElement("placeholder");
			_XmlWriter.WriteAttributeString("name", tag.Properties.DisplayText);
			if (tag.Properties.HasTextEquivalent)
			{
				_XmlWriter.WriteAttributeString("textEquiv", tag.Properties.TextEquivalent);
			}
			_XmlWriter.WriteEndElement();
			if (_IsExternal)
			{
				_XmlWriter.WriteEndElement();
			}
		}

		public void VisitText(IText text)
		{
			if (_IsExternal)
			{
				_XmlWriter.WriteStartElement("external");
			}
			else
			{
				_XmlWriter.WriteStartElement("text");
			}
			_XmlWriter.WriteString(CleanXmlString(text.Properties.Text));
			_XmlWriter.WriteEndElement();
		}

		public void VisitSegment(ISegment segment)
		{
			_IsExternal = false;
			_XmlWriter.WriteStartElement("segment");
			ITranslationOrigin translationOrigin = segment.Properties.TranslationOrigin;
			if (translationOrigin != null)
			{
				string text = translationOrigin.OriginType;
				if (text == "tm" && translationOrigin.MatchPercent >= 100)
				{
					text = "context-match";
				}
				_XmlWriter.WriteAttributeString("translationOrigin", text);
			}
			if (segment.Properties.IsLocked)
			{
				_XmlWriter.WriteStartElement("locked");
			}
			foreach (IAbstractMarkupData item in segment)
			{
				item.AcceptVisitor(this);
			}
			if (segment.Properties.IsLocked)
			{
				_XmlWriter.WriteEndElement();
			}
			_XmlWriter.WriteEndElement();
			_IsExternal = true;
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Severity severity = Severity.Undefined;
			for (int i = 0; i < commentMarker.Comments.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append("; ");
				}
				IComment item = commentMarker.Comments.GetItem(i);
				stringBuilder.Append("[");
				stringBuilder.Append(item.Author);
				stringBuilder.Append(", ");
				stringBuilder.Append(item.Date.ToLongTimeString());
				stringBuilder.Append("]: ");
				stringBuilder.Append(item.Text);
				switch (item.Severity)
				{
				case Severity.High:
					severity = Severity.High;
					break;
				case Severity.Medium:
					if (severity != Severity.High)
					{
						severity = Severity.Medium;
					}
					break;
				default:
					severity = Severity.Low;
					break;
				}
			}
			_XmlWriter.WriteStartElement("comment");
			_XmlWriter.WriteAttributeString("severity", severity.ToString());
			_XmlWriter.WriteAttributeString("text", stringBuilder.ToString());
			_XmlWriter.WriteAttributeString("commentId", "comment" + _commentCount.ToString());
			foreach (IAbstractMarkupData item2 in commentMarker)
			{
				item2.AcceptVisitor(this);
			}
			_XmlWriter.WriteEndElement();
			_commentCount++;
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			foreach (IAbstractMarkupData item in marker)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			_XmlWriter.WriteStartElement("locked");
			foreach (IAbstractMarkupData item in lockedContent.Content)
			{
				item.AcceptVisitor(this);
			}
			_XmlWriter.WriteEndElement();
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			_XmlWriter.WriteStartElement("revision");
			if (revisionMarker.Properties.RevisionType == RevisionType.Delete)
			{
				_XmlWriter.WriteAttributeString("deleted", "true");
			}
			foreach (IAbstractMarkupData item in revisionMarker)
			{
				item.AcceptVisitor(this);
			}
			_XmlWriter.WriteEndElement();
		}

		public void GetProposedFileInfo(IDocumentProperties documentInfo, IOutputFileInfo proposedFileInfo)
		{
			if (proposedFileInfo != null)
			{
				proposedFileInfo.Filename = "BilingualDocumentHtmlPreview.html";
				proposedFileInfo.FileDialogWildcardExpression = "*.html;*.htm";
				proposedFileInfo.FileTypeName = StringResources.HtmlFileTypeName;
			}
		}

		public void SetOutputProperties(IBilingualDocumentOutputProperties outputProperties)
		{
			_outputProperties = outputProperties;
		}

		private string CleanXmlString(string xmlString)
		{
			if (string.IsNullOrEmpty(xmlString))
			{
				return xmlString;
			}
			StringBuilder stringBuilder = new StringBuilder(xmlString.Length);
			foreach (char c in xmlString)
			{
				if (IsValidXml(c))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private bool IsValidXml(char character)
		{
			if (character != '\t' && character != '\n' && character != '\r' && (character < ' ' || character > '\ud7ff'))
			{
				if (character >= '\ue000')
				{
					return character <= '\ufffd';
				}
				return false;
			}
			return true;
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
				Close();
			}
		}

		private void Close()
		{
			if (_XmlWriter != null)
			{
				_XmlWriter.Close();
				_XmlWriter = null;
			}
		}

		~XliffPreviewXmlWriter()
		{
			Dispose(disposing: false);
		}
	}
}
