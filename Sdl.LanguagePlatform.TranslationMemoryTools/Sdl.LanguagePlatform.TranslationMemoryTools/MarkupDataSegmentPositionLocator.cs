using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	internal class MarkupDataSegmentPositionLocator : ISegmentElementVisitor
	{
		private bool _advanceFieldAfterMatch = true;

		private int _uptoOffset = -1;

		public Location Location
		{
			get;
			set;
		}

		public int OffsetIntoTextField
		{
			get;
			set;
		}

		public IAbstractMarkupData ItemAtLocation => Location.ItemAtLocation;

		public MarkupDataSegmentPositionLocator(ISegment markupDataSegment)
		{
			Location = new Location(markupDataSegment, firstLocation: true);
			OffsetIntoTextField = 0;
		}

		public void StepOver(SegmentElement element)
		{
			_advanceFieldAfterMatch = true;
			_uptoOffset = -1;
			element?.AcceptSegmentElementVisitor(this);
		}

		public void StepInto(SegmentElement element, int uptoOffset)
		{
			_advanceFieldAfterMatch = false;
			_uptoOffset = uptoOffset;
			element?.AcceptSegmentElementVisitor(this);
		}

		public void AdjustToDataItemPosition()
		{
			do
			{
				if (Location.ItemAtLocation is IAbstractDataContent)
				{
					return;
				}
			}
			while (Location.MoveNext());
			throw new InvalidSegmentContentException("no more items in the markup data!");
		}

		public void VisitText(Text text)
		{
			AdjustToDataItemPosition();
			IText obj = (Location.ItemAtLocation as IText) ?? throw new InvalidSegmentContentException("Expected a text at this position in the markup data!");
			string text2 = text.Value;
			if (_uptoOffset != -1)
			{
				text2 = text.Value.Substring(0, _uptoOffset);
			}
			int length = text2.Length;
			string text3 = obj.Properties.Text;
			if (text3.Length < OffsetIntoTextField + length)
			{
				int num = text3.Length - OffsetIntoTextField;
				if (string.Compare(text2.Substring(0, num), text3.Substring(OffsetIntoTextField), StringComparison.Ordinal) != 0)
				{
					throw new InvalidSegmentContentException("Text in the markup data does not match the lingua element text!");
				}
				OffsetIntoTextField = 0;
				Location.MoveNext();
				Text text4 = new Text(text2.Substring(num));
				if (_uptoOffset != -1)
				{
					_uptoOffset -= num;
				}
				VisitText(text4);
			}
			else
			{
				if (string.Compare(text2, text3.Substring(OffsetIntoTextField, length), StringComparison.Ordinal) != 0)
				{
					throw new InvalidSegmentContentException("Text in the markup data does not match the lingua element text!");
				}
				OffsetIntoTextField += length;
				if (_advanceFieldAfterMatch && text3.Length == OffsetIntoTextField)
				{
					Location.MoveNext();
					OffsetIntoTextField = 0;
				}
			}
		}

		public void VisitTag(Tag tag)
		{
			if (tag.Type == TagType.End && Location.ItemAtLocation == null && Location.BottomLevel.Parent is ITagPair)
			{
				if (_advanceFieldAfterMatch)
				{
					Location.MoveNext();
				}
				return;
			}
			AdjustToDataItemPosition();
			if (tag.Type == TagType.LockedContent)
			{
				if (!(Location.ItemAtLocation is ILockedContent))
				{
					throw new InvalidSegmentContentException("Expected locked content at this position in the markup data!");
				}
				if (_advanceFieldAfterMatch)
				{
					Location.MoveNext();
				}
			}
			else
			{
				if (!(Location.ItemAtLocation is IAbstractTag))
				{
					throw new InvalidSegmentContentException("Expected a tag at this position in the markup data!");
				}
				if (_advanceFieldAfterMatch)
				{
					Location.MoveNext();
				}
			}
		}

		public void VisitDateTimeToken(DateTimeToken token)
		{
			throw new InvalidSegmentContentException("Unexpected SegmentElement type!");
		}

		public void VisitNumberToken(NumberToken token)
		{
			throw new InvalidSegmentContentException("Unexpected SegmentElement type!");
		}

		public void VisitMeasureToken(MeasureToken token)
		{
			throw new InvalidSegmentContentException("Unexpected SegmentElement type!");
		}

		public void VisitSimpleToken(SimpleToken token)
		{
			throw new InvalidSegmentContentException("Unexpected SegmentElement type!");
		}

		public void VisitTagToken(TagToken token)
		{
			throw new InvalidSegmentContentException("Unexpected SegmentElement type!");
		}
	}
}
