using Sdl.LanguagePlatform.Stat.WordAlignment;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class ExtendableBilingualPhrase : BilingualPhrase
	{
		public List<ExtensionPoint> ExtensionPoints;

		public ExtendableBilingualPhrase(BilingualPhrase other)
			: base(other)
		{
		}

		public ExtendableBilingualPhrase(int fromSrc, int intoSrc, int fromTrg, int intoTrg, double association)
			: base(fromSrc, intoSrc, fromTrg, intoTrg, association)
		{
		}

		public ExtendableBilingualPhrase(int srcPosition, int trgPosition, double association)
			: base(srcPosition, trgPosition, association)
		{
		}

		public ExtendableBilingualPhrase(BilingualPhrase first, BilingualPhrase second, double jointAssociation)
			: base(first, second, jointAssociation)
		{
		}

		public void ClearExtensionPoints()
		{
			ExtensionPoints = null;
		}
	}
}
