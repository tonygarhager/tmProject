using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[TranslationProviderFactory(Id = "FileBasedTranslationMemoryFactory", Name = "FileBasedTranslationMemoryFactory", Description = "FileBasedTranslationMemoryFactory")]
	internal class FileBasedTranslationMemoryFactory : ITranslationProviderFactory
	{
		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return string.Equals(translationProviderUri.Scheme, FileBasedTranslationMemory.GetFileBasedTranslationMemoryScheme(), StringComparison.OrdinalIgnoreCase);
		}

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			FileBasedTranslationMemory fileBasedTranslationMemory = new FileBasedTranslationMemory(translationProviderUri);
			if (fileBasedTranslationMemory.IsProtected)
			{
				TranslationProviderCredential credential = credentialStore.GetCredential(translationProviderUri);
				if (credential == null)
				{
					throw new TranslationProviderAuthenticationException();
				}
				fileBasedTranslationMemory.Unlock(credential.Credential);
			}
			return fileBasedTranslationMemory;
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				Name = FileBasedTranslationMemory.GetFileBasedTranslationMemoryName(translationProviderUri),
				TranslationMethod = TranslationMethod.TranslationMemory
			};
		}
	}
}
