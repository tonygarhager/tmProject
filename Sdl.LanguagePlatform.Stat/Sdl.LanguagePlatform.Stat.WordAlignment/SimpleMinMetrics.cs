using System;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal class SimpleMinMetrics : ICombineMetrics
	{
		public double GetCombinedGain(BilingualPhrase bp, double extensionPointAssociation)
		{
			return extensionPointAssociation;
		}

		public double GetCombinedGain(BilingualPhrase bp, BilingualPhrase otherPhrase)
		{
			return Math.Min(bp.Association, otherPhrase.Association);
		}
	}
}
