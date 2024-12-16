using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationProviderSummaryFactory
	{
		ITranslationProvider GetTranslationProviderSummary(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore);
	}
}
