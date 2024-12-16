using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationProviderFactory
	{
		bool SupportsTranslationProviderUri(Uri translationProviderUri);

		ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore);

		TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState);
	}
}
