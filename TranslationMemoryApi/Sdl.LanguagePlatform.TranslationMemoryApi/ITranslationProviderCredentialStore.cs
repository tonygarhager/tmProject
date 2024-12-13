using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This interface represents a storage mechanism for translation provider credentials.
	/// The host application will provide an implementation of this object, which will be passed on to
	/// the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderFactory" /> and <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderWinFormsUI" />
	/// components so they can retrieve and add credentials from and to the store. It is up to the store implementation 
	/// whether it for instance persists its contents or not.
	/// </summary>
	public interface ITranslationProviderCredentialStore
	{
		/// <summary>
		/// Gets a credential for the given URI.
		/// </summary>
		/// <param name="uri">The URI for which to retrieve the credential. Note that this does not necessarily have to be 
		/// the translation provider URI itself. It can be a more general URI, for instance identifying the server on which the translation provider lives.
		/// This allows for reusing credentials for different translation providers hosted on the same server.</param>
		/// <returns>A serialized credential; or null if no credential for the given URI is available.</returns>
		TranslationProviderCredential GetCredential(Uri uri);

		/// <summary>
		/// Adds or updates the credential for the specified URI.
		/// </summary>
		/// <param name="uri">The URI for which to add or update the credential. Note that this does not necessarily have to be 
		/// the translation provider URI itself. It can be a more general URI, for instance identifying the server on which the translation provider lives.
		/// This allows for reusing credentials for different translation providers hosted on the same server.</param>
		/// <param name="credential">A serialized credential.</param>
		void AddCredential(Uri uri, TranslationProviderCredential credential);

		/// <summary>
		/// Removes the credential for the specified URI.
		/// </summary>
		/// <param name="uri">The URI for which to remove the credential. Note that this does not necessarily have to be 
		/// the translation provider URI itself. It can be a more general URI, for instance identifying the server on which the translation provider lives.
		/// This allows for reusing credentials for different translation providers hosted on the same server.</param>
		void RemoveCredential(Uri uri);

		/// <summary>
		/// Removes all credentials stored in this store.
		/// </summary>
		void Clear();
	}
}
