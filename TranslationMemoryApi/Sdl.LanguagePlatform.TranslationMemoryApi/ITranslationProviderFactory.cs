using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This interface defines a Translation Provider Factory class that can be used as a plug-in based 
	/// on the Sdl.Core.PluginFramework Extensions API
	/// </summary>
	/// <remarks>
	/// <para>Implementations of this class should be marked up with a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderFactoryAttribute" />
	/// extension attribute in order for the factory to be available.</para>
	/// </remarks>
	public interface ITranslationProviderFactory
	{
		/// <summary>
		/// Returns true if this factory supports the specified URI.
		/// </summary>
		/// <param name="translationProviderUri">The Uri.</param>
		/// <returns>True if this factory supports the specified URI.</returns>
		bool SupportsTranslationProviderUri(Uri translationProviderUri);

		/// <summary>
		/// Creates an instance of the translation provider defined by the specified URI.
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
		ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore);

		/// <summary>
		/// Gets general information for the specified translation provider.
		/// </summary>
		/// <param name="translationProviderUri">A translation provider URI, representing the translation provider.</param>
		/// <param name="translationProviderState">Optional translation provider state information, which can be used to determine 
		/// certain aspects of the general information.</param>
		/// <remarks>Note that this method can potentially be called very frequently so it is not advisable to instantiate the
		/// translation provider within its implementation.</remarks>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderInfo" /> object, containing general information that allows
		/// an application to query the translation provider without having to instantiate it.</returns>
		TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState);
	}
}
