using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	public class FeatureToRangeMapping
	{
		public int Feature
		{
			get;
			set;
		}

		public SegmentRange Range
		{
			get;
			set;
		}

		public FeatureToRangeMapping(int feature, SegmentRange range)
		{
			Feature = feature;
			Range = range;
		}
	}
}
