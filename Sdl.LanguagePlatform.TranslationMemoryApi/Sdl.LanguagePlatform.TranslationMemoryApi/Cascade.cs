using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class Cascade<T> where T : CascadeEntry
	{
		private interface ISearchMaskedCommand
		{
			bool[] Mask
			{
				get;
				set;
			}
		}

		private abstract class SearchCommand<R>
		{
			public virtual bool ShouldExecute(T cascadeEntry, SearchSettings searchSettings)
			{
				return GetUnsupportedCascadeMessages(cascadeEntry, searchSettings).Count == 0;
			}

			public virtual IList<CascadeMessage> GetUnsupportedCascadeMessages(T cascadeEntry, SearchSettings searchSettings)
			{
				List<CascadeMessage> list = new List<CascadeMessage>();
				ITranslationProvider translationProvider = cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider;
				if (!searchSettings.IsConcordanceSearch && !translationProvider.SupportsTranslation)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportTranslation));
				}
				if (searchSettings.IsConcordanceSearch && !translationProvider.SupportsConcordanceSearch)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportConcordanceSearch));
				}
				if (searchSettings.Mode == SearchMode.ConcordanceSearch && !translationProvider.SupportsSourceConcordanceSearch)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportSourceConcordanceSearch));
				}
				if (searchSettings.Mode == SearchMode.TargetConcordanceSearch && !translationProvider.SupportsTargetConcordanceSearch)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportTargetConcordanceSearch));
				}
				return list;
			}

			public abstract R Execute(T cascadeEntry, SearchSettings searchSettings);
		}

		private class SearchSegmentCommand : SearchCommand<SearchResults>
		{
			private readonly Segment _segment;

			public SearchSegmentCommand(Segment segment)
			{
				_segment = segment;
			}

			public override SearchResults Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchSegment(searchSettings, _segment);
			}
		}

		private class SearchTextCommand : SearchCommand<SearchResults>
		{
			private readonly string _segment;

			public SearchTextCommand(string segment)
			{
				_segment = segment;
			}

			public override SearchResults Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchText(searchSettings, _segment);
			}
		}

		private class SearchTranslationUnitCommand : SearchCommand<SearchResults>
		{
			private readonly TranslationUnit _translationUnit;

			public SearchTranslationUnitCommand(TranslationUnit translationUnit)
			{
				_translationUnit = translationUnit;
			}

			public override IList<CascadeMessage> GetUnsupportedCascadeMessages(T cascadeEntry, SearchSettings searchSettings)
			{
				List<CascadeMessage> list = new List<CascadeMessage>();
				if (!cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider.SupportsSearchForTranslationUnits)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportSearchForTranslationUnits));
				}
				list.AddRange(base.GetUnsupportedCascadeMessages(cascadeEntry, searchSettings));
				return list;
			}

			public override SearchResults Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchTranslationUnit(searchSettings, _translationUnit);
			}
		}

		private class SearchSegmentsMaskedCommand : SearchCommand<SearchResults[]>, ISearchMaskedCommand
		{
			private readonly Segment[] _segments;

			private bool[] _mask;

			public bool[] Mask
			{
				get
				{
					return _mask;
				}
				set
				{
					_mask = value;
				}
			}

			public SearchSegmentsMaskedCommand(Segment[] segments, bool[] mask)
			{
				_segments = segments;
				_mask = mask;
			}

			public override SearchResults[] Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchSegmentsMasked(searchSettings, _segments, _mask);
			}
		}

		private class SearchSegmentAndSubSegmentMaskedCommand : SearchCommand<SegmentAndSubsegmentSearchResults[]>, ISearchMaskedCommand
		{
			private readonly SubsegmentSearchSettings _subsegmentSearchSettings;

			private readonly SubsegmentSearchCondition _subsegmentSearchCondition;

			private readonly Segment[] _segments;

			private bool[] _mask;

			public bool[] Mask
			{
				get
				{
					return _mask;
				}
				set
				{
					_mask = value;
				}
			}

			public SearchSegmentAndSubSegmentMaskedCommand(Segment[] segments, bool[] mask, SubsegmentSearchSettings subsegmentSearchSettings, SubsegmentSearchCondition subsegmentSearchCondition)
			{
				_segments = segments;
				_mask = mask;
				_subsegmentSearchSettings = subsegmentSearchSettings;
				_subsegmentSearchCondition = subsegmentSearchCondition;
			}

			public override SegmentAndSubsegmentSearchResults[] Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				ISubsegmentTranslationMemoryLanguageDirection subsegmentTranslationMemoryLanguageDirection = cascadeEntry.TranslationProviderLanguageDirection as ISubsegmentTranslationMemoryLanguageDirection;
				if (subsegmentTranslationMemoryLanguageDirection != null)
				{
					return subsegmentTranslationMemoryLanguageDirection.SearchSegmentsMasked(searchSettings, _subsegmentSearchSettings, _subsegmentSearchCondition, _segments, _mask);
				}
				SearchResults[] results = cascadeEntry.TranslationProviderLanguageDirection.SearchSegmentsMasked(searchSettings, _segments, _mask);
				return TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}
		}

		private class SearchSegmentAndSubSegmentTranslationUnitsMaskedCommand : SearchCommand<SegmentAndSubsegmentSearchResults[]>, ISearchMaskedCommand
		{
			private readonly SubsegmentSearchSettings _subsegmentSearchSettings;

			private readonly SubsegmentSearchCondition _subsegmentSearchCondition;

			private readonly TranslationUnit[] _translationUnits;

			private bool[] _mask;

			public bool[] Mask
			{
				get
				{
					return _mask;
				}
				set
				{
					_mask = value;
				}
			}

			public SearchSegmentAndSubSegmentTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, bool[] mask, SubsegmentSearchSettings subsegmentSearchSettings, SubsegmentSearchCondition subsegmentSearchCondition)
			{
				_translationUnits = translationUnits;
				_mask = mask;
				_subsegmentSearchSettings = subsegmentSearchSettings;
				_subsegmentSearchCondition = subsegmentSearchCondition;
			}

			public override SegmentAndSubsegmentSearchResults[] Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				ISubsegmentTranslationMemoryLanguageDirection subsegmentTranslationMemoryLanguageDirection = cascadeEntry.TranslationProviderLanguageDirection as ISubsegmentTranslationMemoryLanguageDirection;
				if (subsegmentTranslationMemoryLanguageDirection != null)
				{
					return subsegmentTranslationMemoryLanguageDirection.SearchTranslationUnitsMasked(searchSettings, _subsegmentSearchSettings, _subsegmentSearchCondition, _translationUnits, _mask);
				}
				SearchResults[] results = cascadeEntry.TranslationProviderLanguageDirection.SearchTranslationUnitsMasked(searchSettings, _translationUnits, _mask);
				return TranslationMemorySearchResultConverters.ToSegmentAndSubsegmentSearchResults(results);
			}

			public override IList<CascadeMessage> GetUnsupportedCascadeMessages(T cascadeEntry, SearchSettings searchSettings)
			{
				List<CascadeMessage> list = new List<CascadeMessage>();
				if (!cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider.SupportsSearchForTranslationUnits)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportSearchForTranslationUnits));
				}
				list.AddRange(base.GetUnsupportedCascadeMessages(cascadeEntry, searchSettings));
				return list;
			}
		}

		private class SearchTranslationUnitsMaskedCommand : SearchCommand<SearchResults[]>, ISearchMaskedCommand
		{
			private readonly TranslationUnit[] _translationUnits;

			private bool[] _mask;

			public bool[] Mask
			{
				get
				{
					return _mask;
				}
				set
				{
					_mask = value;
				}
			}

			public SearchTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, bool[] mask)
			{
				_translationUnits = translationUnits;
				_mask = mask;
			}

			public override IList<CascadeMessage> GetUnsupportedCascadeMessages(T cascadeEntry, SearchSettings searchSettings)
			{
				List<CascadeMessage> list = new List<CascadeMessage>();
				if (!cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider.SupportsSearchForTranslationUnits)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportSearchForTranslationUnits));
				}
				list.AddRange(base.GetUnsupportedCascadeMessages(cascadeEntry, searchSettings));
				return list;
			}

			public override SearchResults[] Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchTranslationUnitsMasked(searchSettings, _translationUnits, _mask);
			}
		}

		private abstract class UpdateCommand<R>
		{
			public virtual bool ShouldExecute(T cascadeEntry, ImportSettings importSettings)
			{
				return GetUnsupportedCascadeMessages(cascadeEntry, importSettings).Count == 0;
			}

			public virtual IList<CascadeMessage> GetUnsupportedCascadeMessages(T cascadeEntry, ImportSettings importSettings)
			{
				List<CascadeMessage> list = new List<CascadeMessage>();
				if (cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider.IsReadOnly)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderIsReadOnly));
				}
				if (!cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider.SupportsUpdate)
				{
					list.Add(new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderDoesNotSupportUpdate));
				}
				return list;
			}

			public abstract R Execute(T cascadeEntry, ImportSettings importSettings);
		}

		private class AddTranslationUnitCommand : UpdateCommand<ImportResult>
		{
			private readonly TranslationUnit _translationUnit;

			public AddTranslationUnitCommand(TranslationUnit translationUnit)
			{
				_translationUnit = translationUnit;
			}

			public override ImportResult Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddTranslationUnit(_translationUnit, importSettings);
			}
		}

		private class UpdateTranslationUnitCommand : UpdateCommand<ImportResult>
		{
			private readonly TranslationUnit _translationUnit;

			public UpdateTranslationUnitCommand(TranslationUnit translationUnit)
			{
				_translationUnit = translationUnit;
			}

			public override ImportResult Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.UpdateTranslationUnit(_translationUnit);
			}
		}

		private class AddOrUpdateTranslationUnitsCommand : UpdateCommand<ImportResult[]>
		{
			private readonly TranslationUnit[] _translationUnits;

			private readonly int[] _previousTranslationHashes;

			public AddOrUpdateTranslationUnitsCommand(TranslationUnit[] translationUnits, int[] previousTranslationHashes)
			{
				_translationUnits = translationUnits;
				_previousTranslationHashes = previousTranslationHashes;
			}

			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddOrUpdateTranslationUnits(_translationUnits, _previousTranslationHashes, importSettings);
			}
		}

		private class AddOrUpdateTranslationUnitsMaskedCommand : UpdateCommand<ImportResult[]>, ISearchMaskedCommand
		{
			private readonly TranslationUnit[] _translationUnits;

			private readonly int[] _previousTranslationHashes;

			private bool[] _mask;

			public bool[] Mask
			{
				get
				{
					return _mask;
				}
				set
				{
					_mask = value;
				}
			}

			public AddOrUpdateTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, int[] previousTranslationHashes, bool[] mask)
			{
				_translationUnits = translationUnits;
				_previousTranslationHashes = previousTranslationHashes;
				_mask = mask;
			}

			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddOrUpdateTranslationUnitsMasked(_translationUnits, _previousTranslationHashes, importSettings, _mask);
			}
		}

		private class AddTranslationUnitsCommand : UpdateCommand<ImportResult[]>
		{
			private readonly TranslationUnit[] _translationUnits;

			public AddTranslationUnitsCommand(TranslationUnit[] translationUnits)
			{
				_translationUnits = translationUnits;
			}

			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddTranslationUnits(_translationUnits, importSettings);
			}
		}

		private class AddTranslationUnitsMaskedCommand : UpdateCommand<ImportResult[]>, ISearchMaskedCommand
		{
			private readonly TranslationUnit[] _translationUnits;

			private bool[] _mask;

			public bool[] Mask
			{
				get
				{
					return _mask;
				}
				set
				{
					_mask = value;
				}
			}

			public AddTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, bool[] mask)
			{
				_translationUnits = translationUnits;
				_mask = mask;
			}

			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddTranslationUnitsMasked(_translationUnits, importSettings, _mask);
			}
		}

		private class UpdateTranslationUnitsCommand : UpdateCommand<ImportResult[]>
		{
			private readonly TranslationUnit[] _translationUnits;

			public UpdateTranslationUnitsCommand(TranslationUnit[] translationUnits)
			{
				_translationUnits = translationUnits;
			}

			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.UpdateTranslationUnits(_translationUnits);
			}
		}

		public ReadOnlyCollection<T> CascadeEntries
		{
			get;
			private set;
		}

		public bool StopSearchingWhenResultsFound
		{
			get;
			private set;
		}

		public bool RemoveDuplicates
		{
			get;
			private set;
		}

		public CultureInfo SourceLanguage
		{
			get;
			private set;
		}

		public CultureInfo TargetLanguage
		{
			get;
			private set;
		}

		public ConcurrentQueue<int> CascadeEntryIndexesUsed
		{
			get;
			private set;
		}

		public Cascade(IEnumerable<T> cascadeEntries, bool stopSearchingWhenResultsFound, bool removeDuplicates, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (cascadeEntries == null)
			{
				throw new ArgumentNullException("cascadeEntries");
			}
			if (sourceLanguage == null)
			{
				throw new ArgumentNullException("sourceLanguage");
			}
			if (targetLanguage == null)
			{
				throw new ArgumentNullException("targetLanguage");
			}
			CascadeEntries = new ReadOnlyCollection<T>(new List<T>(cascadeEntries));
			StopSearchingWhenResultsFound = stopSearchingWhenResultsFound;
			RemoveDuplicates = removeDuplicates;
			SourceLanguage = sourceLanguage;
			TargetLanguage = targetLanguage;
			CascadeEntryIndexesUsed = new ConcurrentQueue<int>();
		}

		public string GetTranslationProviderNames()
		{
			List<string> list = new List<string>();
			foreach (T cascadeEntry in CascadeEntries)
			{
				list.Add(cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider.Name);
			}
			return string.Join(",", list.ToArray());
		}

		public SubsegmentSearchResultsCollection[] SubsegmentSearchSegments(SubsegmentSearchSettings subsegmentSearchSettings, Segment[] segments, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			List<CascadeMessage> list = new List<CascadeMessage>();
			if (subsegmentSearchSettings == null)
			{
				throw new ArgumentNullException("subsegmentSearchSettings");
			}
			if (segments == null || segments.Length == 0)
			{
				throw new ArgumentNullException("segments");
			}
			List<SubsegmentSearchResultsCollection> list2 = new List<SubsegmentSearchResultsCollection>();
			for (int i = 0; i < CascadeEntries.Count; i++)
			{
				T val = CascadeEntries[i];
				try
				{
					ISubsegmentTranslationMemoryLanguageDirection subsegmentTranslationMemoryLanguageDirection = val.TranslationProviderLanguageDirection as ISubsegmentTranslationMemoryLanguageDirection;
					if (subsegmentTranslationMemoryLanguageDirection != null)
					{
						SubsegmentSearchResultsCollection[] array = subsegmentTranslationMemoryLanguageDirection.SubsegmentSearchSegments(subsegmentSearchSettings, segments);
						if (array != null)
						{
							SetCascadeEntryIndex(array, i);
							list2.AddRange(array);
						}
					}
				}
				catch (Exception exception)
				{
					list.Add(new CascadeMessage(val, exception));
				}
			}
			cascadeMessages = list;
			return list2.ToArray();
		}

		private void SetCascadeEntryIndex(SubsegmentSearchResultsCollection[] subsegmentSearchResultsCollections, int cascadeEntryIndex)
		{
			if (subsegmentSearchResultsCollections != null)
			{
				foreach (SubsegmentSearchResultsCollection subsegmentSearchResultsCollection in subsegmentSearchResultsCollections)
				{
					foreach (SubsegmentSearchResults item in subsegmentSearchResultsCollection.ResultsPerFragment)
					{
						foreach (SubsegmentSearchResult item2 in (IEnumerable<SubsegmentSearchResult>)item)
						{
							item2.CascadeEntryIndex = cascadeEntryIndex;
						}
					}
				}
			}
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults result = SearchSegment(settings, segment, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentCommand(segment), settings, out cascadeMessages);
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults result = SearchText(settings, segment, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public SearchResults SearchText(SearchSettings settings, string segment, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			if (string.IsNullOrEmpty(segment))
			{
				cascadeMessages = new List<CascadeMessage>();
				return null;
			}
			return ExecuteSearchCommand(new SearchTextCommand(segment), settings, out cascadeMessages);
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults result = SearchTranslationUnit(settings, translationUnit, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchTranslationUnitCommand(translationUnit), settings, out cascadeMessages);
		}

		private SearchResults ExecuteSearchCommand(SearchCommand<SearchResults> searchCommand, SearchSettings searchSettings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			if (searchSettings == null)
			{
				throw new ArgumentNullException("searchSettings");
			}
			List<CascadeMessage> list = new List<CascadeMessage>();
			SearchResults searchResults = null;
			ClearCascadeIndexes();
			for (int i = 0; i < CascadeEntries.Count; i++)
			{
				T val = CascadeEntries[i];
				try
				{
					if (!searchCommand.ShouldExecute(val, searchSettings))
					{
						list.AddRange(searchCommand.GetUnsupportedCascadeMessages(val, searchSettings));
					}
					else
					{
						if (!CascadeEntryIndexesUsed.Contains(i))
						{
							CascadeEntryIndexesUsed.Enqueue(i);
						}
						SetSearchPenaltySettings(val, searchSettings);
						SearchResults searchResults2 = searchCommand.Execute(val, searchSettings);
						if (searchResults2 != null && searchResults2.Count > 0)
						{
							SetCascadeEntryIndex(searchResults2, i);
							if (searchResults == null)
							{
								searchResults = searchResults2;
								if (StopSearchingWhenResultsFound)
								{
									break;
								}
							}
							else if (!IsVirtualTranslationResult(searchResults2, val))
							{
								searchResults.Merge(searchResults2, RemoveDuplicates);
							}
						}
					}
				}
				catch (Exception exception)
				{
					list.Add(new CascadeMessage(val, exception));
				}
			}
			ProcessSearchResults(searchResults, searchSettings);
			cascadeMessages = list;
			return searchResults;
		}

		private void ClearCascadeIndexes()
		{
			int result;
			while (CascadeEntryIndexesUsed.TryDequeue(out result))
			{
			}
		}

		private bool IsVirtualTranslationResult(SearchResults cascadeEntrySearchResults, T cascadeEntry)
		{
			if (cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider is ITranslationMemory && cascadeEntrySearchResults != null && cascadeEntrySearchResults.Count == 1 && cascadeEntrySearchResults[0].TranslationProposal != null && (cascadeEntrySearchResults[0].TranslationProposal.Origin == TranslationUnitOrigin.MachineTranslation || cascadeEntrySearchResults[0].TranslationProposal.Origin == TranslationUnitOrigin.AdaptiveMachineTranslation))
			{
				return true;
			}
			return false;
		}

		private void SetSearchPenaltySettings(CascadeEntry cascadeEntry, SearchSettings searchSettings)
		{
			searchSettings.RemovePenalty(PenaltyType.ProviderPenalty);
			if (cascadeEntry.Penalty > 0)
			{
				searchSettings.AddPenalty(PenaltyType.ProviderPenalty, cascadeEntry.Penalty);
			}
		}

		private void ProcessSearchResults(SearchResults searchResults, SearchSettings searchSettings)
		{
			if (searchResults != null)
			{
				searchResults.CheckForMultipleTranslations(searchSettings);
				SortSearchResults(searchResults, searchSettings);
				searchResults.Cap(searchSettings.MaxResults);
			}
		}

		private void SortSearchResults(SearchResults searchResults, SearchSettings searchSettings)
		{
			SearchResults.SearchResultComparer disambiguator = null;
			if (CascadeEntries.Count > 1)
			{
				disambiguator = SortSearchResultsAccordingToProviderOrder;
			}
			if (searchSettings.SortSpecification == null)
			{
				if (searchSettings.IsConcordanceSearch)
				{
					searchResults.Sort(new SortSpecification(SearchResults.DefaultSortOrderConcordance), disambiguator);
				}
				else
				{
					searchResults.Sort(new SortSpecification(SearchResults.DefaultSortOrder), disambiguator);
				}
			}
			else
			{
				searchResults.Sort(searchSettings.SortSpecification, disambiguator);
			}
		}

		private int SortSearchResultsAccordingToProviderOrder(SearchResult searchResultA, SearchResult searchResultB)
		{
			return searchResultA.CascadeEntryIndex - searchResultB.CascadeEntryIndex;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] result = SearchSegments(settings, segments, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return SearchSegmentsMasked(settings, segments, null, out cascadeMessages);
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] array = SearchSegmentsMasked(settings, segments, mask, out cascadeMessages);
			SearchResults[] result = array;
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public SearchResultsMerged[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentsMaskedCommand(segments, mask), settings, segments.Length, out cascadeMessages);
		}

		public SegmentAndSubsegmentSearchResultsMerged[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition subsegmentSearchCondition, Segment[] segments, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentAndSubSegmentMaskedCommand(segments, mask, subsegmentSettings, subsegmentSearchCondition), settings, segments.Length, out cascadeMessages);
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] tus)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] result = SearchTranslationUnits(settings, tus, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] tus, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return SearchTranslationUnitsMasked(settings, tus, null, out cascadeMessages);
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] array = SearchTranslationUnitsMasked(settings, translationUnits, mask, out cascadeMessages);
			SearchResults[] result = array;
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public SearchResultsMerged[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchTranslationUnitsMaskedCommand(translationUnits, mask), settings, translationUnits.Length, out cascadeMessages);
		}

		public SegmentAndSubsegmentSearchResultsMerged[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition subsegmentSearchCondition, TranslationUnit[] translationUnits, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentAndSubSegmentTranslationUnitsMaskedCommand(translationUnits, mask, subsegmentSettings, subsegmentSearchCondition), settings, translationUnits.Length, out cascadeMessages);
		}

		private SearchResultsMerged[] ExecuteSearchCommand(SearchCommand<SearchResults[]> searchCommand, SearchSettings searchSettings, int searchResultsCount, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			if (searchSettings == null)
			{
				throw new ArgumentNullException("searchSettings");
			}
			List<CascadeMessage> list = new List<CascadeMessage>();
			SearchResultsMerged[] array = new SearchResultsMerged[searchResultsCount];
			bool[] searchMask = null;
			bool[] searchMtMask = null;
			GetSearchMask(searchCommand as ISearchMaskedCommand, out searchMask, out searchMtMask);
			ClearCascadeIndexes();
			for (int i = 0; i < CascadeEntries.Count; i++)
			{
				T val = CascadeEntries[i];
				TranslationMethod translationMethod = val.TranslationProviderLanguageDirection.TranslationProvider.TranslationMethod;
				try
				{
					if (!searchCommand.ShouldExecute(val, searchSettings))
					{
						list.AddRange(searchCommand.GetUnsupportedCascadeMessages(val, searchSettings));
					}
					else
					{
						ISearchMaskedCommand searchMaskedCommand = searchCommand as ISearchMaskedCommand;
						SearchResults[] searchResults = array;
						UpdateSearchCommandMask(searchMaskedCommand, searchSettings, searchResults, translationMethod, searchMask, searchMtMask);
						if (!CascadeEntryIndexesUsed.Contains(i))
						{
							CascadeEntryIndexesUsed.Enqueue(i);
						}
						SetSearchPenaltySettings(val, searchSettings);
						SearchResults[] array2 = searchCommand.Execute(val, searchSettings);
						if (array2 != null)
						{
							SetCascadeEntryIndex(array2, i);
							MergeSearchResults(val, array, array2, i);
						}
					}
				}
				catch (Exception exception)
				{
					list.Add(new CascadeMessage(val, exception));
				}
			}
			ResetSearchCommandMask(searchCommand as ISearchMaskedCommand, searchMask);
			SearchResultsMerged[] array3 = array;
			foreach (SearchResultsMerged searchResults2 in array3)
			{
				ProcessSearchResults(searchResults2, searchSettings);
			}
			cascadeMessages = list;
			return array;
		}

		private SegmentAndSubsegmentSearchResultsMerged[] ExecuteSearchCommand(SearchCommand<SegmentAndSubsegmentSearchResults[]> searchCommand, SearchSettings searchSettings, int searchResultsCount, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			if (searchSettings == null)
			{
				throw new ArgumentNullException("searchSettings");
			}
			List<CascadeMessage> list = new List<CascadeMessage>();
			SegmentAndSubsegmentSearchResultsMerged[] array = new SegmentAndSubsegmentSearchResultsMerged[searchResultsCount];
			GetSearchMask(searchCommand as ISearchMaskedCommand, out bool[] searchMask, out bool[] searchMtMask);
			ClearCascadeIndexes();
			for (int i = 0; i < CascadeEntries.Count; i++)
			{
				T val = CascadeEntries[i];
				TranslationMethod translationMethod = val.TranslationProviderLanguageDirection.TranslationProvider.TranslationMethod;
				try
				{
					if (!searchCommand.ShouldExecute(val, searchSettings))
					{
						list.AddRange(searchCommand.GetUnsupportedCascadeMessages(val, searchSettings));
					}
					else
					{
						ISearchMaskedCommand searchMaskedCommand = searchCommand as ISearchMaskedCommand;
						SearchResults[] searchResults = array;
						UpdateSearchCommandMask(searchMaskedCommand, searchSettings, searchResults, translationMethod, searchMask, searchMtMask);
						if (!CascadeEntryIndexesUsed.Contains(i))
						{
							CascadeEntryIndexesUsed.Enqueue(i);
						}
						SetSearchPenaltySettings(val, searchSettings);
						SegmentAndSubsegmentSearchResults[] array2 = searchCommand.Execute(val, searchSettings);
						if (array2 != null)
						{
							SetCascadeEntryIndex(array2, i);
							MergeSearchResults(val, array, array2, i);
						}
					}
				}
				catch (Exception exception)
				{
					list.Add(new CascadeMessage(val, exception));
				}
			}
			ResetSearchCommandMask(searchCommand as ISearchMaskedCommand, searchMask);
			SegmentAndSubsegmentSearchResultsMerged[] array3 = array;
			foreach (SegmentAndSubsegmentSearchResultsMerged searchResults2 in array3)
			{
				ProcessSearchResults(searchResults2, searchSettings);
			}
			foreach (T cascadeEntry in CascadeEntries)
			{
				if (GetWarningMessage(cascadeEntry, searchSettings).Count != 0)
				{
					list.AddRange(GetWarningMessage(cascadeEntry, searchSettings));
				}
			}
			cascadeMessages = list;
			return array;
		}

		public IList<CascadeMessage> GetWarningMessage(T cascadeEntry, SearchSettings searchSettings)
		{
			List<CascadeMessage> list = new List<CascadeMessage>();
			ITranslationMemory translationMemory = cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider as ITranslationMemory;
			if (TranslationMemoryUpgradeUtil.TranslationMemoryWithAsianLanguageRequiresUpgrade(translationMemory))
			{
				CascadeMessage item = new CascadeMessage(cascadeEntry, CascadeMessageCode.TranslationProviderNeedsUpgrade);
				list.Add(item);
			}
			return list;
		}

		private static void UpdateSearchCommandMask(ISearchMaskedCommand searchMaskedCommand, SearchSettings searchSettings, SearchResults[] searchResults, TranslationMethod translationMethod, bool[] searchMask, bool[] mtSearchMask)
		{
			if (searchMaskedCommand == null)
			{
				return;
			}
			if (translationMethod == TranslationMethod.MachineTranslation)
			{
				for (int i = 0; i < searchResults.Count(); i++)
				{
					if (!searchMask[i])
					{
						continue;
					}
					switch (searchSettings.MachineTranslationLookup)
					{
					case MachineTranslationLookupMode.WhenNoTranslationMemoryMatch:
					{
						SearchResults searchResults2 = searchResults[i];
						if (searchResults2 != null && searchResults2.Results.Any((SearchResult result) => result.TranslationProposal != null && result.MemoryTranslationUnit.Origin == TranslationUnitOrigin.TM))
						{
							mtSearchMask[i] = false;
						}
						break;
					}
					case MachineTranslationLookupMode.Always:
						mtSearchMask[i] = true;
						break;
					default:
						mtSearchMask[i] = false;
						break;
					}
				}
				searchMaskedCommand.Mask = mtSearchMask;
			}
			else
			{
				searchMaskedCommand.Mask = searchMask;
			}
		}

		private static void ResetSearchCommandMask(ISearchMaskedCommand searchMaskedCommand, bool[] searchMask)
		{
			if (searchMaskedCommand != null)
			{
				searchMaskedCommand.Mask = searchMask;
			}
		}

		private static void GetSearchMask(ISearchMaskedCommand searchMaskedCommand, out bool[] searchMask, out bool[] searchMtMask)
		{
			searchMask = null;
			searchMtMask = null;
			if (searchMaskedCommand != null)
			{
				searchMask = new bool[searchMaskedCommand.Mask.Length];
				searchMtMask = new bool[searchMaskedCommand.Mask.Length];
				for (int i = 0; i < searchMaskedCommand.Mask.Length; i++)
				{
					searchMask[i] = searchMaskedCommand.Mask[i];
					searchMtMask[i] = searchMaskedCommand.Mask[i];
				}
			}
		}

		private void SetCascadeEntryIndex(IList<SearchResults> searchResultsList, int cascadeEntryIndex)
		{
			if (searchResultsList != null)
			{
				foreach (SearchResults searchResults in searchResultsList)
				{
					SetCascadeEntryIndex(searchResults, cascadeEntryIndex);
					SegmentAndSubsegmentSearchResults segmentAndSubsegmentSearchResults = searchResults as SegmentAndSubsegmentSearchResults;
					if (segmentAndSubsegmentSearchResults != null && segmentAndSubsegmentSearchResults.SubsegmentSearchResultsCollection != null && segmentAndSubsegmentSearchResults.SubsegmentSearchResultsCollection.ResultsPerFragment != null)
					{
						foreach (SubsegmentSearchResults item in segmentAndSubsegmentSearchResults.SubsegmentSearchResultsCollection.ResultsPerFragment)
						{
							SetCascadeEntryIndex(item, cascadeEntryIndex);
						}
					}
				}
			}
		}

		private void SetCascadeEntryIndex(SearchResults searchResults, int cascadeEntryIndex)
		{
			if (searchResults != null)
			{
				foreach (SearchResult item in (IEnumerable<SearchResult>)searchResults)
				{
					item.CascadeEntryIndex = cascadeEntryIndex;
				}
			}
		}

		private void SetCascadeEntryIndex(SubsegmentSearchResults searchResults, int cascadeEntryIndex)
		{
			if (searchResults != null)
			{
				foreach (SubsegmentSearchResult item in (IEnumerable<SubsegmentSearchResult>)searchResults)
				{
					item.CascadeEntryIndex = cascadeEntryIndex;
				}
			}
		}

		private void MergeSearchResults(T cascadeEntry, SearchResultsMerged[] searchResults, SearchResults[] cascadeEntrySearchResults, int cascadeEntryIndex)
		{
			for (int i = 0; i < cascadeEntrySearchResults.Length; i++)
			{
				if (cascadeEntrySearchResults[i] == null)
				{
					continue;
				}
				if (searchResults[i] == null || searchResults[i].Results == null || (searchResults[i].Results.Count == 0 && cascadeEntrySearchResults[i].Results.Count > 0))
				{
					if (searchResults[i] != null)
					{
						CheckTokensAndPlaceables(searchResults[i], cascadeEntrySearchResults[i]);
					}
					searchResults[i] = new SearchResultsMerged(cascadeEntrySearchResults[i]);
				}
				else if (!StopSearchingWhenResultsFound && !IsVirtualTranslationResult(cascadeEntrySearchResults[i], cascadeEntry))
				{
					searchResults[i].Merge(cascadeEntrySearchResults[i], RemoveDuplicates, cascadeEntryIndex);
				}
			}
		}

		private void MergeSearchResults(T cascadeEntry, SegmentAndSubsegmentSearchResultsMerged[] searchResults, SegmentAndSubsegmentSearchResults[] cascadeEntrySearchResults, int cascadeEntryIndex)
		{
			for (int i = 0; i < cascadeEntrySearchResults.Length; i++)
			{
				if (cascadeEntrySearchResults[i] == null)
				{
					continue;
				}
				if (searchResults[i] == null || searchResults[i].Results == null || (searchResults[i].Results.Count == 0 && cascadeEntrySearchResults[i].Results.Count > 0))
				{
					if (searchResults[i] != null)
					{
						CheckTokensAndPlaceables(searchResults[i], cascadeEntrySearchResults[i]);
					}
					if (searchResults[i] != null && searchResults[i].SubsegmentSearchResultsCollectionList != null && searchResults[i].SubsegmentSearchResultsCollectionList.Count > 0)
					{
						searchResults[i].Merge(cascadeEntrySearchResults[i], RemoveDuplicates, cascadeEntryIndex);
					}
					else
					{
						searchResults[i] = new SegmentAndSubsegmentSearchResultsMerged(cascadeEntrySearchResults[i]);
					}
				}
				else if (!StopSearchingWhenResultsFound && !IsVirtualTranslationResult(cascadeEntrySearchResults[i], cascadeEntry))
				{
					searchResults[i].Merge(cascadeEntrySearchResults[i], RemoveDuplicates, cascadeEntryIndex);
				}
			}
		}

		private void CheckTokensAndPlaceables(SearchResults currentSearchResults, SearchResults newSearchResults)
		{
			if (currentSearchResults != null && newSearchResults != null)
			{
				if (currentSearchResults.DocumentPlaceables != null && newSearchResults.DocumentPlaceables == null)
				{
					newSearchResults.DocumentPlaceables = currentSearchResults.DocumentPlaceables;
				}
				if (currentSearchResults.SourceSegment != null && currentSearchResults.SourceSegment.Tokens != null && newSearchResults.SourceSegment != null && newSearchResults.SourceSegment.Tokens == null)
				{
					newSearchResults.SourceSegment.Tokens = currentSearchResults.SourceSegment.Tokens;
				}
			}
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult result = AddTranslationUnit(translationUnit, settings, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddTranslationUnitCommand(translationUnit), settings, out cascadeMessages);
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult result = UpdateTranslationUnit(translationUnit, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new UpdateTranslationUnitCommand(translationUnit), null, out cascadeMessages);
		}

		private ImportResult ExecuteUpdateCommand(UpdateCommand<ImportResult> updateCommand, ImportSettings importSettings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			List<CascadeMessage> list = new List<CascadeMessage>();
			ImportResult importResult = null;
			foreach (T cascadeEntry in CascadeEntries)
			{
				try
				{
					if (!updateCommand.ShouldExecute(cascadeEntry, importSettings))
					{
						list.AddRange(updateCommand.GetUnsupportedCascadeMessages(cascadeEntry, importSettings));
					}
					else
					{
						ImportResult importResult2 = updateCommand.Execute(cascadeEntry, importSettings);
						if (importResult == null)
						{
							importResult = importResult2;
						}
					}
				}
				catch (Exception exception)
				{
					list.Add(new CascadeMessage(cascadeEntry, exception));
				}
			}
			cascadeMessages = list;
			return importResult;
		}

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddOrUpdateTranslationUnits(translationUnits, previousTranslationHashes, settings, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddOrUpdateTranslationUnitsCommand(translationUnits, previousTranslationHashes), settings, out cascadeMessages);
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddOrUpdateTranslationUnitsMasked(translationUnits, previousTranslationHashes, settings, mask, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddOrUpdateTranslationUnitsMaskedCommand(translationUnits, previousTranslationHashes, mask), settings, out cascadeMessages);
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddTranslationUnits(translationUnits, settings, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddTranslationUnitsCommand(translationUnits), settings, out cascadeMessages);
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddTranslationUnitsMasked(translationUnits, settings, mask, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddTranslationUnitsMaskedCommand(translationUnits, mask), settings, out cascadeMessages);
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = UpdateTranslationUnits(translationUnits, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new UpdateTranslationUnitsCommand(translationUnits), null, out cascadeMessages);
		}

		private ImportResult[] ExecuteUpdateCommand(UpdateCommand<ImportResult[]> updateCommand, ImportSettings importSettings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			List<CascadeMessage> list = new List<CascadeMessage>();
			ImportResult[] array = null;
			foreach (T cascadeEntry in CascadeEntries)
			{
				try
				{
					if (!updateCommand.ShouldExecute(cascadeEntry, importSettings))
					{
						list.AddRange(updateCommand.GetUnsupportedCascadeMessages(cascadeEntry, importSettings));
					}
					else
					{
						ImportResult[] array2 = updateCommand.Execute(cascadeEntry, importSettings);
						if (array == null)
						{
							array = array2;
						}
					}
				}
				catch (Exception exception)
				{
					list.Add(new CascadeMessage(cascadeEntry, exception));
				}
			}
			cascadeMessages = list;
			return array;
		}

		private void ThrowCascadeException(IEnumerable<CascadeMessage> cascadeMessages)
		{
			if (cascadeMessages.Count() > 0)
			{
				ThrowCascadeException(cascadeMessages.First());
			}
		}

		private void ThrowCascadeException(CascadeMessage cascadeMessage)
		{
			throw new CascadeException(cascadeMessage);
		}
	}
}
