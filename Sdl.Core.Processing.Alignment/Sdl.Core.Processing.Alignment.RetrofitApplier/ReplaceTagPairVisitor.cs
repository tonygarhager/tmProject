using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class ReplaceTagPairVisitor : IMarkupDataVisitor
	{
		private Segment LinguaSegment
		{
			get;
			set;
		}

		private int LinguaIndex
		{
			get;
			set;
		}

		public ReplaceTagPairVisitor(Segment linguaSegment)
		{
			LinguaSegment = linguaSegment;
			LinguaIndex = linguaSegment.Tokens.Count - 1;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			ReplaceMarkupTagWithFakeTag(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			LinguaIndex--;
		}

		public void VisitText(IText textItem)
		{
			string str = "";
			Token token = LinguaSegment.Tokens[LinguaIndex];
			if (token.Text != " ")
			{
				string text = textItem.Properties.Text;
				textItem.Properties.Text = textItem.Properties.Text.TrimEnd();
				str = text.Substring(textItem.Properties.Text.Length);
			}
			IAbstractMarkupDataContainer parent = textItem.Parent;
			if (token.Text.Length < textItem.Properties.Text.Length)
			{
				IText text2 = textItem.Split(textItem.Properties.Text.Length - token.Text.Length);
				parent.Insert(textItem.IndexInParent + 1, text2);
				if (token.Text != " ")
				{
					text2.Properties.Text = text2.Properties.Text + str;
				}
				token = LinguaSegment.Tokens[--LinguaIndex];
			}
			while (token.Text.Length < textItem.Properties.Text.Length)
			{
				IText text2 = textItem.Split(textItem.Properties.Text.Length - token.Text.Length);
				parent.Insert(textItem.IndexInParent + 1, text2);
				token = LinguaSegment.Tokens[--LinguaIndex];
			}
			LinguaIndex--;
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
			ReplaceMarkupTagWithFakeTag(commentMarker);
		}

		private void ReplaceMarkupTagWithFakeTag(IAbstractMarkupDataContainer container)
		{
			IAbstractMarkupData abstractMarkupData = container as IAbstractMarkupData;
			IAbstractMarkupDataContainer parent = abstractMarkupData.Parent;
			int indexInParent = abstractMarkupData.IndexInParent;
			TagToken linguaTagToken = LinguaSegment.Tokens[LinguaIndex] as TagToken;
			LinguaIndex--;
			VisitChildren(container);
			TagToken linguaTagToken2 = LinguaSegment.Tokens[LinguaIndex] as TagToken;
			LinguaIndex--;
			BilingualTagPairToken item = new BilingualTagPairToken(linguaTagToken, container);
			parent.Insert(indexInParent + 1, item);
			BilingualTagPairToken item2 = new BilingualTagPairToken(linguaTagToken2, container);
			parent.Insert(indexInParent, item2);
			container.MoveAllItemsTo(parent, abstractMarkupData.IndexInParent);
			abstractMarkupData.RemoveFromParent();
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
			for (int num = container.Count - 1; num >= 0; num--)
			{
				container[num].AcceptVisitor(this);
			}
		}
	}
}
