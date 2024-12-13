namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a credential for a translation provider that can be stored in a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderCredentialStore" />.
	/// </summary>
	public sealed class TranslationProviderCredential
	{
		/// <summary>
		/// Credential property represents the credential.
		/// </summary>
		public string Credential
		{
			get;
			private set;
		}

		/// <summary>
		/// Persist property represents whether the credential will be persisted.
		/// </summary>
		public bool Persist
		{
			get;
			private set;
		}

		/// <summary>
		/// Constructor that takes the given credential and whether to persist.
		/// </summary>
		/// <param name="credential">credential</param>
		/// <param name="persist">persist</param>
		public TranslationProviderCredential(string credential, bool persist)
		{
			Credential = credential;
			Persist = persist;
		}
	}
}
