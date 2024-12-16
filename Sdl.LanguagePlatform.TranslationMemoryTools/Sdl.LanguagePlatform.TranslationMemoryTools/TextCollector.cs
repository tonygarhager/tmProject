using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	internal class TextCollector : IMarkupDataVisitor
	{
		private readonly StringBuilder _result = new StringBuilder();

		public string Result => _result.ToString();

		public static string CollectText(IAbstractMarkupData markupData)
		{
			TextCollector textCollector = new TextCollector();
			markupData.AcceptVisitor(textCollector);
			return textCollector.Result;
		}

		public void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChildren(lockedContent.Content);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			if (revisionMarker.Properties.RevisionType != RevisionType.Delete)
			{
				VisitChildren(revisionMarker);
			}
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			VisitChildren(tagPair);
		}

		public void VisitText(IText text)
		{
			_result.Append(text.Properties.Text);
		}
	}
}
