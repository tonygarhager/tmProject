using Sdl.LanguagePlatform.Core;
using System;
using System.Windows.Forms;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationProviderWinFormsUI
	{
		string TypeName
		{
			get;
		}

		string TypeDescription
		{
			get;
		}

		bool SupportsEditing
		{
			get;
		}

		bool SupportsTranslationProviderUri(Uri translationProviderUri);

		ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore);

		bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore);

		TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState);

		bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore);
	}
}
