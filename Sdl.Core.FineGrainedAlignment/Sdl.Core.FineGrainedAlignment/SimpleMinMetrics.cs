using Sdl.LanguagePlatform.Stat.WordAlignment;
using System;

namespace Sdl.Core.FineGrainedAlignment
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
