using Sdl.LanguagePlatform.Core;
using System;
using System.Windows.Forms;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This interface should be implemented by translation provider implementers in order to provide user interface functionality in SDL Trados Studio
	/// specific to that type of translation provider. Implementation of this interface should be marked up with the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderWinFormsUiAttribute" />
	/// attribute for registration it with the plug-in manager.
	/// </summary>
	public interface ITranslationProviderWinFormsUI
	{
		/// <summary>
		/// Gets the type name of the factory; e.g. "Language Weaver Translation Provider"
		/// </summary>
		string TypeName
		{
			get;
		}

		/// <summary>
		/// Gets the type description of the factory; e.g. "A plug-in provider to use Language Weaver machine translation engines."
		/// </summary>
		string TypeDescription
		{
			get;
		}

		/// <summary>
		/// Determines whether this  supplied translation provider can be edited (i.e. whether any settings can be changed).
		/// </summary>
		/// <returns><c>true</c> if the provider's settings can be changed, and <c>false</c> otherwise.</returns>
		bool SupportsEditing
		{
			get;
		}

		/// <summary>
		/// Returns true if this component supports the specified translation provider URI.
		/// </summary>
		/// <param name="translationProviderUri">The uri.</param>
		/// <returns>True if this component supports the specified translation provider URI.</returns>
		bool SupportsTranslationProviderUri(Uri translationProviderUri);

		/// <summary>
		/// Displays a dialog to interactively browse for one or more translation providers. 
		/// </summary>
		/// <param name="owner">The window that will own the dialog</param>
		/// <param name="languagePairs">
		/// A collection of language pairs. If provided, the list of available translation providers will be filtered by these language directions.</param>
		/// <param name="credentialStore">A credential store object that can be used to retrieve credentials required. </param>
		/// <returns>
		/// A collection of translation providers selected by the user, or <c>null</c> if none were selected or available or the browse was cancelled.</returns>
		ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore);

		/// <summary>
		/// Displays a dialog to interactively change any of the translation provider settings.
		/// </summary>
		/// <param name="owner">The window that will own the dialog</param>
		/// <param name="translationProvider">A translation provider descriptor, representing the translation provider to edit.</param>
		/// <param name="languagePairs">
		/// A collection of language pairs. If provided, the list of available translation providers will be filtered by these language directions.</param>
		/// <param name="credentialStore">A credential store object that can be used to retrieve credentials required. </param>
		/// <returns>True if changes were made to the translation provider; false otherwise.</returns>
		/// <exception cref="T:System.InvalidOperationException">Thrown when calling this method while <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderWinFormsUI.SupportsEditing" /> return <code>false</code>.</exception>
		bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore);

		/// <summary>
		/// Gets display information for the specified translation provider.
		/// </summary>
		/// <param name="translationProviderUri">A translation provider URI, representing the translation provider.</param>
		/// <param name="translationProviderState">Optional translation provider state information, which can be used to determine 
		/// certain aspects of the display information.</param>
		/// <remarks>Note that this method can potentially be called very frequently so it is not advisable to instantiate the
		/// translation provider within its implementation.</remarks>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderDisplayInfo" /> object, containing display information that allows
		/// an application to represent the translation provider without having to instantiate it.</returns>
		TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState);

		/// <summary>
		/// Gets the credentials from the user and puts these credentials in the credential store.
		/// </summary>
		/// <param name="owner">The window that will own the dialog</param>
		/// <param name="translationProviderUri">translation provider uri</param>
		/// <param name="translationProviderState">translation provider state</param>
		/// <param name="credentialStore">credential store</param>
		/// <returns>true if the user provided credentials or false if the user canceled</returns>
		bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore);
	}
}
