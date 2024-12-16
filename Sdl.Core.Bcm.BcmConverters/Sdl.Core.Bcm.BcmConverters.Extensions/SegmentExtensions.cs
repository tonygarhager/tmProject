using Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Operations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Bcm.BcmConverters.Extensions
{
	public static class SegmentExtensions
	{
		[Obsolete]
		public static Sdl.LanguagePlatform.Core.Segment ToLinguaSegment(this Sdl.Core.Bcm.BcmModel.Segment segment, CultureInfo culture)
		{
			List<SegmentElement> list = new List<SegmentElement>();
			BcmToSegmentElementVisitor bcmToSegmentElementVisitor = new BcmToSegmentElementVisitor(list, segment.ParentParagraphUnit.ParentFile.Skeleton);
			bcmToSegmentElementVisitor.VisitSegment(segment);
			return new Sdl.LanguagePlatform.Core.Segment
			{
				Culture = culture,
				Elements = list
			};
		}

		public static Sdl.LanguagePlatform.Core.Segment ToLinguaSegment(this Sdl.Core.Bcm.BcmModel.Segment segment, CultureInfo culture, FileSkeleton fileSkeleton)
		{
			segment?.NormalizeTextItems();
			List<SegmentElement> list = new List<SegmentElement>();
			BcmToSegmentElementVisitor bcmToSegmentElementVisitor = new BcmToSegmentElementVisitor(list, fileSkeleton);
			bcmToSegmentElementVisitor.VisitSegment(segment);
			return new Sdl.LanguagePlatform.Core.Segment
			{
				Culture = culture,
				Elements = list
			};
		}
	}
}
