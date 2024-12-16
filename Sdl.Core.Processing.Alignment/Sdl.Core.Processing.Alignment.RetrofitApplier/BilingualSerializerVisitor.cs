using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Text;

namespace Sdl.Core.Processing.Alignment.RetrofitApplier
{
	public class BilingualSerializerVisitor : IMarkupDataVisitor
	{
		public StringBuilder ContentString
		{
			get;
			set;
		}

		private BilingualSerializationFlags Flags
		{
			get;
			set;
		}

		public BilingualSerializerVisitor(BilingualSerializationFlags flags)
		{
			ContentString = new StringBuilder();
			Flags = flags;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			string str = "";
			if (Flags != null && Flags.TagID)
			{
				str = tagPair.TagProperties.TagId.Id;
			}
			ContentString.Append("<t" + str + ">");
			VisitChildren(tagPair);
			ContentString.Append("</t" + str + ">");
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			ContentString.Append("<ph/>");
		}

		public void VisitText(IText textItem)
		{
			ContentString.Append(((Text)textItem).Properties.Text);
		}

		public void VisitSegment(ISegment segment)
		{
			ContentString.Append("<s>");
			VisitChildren(segment);
			ContentString.Append("</s>");
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			ContentString.Append("<c>");
			VisitChildren(commentMarker);
			ContentString.Append("</c>");
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			ContentString.Append("<l>");
			VisitChildren(lockedContent.Content);
			ContentString.Append("</l>");
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			ContentString.Append(string.Format("<r{0}>", (revisionMarker.Properties.RevisionType == RevisionType.Insert) ? "i" : "d"));
			VisitChildren(revisionMarker);
			ContentString.Append(string.Format("</r{0}>", (revisionMarker.Properties.RevisionType == RevisionType.Insert) ? "i" : "d"));
		}

		public void VisitTagPairToken(BilingualTagPairToken tagPairToken)
		{
			if (tagPairToken.IsStart)
			{
				ContentString.Append("<m" + tagPairToken.Anchor + ">");
			}
			else
			{
				ContentString.Append("</m" + tagPairToken.Anchor + ">");
			}
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			for (int i = 0; i < container.Count; i++)
			{
				if (container[i] is BilingualTagPairToken)
				{
					VisitTagPairToken(container[i] as BilingualTagPairToken);
				}
				else
				{
					container[i].AcceptVisitor(this);
				}
			}
		}
	}
}
