using Sdl.LanguagePlatform.Stat.WordAlignment;

namespace Sdl.Core.FineGrainedAlignment
{
	internal interface ICombineMetrics
	{
		double GetCombinedGain(BilingualPhrase bp, double extensionPointAssociation);

		double GetCombinedGain(BilingualPhrase bp, BilingualPhrase otherPhrase);
	}
}
