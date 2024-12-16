using Sdl.LanguagePlatform.Core;

namespace Sdl.Core.LanguageProcessing.AutoSuggest
{
	public class PhraseMappingPair
	{
		public Segment Source
		{
			get;
			set;
		}

		public Segment Target
		{
			get;
			set;
		}

		public int Frequency
		{
			get;
			set;
		}

		public PhraseMappingPair()
		{
		}

		public PhraseMappingPair(Segment source, Segment target, int frequency)
		{
			Source = source;
			Target = target;
			Frequency = frequency;
		}
	}
}
