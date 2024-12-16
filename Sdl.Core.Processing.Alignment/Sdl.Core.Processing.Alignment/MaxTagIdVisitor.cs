using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.Core.Processing.Alignment
{
	internal class MaxTagIdVisitor : IMarkupDataVisitor
	{
		public int TagId
		{
			get;
			set;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			UpdateMaxTagID(tagPair.TagProperties.TagId.Id);
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			UpdateMaxTagID(tag.Properties.TagId.Id);
		}

		private void UpdateMaxTagID(string stringTagId)
		{
			if (int.TryParse(stringTagId, out int result))
			{
				TagId = Math.Max(TagId, result);
			}
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
