namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal class SimpleAverageMetrics : ICombineMetrics
	{
		public double GetCombinedGain(BilingualPhrase bp, double extensionPointAssociation)
		{
			return (bp.Association + extensionPointAssociation) / 2.0;
		}

		public double GetCombinedGain(BilingualPhrase bp, BilingualPhrase otherPhrase)
		{
			return (bp.Association + otherPhrase.Association) / 2.0;
		}
	}
}
