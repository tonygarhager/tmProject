using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua;
using Sdl.Core.Bcm.BcmModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class ApplyTranslationFromTu
	{
		public void ApplyTranslation(SegmentPair segmentPair, TranslationUnit translationUnit)
		{
			ValidateArguments(segmentPair, translationUnit);
			Sdl.Core.Bcm.BcmModel.Segment source = segmentPair.Source;
			Sdl.Core.Bcm.BcmModel.Segment target = segmentPair.Target;
			target.Clear();
			SegmentElementVisitorWithTagMatching matcher = new SegmentElementVisitorWithTagMatching(source, target);
			translationUnit.TargetSegment.Elements.ForEach(delegate(SegmentElement element)
			{
				element?.AcceptSegmentElementVisitor(matcher);
			});
			source.CopyMetadataFromLinguaTu(translationUnit, includeUserNameSystemFields: false);
			target.CopyMetadataFromLinguaTu(translationUnit, includeUserNameSystemFields: false);
			target.SegmentNumber = source.SegmentNumber;
			target.AddMetadataFrom(source.Metadata);
		}

		private void ValidateArguments(SegmentPair segmentPair, TranslationUnit translationUnit)
		{
			if (segmentPair == null)
			{
				throw new ArgumentNullException("segmentPair");
			}
			if (segmentPair.Source == null)
			{
				throw new ArgumentException("Source segment should not be null.");
			}
			if (segmentPair.Target == null)
			{
				throw new ArgumentException("Target segment should not be null.");
			}
			if (translationUnit == null)
			{
				throw new ArgumentNullException("translationUnit");
			}
		}
	}
}
