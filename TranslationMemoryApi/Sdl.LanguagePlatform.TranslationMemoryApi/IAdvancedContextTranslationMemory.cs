using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Provides information about TM support for advanced context matching functionality
	/// </summary>
	public interface IAdvancedContextTranslationMemory
	{
		/// <summary>
		/// Returns true if the TM is using legacy segment hashing (and can therefore consume legacy context information in TMX without conversion)
		/// </summary>
		bool UsesLegacyHashes
		{
			get;
			set;
		}

		/// <summary>
		///  Returns true if the TM was created with support for ID-based context matching
		/// </summary>
		bool UsesIdContextMatching
		{
			get;
		}

		/// <summary>
		/// Returns the type of <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.IAdvancedContextTranslationMemory.TextContextMatchType" /> specified when the TM was created
		/// </summary>
		TextContextMatchType TextContextMatchType
		{
			get;
		}
	}
}
