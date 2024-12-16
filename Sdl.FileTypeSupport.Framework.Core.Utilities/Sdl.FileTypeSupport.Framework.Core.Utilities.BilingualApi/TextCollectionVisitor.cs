using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class TextCollectionVisitor : IMarkupDataVisitor
	{
		private bool _tagContentAsText;

		private StringBuilder _collectedText;

		public string CollectedText => _collectedText.ToString();

		public TextCollectionVisitor(bool tagContentAsText)
		{
			_tagContentAsText = tagContentAsText;
			_collectedText = new StringBuilder();
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (_tagContentAsText)
			{
				_collectedText.Append(tagPair.StartTagProperties.TagContent);
			}
			VisitChildren(tagPair);
			if (_tagContentAsText)
			{
				_collectedText.Append(tagPair.EndTagProperties.TagContent);
			}
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (_tagContentAsText)
			{
				_collectedText.Append(tag.Properties.TagContent);
			}
		}

		public void VisitText(IText text)
		{
			_collectedText.Append(text.Properties.Text);
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
			if (revisionMarker.Properties.RevisionType == RevisionType.Insert)
			{
				VisitChildren(revisionMarker);
			}
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
