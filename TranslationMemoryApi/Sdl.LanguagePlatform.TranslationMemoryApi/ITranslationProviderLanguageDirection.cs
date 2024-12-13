using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Exposes translation provider functionality for a specific language direction.
	/// </summary>
	public interface ITranslationProviderLanguageDirection
	{
		/// <summary>
		/// The translation provider to which this language direction belongs.
		/// </summary>
		ITranslationProvider TranslationProvider
		{
			get;
		}

		/// <summary>
		/// Gets the source language.
		/// </summary>
		CultureInfo SourceLanguage
		{
			get;
		}

		/// <summary>
		/// Gets the target language.
		/// </summary>
		CultureInfo TargetLanguage
		{
			get;
		}

		/// <summary>
		/// Gets a flag which indicates whether the translation provider supports 
		/// searches in the reversed language direction.
		/// </summary>
		bool CanReverseLanguageDirection
		{
			get;
		}

		/// <summary>
		/// Performs a segment search.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segment">The segment to search for.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.</returns>
		SearchResults SearchSegment(SearchSettings settings, Segment segment);

		/// <summary>
		/// Performs a search for an array of segments.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.</returns>
		SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments);

		/// <summary>
		/// Performs a search for an array of segments, specifying a mask which specifies which segments should actually be
		/// searched (only those for which the corresponding mask bit is <c>true</c> are searched). If the mask is <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" />. Passing a mask only makes sense in document search contexts (<see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchSettings.IsDocumentSearch" />
		/// set to <c>true</c>).
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <param name="mask">The array containing the segments to search for.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.</returns>
		SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask);

		/// <summary>
		/// Performs a text search.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segment">The text to search for.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.</returns>
		SearchResults SearchText(SearchSettings settings, string segment);

		/// <summary>
		/// Performs a translation unit search.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="translationUnit">The translation unit to search for.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.</returns>
		SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit);

		/// <summary>
		/// Performs a translation unit search for an array of translation units.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.</returns>
		SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits);

		/// <summary>
		/// Similar to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />, but allows passing a mask which specifies which TUs are actually searched. This is useful
		/// in document search contexts where some TUs are passed which should be used to establish a (text) context, but which should not be
		/// processed.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <param name="mask">A <c>bool</c> array which specifies which TUs are actually searched (mask[i] = <c>true</c>). If <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />.
		/// </param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.</returns>
		SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask);

		/// <summary>
		/// Adds a translation unit to the database. If the provider doesn't support adding/updating, the 
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </summary>
		/// <param name="translationUnit">The translation unit.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <returns>An <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> which represents the status of the operation (succeeded, ignored, etc).</returns>
		ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings);

		/// <summary>
		/// Adds an array of translation units to the database. If the provider doesn't support adding/updating, the 
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.</returns>
		ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings);

		/// <summary>
		/// Adds an array of translation units to the database. If hash codes of the previous translations are provided, 
		/// a found translation will be overwritten. If none is found, or the hash is 0 or the collection is <c>null</c>, 
		/// the operation behaves identical to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" />.
		/// <para>
		/// If the provider doesn't support adding/updating, the 
		/// implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> but should not throw an exception.
		/// </para>
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="previousTranslationHashes">If provided, a corresponding array of a the hash code of a previous translation.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult" /> objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.</returns>
		ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings);

		/// <summary>
		/// Adds an array of translation units to the database, but will only add those
		/// for which the corresponding mask field is <c>true</c>. If the provider doesn't support adding/updating, the 
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <param name="mask">A boolean array with the same cardinality as the TU array, specifying which TUs to add.</param>
		/// <returns>An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.</returns>
		ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask);

		/// <summary>
		/// Adds an array of translation units to the database, but will only add those
		/// for which the corresponding mask field is true. If the previous translation hashes are provided,
		/// existing translations will be updated if the target segment hash changed.
		/// <para>
		/// If the provider doesn't support adding/updating, the 
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// </para>
		/// </summary>
		/// <param name="translationUnits">An arrays of translation units to be added.</param>
		/// <param name="previousTranslationHashes">Corresponding hash codes of a previous translation (0 if unknown). The parameter may be null.</param>
		/// <param name="settings">The settings used for this operation.</param>
		/// <param name="mask">A boolean array with the same cardinality as the TU array, specifying which TUs to add.</param>
		/// <returns>An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
		/// status of each add operation for each particular translation unit with the same index within the array.</returns>
		ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask);

		/// <summary>
		/// Updates the properties and fields of an existing translation unit if the source and target segments are unchanged or
		/// adds a new translation unit otherwise. If the provider doesn't support adding/updating, the 
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// <para>The translation unit should be initialized in a previous call to the translation memory, so that the ID property is set to a 
		/// valid value.</para>
		/// </summary>
		/// <param name="translationUnit">The translation unit to be updated.</param>
		/// <returns>The result of the operation.</returns>
		ImportResult UpdateTranslationUnit(TranslationUnit translationUnit);

		/// <summary>
		/// Updates the properties and fields of an array of existing translation units if the source and target segments are unchanged or
		/// adds new translation units otherwise. If the provider doesn't support adding/updating, the 
		/// implementation should return a reasonable ImportResult but should not throw an exception.
		/// <para>The translation units should be initialized in previous calls to the translation memory, so that their ID properties
		/// are set to valid values.</para>
		/// </summary>
		/// <param name="translationUnits">The translation unit array to be updated.</param>
		/// <returns>An array of results which mirrors the translation unit array. It has the exact same size and contains the 
		/// results for each translation unit with the same index within the translation unit array.</returns>
		ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits);
	}
}
