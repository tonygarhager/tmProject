namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Enumerates the ways how a translation provider can compute a translation. See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.TranslationMethod" />.
	/// </summary>
	public enum TranslationMethod
	{
		/// <summary>
		/// The method of translation is unknown.
		/// </summary>
		Unknown,
		/// <summary>
		/// A TM is used to obtain the translation.
		/// </summary>
		TranslationMemory,
		/// <summary>
		/// A machine translation engine is used to obtain the translation.
		/// </summary>
		MachineTranslation,
		/// <summary>
		/// A pseudo translation engine is used to obtain the translation. The translations
		/// typically are meaningless and are not supposed to be used or persistently stored.
		/// </summary>
		PseudoTranslation,
		/// <summary>
		/// A mixed model (different methods) are used to obtain the translation.
		/// </summary>
		Mixed,
		/// <summary>
		/// A pseudo translation generated by copying the input (source) segment, potentially 
		/// applying auto-localizations.
		/// </summary>
		CopySource,
		/// <summary>
		/// An unspecified method is used to obtain the translation.
		/// </summary>
		Other
	}
}