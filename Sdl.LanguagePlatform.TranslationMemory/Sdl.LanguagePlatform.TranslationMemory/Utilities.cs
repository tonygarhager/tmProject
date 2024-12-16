using Sdl.LanguagePlatform.Core;
using System;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public static class Utilities
	{
		public static bool VerifyLanguageDirectionCompatibility(LanguagePair l1, LanguagePair l2)
		{
			if (l1 == null)
			{
				throw new ArgumentNullException("l1");
			}
			if (l2 == null)
			{
				throw new ArgumentNullException("l2");
			}
			if (l1.SourceCulture == null)
			{
				throw new ArgumentNullException("SourceCulture");
			}
			if (l2.SourceCulture == null)
			{
				throw new ArgumentNullException("SourceCulture");
			}
			bool flag = CultureInfoExtensions.AreCompatible(l1.SourceCulture, l2.SourceCulture);
			if (flag && l1.TargetCulture != null && l2.TargetCulture != null)
			{
				flag = CultureInfoExtensions.AreCompatible(l1.TargetCulture, l2.TargetCulture);
			}
			return flag;
		}
	}
}
