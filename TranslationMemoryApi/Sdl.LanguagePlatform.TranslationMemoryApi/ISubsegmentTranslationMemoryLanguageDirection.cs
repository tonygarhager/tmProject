using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Exposes translation provider subsegment functionality for a specific language direction.
	/// </summary>
	public interface ISubsegmentTranslationMemoryLanguageDirection
	{
		/// <summary>
		/// Searches the TM for subsegment matches for a given segment
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segment">The segment for which subsegment matches should be sought, aka 'query segment'</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchResultsCollection" /> object containing any subsegment matches found.</returns>
		/// <remarks>See <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings" /> remarks for further information.
		/// </remarks>
		SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment);

		/// <summary>
		/// Searches the TM for subsegment matches for an array of segments.
		/// </summary>
		/// <param name="settings">The settings that define the search parameters.</param>
		/// <param name="segments">The array containing the segments for which subsegment matches should be sought</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchResultsCollection" /> object containing any subsegment matches found.</returns>
		/// <remarks>See <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings" /> remarks for further information.
		/// </remarks>
		SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments);

		/// <summary>
		/// Reports the subsegment match types supported by this TM.
		/// </summary>
		/// <returns></returns>
		List<SubsegmentMatchType> SupportedSubsegmentMatchTypes();

		/// <summary>
		/// Performs a segment-level search, and optionally a subsegment search
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="segment">The segment to search for.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object containing the results or an empty object if no results were found.</returns>
		SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment);

		/// <summary>
		/// Performs a segment-level search, and optionally a subsegment search, for an array of segments.
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.</returns>
		SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments);

		/// <summary>
		/// Performs a segment-level search, and optionally a subsegment search, for an array of segments, specifying a mask which specifies which segments should actually be
		/// searched (only those for which the corresponding mask bit is <c>true</c> are searched). If the mask is <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.Core.Segment[])" />. Passing a mask only makes sense in document search contexts (<see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchSettings.IsDocumentSearch" />
		/// set to <c>true</c>).
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="segments">The array containing the segments to search for.</param>
		/// <param name="mask">The array containing the segments to search for.</param>
		/// <returns>An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects, which mirrors the segments array. It has the exact same size and contains the
		/// search results for each segment with the same index within the segments array.</returns>
		SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask);

		/// <summary>
		/// Performs a translation unit search, and optionally a subsegment search on the source segment
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="translationUnit">The translation unit to search for.</param>
		/// <returns>An object containing the results or an empty object if no results were found.</returns>
		SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit);

		/// <summary>
		/// Performs a translation unit search, and optionally a subsegment search on the source segment, for an array of translation units.
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <returns>An array of objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.</returns>
		SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits);

		/// <summary>
		/// Similar to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />, but allows passing a mask which specifies which TUs are actually searched. This is useful
		/// in document search contexts where some TUs are passed which should be used to establish a (text) context, but which should not be
		/// processed.
		/// </summary>
		/// <param name="settings">The settings that define the segment-level search parameters.</param>
		/// <param name="subsegmentSettings">The settings that define the subsegment search parameters, or null if a subsegment search should not be performed.</param>
		/// <param name="condition">If <paramref name="subsegmentSettings" /> is not null, specifies the conditions under which a subsegment search will be performed.</param>
		/// <param name="translationUnits">The array containing the translation units to search for.</param>
		/// <param name="mask">A <c>bool</c> array which specifies which TUs are actually searched (mask[i] = <c>true</c>). If <c>null</c>, the method
		/// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />.
		/// </param>
		/// <returns>An array of objects, which mirrors the translation unit array. It has the exact same size and contains
		/// the search results for each translation unit with the same index within the translation unit array.</returns>
		SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits, bool[] mask);
	}
}
