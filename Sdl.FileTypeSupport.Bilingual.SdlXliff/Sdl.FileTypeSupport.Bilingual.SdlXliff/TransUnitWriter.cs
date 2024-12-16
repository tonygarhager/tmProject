using Sdl.FileTypeSupport.Bilingual.SdlXliff.XmlNodeBuilder;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	internal class TransUnitWriter : IMarkupDataVisitor
	{
		private XliffWriter _writer;

		private List<object> _content = new List<object>();

		private bool _segmentsAsMrk = true;

		private XmlBuilder _standardTuBuilder;

		private List<XmlBuilder> _lockedContentTuBuilderList = new List<XmlBuilder>();

		private Stack<XmlBuilder> _builderStack = new Stack<XmlBuilder>();

		private int _writerLevel;

		public List<object> Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}

		protected XmlBuilder CurrentLockedContentBuilder
		{
			get
			{
				if (_writerLevel > 0 && _lockedContentTuBuilderList.Count > 0)
				{
					return _lockedContentTuBuilderList[_writerLevel - 1];
				}
				return null;
			}
		}

		public List<XmlBuilder> LockedContentBuilders => _lockedContentTuBuilderList;

		public bool SegmentsAsMrk
		{
			get
			{
				return _segmentsAsMrk;
			}
			set
			{
				_segmentsAsMrk = value;
			}
		}

		public TransUnitWriter(XliffWriter writer, XmlBuilder tuBuilder)
		{
			_writer = writer;
			_standardTuBuilder = tuBuilder;
		}

		public string WriteTransUnitContentForChildren(IAbstractMarkupDataContainer container, bool segmentsAsMrk)
		{
			if (container == null || container.Count == 0)
			{
				return null;
			}
			SegmentsAsMrk = segmentsAsMrk;
			VisitChildren(container);
			return _standardTuBuilder.BuildXmlString(TreeGeneration.FullTree);
		}

		public void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				if (item is IStructureTag)
				{
					VisitStructureTag(item as IStructureTag);
				}
				else
				{
					item.AcceptVisitor(this);
				}
			}
		}

		private static string GetXidAttributeValue(IEnumerable<ISubSegmentReference> subsegments)
		{
			if (subsegments == null || subsegments.Count() == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ISubSegmentReference subsegment in subsegments)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append(subsegment.ParagraphUnitId.Id);
			}
			return stringBuilder.ToString();
		}

		public void VisitStructureTag(IStructureTag tag)
		{
			_standardTuBuilder.StartElement("x");
			_standardTuBuilder.AddAttribute("id", tag.Properties.TagId.Id);
			string xidAttributeValue = GetXidAttributeValue(tag.SubSegments);
			if (!string.IsNullOrEmpty(xidAttributeValue))
			{
				_standardTuBuilder.AddAttribute("xid", xidAttributeValue);
			}
			_standardTuBuilder.EndElement();
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			_standardTuBuilder.StartElement("mrk");
			_standardTuBuilder.AddAttribute("mtype", "x-sdl-comment");
			_standardTuBuilder.AddAttribute("sdl:cid", _writer.DocSkeleton.StoreComments(commentMarker.Comments));
			if (commentMarker.Count > 0)
			{
				WriteTransUnitContentForChildren(commentMarker, _segmentsAsMrk);
			}
			_standardTuBuilder.EndElement();
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			_standardTuBuilder.StartElement("mrk");
			_standardTuBuilder.AddAttribute("mtype", "x-sdl-location");
			if (!string.IsNullOrEmpty(location.MarkerId.Id))
			{
				_standardTuBuilder.AddAttribute("mid", location.MarkerId.Id);
			}
			_standardTuBuilder.EndElement();
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			_writerLevel++;
			if (_lockedContentTuBuilderList.Count < _writerLevel)
			{
				_lockedContentTuBuilderList.Add(new XmlBuilder());
			}
			_standardTuBuilder.StartElement("x");
			string value = "locked" + _writer.NextLockedContentId.ToString();
			int num = ++_writer.NextLockedContentId;
			_standardTuBuilder.AddAttribute("id", value);
			CurrentLockedContentBuilder.StartElement("trans-unit");
			string value2 = "lockTU_" + Guid.NewGuid().ToString();
			CurrentLockedContentBuilder.AddAttribute("id", value2);
			CurrentLockedContentBuilder.AddAttribute("translate", "no");
			_standardTuBuilder.AddAttribute("xid", value2);
			_standardTuBuilder.EndElement();
			CurrentLockedContentBuilder.AddAttribute("sdl:locktype", XliffWriter.GetLockString(lockedContent.Properties.LockType));
			CurrentLockedContentBuilder.StartElement("source");
			_builderStack.Push(_standardTuBuilder);
			_standardTuBuilder = CurrentLockedContentBuilder;
			WriteTransUnitContentForChildren(lockedContent.Content, _segmentsAsMrk);
			_standardTuBuilder = _builderStack.Pop();
			CurrentLockedContentBuilder.EndElement();
			CurrentLockedContentBuilder.EndElement();
			_writerLevel--;
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			_standardTuBuilder.StartElement("mrk");
			_standardTuBuilder.AddAttribute("mtype", "x-" + ((marker.MarkerType != null) ? marker.MarkerType : ""));
			if (!string.IsNullOrEmpty(marker.Id))
			{
				_standardTuBuilder.AddAttribute("mid", marker.Id);
			}
			if (marker.Count > 0)
			{
				WriteTransUnitContentForChildren(marker, _segmentsAsMrk);
			}
			_standardTuBuilder.EndElement();
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			_standardTuBuilder.StartElement("x");
			_standardTuBuilder.AddAttribute("id", tag.TagProperties.TagId.Id);
			string xidAttributeValue = GetXidAttributeValue(tag.SubSegments);
			if (!string.IsNullOrEmpty(xidAttributeValue))
			{
				_standardTuBuilder.AddAttribute("xid", xidAttributeValue);
			}
			_standardTuBuilder.EndElement();
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			_standardTuBuilder.StartElement("g");
			_standardTuBuilder.AddAttribute("id", tagPair.StartTagProperties.TagId.Id);
			string xidAttributeValue = GetXidAttributeValue(tagPair.SubSegments);
			if (!string.IsNullOrEmpty(xidAttributeValue))
			{
				_standardTuBuilder.AddAttribute("xid", xidAttributeValue);
			}
			if (tagPair.IsStartTagGhost)
			{
				_standardTuBuilder.AddAttribute("sdl:start", XliffWriter.XliffBoolValue(!tagPair.IsStartTagGhost));
			}
			if (tagPair.IsEndTagGhost)
			{
				_standardTuBuilder.AddAttribute("sdl:end", XliffWriter.XliffBoolValue(!tagPair.IsEndTagGhost));
			}
			if (tagPair.StartTagRevisionProperties != null)
			{
				_standardTuBuilder.AddAttribute("sdl:start-revid", _writer.DocSkeleton.StoreRevision(tagPair.StartTagRevisionProperties));
			}
			if (tagPair.EndTagRevisionProperties != null)
			{
				_standardTuBuilder.AddAttribute("sdl:end-revid", _writer.DocSkeleton.StoreRevision(tagPair.EndTagRevisionProperties));
			}
			WriteTransUnitContentForChildren(tagPair, _segmentsAsMrk);
			_standardTuBuilder.EndElement();
		}

		public void VisitSegment(ISegment segment)
		{
			if (!_segmentsAsMrk)
			{
				WriteTransUnitContentForChildren(segment, _segmentsAsMrk);
				return;
			}
			_standardTuBuilder.StartElement("mrk");
			_standardTuBuilder.AddAttribute("mtype", "seg");
			string value = segment.Properties.Id.Id.Replace(" ", "_x0020_");
			if (!string.IsNullOrEmpty(value))
			{
				_standardTuBuilder.AddAttribute("mid", value);
			}
			WriteTransUnitContentForChildren(segment, _segmentsAsMrk);
			_writer.SaveSegmentInfo(segment.Properties);
			_standardTuBuilder.EndElement();
		}

		public void VisitText(IText text)
		{
			if (!string.IsNullOrEmpty(text.Properties.Text))
			{
				_standardTuBuilder.AddText(text.Properties.Text);
			}
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			_standardTuBuilder.StartElement("mrk");
			switch (revisionMarker.Properties.RevisionType)
			{
			case RevisionType.Delete:
				_standardTuBuilder.AddAttribute("mtype", "x-sdl-deleted");
				break;
			case RevisionType.Insert:
				_standardTuBuilder.AddAttribute("mtype", "x-sdl-added");
				break;
			case RevisionType.FeedbackComment:
				_standardTuBuilder.AddAttribute("mtype", "x-sdl-feedback-comment");
				break;
			case RevisionType.FeedbackAdded:
				_standardTuBuilder.AddAttribute("mtype", "x-sdl-feedback-added");
				break;
			case RevisionType.FeedbackDeleted:
				_standardTuBuilder.AddAttribute("mtype", "x-sdl-feedback-deleted");
				break;
			case RevisionType.Unchanged:
				_standardTuBuilder.AddAttribute("mtype", "x-sdl-added");
				break;
			}
			_standardTuBuilder.AddAttribute("sdl:revid", _writer.DocSkeleton.StoreRevision(revisionMarker.Properties));
			if (revisionMarker.Count > 0)
			{
				WriteTransUnitContentForChildren(revisionMarker, _segmentsAsMrk);
			}
			_standardTuBuilder.EndElement();
		}
	}
}
