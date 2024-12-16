using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class CascadeEntry
	{
		public ITranslationProviderLanguageDirection TranslationProviderLanguageDirection
		{
			get;
			private set;
		}

		public int Penalty
		{
			get;
			private set;
		}

		public CascadeEntry(ITranslationProviderLanguageDirection translationProviderLanguageDirection, int penalty)
		{
			TranslationProviderLanguageDirection = (translationProviderLanguageDirection ?? throw new ArgumentNullException("translationProviderLanguageDirection"));
			Penalty = penalty;
		}
	}
}
