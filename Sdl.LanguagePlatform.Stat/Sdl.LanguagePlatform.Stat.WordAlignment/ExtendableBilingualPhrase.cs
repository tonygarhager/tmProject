using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal class ExtendableBilingualPhrase : BilingualPhrase
	{
		public List<ExtensionPoint> ExtensionPoints;

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
