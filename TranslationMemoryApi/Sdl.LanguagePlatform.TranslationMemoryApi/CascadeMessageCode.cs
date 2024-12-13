namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// CascadeMessageCode enumeration represents the cascade message codes.
	/// </summary>
	public enum CascadeMessageCode
	{
		/// <summary>
		/// The translation provider threw an exception.
		/// </summary>
		TranslationProviderThrewException,
		/// <summary>
		/// The cascade method requires that the translation provider supports concordance search but it does not.
		/// </summary>
		TranslationProviderDoesNotSupportConcordanceSearch,
		/// <summary>
		/// The cascade method requires that the translation provider supports document search but it does not.
		/// </summary>
		TranslationProviderDoesNotSupportDocumentSearch,
		/// <summary>
		/// The cascade method requires that the translation provider supports translation but it does not.
		/// </summary>
		TranslationProviderDoesNotSupportTranslation,
		/// <summary>
		/// The cascade method requires that the translation provider supports source concordance search but it does not.
		/// </summary>
		TranslationProviderDoesNotSupportSourceConcordanceSearch,
		/// <summary>
		/// The cascade method requires that the translation provider supports target concordance search but it does not.
		/// </summary>
		TranslationProviderDoesNotSupportTargetConcordanceSearch,
		/// <summary>
		/// The cascade method requires that the translation provider supports searching for translation units but it does not.
		/// </summary>
		TranslationProviderDoesNotSupportSearchForTranslationUnits,
		/// <summary>
		/// The cascade method requires the the translation provider is write-able but it is read-only.
		/// </summary>
		TranslationProviderIsReadOnly,
		/// <summary>
		/// The cascade method requires that the translation providers supports update but it does not.
		/// </summary>
		TranslationProviderDoesNotSupportUpdate,
		/// <summary>
		/// The cascade method requires that the translation providers needs upgrade
		/// </summary>
		TranslationProviderNeedsUpgrade
	}
}
