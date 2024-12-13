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
	/// <summary>
	/// A cascade is used to perform searches and updates across many 
	/// different translation provider language directions. Each translation provider language direction is
	/// represented by a cascade entry that contains other information with regards to how it should be treated
	/// - e.g. what penalties should be applied during searches. 
	/// </summary>
	/// <typeparam name="T">
	/// T represents the cascade entry type. A simple cascade can just use <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.CascadeEntry" /> but, for example, 
	/// project cascades need more information associated with each cascade entry and uses ProjectCascadeEntry.
	/// </typeparam>
	public class Cascade<T> where T : CascadeEntry
	{
		/// <summary>
		/// Marker interface for masked search commands.
		/// </summary>
		private interface ISearchMaskedCommand
		{
			/// <summary>
			/// Returns the mask used by the search command.
			/// </summary>
			bool[] Mask
			{
				get;
				set;
			}
		}

		/// <summary>
		/// SearchCommand class represents an search command.
		/// </summary>
		/// <typeparam name="R">type of the search results</typeparam>
		private abstract class SearchCommand<R>
		{
			/// <summary>
			/// Determines whether the search command should be executed.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>whether search command should be executed</returns>
			public virtual bool ShouldExecute(T cascadeEntry, SearchSettings searchSettings)
			{
				return GetUnsupportedCascadeMessages(cascadeEntry, searchSettings).Count == 0;
			}

			/// <summary>
			/// Gets the unsupported cascade messages.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>unsupported casacade messages</returns>
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

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
			public abstract R Execute(T cascadeEntry, SearchSettings searchSettings);
		}

		/// <summary>
		/// SearchSegmentCommand class represents a search command corresponding to the method SearchSegment.
		/// </summary>
		private class SearchSegmentCommand : SearchCommand<SearchResults>
		{
			private readonly Segment _segment;

			/// <summary>
			/// Constructor that takes the given segment.
			/// </summary>
			/// <param name="segment">segment</param>
			public SearchSegmentCommand(Segment segment)
			{
				_segment = segment;
			}

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
			public override SearchResults Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchSegment(searchSettings, _segment);
			}
		}

		/// <summary>
		/// SearchTextCommand class represents a search command corresponding to the method SearchText.
		/// </summary>
		private class SearchTextCommand : SearchCommand<SearchResults>
		{
			private readonly string _segment;

			/// <summary>
			/// Constructor that takes the given segment.
			/// </summary>
			/// <param name="segment">segment</param>
			public SearchTextCommand(string segment)
			{
				_segment = segment;
			}

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
			public override SearchResults Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchText(searchSettings, _segment);
			}
		}

		/// <summary>
		/// SearchTranslationUnitCommand class represents a search command corresponding to the method SearchTranslationUnit.
		/// </summary>
		private class SearchTranslationUnitCommand : SearchCommand<SearchResults>
		{
			private readonly TranslationUnit _translationUnit;

			/// <summary>
			/// Constructor that takes the given translation unit.
			/// </summary>
			/// <param name="translationUnit">translation unit</param>
			public SearchTranslationUnitCommand(TranslationUnit translationUnit)
			{
				_translationUnit = translationUnit;
			}

			/// <summary>
			/// Gets the unsupported cascade messages.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>unsupported casacade messages</returns>
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

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
			public override SearchResults Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchTranslationUnit(searchSettings, _translationUnit);
			}
		}

		/// <summary>
		/// SearchSegmentsMaskedCommand class represents a search command corresponding to the method SearchSegmentsMasked.
		/// </summary>
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

			/// <summary>
			/// Constructor that takes the given segments and mask.
			/// </summary>
			/// <param name="segments">segments</param>
			/// <param name="mask">mask</param>
			public SearchSegmentsMaskedCommand(Segment[] segments, bool[] mask)
			{
				_segments = segments;
				_mask = mask;
			}

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
			public override SearchResults[] Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchSegmentsMasked(searchSettings, _segments, _mask);
			}
		}

		/// <summary>
		/// SearchSegmentAndSubSegmentMaskedCommand class represents a search command corresponding to the method SearchSegmentAndSubsegmentsMasked.
		/// </summary>
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

			/// <summary>
			/// Constructor that takes the given segments, the mask and the subsegment search parameters.
			/// </summary>
			/// <param name="segments">segments</param>
			/// <param name="mask">mask</param>
			/// <param name="subsegmentSearchSettings">subsegmentSearchSettings</param>
			/// <param name="subsegmentSearchCondition">subsegmentSearchCondition</param>
			public SearchSegmentAndSubSegmentMaskedCommand(Segment[] segments, bool[] mask, SubsegmentSearchSettings subsegmentSearchSettings, SubsegmentSearchCondition subsegmentSearchCondition)
			{
				_segments = segments;
				_mask = mask;
				_subsegmentSearchSettings = subsegmentSearchSettings;
				_subsegmentSearchCondition = subsegmentSearchCondition;
			}

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
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

		/// <summary>
		/// SearchSegmentAndSubSegmentMaskedCommand class represents a search command corresponding to the method SearchSegmentAndSubsegmentsMasked.
		/// </summary>
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

			/// <summary>
			/// Constructor that takes the given segments, the mask and the subsegment search parameters.
			/// </summary>
			/// <param name="translationUnits">translation units</param>
			/// <param name="mask">mask</param>
			/// <param name="subsegmentSearchSettings">subsegment search settings</param>
			/// <param name="subsegmentSearchCondition">subsegment search condition</param>
			public SearchSegmentAndSubSegmentTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, bool[] mask, SubsegmentSearchSettings subsegmentSearchSettings, SubsegmentSearchCondition subsegmentSearchCondition)
			{
				_translationUnits = translationUnits;
				_mask = mask;
				_subsegmentSearchSettings = subsegmentSearchSettings;
				_subsegmentSearchCondition = subsegmentSearchCondition;
			}

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
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

			/// <summary>
			/// Gets the unsupported cascade messages.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>unsupported casacade messages</returns>
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

		/// <summary>
		/// SearchTranslationUnitsMaskedCommand class represents a search command corresponding to the method SearchTranslationUnitsMasked.
		/// </summary>
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

			/// <summary>
			/// Constructor that takes the given translation units and mask.
			/// </summary>
			/// <param name="translationUnits">translation units</param>
			/// <param name="mask">mask</param>
			public SearchTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, bool[] mask)
			{
				_translationUnits = translationUnits;
				_mask = mask;
			}

			/// <summary>
			/// Gets the unsupported cascade messages.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>unsupported casacade messages</returns>
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

			/// <summary>
			/// Executes the search command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="searchSettings">search settings</param>
			/// <returns>search results</returns>
			public override SearchResults[] Execute(T cascadeEntry, SearchSettings searchSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.SearchTranslationUnitsMasked(searchSettings, _translationUnits, _mask);
			}
		}

		/// <summary>
		/// UpdateCommand class represents an update command.
		/// </summary>
		/// <typeparam name="R">type of the import results</typeparam>
		private abstract class UpdateCommand<R>
		{
			/// <summary>
			/// Determines whether the update command should be executed.
			/// </summary>
			/// <param name="cascadeEntry">cascadeEntry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>whether update command should be executed</returns>
			public virtual bool ShouldExecute(T cascadeEntry, ImportSettings importSettings)
			{
				return GetUnsupportedCascadeMessages(cascadeEntry, importSettings).Count == 0;
			}

			/// <summary>
			/// Gets the unsupported cascade messages.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>unsupported cascade messages</returns>
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

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public abstract R Execute(T cascadeEntry, ImportSettings importSettings);
		}

		/// <summary>
		/// AddTranslationUnitCommand class represents an update command corresponding to the method AddTranslationUnit.
		/// </summary>
		private class AddTranslationUnitCommand : UpdateCommand<ImportResult>
		{
			private readonly TranslationUnit _translationUnit;

			/// <summary>
			/// Constructor that takes the given translation unit.
			/// </summary>
			/// <param name="translationUnit">translation unit</param>
			public AddTranslationUnitCommand(TranslationUnit translationUnit)
			{
				_translationUnit = translationUnit;
			}

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public override ImportResult Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddTranslationUnit(_translationUnit, importSettings);
			}
		}

		/// <summary>
		/// UpdateTranslationUnitCommand class represents an update command corresponding to the method UpdateTranslationUnit.
		/// </summary>
		private class UpdateTranslationUnitCommand : UpdateCommand<ImportResult>
		{
			private readonly TranslationUnit _translationUnit;

			/// <summary>
			/// Constructor that takes the given translation unit
			/// </summary>
			/// <param name="translationUnit">translation unit</param>
			public UpdateTranslationUnitCommand(TranslationUnit translationUnit)
			{
				_translationUnit = translationUnit;
			}

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public override ImportResult Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.UpdateTranslationUnit(_translationUnit);
			}
		}

		/// <summary>
		/// AddOrUpdateTranslationUnitsCommand class represents an update command corresponding to the method AddOrUpdateTranslationUnits.
		/// </summary>
		private class AddOrUpdateTranslationUnitsCommand : UpdateCommand<ImportResult[]>
		{
			private readonly TranslationUnit[] _translationUnits;

			private readonly int[] _previousTranslationHashes;

			/// <summary>
			/// Constructor that takes the given translation units and previous translation hashes.
			/// </summary>
			/// <param name="translationUnits">translation units</param>
			/// <param name="previousTranslationHashes">previous translation hashes</param>
			public AddOrUpdateTranslationUnitsCommand(TranslationUnit[] translationUnits, int[] previousTranslationHashes)
			{
				_translationUnits = translationUnits;
				_previousTranslationHashes = previousTranslationHashes;
			}

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddOrUpdateTranslationUnits(_translationUnits, _previousTranslationHashes, importSettings);
			}
		}

		/// <summary>
		/// AddOrUpdateTranslationUnitsMaskedCommand class represents an update command corresponding to the method AddOrUpdateTranslationUnitsMasked.
		/// </summary>
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

			/// <summary>
			/// Constructor that takes the given translation units, previous translation hashes, and mask.
			/// </summary>
			/// <param name="translationUnits">translation units</param>
			/// <param name="previousTranslationHashes">previous translation hashes</param>
			/// <param name="mask">mask</param>
			public AddOrUpdateTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, int[] previousTranslationHashes, bool[] mask)
			{
				_translationUnits = translationUnits;
				_previousTranslationHashes = previousTranslationHashes;
				_mask = mask;
			}

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascadeEntry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddOrUpdateTranslationUnitsMasked(_translationUnits, _previousTranslationHashes, importSettings, _mask);
			}
		}

		/// <summary>
		/// AddTranslationUnitsCommand class represents an update command corresponding to the method AddTranslationUnits.
		/// </summary>
		private class AddTranslationUnitsCommand : UpdateCommand<ImportResult[]>
		{
			private readonly TranslationUnit[] _translationUnits;

			/// <summary>
			/// Constructor that takes the given translation units.
			/// </summary>
			/// <param name="translationUnits">translation units</param>
			public AddTranslationUnitsCommand(TranslationUnit[] translationUnits)
			{
				_translationUnits = translationUnits;
			}

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddTranslationUnits(_translationUnits, importSettings);
			}
		}

		/// <summary>
		/// AddTranslationUnitsMaskedCommand class represents an update command corresponding to the method AddTranslationUnitsMasked.
		/// </summary>
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

			/// <summary>
			/// Constructor that takes the given translation units and mask.
			/// </summary>
			/// <param name="translationUnits">translation units</param>
			/// <param name="mask">mask</param>
			public AddTranslationUnitsMaskedCommand(TranslationUnit[] translationUnits, bool[] mask)
			{
				_translationUnits = translationUnits;
				_mask = mask;
			}

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.AddTranslationUnitsMasked(_translationUnits, importSettings, _mask);
			}
		}

		/// <summary>
		/// UpdateTranslationUnitsCommand class represents an update command corresponding to the method UpdateTranslationUnits.
		/// </summary>
		private class UpdateTranslationUnitsCommand : UpdateCommand<ImportResult[]>
		{
			private readonly TranslationUnit[] _translationUnits;

			/// <summary>
			/// Constructor that takes the given translation units.
			/// </summary>
			/// <param name="translationUnits">translation units</param>
			public UpdateTranslationUnitsCommand(TranslationUnit[] translationUnits)
			{
				_translationUnits = translationUnits;
			}

			/// <summary>
			/// Executes the update command.
			/// </summary>
			/// <param name="cascadeEntry">cascade entry</param>
			/// <param name="importSettings">import settings</param>
			/// <returns>import results</returns>
			public override ImportResult[] Execute(T cascadeEntry, ImportSettings importSettings)
			{
				return cascadeEntry.TranslationProviderLanguageDirection.UpdateTranslationUnits(_translationUnits);
			}
		}

		/// <summary>
		/// Gets the cascade entries, which refer to a translation provider language direction and 
		/// specify and optional penalty to apply when perforing look-ups.
		/// </summary>
		public ReadOnlyCollection<T> CascadeEntries
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets whether to stop searching when results have been found.
		/// </summary>
		/// <remarks>
		/// "Stop searching" means that the cascade will not search the next translation provider 
		/// when results have been found in the current translation provider.
		/// </remarks>
		public bool StopSearchingWhenResultsFound
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets whether duplicates should be removed from search results.
		/// </summary>
		public bool RemoveDuplicates
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the source language of this cascade.
		/// </summary>
		public CultureInfo SourceLanguage
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the target language of this cascade.
		/// </summary>
		public CultureInfo TargetLanguage
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the Cascade Entry Indexes that were used in the search.
		/// </summary>
		public ConcurrentQueue<int> CascadeEntryIndexesUsed
		{
			get;
			private set;
		}

		/// <summary>
		/// Constructor that takes the cascade entries, cascade search mode, remove duplicates, 
		/// source language, and target language.
		/// </summary>
		/// <param name="cascadeEntries">cascade entries</param>
		/// <param name="stopSearchingWhenResultsFound">whether to stop searching when results have been found</param>
		/// <param name="removeDuplicates">remove duplicates</param>
		/// <param name="sourceLanguage">source language</param>
		/// <param name="targetLanguage">target language</param>
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

		/// <summary>
		/// Gets a comma-delimited string of translation provider names.
		/// </summary>
		/// <returns>translation provider names</returns>
		public string GetTranslationProviderNames()
		{
			List<string> list = new List<string>();
			foreach (T cascadeEntry in CascadeEntries)
			{
				list.Add(cascadeEntry.TranslationProviderLanguageDirection.TranslationProvider.Name);
			}
			return string.Join(",", list.ToArray());
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ISubsegmentTranslationMemoryLanguageDirection.SubsegmentSearchSegments(Sdl.LanguagePlatform.TranslationMemory.SubsegmentSearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> on the current translation provider cascade.
		/// </summary>
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

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegment(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment)" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults result = SearchSegment(settings, segment, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegment(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment)" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults SearchSegment(SearchSettings settings, Segment segment, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentCommand(segment), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchText(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,System.String)" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults result = SearchText(settings, segment, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchText(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,System.String)" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults SearchText(SearchSettings settings, string segment, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			if (string.IsNullOrEmpty(segment))
			{
				cascadeMessages = new List<CascadeMessage>();
				return null;
			}
			return ExecuteSearchCommand(new SearchTextCommand(segment), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults result = SearchTranslationUnit(settings, translationUnit, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchTranslationUnitCommand(translationUnit), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes the given search command with the given search settings.
		/// </summary>
		/// <param name="searchCommand">search command</param>
		/// <param name="searchSettings">search settings</param>
		/// <param name="cascadeMessages">cascade messages</param>
		/// <returns>search results</returns>
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

		/// <summary>
		/// Sets the search penalty settings.
		/// </summary>
		/// <param name="cascadeEntry">cascade entry</param>
		/// <param name="searchSettings">search settings</param>
		private void SetSearchPenaltySettings(CascadeEntry cascadeEntry, SearchSettings searchSettings)
		{
			searchSettings.RemovePenalty(PenaltyType.ProviderPenalty);
			if (cascadeEntry.Penalty > 0)
			{
				searchSettings.AddPenalty(PenaltyType.ProviderPenalty, cascadeEntry.Penalty);
			}
		}

		/// <summary>
		/// Processes the given search results; it applies multiple translation penalties, sorts the results, 
		/// and truncates the results.
		/// </summary>
		/// <param name="searchResults">search results</param>
		/// <param name="searchSettings">search settings</param>
		private void ProcessSearchResults(SearchResults searchResults, SearchSettings searchSettings)
		{
			if (searchResults != null)
			{
				searchResults.CheckForMultipleTranslations(searchSettings);
				SortSearchResults(searchResults, searchSettings);
				searchResults.Cap(searchSettings.MaxResults);
			}
		}

		/// <summary>
		/// Sorts the search results.
		/// </summary>
		/// <param name="searchResults">search results</param>
		/// <param name="searchSettings">search settings</param>
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

		/// <summary>
		/// Sorts the search results according to the provider order.
		/// </summary>
		/// <param name="searchResultA">search result a</param>
		/// <param name="searchResultB">search result b</param>
		/// <returns>comparison between search result a and search result b</returns>
		private int SortSearchResultsAccordingToProviderOrder(SearchResult searchResultA, SearchResult searchResultB)
		{
			return searchResultA.CascadeEntryIndex - searchResultB.CascadeEntryIndex;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] result = SearchSegments(settings, segments, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return SearchSegmentsMasked(settings, segments, null, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] array = SearchSegmentsMasked(settings, segments, mask, out cascadeMessages);
			SearchResults[] result = array;
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResultsMerged[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentsMaskedCommand(segments, mask), settings, segments.Length, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> on the current translation provider cascade.
		/// </summary>
		public SegmentAndSubsegmentSearchResultsMerged[] SearchSegmentsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition subsegmentSearchCondition, Segment[] segments, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentAndSubSegmentMaskedCommand(segments, mask, subsegmentSettings, subsegmentSearchCondition), settings, segments.Length, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] tus)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] result = SearchTranslationUnits(settings, tus, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] tus, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return SearchTranslationUnitsMasked(settings, tus, null, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Boolean[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			SearchResults[] array = SearchTranslationUnitsMasked(settings, translationUnits, mask, out cascadeMessages);
			SearchResults[] result = array;
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Boolean[])" /> on the current translation provider cascade.
		/// </summary>
		public SearchResultsMerged[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchTranslationUnitsMaskedCommand(translationUnits, mask), settings, translationUnits.Length, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])" /> on the current translation provider cascade.
		/// </summary>
		public SegmentAndSubsegmentSearchResultsMerged[] SearchTranslationUnitsMasked(SearchSettings settings, SubsegmentSearchSettings subsegmentSettings, SubsegmentSearchCondition subsegmentSearchCondition, TranslationUnit[] translationUnits, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteSearchCommand(new SearchSegmentAndSubSegmentTranslationUnitsMaskedCommand(translationUnits, mask, subsegmentSettings, subsegmentSearchCondition), settings, translationUnits.Length, out cascadeMessages);
		}

		/// <summary>
		/// Executes the given search command with the given search settings.
		/// </summary>
		/// <param name="searchCommand">search command</param>
		/// <param name="searchSettings">search settings</param>
		/// <param name="searchResultsCount">search results count</param>
		/// <param name="cascadeMessages">cascade messages</param>
		/// <returns>search results</returns>
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

		/// <summary>
		/// Executes the given search command with the given search settings.
		/// </summary>
		/// <param name="searchCommand">search command</param>
		/// <param name="searchSettings">search settings</param>
		/// <param name="searchResultsCount">search results count</param>
		/// <param name="cascadeMessages">cascade messages</param>
		/// <returns>search results</returns>
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

		/// <summary>
		/// Getting Warning messages for Upgrade requirement.
		/// </summary>
		/// <param name="cascadeEntry"></param>
		/// <param name="searchSettings"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Sets the cascade entry index on the given list of search results.
		/// </summary>
		/// <param name="searchResultsList">search results list</param>
		/// <param name="cascadeEntryIndex">cascade entry index</param>
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

		/// <summary>
		/// Sets the cascade entry index on the search results.
		/// </summary>
		/// <param name="searchResults">search results</param>
		/// <param name="cascadeEntryIndex">cascade entry index</param>
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

		/// <summary>
		/// Sets the cascade entry index on the search results.
		/// </summary>
		/// <param name="searchResults">search results</param>
		/// <param name="cascadeEntryIndex">cascade entry index</param>
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

		/// <summary>
		/// Merges the cascade entry search results into the search results.
		/// </summary>
		/// <param name="cascadeEntry">Cascade Entry</param>
		/// <param name="searchResults">search results</param>
		/// <param name="cascadeEntrySearchResults">cascade entry search results</param>
		/// <param name="cascadeEntryIndex">cascade entry index</param>
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

		/// <summary>
		/// Merges the cascade entry search results into the search results.
		/// </summary>
		/// <param name="cascadeEntry">Cascade Entry</param>
		/// <param name="searchResults">search results</param>
		/// <param name="cascadeEntrySearchResults">cascade entry search results</param>
		/// <param name="cascadeEntryIndex">cascade entry index</param>
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

		/// <summary>
		/// Not all translation providers tokenize the source segment and evaluate the document placeables
		/// so check that this information is not lost when new results overwrite the current set of results. 
		/// The most common occurrence is when a cascade contains a translation memory and a machine
		/// translation provider. The TM provider returns no matches but tokenizes the source segment. This 
		/// will be overwritten by the MT result which probably won't tokenise the segment. </summary>
		/// <param name="currentSearchResults">Current search results.</param>
		/// <param name="newSearchResults">New search results.</param>
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

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit,Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" /> on the current translation provider.
		/// </summary>
		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult result = AddTranslationUnit(translationUnit, settings, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit,Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" /> on the current translation provider.
		/// </summary>
		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddTranslationUnitCommand(translationUnit), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.UpdateTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)" /> on the current translation provider.
		/// </summary>
		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult result = UpdateTranslationUnit(translationUnit, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.UpdateTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)" /> on the current translation provider.
		/// </summary>
		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new UpdateTranslationUnitCommand(translationUnit), null, out cascadeMessages);
		}

		/// <summary>
		/// Executes the given update command with the given import settings.
		/// </summary>
		/// <param name="updateCommand">update command</param>
		/// <param name="importSettings">import settings</param>
		/// <param name="cascadeMessages">cascade messages</param>
		/// <returns>import results</returns>
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

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddOrUpdateTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Int32[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddOrUpdateTranslationUnits(translationUnits, previousTranslationHashes, settings, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddOrUpdateTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Int32[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddOrUpdateTranslationUnitsCommand(translationUnits, previousTranslationHashes), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddOrUpdateTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Int32[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings,System.Boolean[])" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddOrUpdateTranslationUnitsMasked(translationUnits, previousTranslationHashes, settings, mask, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddOrUpdateTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Int32[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings,System.Boolean[])" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddOrUpdateTranslationUnitsMaskedCommand(translationUnits, previousTranslationHashes, mask), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddTranslationUnits(translationUnits, settings, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddTranslationUnitsCommand(translationUnits), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings,System.Boolean[])" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = AddTranslationUnitsMasked(translationUnits, settings, mask, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings,System.Boolean[])" /> on the current translation provider.
		/// </summary>
		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new AddTranslationUnitsMaskedCommand(translationUnits, mask), settings, out cascadeMessages);
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.UpdateTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" /> on the current translation provider.
		/// </summary>
		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			IEnumerable<CascadeMessage> cascadeMessages;
			ImportResult[] result = UpdateTranslationUnits(translationUnits, out cascadeMessages);
			ThrowCascadeException(cascadeMessages);
			return result;
		}

		/// <summary>
		/// Executes <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.UpdateTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])" /> on the current translation provider.
		/// </summary>
		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits, out IEnumerable<CascadeMessage> cascadeMessages)
		{
			return ExecuteUpdateCommand(new UpdateTranslationUnitsCommand(translationUnits), null, out cascadeMessages);
		}

		/// <summary>
		/// Executes the given update command with the given import settings.
		/// </summary>
		/// <param name="updateCommand">update command</param>
		/// <param name="importSettings">import settings</param>
		/// <param name="cascadeMessages">cascade messages</param>
		/// <returns>import results</returns>
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

		/// <summary>
		/// Throws a cascade exception if there are any cascade messages.
		/// </summary>
		/// <param name="cascadeMessages">cascade messages</param>
		private void ThrowCascadeException(IEnumerable<CascadeMessage> cascadeMessages)
		{
			if (cascadeMessages.Count() > 0)
			{
				ThrowCascadeException(cascadeMessages.First());
			}
		}

		/// <summary>
		/// Throws a cascade exception for the given cascade message.
		/// </summary>
		/// <param name="cascadeMessage">cascade message</param>
		private void ThrowCascadeException(CascadeMessage cascadeMessage)
		{
			throw new CascadeException(cascadeMessage);
		}
	}
}
