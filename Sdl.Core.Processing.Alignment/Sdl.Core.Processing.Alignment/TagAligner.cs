using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment
{
	internal static class TagAligner
	{
		private class TagCollector : IMarkupDataVisitor
		{
			private List<IAbstractTag> _tags = new List<IAbstractTag>();

			public List<IAbstractTag> Tags => _tags;

			public void VisitCommentMarker(ICommentMarker commentMarker)
			{
				foreach (IAbstractMarkupData item in commentMarker)
				{
					item.AcceptVisitor(this);
				}
			}

			public void VisitLocationMarker(ILocationMarker location)
			{
			}

			public void VisitLockedContent(ILockedContent lockedContent)
			{
				foreach (IAbstractMarkupData item in lockedContent.Content)
				{
					item.AcceptVisitor(this);
				}
			}

			public void VisitOtherMarker(IOtherMarker marker)
			{
				foreach (IAbstractMarkupData item in marker)
				{
					item.AcceptVisitor(this);
				}
			}

			public void VisitPlaceholderTag(IPlaceholderTag tag)
			{
				_tags.Add(tag);
			}

			public void VisitRevisionMarker(IRevisionMarker revisionMarker)
			{
			}

			public void VisitSegment(ISegment segment)
			{
				foreach (IAbstractMarkupData item in segment)
				{
					item.AcceptVisitor(this);
				}
			}

			public void VisitTagPair(ITagPair tagPair)
			{
				_tags.Add(tagPair);
				foreach (IAbstractMarkupData item in tagPair)
				{
					item.AcceptVisitor(this);
				}
			}

			public void VisitText(IText text)
			{
			}
		}

		private static int _maxTagId;

		public static void AlignTags(ISegment source, ISegment target)
		{
			List<IAbstractTag> list = CollectTags(source);
			List<IAbstractTag> list2 = CollectTags(target);
			if (list.Count == 0 || list2.Count == 0)
			{
				if (list.Count > 0)
				{
					AssignRandomTagId(list);
				}
				else if (list2.Count > 0)
				{
					AssignRandomTagId(list2);
				}
				return;
			}
			BitArray pairedTagFlags = GetPairedTagFlags(list);
			BitArray pairedTagFlags2 = GetPairedTagFlags(list2);
			BitArray bitArray = new BitArray(list2.Count, defaultValue: false);
			for (int i = 0; i < list.Count; i++)
			{
				bool flag = false;
				for (int j = 0; j < list2.Count; j++)
				{
					if (flag)
					{
						break;
					}
					if (!bitArray[j] && pairedTagFlags[i] == pairedTagFlags2[j])
					{
						IAbstractTag sourceTag = list[i];
						IAbstractTag targetTag = list2[j];
						if (AttemptAlignTags(sourceTag, targetTag))
						{
							bitArray[j] = true;
							flag = true;
						}
					}
				}
				if (!flag)
				{
					AssignRandomTagId(list[i]);
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				if (!bitArray[k])
				{
					AssignRandomTagId(list2[k]);
				}
			}
		}

		private static void AssignRandomTagId(IAbstractTag tag)
		{
			IPlaceholderTag placeholderTag = tag as IPlaceholderTag;
			if (placeholderTag != null)
			{
				placeholderTag.Properties.TagId = new TagId((++_maxTagId).ToString(CultureInfo.InvariantCulture));
				return;
			}
			ITagPair tagPair = tag as ITagPair;
			tagPair.StartTagProperties.TagId = new TagId((++_maxTagId).ToString(CultureInfo.InvariantCulture));
		}

		private static void AssignRandomTagId(IEnumerable<IAbstractTag> tags)
		{
			foreach (IAbstractTag tag in tags)
			{
				AssignRandomTagId(tag);
			}
		}

		private static bool AttemptAlignTags(IAbstractTag sourceTag, IAbstractTag targetTag)
		{
			if (sourceTag is IPlaceholderTag != targetTag is IPlaceholderTag)
			{
				return false;
			}
			if (sourceTag is IPlaceholderTag)
			{
				IPlaceholderTag placeholderTag = sourceTag as IPlaceholderTag;
				IPlaceholderTag placeholderTag2 = targetTag as IPlaceholderTag;
				if (AreTagsAlignable(placeholderTag, placeholderTag2))
				{
					placeholderTag2.Properties = placeholderTag.Properties;
					return true;
				}
			}
			else
			{
				ITagPair tagPair = sourceTag as ITagPair;
				ITagPair tagPair2 = targetTag as ITagPair;
				if (AreTagsAlignable(tagPair, tagPair2))
				{
					tagPair2.StartTagProperties = tagPair.StartTagProperties;
					tagPair2.EndTagProperties = tagPair.EndTagProperties;
					return true;
				}
			}
			return false;
		}

		public static bool AreTagsAlignable(IAbstractTag srcTag, IAbstractTag trgTag)
		{
			IPlaceholderTag placeholderTag = srcTag as IPlaceholderTag;
			IPlaceholderTag placeholderTag2 = trgTag as IPlaceholderTag;
			if (placeholderTag != null && placeholderTag2 != null)
			{
				return AreTagsAlignable(placeholderTag, placeholderTag2);
			}
			ITagPair tagPair = srcTag as ITagPair;
			ITagPair tagPair2 = trgTag as ITagPair;
			if (tagPair != null && tagPair2 != null)
			{
				return AreTagsAlignable(tagPair, tagPair2);
			}
			return false;
		}

		private static bool AreTagsAlignable(IPlaceholderTag srcTag, IPlaceholderTag tgtTag)
		{
			if (srcTag.Properties.SegmentationHint == tgtTag.Properties.SegmentationHint && srcTag.Properties.IsWordStop == tgtTag.Properties.IsWordStop && srcTag.Properties.IsSoftBreak == tgtTag.Properties.IsSoftBreak && srcTag.Properties.IsBreakableWhiteSpace == tgtTag.Properties.IsBreakableWhiteSpace && srcTag.Properties.CanHide == tgtTag.Properties.CanHide && string.Equals(srcTag.Properties.DisplayText, tgtTag.Properties.DisplayText, StringComparison.OrdinalIgnoreCase))
			{
				return IsLocalizableContentAlignable(srcTag, tgtTag);
			}
			return false;
		}

		private static bool AreTagsAlignable(ITagPair srcTag, ITagPair tgtTag)
		{
			if (srcTag.StartTagProperties.CanHide == tgtTag.StartTagProperties.CanHide && string.Equals(srcTag.StartTagProperties.DisplayText, tgtTag.StartTagProperties.DisplayText, StringComparison.OrdinalIgnoreCase) && IsLocalizableContentAlignable(srcTag, tgtTag) && srcTag.StartTagProperties.IsSoftBreak == tgtTag.StartTagProperties.IsSoftBreak && srcTag.StartTagProperties.SegmentationHint == tgtTag.StartTagProperties.SegmentationHint)
			{
				return srcTag.StartTagProperties.IsWordStop == tgtTag.StartTagProperties.IsWordStop;
			}
			return false;
		}

		private static bool IsLocalizableContentAlignable(ITagPair srcTag, ITagPair tgtTag)
		{
			if (srcTag.StartTagProperties.HasLocalizableContent != tgtTag.StartTagProperties.HasLocalizableContent)
			{
				return false;
			}
			if (!srcTag.StartTagProperties.HasLocalizableContent && !srcTag.StartTagProperties.TagContent.Equals(tgtTag.StartTagProperties.TagContent))
			{
				return false;
			}
			return true;
		}

		private static bool IsLocalizableContentAlignable(IPlaceholderTag srcTag, IPlaceholderTag tgtTag)
		{
			if (srcTag.TagProperties.HasLocalizableContent != tgtTag.TagProperties.HasLocalizableContent)
			{
				return false;
			}
			if (!srcTag.TagProperties.HasLocalizableContent && !srcTag.TagProperties.TagContent.Equals(tgtTag.TagProperties.TagContent))
			{
				return false;
			}
			return true;
		}

		private static List<IAbstractTag> CollectTags(ISegment segment)
		{
			TagCollector tagCollector = new TagCollector();
			segment.AcceptVisitor(tagCollector);
			return tagCollector.Tags;
		}

		private static BitArray GetPairedTagFlags(List<IAbstractTag> tags)
		{
			BitArray bitArray = new BitArray(tags.Count);
			for (int i = 0; i < tags.Count; i++)
			{
				bitArray[i] = (tags[i] is ITagPair);
			}
			return bitArray;
		}

		public static void AlignTags(IParagraphUnit paragraph)
		{
			FindMaxTagId(paragraph);
			foreach (ISegmentPair segmentPair in paragraph.SegmentPairs)
			{
				AlignTags(segmentPair.Source, segmentPair.Target);
			}
		}

		private static void FindMaxTagId(IParagraphUnit paragraph)
		{
			_maxTagId = 0;
			foreach (ISegmentPair segmentPair in paragraph.SegmentPairs)
			{
				foreach (IAbstractTag item in CollectTags(segmentPair.Source))
				{
					SetMaxTagId(item.TagProperties.TagId.Id);
				}
				foreach (IAbstractTag item2 in CollectTags(segmentPair.Target))
				{
					SetMaxTagId(item2.TagProperties.TagId.Id);
				}
			}
		}

		private static void SetMaxTagId(string id)
		{
			if (int.TryParse(id, out int result) && result > _maxTagId)
			{
				_maxTagId = result;
			}
		}
	}
}
