using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Segmentation.SRX
{
	public class LanguageRule
	{
		public string Name;

		private List<Rule> _rules;

		public List<Rule> Rules => _rules ?? (_rules = new List<Rule>());
	}
}
