using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters
{
	public class LinguaToBcmMap
	{
		public List<KeyValuePair<Tag, string>> TagAssociations;

		public List<KeyValuePair<Text, string>> TextAssociations;

		public LinguaToBcmMap()
		{
			TagAssociations = new List<KeyValuePair<Tag, string>>();
			TextAssociations = new List<KeyValuePair<Text, string>>();
		}
	}
}
