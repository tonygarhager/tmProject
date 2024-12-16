using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.Processing.Alignment
{
	internal class ReplaceTagIdVisitor : IMarkupDataVisitor
	{
		private string tagIdPrefix;

		public int TagId
		{
			get;
			set;
		}

		public ReplaceTagIdVisitor(int tagid, string paragraphUnitId)
		{
			TagId = tagid;
			tagIdPrefix = "align" + paragraphUnitId;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			tagPair.TagProperties.TagId = new TagId(tagIdPrefix + TagId++);
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			tag.Properties.TagId = new TagId(tagIdPrefix + TagId++);
		}

		public void VisitText(IText text)
		{
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChildren(lockedContent.Content);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
