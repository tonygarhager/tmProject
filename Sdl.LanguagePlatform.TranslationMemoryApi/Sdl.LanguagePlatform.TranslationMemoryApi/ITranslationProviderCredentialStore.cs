using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationProviderCredentialStore
	{
		TranslationProviderCredential GetCredential(Uri uri);

		void AddCredential(Uri uri, TranslationProviderCredential credential);

		void RemoveCredential(Uri uri);

		void Clear();
	}
}
