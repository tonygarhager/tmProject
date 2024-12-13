namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represenst a language direction and its associated settings and data within a legacy translation memory (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyTranslationMemory" />).
	/// </summary>
	public interface ILegacyLanguageDirectionData
	{
		/// <summary>
		/// Gets the source language.
		/// </summary>
		LegacyLanguage SourceLanguage
		{
			get;
		}

		/// <summary>
		/// Gets the target language.
		/// </summary>
		LegacyLanguage TargetLanguage
		{
			get;
		}

		/// <summary>
		/// Gets the number of translation units exist with this language direction. This is <code>-1</code>
		/// if the count is not known (or cannot be easily obtained).
		/// </summary>
		int TranslationUnitCount
		{
			get;
		}

		/// <summary>
		/// Gets a number of sets of language resources which could be valid for use in the output translation memory.
		/// It is a user choice as to which one will be used (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITranslationMemorySetupOptions" />).
		/// </summary>
		ILegacyLanguageResources[] AvailableLanguageResources
		{
			get;
		}

		/// <summary>
		/// Gets the language resources that the system suggest to use. This can be one of the available
		/// language resources (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyLanguageDirectionData.AvailableLanguageResources" />) or null, which means the default language resources are suggested.
		/// </summary>
		ILegacyLanguageResources SuggestedLanguageResources
		{
			get;
		}
	}
}
