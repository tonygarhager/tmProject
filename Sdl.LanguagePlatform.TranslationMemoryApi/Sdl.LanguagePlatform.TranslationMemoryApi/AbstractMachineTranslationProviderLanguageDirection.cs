using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public abstract class AbstractMachineTranslationProviderLanguageDirection : ISubsegmentTranslationMemoryLanguageDirection, ITranslationProviderLanguageDirection
	{
		private AbstractMachineTranslationProvider _TranslationProvider;

		private LanguagePair _languageDirection;

		public ITranslationProvider TranslationProvider => _TranslationProvider;

		public LanguagePair LanguageDirection => _languageDirection;

		public CultureInfo SourceLanguage => LanguageDirection.SourceCulture;

		public CultureInfo TargetLanguage => LanguageDirection.TargetCulture;

		public string SourceLanguageCode => LanguageDirection.SourceCultureName;

		public string TargetLanguageCode => LanguageDirection.TargetCultureName;

		public virtual bool CanReverseLanguageDirection => false;

		protected AbstractMachineTranslationProviderLanguageDirection(AbstractMachineTranslationProvider translationProvider, LanguagePair languageDirection)
		{
			if (translationProvider == null)
			{
				throw new ArgumentNullException("translationProvider");
			}
			if (languageDirection == null)
			{
				throw new ArgumentNullException("languageDirection");
			}
			_TranslationProvider = translationProvider;
			_languageDirection = languageDirection;
		}

		protected abstract SearchResults SearchSingleSegmentInternal(SearchSettings settings, Segment segment);

		protected abstract IList<SearchResults> SearchMultipleSegmentsInternal(SearchSettings settings, IList<Segment> segments);

		protected virtual SearchResults CreateSearchResultFromTranslation(Segment searchSegment, Segment translation)
		{
			if (translation == null)
			{
				return null;
			}
			SearchResults searchResults = new SearchResults();
			searchResults.SourceSegment = searchSegment.Duplicate();
			searchResults.Add(CreateSearchResult(searchSegment, translation));
			return searchResults;
		}

		protected virtual IList<SearchResults> CreateSearchResultsFromTranslations(IList<Segment> searchSegments, IList<Segment> translations)
		{
			if (searchSegments == null || translations == null)
			{
				return null;
			}
			List<SearchResults> list = new List<SearchResults>();
			for (int i = 0; i < translations.Count; i++)
			{
				SearchResults searchResults = new SearchResults();
				searchResults.SourceSegment = searchSegments[i].Duplicate();
				searchResults.Add(CreateSearchResult(searchSegments[i], translations[i]));
				list.Add(searchResults);
			}
			return list;
		}

		protected virtual SearchResult CreateSearchResult(Segment searchSegment, Segment translation)
		{
			TranslationUnit translationUnit = new TranslationUnit();
			translationUnit.SourceSegment = searchSegment.Duplicate();
			translationUnit.TargetSegment = translation;
			translationUnit.ResourceId = new PersistentObjectToken(translationUnit.GetHashCode(), Guid.Empty);
			translationUnit.Origin = TranslationUnitOrigin.MachineTranslation;
			SearchResult searchResult = new SearchResult(translationUnit);
			searchResult.ScoringResult = new ScoringResult();
			if (translationUnit.SystemFields == null)
			{
				translationUnit.SystemFields = new SystemFields();
			}
			translationUnit.SystemFields.CreationDate = DateTime.UtcNow;
			translationUnit.SystemFields.ChangeDate = translationUnit.SystemFields.CreationDate;
			searchResult.TranslationProposal = translationUnit.Duplicate();
			return searchResult;
		}

		protected virtual void CheckSettings(SearchSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			if (settings.Mode != 0 && settings.Mode != SearchMode.FullSearch && settings.Mode != SearchMode.NormalSearch)
			{
				throw new NotSupportedException("Search mode is not supported by this provider");
			}
		}

		public virtual SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			CheckSettings(settings);
			return SearchSingleSegmentInternal(settings, segment);
		}

		public virtual Segment Translate(Segment segment)
		{
			SearchSettings searchSettings = new SearchSettings();
			searchSettings.Mode = SearchMode.NormalSearch;
			SearchResults searchResults = SearchSegment(searchSettings, segment);
			if (searchResults == null || searchResults.Count == 0 || searchResults[0].MemoryTranslationUnit == null)
			{
				return null;
			}
			return searchResults[0].MemoryTranslationUnit.TargetSegment;
		}

		public virtual string Translate(string text)
		{
			Segment segment = new Segment(LanguageDirection.SourceCulture);
			segment.Add(text);
			SearchSettings searchSettings = new SearchSettings();
			searchSettings.Mode = SearchMode.NormalSearch;
			SearchResults searchResults = SearchSegment(searchSettings, segment);
			if (searchResults == null || searchResults.Count == 0 || searchResults[0].MemoryTranslationUnit == null)
			{
				return null;
			}
			return searchResults[0].MemoryTranslationUnit.TargetSegment.ToPlain();
		}

		public virtual SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			if (segments == null)
			{
				return null;
			}
			if (segments.Length == 1)
			{
				return new SearchResults[1]
				{
					SearchSegment(settings, segments[0])
				};
			}
			CheckSettings(settings);
			return SearchMultipleSegmentsInternal(settings, segments).ToArray();
		}

		public virtual SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			if (segments == null)
			{
				throw new ArgumentNullException("segments");
			}
			if (mask == null)
			{
				throw new ArgumentNullException("mask");
			}
			if (mask.Length != segments.Length)
			{
				throw new ArgumentException("The length of the mask parameter should match the number segments.", "mask");
			}
			List<int> list = new List<int>();
			List<Segment> list2 = new List<Segment>();
			SearchResults[] array = new SearchResults[segments.Length];
			for (int i = 0; i < segments.Length; i++)
			{
				if (segments[i] != null && (mask == null || mask[i]))
				{
					list.Add(i);
					list2.Add(segments[i]);
				}
			}
			IList<SearchResults> list3 = SearchMultipleSegmentsInternal(settings, list2);
			for (int j = 0; j < list3.Count; j++)
			{
				array[list[j]] = list3[j];
			}
			return array;
		}

		public virtual SearchResults SearchText(SearchSettings settings, string segment)
		{
			Segment segment2 = new Segment(LanguageDirection.SourceCulture);
			segment2.Add(segment);
			return SearchSegment(settings, segment2);
		}

		public virtual SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			if (translationUnit == null)
			{
				return null;
			}
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public virtual SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			if (translationUnits == null)
			{
				return null;
			}
			Segment[] array = new Segment[translationUnits.Length];
			for (int i = 0; i < translationUnits.Length; i++)
			{
				array[i] = translationUnits[i].SourceSegment;
			}
			return SearchSegments(settings, array);
		}

		public virtual SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			if (translationUnits == null)
			{
				throw new ArgumentNullException("translationUnits");
			}
			if (mask == null)
			{
				throw new ArgumentNullException("mask");
			}
			if (mask.Length != translationUnits.Length)
			{
				throw new ArgumentException("The length of the mask parameter should match the number translation units.", "mask");
			}
			Segment[] array = new Segment[translationUnits.Length];
			for (int i = 0; i < translationUnits.Length; i++)
			{
				if (translationUnits[i] != null && (mask == null || mask[i]))
				{
					array[i] = translationUnits[i].SourceSegment;
				}
			}
			return SearchSegmentsMasked(settings, array, mask);
		}

		public virtual ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			return null;
		}

		public virtual ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			return null;
		}

		public virtual ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			return null;
		}

		public virtual ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			return null;
		}

		public virtual ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			return null;
		}

		public virtual ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			return null;
		}

		public virtual ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			return null;
		}

		public virtual SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment)
		{
			return null;
		}

		public virtual SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments)
		{
			return null;
		}

		public virtual List<SubsegmentMatchType> SupportedSubsegmentMatchTypes()
		{
			return null;
		}

		public virtual SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment)
		{
			return null;
		}

		public virtual SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments)
		{
			return null;
		}

		public virtual SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask)
		{
			return null;
		}

		public virtual SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit)
		{
			return null;
		}

		public virtual SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits)
		{
			return null;
		}

		public virtual SegmentAndSubsegmentSearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits, bool[] mask)
		{
			SearchResults[] array = SearchTranslationUnitsMasked(settings, translationUnits, mask);
			SegmentAndSubsegmentSearchResults[] array2 = new SegmentAndSubsegmentSearchResults[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = ((array[i] == null) ? null : new SegmentAndSubsegmentSearchResults(array[i], null));
			}
			return array2;
		}
	}
}
