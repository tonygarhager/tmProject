using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataSegmentBuilder : ISegmentElementVisitor
	{
		private ISegment _result;

		private IAbstractMarkupDataContainer _activeContainer;

		private Stack<int> _openPairedTagAnchors;

		public bool TreatAllTagsAsStandalone
		{
			get;
			set;
		}

		public IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		public ISegment Result
		{
			get
			{
				CheckForUnclosedContainers();
				return _result;
			}
			set
			{
				_result = value;
				_activeContainer = _result;
			}
		}

		public MarkupDataSegmentBuilder(IDocumentItemFactory itemFactory)
		{
			ItemFactory = (itemFactory ?? throw new ArgumentNullException());
			_result = itemFactory.CreateSegment(null);
			_activeContainer = _result;
		}

		public void VisitLinguaSegment(Segment segment)
		{
			_openPairedTagAnchors = new Stack<int>();
			foreach (SegmentElement element in segment.Elements)
			{
				if (element != null)
				{
					element.AcceptSegmentElementVisitor(this);
				}
				else
				{
					ILocationMarker item = ItemFactory.CreateLocationMarker();
					_activeContainer.Add(item);
				}
			}
			CheckForUnclosedContainers();
		}

		private void CheckForUnclosedContainers()
		{
			if (_activeContainer == _result)
			{
				return;
			}
			ITagPair tagPair = _activeContainer as ITagPair;
			if (tagPair != null)
			{
				throw new InvalidSegmentContentException("The start tag with ID '" + tagPair.StartTagProperties.TagId.Id + "' does not have a matching end tag within the segment.");
			}
			throw new InvalidSegmentContentException("The segment has containers that were not fully closed.");
		}

		public void VisitText(Text text)
		{
			IText item = ItemFactory.CreateText(ItemFactory.PropertiesFactory.CreateTextProperties(text.Value));
			_activeContainer.Add(item);
		}

		public void VisitTag(Tag tag)
		{
			if (TreatAllTagsAsStandalone)
			{
				CreatePlaceholderTag(tag);
				return;
			}
			switch (tag.Type)
			{
			case TagType.Start:
				if (tag.TagID.Contains("|Delete|") || tag.TagID.Contains("|Insert|") || tag.TagID.Contains("|FeedbackAdded|") || tag.TagID.Contains("|FeedbackDeleted|") || tag.TagID.Contains("|FeedbackComment|"))
				{
					string[] array = tag.TagID.Split('|');
					if (array.Length != 3)
					{
						throw new InvalidSegmentContentException("The revision marker ID '" + tag.TagID + "' is in an incorrect format");
					}
					if (!(array[1] == "FeedbackAdded") && !(array[1] == "FeedbackDeleted") && !(array[1] == "FeedbackComment"))
					{
						IRevisionProperties revisionProperties = ItemFactory.PropertiesFactory.CreateRevisionProperties((!(array[1] == "Insert")) ? RevisionType.Delete : RevisionType.Insert);
						revisionProperties.Author = array[0];
						revisionProperties.Date = (DateTime.TryParse(array[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result) ? result : DateTime.Now);
						_openPairedTagAnchors.Push(tag.Anchor);
						IRevisionMarker revisionMarker2 = ItemFactory.CreateRevision(revisionProperties);
						_activeContainer.Add(revisionMarker2);
						_activeContainer = revisionMarker2;
						break;
					}
					RevisionType type;
					switch (array[1])
					{
					case "FeedbackAdded":
						type = RevisionType.FeedbackAdded;
						break;
					case "FeedbackDeleted":
						type = RevisionType.FeedbackDeleted;
						break;
					case "FeedbackComment":
						type = RevisionType.FeedbackComment;
						break;
					default:
						type = RevisionType.FeedbackComment;
						break;
					}
					IRevisionProperties revisionProperties2 = ItemFactory.PropertiesFactory.CreateFeedbackProperties(type);
					revisionProperties2.Author = array[0];
					revisionProperties2.Date = (DateTime.TryParse(array[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result2) ? result2 : DateTime.Now);
					_openPairedTagAnchors.Push(tag.Anchor);
					IRevisionMarker revisionMarker3 = ItemFactory.CreateFeedback(revisionProperties2);
					_activeContainer.Add(revisionMarker3);
					_activeContainer = revisionMarker3;
				}
				else
				{
					IStartTagProperties startTagProperties = ItemFactory.PropertiesFactory.CreateStartTagProperties(string.Empty);
					startTagProperties.TagId = new TagId(tag.TagID);
					IEndTagProperties endTagInfo = ItemFactory.PropertiesFactory.CreateEndTagProperties(string.Empty);
					_openPairedTagAnchors.Push(tag.Anchor);
					ITagPair tagPair2 = ItemFactory.CreateTagPair(startTagProperties, endTagInfo);
					_activeContainer.Add(tagPair2);
					_activeContainer = tagPair2;
				}
				break;
			case TagType.End:
			{
				IRevisionMarker revisionMarker = _activeContainer as IRevisionMarker;
				if (revisionMarker != null && _openPairedTagAnchors.Count > 0)
				{
					if (_openPairedTagAnchors.Pop() != tag.Anchor)
					{
						throw new InvalidSegmentContentException($"The end tag {tag} does not match the start tag with ID '{revisionMarker.Properties}'.");
					}
					_activeContainer = revisionMarker.Parent;
					break;
				}
				ITagPair tagPair = _activeContainer as ITagPair;
				if (tagPair == null || _openPairedTagAnchors.Count == 0)
				{
					throw new InvalidSegmentContentException($"The end tag {tag} does not have a matching start tag.");
				}
				if (_openPairedTagAnchors.Pop() != tag.Anchor)
				{
					throw new InvalidSegmentContentException($"The end tag {tag} does not match the start tag with ID '{tagPair.StartTagProperties.TagId.Id}'.");
				}
				_activeContainer = tagPair.Parent;
				break;
			}
			case TagType.Standalone:
				CreatePlaceholderTag(tag);
				break;
			case TagType.TextPlaceholder:
				CreatePlaceholderTag(tag);
				break;
			case TagType.LockedContent:
				CreateLockedContent(tag);
				break;
			case TagType.Undefined:
				throw new InvalidSegmentContentException($"Undefined tag type for tag {tag}");
			}
		}

		private void CreateLockedContent(Tag tag)
		{
			ILockedContentProperties properties = ItemFactory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
			ILockedContent lockedContent = ItemFactory.CreateLockedContent(properties);
			if (!string.IsNullOrEmpty(tag.TextEquivalent))
			{
				lockedContent.Content.Add(ItemFactory.CreateText(ItemFactory.PropertiesFactory.CreateTextProperties(tag.TextEquivalent)));
			}
			_activeContainer.Add(lockedContent);
		}

		private void CreatePlaceholderTag(Tag tag)
		{
			IPlaceholderTagProperties placeholderTagProperties = ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(string.Empty);
			if (tag.Type == TagType.TextPlaceholder)
			{
				placeholderTagProperties.TextEquivalent = tag.TextEquivalent;
			}
			placeholderTagProperties.TagId = new TagId(tag.TagID);
			IPlaceholderTag item = ItemFactory.CreatePlaceholderTag(placeholderTagProperties);
			_activeContainer.Add(item);
		}

		public void VisitDateTimeToken(DateTimeToken token)
		{
			throw new InvalidSegmentContentException("Unexpected content: DateTimeToken.");
		}

		public void VisitNumberToken(NumberToken token)
		{
			throw new InvalidSegmentContentException("Unexpected content: NumberToken.");
		}

		public void VisitMeasureToken(MeasureToken token)
		{
			throw new InvalidSegmentContentException("Unexpected content: MeasureToken.");
		}

		public void VisitSimpleToken(SimpleToken token)
		{
			throw new InvalidSegmentContentException("Unexpected content: SimpleToken.");
		}

		public void VisitTagToken(TagToken token)
		{
			throw new InvalidSegmentContentException("Unexpected content: TagToken.");
		}
	}
}
