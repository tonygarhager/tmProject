using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua
{
	internal class SegmentElementVisitor : ISegmentElementVisitor
	{
		private readonly MarkupDataContainer _container;

		private readonly FileSkeleton _fileSkeleton;

		private readonly Stack<TagPair> _openedTagPairs;

		public SegmentElementVisitor(MarkupDataContainer container, FileSkeleton fileSkeleton)
		{
			_container = container;
			_fileSkeleton = fileSkeleton;
			_openedTagPairs = new Stack<TagPair>();
		}

		public void VisitText(Text text)
		{
			TextMarkup result = new TextMarkup
			{
				Id = GetNextId(),
				Text = text.Value
			};
			AddToContainerOrOpenTagPair(result);
		}

		public void VisitTag(Tag tag)
		{
			switch (tag.Type)
			{
			case TagType.Undefined:
				break;
			case TagType.Start:
				OpenTagPair(tag);
				break;
			case TagType.End:
				CloseTagPair(tag);
				break;
			case TagType.Standalone:
				AddTagAsPlaceholder(tag);
				break;
			case TagType.TextPlaceholder:
				AddTagAsPlaceholder(tag);
				break;
			case TagType.LockedContent:
				AddTagAsLockedContent(tag);
				break;
			case TagType.UnmatchedStart:
				throw new InvalidOperationException("Unmatched start tags are not supported.");
			case TagType.UnmatchedEnd:
				throw new InvalidOperationException("Unmatched end tags are not supported.");
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void AddTagAsLockedContent(Tag tag)
		{
			LockedContentContainer lockedContentContainer = new LockedContentContainer
			{
				Id = GetNextId()
			};
			TextMarkup markupData = new TextMarkup
			{
				Id = GetNextId(),
				Text = tag.TextEquivalent
			};
			lockedContentContainer.Add(markupData);
			AddSegmentElementMetadata(tag, lockedContentContainer);
			AddToContainerOrOpenTagPair(lockedContentContainer);
		}

		private void OpenTagPair(Tag tag)
		{
			if (!QuickInsertHelper.TryCreateSkeletonTagPairDefinition(tag.TagID, _fileSkeleton, out TagPairDefinition tagPairDefinition))
			{
				tagPairDefinition = new TagPairDefinition
				{
					StartTagContent = tag.TextEquivalent
				};
				tagPairDefinition = _fileSkeleton.TagPairDefinitions.GetOrAdd(tagPairDefinition);
			}
			TagPair tagPair = new TagPair
			{
				Id = GetNextId(),
				TagPairDefinitionId = tagPairDefinition.Id
			};
			AddSegmentElementMetadata(tag, tagPair);
			AddToContainerOrOpenTagPair(tagPair);
			_openedTagPairs.Push(tagPair);
		}

		private void CloseTagPair(Tag tag)
		{
			TagPair tagPair = _openedTagPairs.Pop();
			TagPairDefinition tagPairDefinition = _fileSkeleton.TagPairDefinitions.First((TagPairDefinition x) => x.Id == tagPair.TagPairDefinitionId);
			tagPairDefinition.EndTagContent = tag.TextEquivalent;
		}

		private void AddTagAsPlaceholder(Tag tag)
		{
			if (!QuickInsertHelper.TryCreateSkeletonPlaceholderTagDefinition(tag.TagID, _fileSkeleton, out PlaceholderTagDefinition placeholderTagDefinition))
			{
				placeholderTagDefinition = new PlaceholderTagDefinition
				{
					TextEquivalent = tag.TextEquivalent
				};
				placeholderTagDefinition = _fileSkeleton.PlaceholderTagDefinitions.GetOrAdd(placeholderTagDefinition);
			}
			PlaceholderTag result = new PlaceholderTag
			{
				Id = GetNextId(),
				PlaceholderTagDefinitionId = placeholderTagDefinition.Id
			};
			AddSegmentElementMetadata(tag, result);
			AddToContainerOrOpenTagPair(result);
		}

		private static void AddSegmentElementMetadata(Tag tag, MarkupData result)
		{
			result.SetMetadata("frameworkOriginalTagId", tag.TagID);
		}

		private void AddToContainerOrOpenTagPair(MarkupData result)
		{
			if (_openedTagPairs.Count > 0)
			{
				_openedTagPairs.Peek().Add(result);
			}
			else
			{
				_container.Add(result);
			}
		}

		public void VisitDateTimeToken(DateTimeToken token)
		{
		}

		public void VisitNumberToken(NumberToken token)
		{
		}

		public void VisitMeasureToken(MeasureToken token)
		{
		}

		public void VisitSimpleToken(SimpleToken token)
		{
		}

		public void VisitTagToken(TagToken token)
		{
		}

		private string GetNextId()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
