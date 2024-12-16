using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationProviderLanguageDirection
	{
		ITranslationProvider TranslationProvider
		{
			get;
		}

		CultureInfo SourceLanguage
		{
			get;
		}

		CultureInfo TargetLanguage
		{
			get;
		}

		bool CanReverseLanguageDirection
		{
			get;
		}

		SearchResults SearchSegment(SearchSettings settings, Segment segment);

		SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments);

		SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask);

		SearchResults SearchText(SearchSettings settings, string segment);

		SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit);

		SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits);

		SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask);

		ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings);

		ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings);

		ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings);

		ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask);

		ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask);

		ImportResult UpdateTranslationUnit(TranslationUnit translationUnit);

		ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits);
	}
}
