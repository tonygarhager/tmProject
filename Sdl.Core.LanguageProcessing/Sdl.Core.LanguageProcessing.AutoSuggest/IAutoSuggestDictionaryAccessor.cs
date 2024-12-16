using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.AutoSuggest
{
	public interface IAutoSuggestDictionaryAccessor : IDisposable
	{
		CultureInfo SourceCulture
		{
			get;
		}

		CultureInfo TargetCulture
		{
			get;
		}

		IList<Segment> ComputeSuggestions(Segment sourceText);

		IList<Segment> ComputeSuggestions(string sourceText);

		PhraseMappingPairs GetMappings(Segment sourceText);

		PhraseMappingPairs GetMappings(string sourceText);

		PhraseMappingPairs GetCommonPhrases(string sourceText, string targetText, out int droppedPhrasePairs);

		PhraseMappingPairs GetCommonPhrases(Segment sourceSegment, Segment targetSegment, out int droppedPhrasePairs);

		double GetCoverage(string sourceText, string targetText);

		double GetCoverage(Segment sourceSegment, Segment targetSegment);
	}
}
