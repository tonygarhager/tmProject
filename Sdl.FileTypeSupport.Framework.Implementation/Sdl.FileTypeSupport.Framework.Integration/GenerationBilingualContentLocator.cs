using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class GenerationBilingualContentLocator : AbstractBilingualContentProcessor, INativeGenerationBilingualContentLocator
	{
		private Dictionary<ParagraphUnitId, IParagraphUnit> _paragraphUnits = new Dictionary<ParagraphUnitId, IParagraphUnit>();

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			_paragraphUnits.Add(paragraphUnit.Properties.ParagraphUnitId, paragraphUnit);
			base.ProcessParagraphUnit(paragraphUnit);
		}

		private TextLocation FindLocationOutsideSegment(ContentRestriction contentType, ParagraphUnitId paragraphUnitId, SegmentId? lastSegmentId, int charactersFromEndOfLastSegment)
		{
			if (!_paragraphUnits.TryGetValue(paragraphUnitId, out IParagraphUnit value))
			{
				return null;
			}
			IParagraph rootContainer = value.Source;
			if (contentType == ContentRestriction.Target)
			{
				rootContainer = value.Target;
			}
			Location location = new Location(rootContainer, firstLocation: true);
			if (lastSegmentId.HasValue)
			{
				location = ((contentType != ContentRestriction.Target) ? value.GetSourceSegmentLocation(lastSegmentId.Value) : value.GetTargetSegmentLocation(lastSegmentId.Value));
				location.MoveNextSibling();
			}
			CharacterCountingIterator characterCountingIterator = new CharacterCountingIterator(location);
			bool flag = false;
			while (characterCountingIterator.CharacterCount <= charactersFromEndOfLastSegment)
			{
				if (!characterCountingIterator.MoveNext())
				{
					flag = true;
					break;
				}
				if (characterCountingIterator.CurrentLocation.ItemAtLocation is ISegment)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return new TextLocation(characterCountingIterator.CurrentLocation, 0);
			}
			characterCountingIterator.MovePrevious();
			if (characterCountingIterator.CharacterCount < charactersFromEndOfLastSegment)
			{
				IText text = characterCountingIterator.CurrentLocation.ItemAtLocation as IText;
				if (text != null)
				{
					return new TextLocation(characterCountingIterator.CurrentLocation, charactersFromEndOfLastSegment - characterCountingIterator.CharacterCount);
				}
			}
			return new TextLocation(characterCountingIterator.CurrentLocation, 0);
		}

		private TextLocation FindLocationInSegment(ContentRestriction contentType, ParagraphUnitId paragraphUnitId, SegmentId segmentId, int charactersFromStartOfSegment)
		{
			if (!_paragraphUnits.TryGetValue(paragraphUnitId, out IParagraphUnit value))
			{
				return null;
			}
			IParagraph paragraph = null;
			ISegment segment = null;
			if (contentType == ContentRestriction.Target)
			{
				paragraph = value.Target;
				segment = value.GetTargetSegment(segmentId);
			}
			else
			{
				paragraph = value.Source;
				segment = value.GetSourceSegment(segmentId);
			}
			if (segment == null)
			{
				return FindLocationOutsideSegment(contentType, paragraphUnitId, null, charactersFromStartOfSegment);
			}
			Location location = new Location(paragraph, segment);
			location.MoveNext();
			CharacterCountingIterator characterCountingIterator = new CharacterCountingIterator(location);
			bool flag = false;
			while (characterCountingIterator.CharacterCount <= charactersFromStartOfSegment)
			{
				if (!characterCountingIterator.MoveNext())
				{
					flag = true;
					break;
				}
				if (characterCountingIterator.CurrentLocation.ItemAtLocation == null && characterCountingIterator.CurrentLocation.BottomLevel.Parent == segment)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return new TextLocation(characterCountingIterator.CurrentLocation, 0);
			}
			characterCountingIterator.MovePrevious();
			if (characterCountingIterator.CharacterCount < charactersFromStartOfSegment)
			{
				IText text = characterCountingIterator.CurrentLocation.ItemAtLocation as IText;
				if (text != null)
				{
					return new TextLocation(characterCountingIterator.CurrentLocation, charactersFromStartOfSegment - characterCountingIterator.CharacterCount);
				}
			}
			return new TextLocation(characterCountingIterator.CurrentLocation, 0);
		}

		public IMessageLocation FindLocation(LocationInfo info)
		{
			if (info == null)
			{
				return null;
			}
			TextLocation paragraphLocation = null;
			if (info.IsInSegment)
			{
				paragraphLocation = FindLocationInSegment(info.SourceOrTarget, info.ParagraphUnitId.Value, info.SegmentId.Value, info.CharactersAfterSegmentStart);
			}
			else if (info.ParagraphUnitId.HasValue)
			{
				paragraphLocation = FindLocationOutsideSegment(info.SourceOrTarget, info.ParagraphUnitId.Value, info.SegmentId, info.SegmentId.HasValue ? info.CharactersAfterSegment : info.CharactersAfterParagraphStart);
			}
			BilingualMessageLocation bilingualMessageLocation = new BilingualMessageLocation();
			bilingualMessageLocation.ParagraphLocation = paragraphLocation;
			bilingualMessageLocation.ParagrahUnitId = info.ParagraphUnitId;
			if (info.ParagraphUnitId.HasValue)
			{
				bilingualMessageLocation.CharactersIntoParagraph = info.CharactersAfterParagraphStart;
			}
			bilingualMessageLocation.SegmentId = info.SegmentId;
			bilingualMessageLocation.SourceOrTarget = info.SourceOrTarget;
			return bilingualMessageLocation;
		}
	}
}
