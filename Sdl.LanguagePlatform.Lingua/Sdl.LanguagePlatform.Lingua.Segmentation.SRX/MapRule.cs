using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Segmentation.SRX
{
	public class MapRule
	{
		public string Name;

		private List<LanguageMap> _languageMaps;

		public List<LanguageMap> LanguageMaps => _languageMaps ?? (_languageMaps = new List<LanguageMap>());
	}
}
