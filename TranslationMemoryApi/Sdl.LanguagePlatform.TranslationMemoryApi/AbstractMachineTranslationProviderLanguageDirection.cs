using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Implements an abstract base class for machine translation providers, and provides
	/// overridable default implementations for the most common properties and methods
	/// of <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection" />.
	/// </summary>
	public abstract class AbstractMachineTranslationProviderLanguageDirection : ISubsegmentTranslationMemoryLanguageDirection, ITranslationProviderLanguageDirection
	{
		private AbstractMachineTranslationProvider _TranslationProvider;

		private LanguagePair _languageDirection;

		/// <summary>
		/// The translation provider to which this language direction belongs.
		/// </summary>
		/// <value></value>
		public ITranslationProvider TranslationProvider => _TranslationProvider;

		/// <summary>
		/// Gets the current language direction of this instance.
		/// </summary>
		public LanguagePair LanguageDirection => _languageDirection;

		/// <summary>
		/// Gets the source language.
		/// </summary>
		public CultureInfo SourceLanguage => LanguageDirection.SourceCulture;

		/// <summary>
		/// Gets the target language.
		/// </summary>
		/// <value></value>
		public CultureInfo TargetLanguage => LanguageDirection.TargetCulture;

		/// <summary>
		/// Gets the source language code.
		/// </summary>
		public string SourceLanguageCode => LanguageDirection.SourceCultureName;

		/// <summary>
		/// Gets the target language code.
		/// </summary>
		/// <value></value>
		public string TargetLanguageCode => LanguageDirection.TargetCultureName;

		/// <summary>
		/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.CanReverseLanguageDirection" />. The default implementation returns <c>false</c>.
		/// </summary>
		public virtual bool CanReverseLanguageDirection => false;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="translationProvider">The machine translation provider to which this language direction belongs.</param>
		/// <param name="languageDirection">The language direction.</param>
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

		/// <summary>
		/// Obtains the translation for a single segment from the machine translation service and 
		/// returns it in the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchResult.TranslationProposal" /> field of 
		/// a <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" />. Obtaining the translation may be through a round-trip
		/// to the server.
		/// <para>This method is abstract and must be implemented by derived classes.</para>
		/// <para>To create a search result from a segment, you can use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProviderLanguageDirection.CreateSearchResult(Sdl.LanguagePlatform.Core.Segment,Sdl.LanguagePlatform.Core.Segment)" />.</para>
		/// </summary>
		/// <param name="segment">The segment to translate (must not be <c>null</c>)</param>
		/// <param name="settings">The search settings to apply</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> data structure if the translation was
		/// successful, and <c>null</c> otherwise.</returns>
		protected abstract SearchResults SearchSingleSegmentInternal(SearchSettings settings, Segment segment);

		/// <summary>
		/// Obtains the translation for a set of segments from the machine translation service and 
		/// returns it in the <see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchResult.TranslationProposal" /> field of 
		/// a <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> collection. Obtaining the translation should, but does
		/// not have to, result in only a single server round-trip.
		/// <para>This method is abstract and must be implemented by derived classes.</para>
		/// <para>To create a search result from a segment, you can use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProviderLanguageDirection.CreateSearchResult(Sdl.LanguagePlatform.Core.Segment,Sdl.LanguagePlatform.Core.Segment)" />.</para>
		/// </summary>
		/// <param name="segments">The segments to translate (must not be <c>null</c>)</param>
		/// <param name="settings">The search settings to apply</param>
		/// <returns>A collection of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> data structures if the translation was
		/// successful, and <c>null</c> otherwise.</returns>
		protected abstract IList<SearchResults> SearchMultipleSegmentsInternal(SearchSettings settings, IList<Segment> segments);

		/// <summary>
		/// Creates a result from the given source and translation segments.
		/// </summary>
		/// <param name="searchSegment">The source segment for a which the translation was requested.</param>
		/// <param name="translation">The translated segment.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> object representing the result of the translation.</returns>
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

		/// <summary>
		/// Creates search results for the specified list of source segments and their translations.
		/// </summary>
		/// <param name="searchSegments">The source segments that have been translated.</param>
		/// <param name="translations">The translations for the search segments.</param>
		/// <returns>A list of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults" /> objects representing the results of the translation.</returns>
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

		/// <summary>
		/// Creates a search result from a pair of source and target language segments.
		/// </summary>
		/// <param name="searchSegment">The search (source) segment</param>
		/// <param name="translation">The result (target) segment</param>
		/// <returns>A new search result, which contains a duplicate of the source segment,
		/// and the translation segment. The origin is set to <see cref="F:Sdl.LanguagePlatform.TranslationMemory.TranslationUnitOrigin.MachineTranslation" />.</returns>
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

		/// <summary>
		/// Verifies that the search settings are correct, and throws an exception if not. The
		/// default implementation will check on <c>null</c>, and whether the search mode
		/// is consistent with an MT provider.
		/// </summary>
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

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegment(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment)" />. The default implementation 
		/// validates the settings and then call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProviderLanguageDirection.SearchSingleSegmentInternal(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment)" /> to
		/// obtain the result.
		/// </summary>
		public virtual SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			CheckSettings(settings);
			return SearchSingleSegmentInternal(settings, segment);
		}

		/// <summary>
		/// A convenience method which returns a single translation for the input segment, using
		/// default settings.
		/// </summary>
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

		/// <summary>
		/// A convenience method which returns a single translation for the input string, using
		/// default settings.
		/// </summary>
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

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" />. The default implementation 
		/// validates the settings and then call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProviderLanguageDirection.SearchMultipleSegmentsInternal(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,System.Collections.Generic.IList{Sdl.LanguagePlatform.Core.Segment})" /> to
		/// obtain the result.
		/// </summary>
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

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegmentsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[],System.Boolean[])" />. The default implementation 
		/// collects the non-masked segments and calls <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProviderLanguageDirection.SearchMultipleSegmentsInternal(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,System.Collections.Generic.IList{Sdl.LanguagePlatform.Core.Segment})" /> to
		/// obtain the result.
		/// </summary>
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

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchText(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,System.String)" />. The default implementation 
		/// creates a new segment from the provided <paramref name="segment" /> and calls
		/// <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractMachineTranslationProviderLanguageDirection.SearchSingleSegmentInternal(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment)" /> to obtain the result.
		/// </summary>
		public virtual SearchResults SearchText(SearchSettings settings, string segment)
		{
			Segment segment2 = new Segment(LanguageDirection.SourceCulture);
			segment2.Add(segment);
			return SearchSegment(settings, segment2);
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)" />. The default implementation 
		/// returns <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegment(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment)" />, called for the source segment of <paramref name="translationUnit" />.
		/// <para>Note that a result is computed and returned although <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSearchForTranslationUnits" /> is
		/// <c>false</c> by default.</para>
		/// </summary>
		public virtual SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			if (translationUnit == null)
			{
				return null;
			}
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />. The default implementation
		/// collects the source segment of the provided translation units and then calls
		/// <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> to obtain the result.
		/// </summary>
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

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Boolean[])" />. The default implementation
		/// collects the source segments of the translation units and then calls
		/// <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegmentsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[],System.Boolean[])" /> to obtain the result.
		/// </summary>
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

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit,Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddOrUpdateTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Int32[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings,System.Boolean[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddOrUpdateTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Int32[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings,System.Boolean[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.UpdateTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.UpdateTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SubsegmentSearchSegment(Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.Core.Segment)" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual SubsegmentSearchResultsCollection SubsegmentSearchSegment(SubsegmentSearchSettings settings, Segment segment)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SubsegmentSearchSegments(Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.Core.Segment[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings settings, Segment[] segments)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SupportedSubsegmentMatchTypes" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual List<SubsegmentMatchType> SupportedSubsegmentMatchTypes()
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchSegment(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.Core.Segment)" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual SegmentAndSubsegmentSearchResults SearchSegment(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment segment)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.Core.Segment[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual SegmentAndSubsegmentSearchResults[] SearchSegments(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchSegmentsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.Core.Segment[],System.Boolean[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual SegmentAndSubsegmentSearchResults[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, Segment[] segments, bool[] mask)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual SegmentAndSubsegmentSearchResults SearchTranslationUnit(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit translationUnit)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
		public virtual SegmentAndSubsegmentSearchResults[] SearchTranslationUnits(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition condition, TranslationUnit[] translationUnits)
		{
			return null;
		}

		/// <summary>
		/// See <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SearchTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchCondition,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Boolean[])" />. The default implementation
		/// simply returns <c>null</c>.
		/// </summary>
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
