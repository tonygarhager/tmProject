using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	internal class TransUnitBuilder : IMarkupDataVisitor
	{
		private readonly XliffWriter _writer;

		private bool _segmentsAsMrk = true;

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

		public bool HasContent
		{
			get;
			set;
		}

		public TransUnitBuilder(XliffWriter writer)
		{
			_writer = writer;
		}

		public static bool BuildTransUnitContentForChildren(IAbstractMarkupDataContainer container, XliffWriter writer, bool segmentsAsMrk)
		{
			if (container == null || container.Count == 0)
			{
				return false;
			}
			TransUnitBuilder transUnitBuilder = new TransUnitBuilder(writer);
			transUnitBuilder.SegmentsAsMrk = segmentsAsMrk;
			transUnitBuilder.VisitChildren(container);
			return transUnitBuilder.HasContent;
		}

		public void VisitChildren(IAbstractMarkupDataContainer container)
		{
			HasContent = false;
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
				HasContent = true;
			}
		}

		public void VisitStructureTag(IStructureTag tag)
		{
			_writer.SaveTagInfo(tag);
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			_writer.DocSkeleton.StoreComments(commentMarker.Comments);
			if (commentMarker.Count > 0)
			{
				BuildTransUnitContentForChildren(commentMarker, _writer, _segmentsAsMrk);
			}
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			BuildTransUnitContentForChildren(lockedContent.Content, _writer, _segmentsAsMrk);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			BuildTransUnitContentForChildren(marker, _writer, _segmentsAsMrk);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			_writer.SaveTagInfo(tag);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagRevisionProperties != null)
			{
				_writer.DocSkeleton.StoreRevision(tagPair.StartTagRevisionProperties);
			}
			if (tagPair.EndTagRevisionProperties != null)
			{
				_writer.DocSkeleton.StoreRevision(tagPair.EndTagRevisionProperties);
			}
			BuildTransUnitContentForChildren(tagPair, _writer, _segmentsAsMrk);
			_writer.SaveTagInfo(tagPair);
		}

		public void VisitSegment(ISegment segment)
		{
			if (!_segmentsAsMrk)
			{
				BuildTransUnitContentForChildren(segment, _writer, _segmentsAsMrk);
			}
		}

		public void VisitText(IText text)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			_writer.DocSkeleton.StoreRevision(revisionMarker.Properties);
			BuildTransUnitContentForChildren(revisionMarker, _writer, _segmentsAsMrk);
		}
	}
}
