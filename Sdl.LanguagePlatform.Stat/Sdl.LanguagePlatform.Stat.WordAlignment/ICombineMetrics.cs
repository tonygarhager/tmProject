namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal interface ICombineMetrics
	{
		double GetCombinedGain(BilingualPhrase bp, double extensionPointAssociation);

		double GetCombinedGain(BilingualPhrase bp, BilingualPhrase otherPhrase);
	}
}
