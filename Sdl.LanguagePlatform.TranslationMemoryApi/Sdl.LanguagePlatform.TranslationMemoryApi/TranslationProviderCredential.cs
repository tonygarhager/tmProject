namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public sealed class TranslationProviderCredential
	{
		public string Credential
		{
			get;
			private set;
		}

		public bool Persist
		{
			get;
			private set;
		}

		public TranslationProviderCredential(string credential, bool persist)
		{
			Credential = credential;
			Persist = persist;
		}
	}
}
