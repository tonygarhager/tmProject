using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Exposes translation provider delete  TU functionality for a specific language direction.
	/// </summary>
	public interface ITranslationProviderLanguageDirectionWithDelete
	{
		/// <summary>
		/// Deletes the specified translation unit from the Translation Provider.
		/// </summary>
		/// <param name="translationUnit">The translation unit to delete.</param>
		/// <returns>true if the translation unit was deleted, false otherwise.</returns>
		bool DeleteTranslationUnit(TranslationUnit translationUnit);

		/// <summary>
		/// Deletes the translation units from the translation provider.
		/// </summary>
		/// <param name="translationUnits">A collection of the translation units to delete</param>
		/// <returns>The number of deleted translation units</returns>
		int DeleteTranslationUnits(TranslationUnit[] translationUnits);
	}
}
