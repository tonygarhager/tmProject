using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class SubsegmentSearchSettings : SearchSettings
	{
		public int MinTM_TDBFeatures;

		public int MinTM_TDBSignificantFeatures;

		public int MinFeatures;

		public int MinSignificantFeatures;

		public HashSet<SubsegmentMatchType> SubsegmentMatchTypes = new HashSet<SubsegmentMatchType>
		{
			SubsegmentMatchType.DTA,
			SubsegmentMatchType.TM_TDB
		};
	}
}
