using Sdl.Core.Bcm.BcmModel;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.Core.Bcm.BcmConverters.Common
{
	public class BcmTuConversionEventArgs : EventArgs
	{
		public SegmentPair SegmentPair
		{
			get;
			set;
		}

		public TranslationUnit TranslationUnit
		{
			get;
			set;
		}

		public BcmTuConversionEventArgs(SegmentPair segmentPair, TranslationUnit translationUnit)
		{
			SegmentPair = segmentPair;
			TranslationUnit = translationUnit;
		}
	}
}
