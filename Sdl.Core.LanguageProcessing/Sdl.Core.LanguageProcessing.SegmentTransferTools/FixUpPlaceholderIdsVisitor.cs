using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.SegmentTransferTools
{
	public class FixUpPlaceholderIdsVisitor : IMarkupDataVisitor
	{
		private readonly TagContainer _source;

		private readonly List<IAbstractTag> _alreadyFoundTags = new List<IAbstractTag>();

		private readonly Stack<ITagPair> _correspondingTagPairsStack = new Stack<ITagPair>();

		private readonly Stack<int> _currentSourceIndexStack = new Stack<int>();

		public FixUpPlaceholderIdsVisitor(TagContainer source)
		{
			_source = source;
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
			if (_correspondingTagPairsStack.Count == 0)
			{
				return;
			}
			bool flag = false;
			IPlaceholderTag placeholderTag = null;
			ITagPair tagPair = _correspondingTagPairsStack.Peek();
			int num = _currentSourceIndexStack.Pop();
			while (num < tagPair.Count)
			{
				IAbstractMarkupData abstractMarkupData = tagPair[num++];
				placeholderTag = (abstractMarkupData as IPlaceholderTag);
				if (placeholderTag != null && placeholderTag.TagProperties.TagContent.Equals(tag.TagProperties.TagContent))
				{
					flag = true;
					break;
				}
			}
			_currentSourceIndexStack.Push(num);
			if (flag)
			{
				CopyTagProperties(tag, placeholderTag);
			}
		}

		private static void CopyTagProperties(IPlaceholderTag targetTag, IPlaceholderTag sourceTag)
		{
			targetTag.Properties = (sourceTag.Properties.Clone() as IPlaceholderTagProperties);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			bool flag = false;
			bool isnewTag;
			ITagPair tagPair2 = _source.FindRelatedTag(tagPair, _alreadyFoundTags, out isnewTag) as ITagPair;
			if (tagPair2 != null)
			{
				_alreadyFoundTags.Add(tagPair);
				_correspondingTagPairsStack.Push(tagPair2);
				_currentSourceIndexStack.Push(0);
				flag = true;
			}
			VisitChildren(tagPair);
			if (flag)
			{
				_correspondingTagPairsStack.Pop();
				_currentSourceIndexStack.Pop();
			}
		}

		public void VisitText(IText text)
		{
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
