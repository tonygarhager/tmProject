using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataTagApplier : IMarkupDataVisitor
	{
		private readonly Dictionary<string, int> _lockedIndexMap = new Dictionary<string, int>();

		private readonly bool _excludeTagsInLockedContentText;

		public IAbstractMarkupDataContainer DocumentContent
		{
			get;
			set;
		}

		public IAbstractMarkupDataContainer SegmentToSearchIn
		{
			get;
			set;
		}

		public List<IAbstractMarkupData> UnmatchedItems
		{
			get;
		} = new List<IAbstractMarkupData>();


		public MarkupDataTagApplier()
			: this(excludeTagsInLockedContentText: true)
		{
		}

		public MarkupDataTagApplier(bool excludeTagsInLockedContentText)
		{
			_excludeTagsInLockedContentText = excludeTagsInLockedContentText;
		}

		public bool ApplyDocumentTags(IAbstractMarkupDataContainer matchContent)
		{
			UnmatchedItems.Clear();
			matchContent.ForEachSubItem(delegate(IAbstractMarkupData item)
			{
				item.AcceptVisitor(this);
			});
			return UnmatchedItems.Count == 0;
		}

		public void RemoveUnmatchedContent()
		{
			foreach (IAbstractMarkupData unmatchedItem in UnmatchedItems)
			{
				(unmatchedItem as IAbstractMarkupDataContainer)?.MoveAllItemsTo(unmatchedItem.Parent, unmatchedItem.IndexInParent + 1);
				unmatchedItem.RemoveFromParent();
			}
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			string lookFor = lockedContent.Content.ToString();
			int currentLockedCount = 0;
			IAbstractMarkupDataContainer abstractMarkupDataContainer = DocumentContent;
			if (SegmentToSearchIn != null)
			{
				abstractMarkupDataContainer = SegmentToSearchIn;
			}
			ILockedContent lockedContent2 = abstractMarkupDataContainer.Find(delegate(IAbstractMarkupData item)
			{
				ILockedContent lockedContent3 = item as ILockedContent;
				if (lockedContent3 == null)
				{
					return false;
				}
				string text = _excludeTagsInLockedContentText ? TextCollector.CollectText(lockedContent3) : lockedContent3.Content.ToString();
				if ((!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(lookFor)) && (text == null || !text.Equals(lookFor, StringComparison.InvariantCulture)))
				{
					return false;
				}
				int currentLockedIndex = currentLockedCount;
				currentLockedCount++;
				return CheckLockCount(text, currentLockedIndex);
			}) as ILockedContent;
			if (lockedContent2 != null)
			{
				UpdateLockCount(lockedContent2, currentLockedCount);
				lockedContent.Content.Clear();
				foreach (IAbstractMarkupData item in lockedContent2.Content)
				{
					lockedContent.Content.Add((IAbstractMarkupData)item.Clone());
				}
			}
			else
			{
				UnmatchedItems.Add(lockedContent);
			}
		}

		private void UpdateLockCount(IAbstractMarkupData matchingContent, int currentLockedIndex)
		{
			string text = TextCollector.CollectText(matchingContent);
			if (string.IsNullOrEmpty(text))
			{
				text = "";
			}
			if (_lockedIndexMap.ContainsKey(text))
			{
				_lockedIndexMap[text] = currentLockedIndex;
			}
			else
			{
				_lockedIndexMap.Add(text, currentLockedIndex);
			}
		}

		private bool CheckLockCount(string lockedText, int currentLockedIndex)
		{
			if (string.IsNullOrEmpty(lockedText))
			{
				lockedText = "";
			}
			if (!_lockedIndexMap.ContainsKey(lockedText))
			{
				return true;
			}
			return _lockedIndexMap[lockedText] == currentLockedIndex;
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			IPlaceholderTag placeholderTag = DocumentContent.Find(delegate(IAbstractMarkupData item)
			{
				IPlaceholderTag placeholderTag2 = item as IPlaceholderTag;
				return placeholderTag2 != null && tag.Properties.TagId == placeholderTag2.Properties.TagId;
			}) as IPlaceholderTag;
			if (placeholderTag == null)
			{
				UnmatchedItems.Add(tag);
				return;
			}
			tag.Properties = placeholderTag.Properties;
			if (placeholderTag.HasSubSegmentReferences)
			{
				tag.AddSubSegmentReferences(placeholderTag.SubSegments);
			}
		}

		public void VisitSegment(ISegment segment)
		{
			ISegment segment2 = DocumentContent.Find(delegate(IAbstractMarkupData item)
			{
				ISegment segment3 = item as ISegment;
				return segment3 != null && segment.Properties.Id == segment3.Properties.Id;
			}) as ISegment;
			if (segment2 == null)
			{
				UnmatchedItems.Add(segment);
			}
			else
			{
				segment.Properties = segment2.Properties;
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			ITagPair tagPair2 = DocumentContent.Find(delegate(IAbstractMarkupData item)
			{
				ITagPair tagPair3 = item as ITagPair;
				return tagPair3 != null && tagPair.StartTagProperties.TagId == tagPair3.StartTagProperties.TagId;
			}) as ITagPair;
			if (tagPair2 == null)
			{
				UnmatchedItems.Add(tagPair);
				return;
			}
			tagPair.StartTagProperties = tagPair2.StartTagProperties;
			tagPair.EndTagProperties = tagPair2.EndTagProperties;
			tagPair.ClearSubSegmentReferences();
			if (tagPair2.HasSubSegmentReferences)
			{
				tagPair.AddSubSegmentReferences(tagPair2.SubSegments);
			}
		}

		public void VisitText(IText text)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
		}
	}
}
