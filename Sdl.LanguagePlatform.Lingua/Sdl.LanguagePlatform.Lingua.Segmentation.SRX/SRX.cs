using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Segmentation.SRX
{
	public class SRX
	{
		public bool SegmentSubflows;

		private List<FormatHandle> _formatHandleRules;

		private List<LanguageRule> _languageRules;

		private List<MapRule> _mapRules;

		public List<FormatHandle> FormatHandleRules => _formatHandleRules ?? (_formatHandleRules = new List<FormatHandle>());

		public List<LanguageRule> LanguageRules => _languageRules ?? (_languageRules = new List<LanguageRule>());

		public List<MapRule> MapRules => _mapRules ?? (_mapRules = new List<MapRule>());
	}
}
