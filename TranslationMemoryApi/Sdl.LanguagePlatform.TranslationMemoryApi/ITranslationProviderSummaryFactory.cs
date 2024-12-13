using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This interface defines an extension of Translation Provider Factory class that can be used as a plug-in based 
	/// on the Sdl.Core.PluginFramework Extensions API
	/// </summary>
	/// <remarks>
	/// <para>Any class implementing this should first implement ITranslationProviderFactory.</para>
	/// </remarks>
	public interface ITranslationProviderSummaryFactory
	{
		/// <summary>
		/// Creates a summary instance of the translation provider defined by the specified URI. No template information will be provided.
		/// </summary>
		/// <param name="translationProviderUri">A URI that identifies the translation provider to create.</param>
		/// <param name="translationProviderState">Serialized state information that should be used for 
		/// configuring the translation provider. This is typically state information that was previously saved 
		/// by calling <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SerializeState" />.</param>
		/// <param name="credentialStore">A credential store object that can be used to retrieve credentials 
		/// required for this translation provider. </param>
		/// <returns>A new translation provider object, ready to be used.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="translationProviderUri" />, <paramref name="translationProviderState" /> or <paramref name="credentialStore" /> 
		/// is null.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown if <paramref name="translationProviderUri" /> is not supported by this factory.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown if <paramref name="translationProviderState" /> is invalid.</exception>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderAuthenticationException">Thrown when no appropriate credentials are available in the credential store.</exception>
		ITranslationProvider GetTranslationProviderSummary(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore);
	}
}
