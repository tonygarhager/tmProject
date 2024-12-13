using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a cascade entry used in a cascade. A cascade entry represents
	/// a translation provider language direction and other information with regards to how it should 
	/// be treated within a cascade - e.g. what penalties should be applied during searches. 
	/// </summary>
	public class CascadeEntry
	{
		/// <summary>
		/// TranslationProviderLanguageDirection represents the translation provider language direction.
		/// </summary>
		public ITranslationProviderLanguageDirection TranslationProviderLanguageDirection
		{
			get;
			private set;
		}

		/// <summary>
		/// Penalty represents the penalty that should be applied to any search results from this cascade entry.
		/// </summary>
		public int Penalty
		{
			get;
			private set;
		}

		/// <summary>
		/// Constructor that takes the given translation provider language direction and penalty.
		/// </summary>
		/// <param name="translationProviderLanguageDirection">translation provider language direction</param>
		/// <param name="penalty">penalty</param>
		public CascadeEntry(ITranslationProviderLanguageDirection translationProviderLanguageDirection, int penalty)
		{
			TranslationProviderLanguageDirection = (translationProviderLanguageDirection ?? throw new ArgumentNullException("translationProviderLanguageDirection"));
			Penalty = penalty;
		}
	}
}
