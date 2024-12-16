using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ISubsegmentTranslationMemoryLanguageDirection
	{
		SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment);

		SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments);

		List<SubsegmentMatchType> SupportedSubsegmentMatchTypes();

		SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment);

		SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments);

		SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask);

		SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit);

		SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits);

		SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits, bool[] mask);
	}
}
