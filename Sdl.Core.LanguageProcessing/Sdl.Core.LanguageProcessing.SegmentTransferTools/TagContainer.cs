using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sdl.Core.LanguageProcessing.SegmentTransferTools
{
	public class TagContainer
	{
		private class TagCollector : IMarkupDataVisitor
		{
			private readonly List<IAbstractTag> _tags = new List<IAbstractTag>();

			private readonly bool _subsegmentReferencesOnly;

			public IEnumerable<IAbstractTag> Tags => _tags;

			public TagCollector(bool subsegmentReferencesOnly)
			{
				_subsegmentReferencesOnly = subsegmentReferencesOnly;
			}

			public List<IAbstractTag> LocateTags(IAbstractMarkupData abstractMarkupData)
			{
				abstractMarkupData.AcceptVisitor(this);
				return _tags;
			}

			private void VisitChildren(IEnumerable<IAbstractMarkupData> container)
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
				if (!_subsegmentReferencesOnly || tag.SubSegments.Any())
				{
					_tags.Add(tag);
				}
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
				if (!_subsegmentReferencesOnly || tagPair.SubSegments.Any())
				{
					_tags.Add(tagPair);
				}
				VisitChildren(tagPair);
			}

			public void VisitText(IText text)
			{
			}
		}

		private IAbstractMarkupData _markupData;

		private readonly bool _subsegmentReferencesOnly;

		public List<IAbstractTag> Tags
		{
			get;
			private set;
		}

		public TagContainer(bool subsegmentReferencesOnly)
		{
			_subsegmentReferencesOnly = subsegmentReferencesOnly;
		}

		public void Initialize(IAbstractMarkupData source)
		{
			if (!object.Equals(_markupData, source))
			{
				_markupData = source;
				TagCollector tagCollector = new TagCollector(_subsegmentReferencesOnly);
				Tags = tagCollector.LocateTags(source);
			}
		}

		public IAbstractTag FindRelatedTag(IAbstractTag tagToFind)
		{
			if (Tags == null)
			{
				return null;
			}
			if (!(tagToFind is ITagPair))
			{
				if (tagToFind is IPlaceholderTag)
				{
					return Tags.FirstOrDefault(delegate(IAbstractTag tag)
					{
						IPlaceholderTag placeholderTag = tag as IPlaceholderTag;
						return placeholderTag != null && ArePlaceholdersIdentical(placeholderTag, tagToFind as IPlaceholderTag);
					});
				}
				return null;
			}
			return Tags.FirstOrDefault(delegate(IAbstractTag tag)
			{
				ITagPair tagPair = tag as ITagPair;
				return tagPair != null && AreTagPairsIdentical(tagPair, tagToFind as ITagPair);
			});
		}

		public IAbstractTag FindRelatedTag(IAbstractTag tagToFind, IEnumerable<IAbstractTag> alreadyFound, out bool isnewTag)
		{
			IAbstractTag abstractTag = null;
			isnewTag = false;
			ITagPair tagPair = tagToFind as ITagPair;
			IPlaceholderTag placeholder = null;
			if (tagPair == null)
			{
				placeholder = (tagToFind as IPlaceholderTag);
			}
			if (Tags != null)
			{
				abstractTag = SearchTagOrPlaceholder(Tags, tagPair, placeholder);
				Tags.Remove(abstractTag);
			}
			if (abstractTag == null)
			{
				abstractTag = SearchTagOrPlaceholder(alreadyFound, tagPair, placeholder);
			}
			else
			{
				isnewTag = true;
			}
			return abstractTag;
		}

		private static IAbstractTag SearchTagOrPlaceholder(IEnumerable<IAbstractTag> tags, ITagPair tagPair, IPlaceholderTag placeholder)
		{
			IAbstractTag result = null;
			if (tagPair != null)
			{
				result = tags.FirstOrDefault(delegate(IAbstractTag tag)
				{
					ITagPair tagPair2 = tag as ITagPair;
					return tagPair2 != null && AreTagPairsIdentical(tagPair2, tagPair);
				});
			}
			else if (placeholder != null)
			{
				result = tags.FirstOrDefault(delegate(IAbstractTag tag)
				{
					IPlaceholderTag placeholderTag = tag as IPlaceholderTag;
					return placeholderTag != null && ArePlaceholdersIdentical(placeholderTag, placeholder);
				});
			}
			return result;
		}

		public bool Contains(string tagId)
		{
			return Tags.FirstOrDefault((IAbstractTag tag) => tag.TagProperties.TagId.Id == tagId) != null;
		}

		private static bool AreTagPairsIdentical(ITagPair item1, ITagPair item2)
		{
			if (AreStartTagPropertiesIdentical(item1.StartTagProperties, item2.StartTagProperties))
			{
				return AreEndTagPropertiesIdentical(item1.EndTagProperties, item2.EndTagProperties);
			}
			return false;
		}

		private static bool AreStartTagPropertiesIdentical(IStartTagProperties item1, IStartTagProperties item2)
		{
			if (item1.CanHide != item2.CanHide)
			{
				return false;
			}
			if (item1.DisplayText != item2.DisplayText)
			{
				return false;
			}
			if (item1.IsSoftBreak != item2.IsSoftBreak)
			{
				return false;
			}
			if (item1.IsWordStop != item2.IsWordStop)
			{
				return false;
			}
			return NormalizeTagText(item1.TagContent) == NormalizeTagText(item2.TagContent);
		}

		private static bool AreEndTagPropertiesIdentical(IEndTagProperties item1, IEndTagProperties item2)
		{
			if (item1.CanHide != item2.CanHide)
			{
				return false;
			}
			if (item1.DisplayText != item2.DisplayText)
			{
				return false;
			}
			if (item1.IsSoftBreak != item2.IsSoftBreak)
			{
				return false;
			}
			if (item1.IsWordStop != item2.IsWordStop)
			{
				return false;
			}
			return NormalizeTagText(item1.TagContent) == NormalizeTagText(item2.TagContent);
		}

		private static string NormalizeTagText(string tagText)
		{
			string input = Regex.Replace(tagText, "[\0-\u001f]", "");
			return Regex.Replace(input, "\\s{2,}", " ");
		}

		private static bool ArePlaceholdersIdentical(IPlaceholderTag item1, IPlaceholderTag item2)
		{
			if (NormalizeTagText(item1.Properties.TagContent) == NormalizeTagText(item2.Properties.TagContent) && item1.Properties.DisplayText == item2.Properties.DisplayText && item1.Properties.CanHide == item2.Properties.CanHide && item1.Properties.HasTextEquivalent == item2.Properties.HasTextEquivalent && item1.Properties.IsBreakableWhiteSpace == item2.Properties.IsBreakableWhiteSpace && item1.Properties.IsSoftBreak == item2.Properties.IsSoftBreak)
			{
				return item1.Properties.IsWordStop == item2.Properties.IsWordStop;
			}
			return false;
		}
	}
}
