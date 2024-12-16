using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Text.RegularExpressions;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class InvalidEncodingRemovalVisitor : IMarkupDataVisitor
	{
		private const string _re = "[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]";

		public void ProcessParagraphUnit(IParagraphUnit pu)
		{
			if (!pu.IsStructure)
			{
				VisitChildren(pu.Source);
				VisitChildren(pu.Target);
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			tagPair.StartTagProperties.TagContent = Regex.Replace(tagPair.StartTagProperties.TagContent, "[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]", "");
			tagPair.EndTagProperties.TagContent = Regex.Replace(tagPair.EndTagProperties.TagContent, "[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]", "");
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			tag.TagProperties.TagContent = Regex.Replace(tag.TagProperties.TagContent, "[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]", "");
		}

		public void VisitText(IText text)
		{
			text.Properties.Text = Regex.Replace(text.Properties.Text, "[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]", "");
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
			foreach (IComment comment in commentMarker.Comments.Comments)
			{
				comment.Text = Regex.Replace(comment.Text, "[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]", "");
			}
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
