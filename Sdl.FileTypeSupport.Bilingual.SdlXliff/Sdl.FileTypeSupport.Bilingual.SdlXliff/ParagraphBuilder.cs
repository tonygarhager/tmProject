using Oasis.Xliff12;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class ParagraphBuilder : AbstractParagraphUnitBuilder
	{
		private IAbstractMarkupDataContainer _Content;

		private Stack<IAbstractMarkupDataContainer> _OpenContainers = new Stack<IAbstractMarkupDataContainer>();

		private FileSkeleton _FileSkeleton;

		private DocSkeleton _DocSkeleton;

		private Dictionary<SegmentId, ISegmentPairProperties> _KnownSegmentIds = new Dictionary<SegmentId, ISegmentPairProperties>();

		private Dictionary<string, ILockedContent> _LockedContent;

		private bool _IsStructure;

		private transunit _CurrentTransunit;

		public FileSkeleton FileSkeleton
		{
			get
			{
				return _FileSkeleton;
			}
			set
			{
				_FileSkeleton = value;
			}
		}

		public DocSkeleton DocSkeleton
		{
			get
			{
				return _DocSkeleton;
			}
			set
			{
				_DocSkeleton = value;
			}
		}

		public IAbstractMarkupDataContainer Content
		{
			get
			{
				return _Content;
			}
			set
			{
				_Content = value;
			}
		}

		public Dictionary<SegmentId, ISegmentPairProperties> KnownSegmentIds
		{
			get
			{
				return _KnownSegmentIds;
			}
			set
			{
				_KnownSegmentIds = value;
			}
		}

		public Dictionary<string, ILockedContent> LockedContent
		{
			get
			{
				return _LockedContent;
			}
			set
			{
				_LockedContent = value;
			}
		}

		public bool IsStructure
		{
			get
			{
				return _IsStructure;
			}
			set
			{
				_IsStructure = value;
			}
		}

		public transunit CurrentTransunit
		{
			set
			{
				_CurrentTransunit = value;
			}
		}

		public ParagraphBuilder()
		{
		}

		public ParagraphBuilder(IDocumentItemFactory itemFactory, IParagraph paragraph)
			: base(itemFactory)
		{
			_Content = paragraph;
		}

		protected override void CheckOutput()
		{
			base.CheckOutput();
			if (_Content == null)
			{
				throw new FileTypeSupportException("Internal error: Output Paragraph not set.");
			}
			if (_FileSkeleton == null)
			{
				throw new XliffParseException("Internal error: Header not set.");
			}
		}

		public override void Text(string text)
		{
			base.Text(text);
			ITextProperties textInfo = base.PropertiesFactory.CreateTextProperties(text);
			AddParagraphContent(base.ItemFactory.CreateText(textInfo));
		}

		public override void PlaceholderTag(x x)
		{
			if (string.IsNullOrEmpty(x.id))
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_MissingTagId, x));
			}
			base.PlaceholderTag(x);
			if (_IsStructure)
			{
				AddPlaceholderTagToStructureParagraph(x);
				return;
			}
			if (x.id.StartsWith("locked"))
			{
				AddLockedPlaceholderToParagraph(x);
				return;
			}
			TagId id = new TagId(x.id);
			IPlaceholderTagProperties placeholderTagProperties = _FileSkeleton.GetPlaceholderTagProperties(id);
			if (placeholderTagProperties == null)
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_MissingTagInformation, id.Id));
			}
			IPlaceholderTag placeholderTag = base.ItemFactory.CreatePlaceholderTag(placeholderTagProperties);
			IList<ISubSegmentReference> subSegments = _FileSkeleton.GetSubSegments(id);
			if (subSegments != null)
			{
				placeholderTag.AddSubSegmentReferences(subSegments);
			}
			if (x.AnyAttr != null)
			{
				ProcessCustomPlaceholderAttributes(x, placeholderTag);
			}
			AddParagraphContent(placeholderTag);
		}

		private void AddPlaceholderTagToStructureParagraph(x x)
		{
			TagId id = new TagId(x.id);
			IStructureTagProperties structureTagProperties = _FileSkeleton.GetStructureTagProperties(id);
			if (structureTagProperties == null)
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_MissingTagInformation, id.Id));
			}
			IStructureTag structureTag = base.ItemFactory.CreateStructureTag(structureTagProperties);
			IList<ISubSegmentReference> subSegments = _FileSkeleton.GetSubSegments(id);
			if (subSegments != null)
			{
				structureTag.AddSubSegmentReferences(subSegments);
			}
			AddParagraphContent(structureTag);
		}

		private void AddLockedPlaceholderToParagraph(x x)
		{
			if (LockedContent == null)
			{
				throw new XliffParseException("Internal error: Locked content dictionary not set!");
			}
			if (LockedContent.Count == 0)
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_LockedContentNotFound, x));
			}
			if (!LockedContent.TryGetValue(x.xid, out ILockedContent value))
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_LockedContentNotFound, x));
			}
			LockedContent.Remove(x.xid);
			AddParagraphContent(value);
		}

		private void ProcessCustomPlaceholderAttributes(x x, IPlaceholderTag tag)
		{
			XmlAttribute[] anyAttr = x.AnyAttr;
			foreach (XmlAttribute xmlAttribute in anyAttr)
			{
				if (xmlAttribute.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0" && xmlAttribute.LocalName == "name")
				{
					tag.Properties.DisplayText = xmlAttribute.Value;
				}
			}
		}

		public override void StartPairedTag(g g)
		{
			if (_IsStructure)
			{
				base.StartPairedTag(g);
				throw new XliffParseException(StringResources.CorruptFile_TagPairInStructureParagraph);
			}
			if (string.IsNullOrEmpty(g.id))
			{
				throw new XliffParseException(string.Format(StringResources.CorruptFile_MissingTagId, g));
			}
			base.StartPairedTag(g);
			TagId id = new TagId(g.id);
			IStartTagProperties startTagProperties = _FileSkeleton.GetStartTagProperties(id);
			if (startTagProperties == null)
			{
				throw new XliffParseException(string.Format(CultureInfo.CurrentCulture, StringResources.CorruptFile_MissingTagInformation, id.Id));
			}
			IEndTagProperties endTagProperties = _FileSkeleton.GetEndTagProperties(id);
			ITagPair tagPair = base.ItemFactory.CreateTagPair(startTagProperties, endTagProperties);
			IList<ISubSegmentReference> subSegments = _FileSkeleton.GetSubSegments(id);
			if (subSegments != null)
			{
				tagPair.AddSubSegmentReferences(subSegments);
			}
			if (g.AnyAttr != null)
			{
				ProcessCustomTagPairAttributes(g, tagPair);
			}
			AddParagraphContent(tagPair);
			_OpenContainers.Push(tagPair);
		}

		private void ProcessCustomTagPairAttributes(g g, ITagPair tag)
		{
			XmlAttribute[] anyAttr = g.AnyAttr;
			foreach (XmlAttribute xmlAttribute in anyAttr)
			{
				if (xmlAttribute.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0")
				{
					switch (xmlAttribute.LocalName)
					{
					case "start":
						tag.IsStartTagGhost = (xmlAttribute.Value == "false");
						continue;
					case "end":
						tag.IsEndTagGhost = (xmlAttribute.Value == "false");
						continue;
					case "start-revid":
						tag.StartTagRevisionProperties = DocSkeleton.GetRevision(xmlAttribute.Value);
						continue;
					case "end-revid":
						tag.EndTagRevisionProperties = DocSkeleton.GetRevision(xmlAttribute.Value);
						continue;
					}
					if (base.MessageReporter != null)
					{
						ILocationMarker item = base.ItemFactory.CreateLocationMarker();
						tag.Add(item);
						base.MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, string.Format(StringResources.UnrecognizedTagPairAttribute, xmlAttribute.OuterXml), new TextLocation(item), null);
					}
				}
				else if (base.MessageReporter != null)
				{
					ILocationMarker item2 = base.ItemFactory.CreateLocationMarker();
					tag.Add(item2);
					base.MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, string.Format(StringResources.UnrecognizedTagPairAttribute, xmlAttribute.OuterXml), new TextLocation(item2), null);
				}
			}
		}

		public override void EndPairedTag(g g)
		{
			base.EndPairedTag(g);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_TagPairInStructureParagraph);
			}
			if (_OpenContainers.Count == 0)
			{
				throw new FileTypeSupportException("Internal error: End tag without start!");
			}
			_OpenContainers.Pop();
		}

		public override void SegmentStart(mrk mrk)
		{
			base.SegmentStart(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_SegmentInsideStructureParagraphUnit);
			}
			SegmentId segmentId = new SegmentId(mrk.mid);
			if (!_KnownSegmentIds.TryGetValue(segmentId, out ISegmentPairProperties value))
			{
				value = base.ItemFactory.CreateSegmentPairProperties();
				value.Id = segmentId;
				_KnownSegmentIds[segmentId] = value;
			}
			ISegment segment = base.ItemFactory.CreateSegment(value);
			if (_CurrentTransunit.Any != null)
			{
				XmlElement[] any = _CurrentTransunit.Any;
				foreach (XmlElement xmlElement in any)
				{
					if (xmlElement.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0" && xmlElement.LocalName == "seg-defs")
					{
						ProcessSegmentDefinitionsElement(xmlElement, mrk, segment);
					}
				}
			}
			AddParagraphContent(segment);
			_OpenContainers.Push(segment);
		}

		private void ProcessSegmentDefinitionsElement(XmlElement element, mrk mrk, ISegment segment)
		{
			XmlElement segDefElementById = GetSegDefElementById(element.ChildNodes, mrk.mid);
			if (segDefElementById != null)
			{
				if (bool.TryParse(segDefElementById.GetAttribute("locked"), out bool result))
				{
					segment.Properties.IsLocked = result;
				}
				else
				{
					segment.Properties.IsLocked = false;
				}
				segment.Properties.ConfirmationLevel = ConfirmationLevel.Unspecified;
				string attribute = segDefElementById.GetAttribute("conf");
				if (!string.IsNullOrEmpty(attribute))
				{
					try
					{
						segment.Properties.ConfirmationLevel = (ConfirmationLevel)Enum.Parse(typeof(ConfirmationLevel), attribute);
					}
					catch (Exception)
					{
						ILocationMarker item = base.ItemFactory.CreateLocationMarker();
						segment.Add(item);
						base.MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Error, string.Format(StringResources.UnrecognizedConfirmationLevel, attribute), new TextLocation(item), null);
					}
				}
				ITranslationOrigin translationOrigin = base.ItemFactory.CreateTranslationOrigin();
				if (ParseTranslationOriginProperties(translationOrigin, segDefElementById))
				{
					segment.Properties.TranslationOrigin = translationOrigin;
				}
			}
		}

		private bool ParseTranslationOriginProperties(ITranslationOrigin origin, XmlElement segElement)
		{
			bool result = false;
			if (segElement.HasAttribute("origin"))
			{
				origin.OriginType = segElement.GetAttribute("origin");
				result = true;
			}
			if (segElement.HasAttribute("origin-system"))
			{
				origin.OriginSystem = segElement.GetAttribute("origin-system");
				result = true;
			}
			if (segElement.HasAttribute("percent"))
			{
				origin.MatchPercent = byte.Parse(segElement.GetAttribute("percent"));
				result = true;
			}
			if (segElement.HasAttribute("struct-match"))
			{
				origin.IsStructureContextMatch = bool.Parse(segElement.GetAttribute("struct-match"));
				result = true;
			}
			if (segElement.HasAttribute("text-match"))
			{
				origin.TextContextMatchLevel = (TextContextMatchLevel)Enum.Parse(typeof(TextContextMatchLevel), segElement.GetAttribute("text-match"));
				result = true;
			}
			foreach (XmlElement childNode in segElement.ChildNodes)
			{
				switch (childNode.LocalName)
				{
				case "rep":
				{
					RepetitionId repetitionId2 = origin.RepetitionTableId = new RepetitionId(childNode.GetAttribute("id"));
					result = true;
					break;
				}
				case "prev-origin":
				{
					ITranslationOrigin translationOrigin = base.ItemFactory.CreateTranslationOrigin();
					ParseTranslationOriginProperties(translationOrigin, childNode);
					origin.OriginBeforeAdaptation = translationOrigin;
					result = true;
					break;
				}
				case "value":
				{
					string attribute = childNode.GetAttribute("key");
					if (string.IsNullOrEmpty(attribute))
					{
						string message = string.Format(StringResources.CorruptFile_ValueWithoutKey, childNode.OuterXml.ToString());
						base.MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Error, message, childNode.ToString());
					}
					else
					{
						result = true;
						origin.SetMetaData(attribute, childNode.InnerText);
					}
					break;
				}
				}
			}
			return result;
		}

		public override void SegmentEnd(mrk mrk)
		{
			base.SegmentEnd(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_SegmentInsideStructureParagraphUnit);
			}
			if (_OpenContainers.Count == 0)
			{
				throw new FileTypeSupportException("Internal error: Segment end without start!");
			}
			_OpenContainers.Pop();
		}

		public override void LocationMarker(mrk mrk)
		{
			base.LocationMarker(mrk);
			ILocationMarker locationMarker = base.ItemFactory.CreateLocationMarker();
			locationMarker.MarkerId = new LocationMarkerId(mrk.mid);
			AddParagraphContent(locationMarker);
		}

		public override void RevisionMarkerStart(mrk mrk)
		{
			base.RevisionMarkerStart(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_RevisionMarkerInStructureParagraph);
			}
			string customAttribute = GetCustomAttribute(mrk, "revid");
			IRevisionProperties revision = _DocSkeleton.GetRevision(customAttribute);
			IRevisionMarker item = (revision.RevisionType != RevisionType.FeedbackComment && revision.RevisionType != RevisionType.FeedbackAdded && revision.RevisionType != RevisionType.FeedbackDeleted) ? base.ItemFactory.CreateRevision(revision) : base.ItemFactory.CreateFeedback(revision);
			AddParagraphContent(item);
			_OpenContainers.Push(item);
		}

		public override void RevisionMarkerEnd(mrk mrk)
		{
			base.RevisionMarkerEnd(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_RevisionMarkerInStructureParagraph);
			}
			if (_OpenContainers.Count == 0)
			{
				throw new FileTypeSupportException("Interal error: Marker end without start!");
			}
			_OpenContainers.Pop();
		}

		public override void CommentsMarkerStart(mrk mrk)
		{
			base.CommentsMarkerStart(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_CommentMarkerInStructureParagraph);
			}
			string customAttribute = GetCustomAttribute(mrk, "cid");
			ICommentMarker item = base.ItemFactory.CreateCommentMarker(_DocSkeleton.GetComments(customAttribute));
			AddParagraphContent(item);
			_OpenContainers.Push(item);
		}

		private static string GetCustomAttribute(mrk mrk, string attributeName)
		{
			if (mrk.AnyAttr != null)
			{
				XmlAttribute[] anyAttr = mrk.AnyAttr;
				foreach (XmlAttribute xmlAttribute in anyAttr)
				{
					if (xmlAttribute.NamespaceURI == "http://sdl.com/FileTypes/SdlXliff/1.0" && xmlAttribute.LocalName == attributeName)
					{
						return xmlAttribute.Value;
					}
				}
			}
			return null;
		}

		public override void CommentsMarkerEnd(mrk mrk)
		{
			base.CommentsMarkerEnd(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_CommentMarkerInStructureParagraph);
			}
			if (_OpenContainers.Count == 0)
			{
				throw new FileTypeSupportException("Interal error: Marker end without start!");
			}
			_OpenContainers.Pop();
		}

		public override void MarkStart(mrk mrk)
		{
			base.MarkStart(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_OtherMarkerInStructureParagraph);
			}
			IOtherMarker otherMarker = base.ItemFactory.CreateOtherMarker();
			otherMarker.Id = mrk.mid;
			AddParagraphContent(otherMarker);
			_OpenContainers.Push(otherMarker);
			otherMarker.MarkerType = mrk.mtype;
			if (otherMarker.MarkerType.StartsWith("x-"))
			{
				otherMarker.MarkerType = otherMarker.MarkerType.Remove(0, 2);
				return;
			}
			string message = string.Format(StringResources.CorruptFile_OtherMarkerNoXPrefix, mrk.mtype);
			TextLocation textLocation = new TextLocation(otherMarker);
			Location obj = (Location)textLocation.Location.Clone();
			int num = ++obj.BottomLevel.Index;
			TextLocation uptoLocation = new TextLocation(obj, 0);
			base.MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, message, textLocation, uptoLocation);
		}

		public override void MarkEnd(mrk mrk)
		{
			base.MarkEnd(mrk);
			if (_IsStructure)
			{
				throw new XliffParseException(StringResources.CorruptFile_OtherMarkerInStructureParagraph);
			}
			if (_OpenContainers.Count == 0)
			{
				throw new FileTypeSupportException("Internal error: Marker end without start!");
			}
			_OpenContainers.Pop();
		}

		public override void UnknownContent(object element)
		{
			base.UnknownContent(element);
			string message = string.Format(StringResources.UnrecognizedElement, element.ToString());
			base.MessageReporter.ReportMessage(this, StringResources.XliffFilterName, ErrorLevel.Warning, message, null);
		}

		protected virtual void AddParagraphContent(IAbstractMarkupData item)
		{
			if (_OpenContainers.Count > 0)
			{
				_OpenContainers.Peek().Add(item);
			}
			else
			{
				_Content.Add(item);
			}
		}

		private XmlElement GetSegDefElementById(XmlNodeList nodeList, string id)
		{
			foreach (XmlNode node in nodeList)
			{
				XmlElement xmlElement = node as XmlElement;
				if (xmlElement != null && xmlElement.GetAttribute("id") == id)
				{
					return xmlElement;
				}
			}
			return null;
		}
	}
}
