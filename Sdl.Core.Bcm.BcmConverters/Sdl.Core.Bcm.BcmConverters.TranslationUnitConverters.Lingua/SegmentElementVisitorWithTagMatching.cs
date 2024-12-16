using Sdl.Core.Bcm.BcmModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua
{
	internal class SegmentElementVisitorWithTagMatching : ISegmentElementVisitor
	{
		private readonly MarkupDataContainer _source;

		private readonly MarkupDataContainer _container;

		protected readonly Stack<TagPair> OpenedTagPairs;

		public SegmentElementVisitorWithTagMatching(MarkupDataContainer source, MarkupDataContainer container)
		{
			_source = source;
			_container = container;
			OpenedTagPairs = new Stack<TagPair>();
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

		protected void AddTagAsPlaceholder(Tag tag)
		{
			PlaceholderTag placeholderTag = _source.AllSubItems.OfType<PlaceholderTag>().FirstOrDefault((PlaceholderTag x) => x.FrameworkId == tag.TagID);
			if (placeholderTag != null)
			{
				AddToContainerOrOpenTagPair(placeholderTag.Clone());
			}
		}

		protected void OpenTagPair(Tag tag)
		{
			TagPair tagPair = _source.AllSubItems.OfType<TagPair>().FirstOrDefault((TagPair x) => x.FrameworkId == tag.TagID);
			if (tagPair != null)
			{
				TagPair tagPair2 = tagPair.Clone();
				tagPair2.Clear();
				AddToContainerOrOpenTagPair(tagPair2);
				OpenedTagPairs.Push(tagPair2);
			}
		}

		protected virtual void CloseTagPair(Tag tag)
		{
			if (_source.AllSubItems.OfType<TagPair>().Any((TagPair x) => x.FrameworkId == tag.TagID) && OpenedTagPairs.Any())
			{
				OpenedTagPairs.Pop();
			}
		}

		protected void AddTagAsLockedContent(Tag tag)
		{
			LockedContentContainer lockedContentContainer = null;
			foreach (LockedContentContainer item in _source.AllSubItems.OfType<LockedContentContainer>())
			{
				BcmTextCollector bcmTextCollector = new BcmTextCollector();
				bcmTextCollector.VisitChildren(item);
				if (tag.TextEquivalent.Equals(bcmTextCollector.Result, StringComparison.InvariantCulture))
				{
					lockedContentContainer = item;
					break;
				}
			}
			if (lockedContentContainer != null)
			{
				LockedContentContainer result = lockedContentContainer.Clone();
				AddToContainerOrOpenTagPair(result);
			}
		}

		protected void AddToContainerOrOpenTagPair(MarkupData result)
		{
			if (OpenedTagPairs.Count > 0)
			{
				OpenedTagPairs.Peek().Add(result);
			}
			else
			{
				_container.Add(result);
			}
		}

		private string GetNextId()
		{
			return Guid.NewGuid().ToString();
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
	}
}
