using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class Searcher
	{
		private const int NonExactMatchTreshold = 95;

		private readonly CallContext _Context;

		private readonly Scorer _Scorer;

		private readonly SortSpecification _DefaultSortOrder;

		private const bool _AllowTokenBundles = true;

		internal static int _exactSearchTestPageSize = -1;

		public SearchSettings Settings
		{
			get;
		}

		public AnnotatedTranslationMemory TM
		{
			get;
		}

		public Searcher(CallContext context, PersistentObjectToken tmId, SearchSettings settings)
		{
			_Context = (context ?? throw new ArgumentNullException("context"));
			TM = _Context.GetAnnotatedTranslationMemory(tmId);
			Settings = settings;
			if (Settings.Mode == SearchMode.ConcordanceSearch || Settings.Mode == SearchMode.TargetConcordanceSearch)
			{
				_DefaultSortOrder = new SortSpecification(SearchResults.DefaultSortOrderConcordance);
				_Scorer = new Scorer(TM, Settings, TM.Tm.NormalizeCharWidths);
				return;
			}
			bool flag = Settings.FindPenalty(PenaltyType.CharacterWidthDifference) != null;
			flag &= TM.Tm.NormalizeCharWidths;
			_Scorer = new Scorer(TM, Settings, flag);
			_DefaultSortOrder = new SortSpecification(SearchResults.DefaultSortOrder);
		}

		private static string GetCurrentStructureContextOverride(AnnotatedTranslationUnit tu)
		{
			if (tu?.TranslationUnit?.FieldValues == null)
			{
				return null;
			}
			MultipleStringFieldValue multipleStringFieldValue = tu.TranslationUnit.FieldValues[Field.StructureContextFieldName] as MultipleStringFieldValue;
			if (multipleStringFieldValue != null && multipleStringFieldValue.Values != null && multipleStringFieldValue.Count == 1)
			{
				return multipleStringFieldValue.Values.First();
			}
			return null;
		}

		public SearchResults Search(AnnotatedTranslationUnit tu)
		{
			if (tu == null)
			{
				throw new ArgumentNullException("tu");
			}
			TuContextData tuContextData = new TuContextData
			{
				IdContext = string.Empty
			};
			if (TM.Tm.IdContextMatch && tu.TranslationUnit.IdContexts != null && tu.TranslationUnit.IdContexts.Length > 0)
			{
				tuContextData.IdContext = tu.TranslationUnit.IdContexts.Values.First();
			}
			if (tu.TranslationUnit.Contexts != null && tu.TranslationUnit.Contexts.Length > 0)
			{
				tuContextData.TextContext = tu.TranslationUnit.Contexts.Values.First();
			}
			return SearchInternal(tu.Source, tu.Target, tuContextData);
		}

		private long GetSegmentHash(AbstractAnnotatedSegment s)
		{
			if (!TM.Tm.UsesLegacyHashes)
			{
				return s.StrictHash;
			}
			return s.Hash;
		}

		public SearchResults[] Search(AnnotatedTranslationUnit[] tus, bool[] mask)
		{
			if (tus == null || tus.Length == 0)
			{
				throw new ArgumentNullException("tus");
			}
			if (mask != null && mask.Length != tus.Length)
			{
				throw new ArgumentException("If a mask is specified, its length must be equal to that of the tus collection");
			}
			SearchResults[] array = new SearchResults[tus.Length];
			long context = 0L;
			long context2 = 0L;
			if (TM.Tm.TextContextMatchType == TextContextMatchType.PrecedingAndFollowingSource && tus.Length > 1)
			{
				context2 = ((tus[1].Source == null) ? (-1) : GetSegmentHash(tus[1].Source));
			}
			TuContext tuContext = new TuContext(context, context2);
			TuContext textContext = Settings.IsDocumentSearch ? tuContext : null;
			for (int i = 0; i < tus.Length; i++)
			{
				if (tus[i] == null)
				{
					array[i] = null;
					textContext = null;
					continue;
				}
				if (mask != null && !mask[i])
				{
					if (Settings.IsDocumentSearch)
					{
						context2 = ((tus[i].Target == null) ? (-1) : GetSegmentHash(tus[i].Target));
						textContext = new TuContext(GetSegmentHash(tus[i].Source), context2);
					}
					array[i] = null;
					continue;
				}
				if (Settings.IsDocumentSearch && TM.Tm.TextContextMatchType == TextContextMatchType.PrecedingAndFollowingSource && i > 0)
				{
					long context3 = (tus[i - 1].Source == null) ? (-1) : GetSegmentHash(tus[i - 1].Source);
					context2 = 0L;
					if (i < tus.Length - 1)
					{
						context2 = ((tus[i + 1].Source == null) ? (-1) : GetSegmentHash(tus[i + 1].Source));
					}
					textContext = new TuContext(context3, context2);
				}
				TuContextData tuContextData = new TuContextData();
				tuContextData.IdContext = GetIdContextFromTu(tus[i]);
				tuContextData.TextContext = textContext;
				tuContextData.CurrentStructureContextOverride = GetCurrentStructureContextOverride(tus[i]);
				array[i] = SearchInternal(tus[i].Source, tus[i].Target, tuContextData);
				if (Settings.IsDocumentSearch && TM.Tm.TextContextMatchType == TextContextMatchType.PrecedingSourceAndTarget)
				{
					textContext = GetMatchContext(array[i]);
				}
			}
			return array;
		}

		private string GetIdContextFromTu(AnnotatedTranslationUnit tu)
		{
			if (!TM.Tm.IdContextMatch || !tu.TranslationUnit.IdContexts.Values.Any())
			{
				return string.Empty;
			}
			return tu.TranslationUnit.IdContexts.Values.First();
		}

		public List<SearchResults> DuplicateSearchBatch(AnnotatedTranslationUnit[] tus, bool[] mask)
		{
			if (tus == null || tus.Length == 0)
			{
				throw new ArgumentNullException("tus");
			}
			if (mask != null && mask.Length != tus.Length)
			{
				throw new ArgumentException("If a mask is specified, its length must be equal to that of the tus collection");
			}
			if (Settings.Mode != SearchMode.DuplicateSearch)
			{
				throw new ArgumentException("Only DuplicateSearch is allowed");
			}
			return DuplicateSearchBatchInternal(tus, mask);
		}

		public List<SearchResults> SearchBatch(AnnotatedTranslationUnit[] tus, bool[] mask, int batchSize, FieldDefinitions fields, bool overwriteTranslation)
		{
			if (tus == null || tus.Length == 0)
			{
				throw new ArgumentNullException("tus");
			}
			if (mask != null && mask.Length != tus.Length)
			{
				throw new ArgumentException("If a mask is specified, its length must be equal to that of the tus collection");
			}
			if (Settings.Mode != 0 && Settings.Mode != SearchMode.NormalSearch)
			{
				throw new ArgumentException("Only ExactSearch and NormalSearch are allowed");
			}
			List<AnnotatedTranslationUnit> list = new List<AnnotatedTranslationUnit>();
			Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory tm = _Context.Storage.GetTm(TM.Tm.ResourceId.Id);
			bool applyHardFilterOnSql = TranslationUnitFilteringStrategyFactory.IsFilterRunnableOnSql(Settings.HardFilter, tm.DataVersion);
			List<SearchResults> list2 = ExactSearchBatch(tus, mask, batchSize, applyHardFilterOnSql, list);
			SetExactMatchContext(tus, list2, overwriteTranslation);
			if (Settings.Mode == SearchMode.NormalSearch && list.Count > 0)
			{
				FuzzySearchBatch(tus, batchSize, applyHardFilterOnSql, list, list2);
			}
			PopulateResultsAndApplyFilters(applyHardFilterOnSql, list2, fields, tus);
			return list2;
		}

		private void SetTuContextPrevious(SearchResult res, AnnotatedTranslationUnit previousTu, bool overwriteTranslation)
		{
			long context = 0L;
			long segmentHash = GetSegmentHash(previousTu.Source);
			if (!overwriteTranslation && previousTu.Target.Segment.Elements.Count != 0)
			{
				context = GetSegmentHash(previousTu.Target);
			}
			TuContext tuContext2 = res.ContextData = new TuContext(segmentHash, context);
		}

		private void SetExactMatchContext(IReadOnlyList<AnnotatedTranslationUnit> tus, IReadOnlyList<SearchResults> results, bool overwriteTranslation)
		{
			switch (TM.Tm.TextContextMatchType)
			{
			case TextContextMatchType.PrecedingSourceAndTarget:
			{
				if (results.Count > 0)
				{
					SearchResults searchResults = results[0];
					if (searchResults != null && searchResults.Results.Count > 0)
					{
						foreach (SearchResult result in results[0].Results)
						{
							result.ContextData = new TuContext(0L, 0L);
						}
					}
				}
				for (int j = 1; j < tus.Count; j++)
				{
					SearchResults searchResults2 = results[j];
					if (searchResults2 != null && searchResults2.Count > 0)
					{
						SearchResults searchResults3 = results[j];
						if (searchResults3 != null && searchResults3.Results.Count > 0)
						{
							SearchResults searchResults4 = results[j];
							SetTuContextPrevious((searchResults4 != null) ? searchResults4.Results[0] : null, tus[j - 1], overwriteTranslation);
						}
					}
				}
				break;
			}
			case TextContextMatchType.PrecedingAndFollowingSource:
			{
				long context = 0L;
				long context2 = (1 < tus.Count) ? GetSegmentHash(tus[1].Source) : (-1);
				foreach (SearchResult result2 in results[0].Results)
				{
					result2.ContextData = new TuContext(context, context2);
				}
				for (int i = 1; i < tus.Count; i++)
				{
					context = ((i - 1 >= 0) ? GetSegmentHash(tus[i - 1].Source) : (-1));
					context2 = ((i + 1 < tus.Count) ? GetSegmentHash(tus[i + 1].Source) : 0);
					foreach (SearchResult result3 in results[i].Results)
					{
						result3.ContextData = new TuContext(context, context2);
					}
				}
				break;
			}
			}
		}

		private List<SearchResults> ExactSearchBatch(AnnotatedTranslationUnit[] tus, bool[] mask, int batchSize, bool applyHardFilterOnSql, List<AnnotatedTranslationUnit> notFoundTus)
		{
			List<SearchResults> list = new List<SearchResults>();
			Translator translator = Settings.ComputeTranslationProposal ? new Translator(Settings, TM.Tm.TextContextMatchType) : null;
			if (translator != null)
			{
				translator.SourceTokenizationContext = TM.SourceTools.GetTokenizationContext();
				translator.TargetTokenizationContext = TM.TargetTools.GetTokenizationContext();
			}
			for (int i = 0; i < tus.Length; i += batchSize)
			{
				List<SearchResults> list2 = new List<SearchResults>();
				List<long> list3 = new List<long>(batchSize);
				for (int j = i; j < tus.Length && j < i + batchSize; j++)
				{
					if (!list3.Contains(GetSegmentHash(tus[j].Source)) && (mask == null || mask[j]))
					{
						list3.Add(GetSegmentHash(tus[j].Source));
					}
				}
				List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> candidateResultTUs = _Context.Storage.ExactSearch(TM.Tm.ResourceId.Id, list3, applyHardFilterOnSql ? Settings.HardFilter : null);
				AddExactCandidateTUsToResults(tus, mask, i, batchSize, candidateResultTUs, list2, translator, notFoundTus);
				CapResults(list2, Settings.MaxResults);
				list.AddRange(list2);
			}
			return list;
		}

		private List<SearchResults> DuplicateSearchBatchInternal(AnnotatedTranslationUnit[] tus, bool[] mask)
		{
			List<SearchResults> list = new List<SearchResults>();
			List<long> list2 = new List<long>(tus.Length);
			list2.AddRange(from atu in tus.Where((AnnotatedTranslationUnit atu, int index) => mask == null || mask[index])
				select GetSegmentHash(atu.Source));
			List<long> list3 = new List<long>(tus.Length);
			list3.AddRange(from atu in tus.Where((AnnotatedTranslationUnit atu, int index) => mask == null || mask[index])
				select GetSegmentHash(atu.Target));
			List<SearchResults> list4 = AddDuplicateCandidateTUsToResults(candidateResultTUs: _Context.Storage.DuplicateSearch(TM.Tm.ResourceId.Id, list2, list3), searchTUs: tus, mask: mask);
			CapResults(list4, Settings.MaxResults);
			list.AddRange(list4);
			return list;
		}

		private void PopulateResultsAndApplyFilters(bool applyHardFilterOnSql, List<SearchResults> batchResults, FieldDefinitions fields, AnnotatedTranslationUnit[] tus)
		{
			DropResultsBelowMinimumScore(batchResults);
			PopulateResults(batchResults, fields, tus);
			if (!applyHardFilterOnSql && Settings.HardFilter != null)
			{
				ApplyHardFilters(batchResults);
			}
			ApplySoftFilters(batchResults);
		}

		private void FuzzySearchBatch(AnnotatedTranslationUnit[] tus, int batchSize, bool applyHardFilterOnSql, List<AnnotatedTranslationUnit> fuzzyTUs, List<SearchResults> results)
		{
			Translator translator = Settings.ComputeTranslationProposal ? new Translator(Settings, TM.Tm.TextContextMatchType) : null;
			if (translator != null)
			{
				translator.SourceTokenizationContext = TM.SourceTools.GetTokenizationContext();
				translator.TargetTokenizationContext = TM.TargetTools.GetTokenizationContext();
			}
			int minScore = Math.Max(SearchSettings.MinscoreLowerbound, Settings.MinScore - 20);
			int num = Settings.MaxResults;
			if (num < 20)
			{
				num = 20;
			}
			for (int j = 0; j < tus.Length; j += batchSize)
			{
				Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
				int i;
				for (i = j; i < tus.Length && i < j + batchSize; i++)
				{
					if (!fuzzyTUs.All((AnnotatedTranslationUnit t) => t != tus[i]))
					{
						dictionary.Add(i, tus[i].Source.TmFeatureVector);
					}
				}
				Dictionary<int, List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>> candidateResultTUs = _Context.Storage.FuzzySearch(TM.Tm.ResourceId.Id, dictionary, minScore, num, applyHardFilterOnSql ? Settings.HardFilter : null);
				AddFuzzyCandidateTUsToResults(tus, j, batchSize, candidateResultTUs, results, translator);
			}
		}

		private void ApplyHardFilters(IEnumerable<SearchResults> batchResults)
		{
			foreach (SearchResults batchResult in batchResults)
			{
				if (batchResult != null)
				{
					batchResult.Results = batchResult.Results.Where((SearchResult x) => Settings.HardFilter.Evaluate(x.MemoryTranslationUnit)).ToList();
				}
			}
		}

		private void ApplySoftFilters(List<SearchResults> batchResults)
		{
			foreach (SearchResults batchResult in batchResults)
			{
				if (batchResult != null)
				{
					foreach (SearchResult item in (IEnumerable<SearchResult>)batchResult)
					{
						_Scorer.ApplySoftFilters(item);
					}
				}
			}
			DropResultsBelowMinimumScore(batchResults);
		}

		private void DropResultsBelowMinimumScore(List<SearchResults> batchResults)
		{
			foreach (SearchResults batchResult in batchResults)
			{
				batchResult?.Results.RemoveAll((SearchResult x) => x.ScoringResult.Match < Settings.MinScore);
				if (Settings.SortSpecification != null && Settings.SortSpecification.Count > 0 && batchResult != null && batchResult.Results.Count > 1)
				{
					batchResult.Sort(Settings.SortSpecification);
				}
				else
				{
					batchResult?.Sort(_DefaultSortOrder);
				}
			}
		}

		private static void CapResults(List<SearchResults> results, int capSize)
		{
			foreach (SearchResults result in results)
			{
				result?.Cap(capSize);
			}
		}

		private TuContext GetMatchContext(AnnotatedSegment s)
		{
			return new TuContext(GetSegmentHash(s), -1L);
		}

		private static TuContext GetMatchContext(SearchResults r)
		{
			if (r == null || r.Count == 0 || r[0] == null)
			{
				return null;
			}
			if (!r.Results[0].ScoringResult.IsExactMatch)
			{
				return null;
			}
			return r.Results[0].ContextData;
		}

		public SearchResults[] Search(AnnotatedSegment[] segments, bool[] mask)
		{
			if (segments == null || segments.Length == 0)
			{
				throw new ArgumentNullException("segments");
			}
			if (mask != null && mask.Length != segments.Length)
			{
				throw new ArgumentException("If a mask is specified, its length must be equal to that of the segments collection");
			}
			SearchResults[] array = new SearchResults[segments.Length];
			long context = 0L;
			long context2 = 0L;
			if (TM.Tm.TextContextMatchType == TextContextMatchType.PrecedingAndFollowingSource && segments.Length > 1)
			{
				context2 = ((segments[1] == null) ? (-1) : GetSegmentHash(segments[1]));
			}
			TuContext tuContext = new TuContext(context, context2);
			TuContext textContext = Settings.IsDocumentSearch ? tuContext : null;
			for (int i = 0; i < segments.Length; i++)
			{
				if (segments[i] == null)
				{
					textContext = null;
					array[i] = null;
					continue;
				}
				if (mask != null && !mask[i])
				{
					if (Settings.IsDocumentSearch)
					{
						textContext = GetMatchContext(segments[i]);
					}
					array[i] = null;
					continue;
				}
				if (Settings.IsDocumentSearch && TM.Tm.TextContextMatchType == TextContextMatchType.PrecedingAndFollowingSource && i > 0)
				{
					long context3 = (segments[i - 1] == null) ? (-1) : GetSegmentHash(segments[i - 1]);
					context2 = 0L;
					if (i < segments.Length - 1)
					{
						context2 = ((segments[i + 1] == null) ? (-1) : GetSegmentHash(segments[i + 1]));
					}
					textContext = new TuContext(context3, context2);
				}
				TuContextData tuContextData = new TuContextData
				{
					TextContext = textContext
				};
				array[i] = SearchInternal(segments[i], null, tuContextData);
				if (Settings.IsDocumentSearch)
				{
					textContext = GetMatchContext(array[i]);
				}
			}
			return array;
		}

		public SearchResults Search(string text)
		{
			bool flag = Settings.Mode == SearchMode.TargetConcordanceSearch;
			Sdl.LanguagePlatform.Core.Segment segment = new Sdl.LanguagePlatform.Core.Segment(flag ? TM.Tm.LanguageDirection.TargetCulture : TM.Tm.LanguageDirection.SourceCulture);
			segment.Add(text);
			AnnotatedSegment annotatedSegment = new AnnotatedSegment(TM, segment, flag, keepTokens: false, keepPeripheralWhitespace: false);
			if (!flag)
			{
				return SearchInternal(annotatedSegment, null, new TuContextData());
			}
			return SearchInternal(null, annotatedSegment, new TuContextData());
		}

		public SearchResults Search(AnnotatedSegment segment)
		{
			if (Settings.Mode != SearchMode.TargetConcordanceSearch)
			{
				return SearchInternal(segment, null, new TuContextData());
			}
			return SearchInternal(null, segment, new TuContextData());
		}

		private ErrorCode CheckLanguages(IAnnotatedSegment docSrcSegment, IAnnotatedSegment docTrgSegment)
		{
			ErrorCode errorCode = CheckSourceLanguage(docSrcSegment);
			if (errorCode == ErrorCode.OK)
			{
				return CheckTargetLanguage(docTrgSegment);
			}
			return errorCode;
		}

		private ErrorCode CheckSourceLanguage(IAnnotatedSegment docSrcSegment)
		{
			if (docSrcSegment == null)
			{
				return ErrorCode.OK;
			}
			if (docSrcSegment.Segment.Culture == null)
			{
				return ErrorCode.TMSourceLanguageMismatch;
			}
			if (docSrcSegment.Segment.Culture.Equals(TM.Tm.LanguageDirection.SourceCulture))
			{
				return ErrorCode.OK;
			}
			if (Settings.CheckMatchingSublanguages || !CultureInfoExtensions.AreCompatible(TM.Tm.LanguageDirection.SourceCulture, docSrcSegment.Segment.Culture))
			{
				return ErrorCode.TMSourceLanguageMismatch;
			}
			return ErrorCode.OK;
		}

		private ErrorCode CheckTargetLanguage(IAnnotatedSegment docTrgSegment)
		{
			if (docTrgSegment == null)
			{
				return ErrorCode.OK;
			}
			if (docTrgSegment.Segment.Culture == null)
			{
				return ErrorCode.TMTargetLanguageMismatch;
			}
			if (docTrgSegment.Segment.Culture.Equals(TM.Tm.LanguageDirection.TargetCulture))
			{
				return ErrorCode.OK;
			}
			if (Settings.CheckMatchingSublanguages || !CultureInfoExtensions.AreCompatible(TM.Tm.LanguageDirection.TargetCulture, docTrgSegment.Segment.Culture))
			{
				return ErrorCode.TMTargetLanguageMismatch;
			}
			docTrgSegment.Segment.Culture = TM.Tm.LanguageDirection.TargetCulture;
			return ErrorCode.OK;
		}

		private void SetSearchParameters(out int maxTuId, out bool descendingOrder, out DateTime lastChangeDate)
		{
			SetSearchParameters(out maxTuId, out descendingOrder, Settings, out lastChangeDate);
		}

		internal static void SetSearchParameters(out int maxTuId, out bool descendingOrder, SearchSettings settings, out DateTime lastChangeDate)
		{
			descendingOrder = (settings.SortSpecification == null || settings.SortSpecification.Criteria.All((SortCriterium criteria) => criteria.Direction != 0 || criteria.FieldName != "chd"));
			maxTuId = (descendingOrder ? int.MaxValue : (-1));
			DateTime dateTime = new DateTime(1800, 1, 1);
			lastChangeDate = (descendingOrder ? DateTime.MaxValue : dateTime);
		}

		private SearchResults SearchInternal(AnnotatedSegment docSrcSegment, AnnotatedSegment docTrgSegment, TuContextData tuContextData)
		{
			bool isConcordanceSearch = Settings.IsConcordanceSearch;
			if (Settings.AutoLocalizationSettings != null && Settings.AutoLocalizationSettings.DisableAutoSubstitution.HasFlag(BuiltinRecognizers.RecognizeAcronyms))
			{
				Settings.AutoLocalizationSettings.DisableAutoSubstitution &= ~BuiltinRecognizers.RecognizeAcronyms;
			}
			bool num = Settings.Mode == SearchMode.TargetConcordanceSearch;
			if (num)
			{
				if (docTrgSegment == null)
				{
					throw new ArgumentNullException("docTrgSegment");
				}
			}
			else if (docSrcSegment == null)
			{
				throw new ArgumentNullException("docSrcSegment");
			}
			if (Settings.Mode == SearchMode.DuplicateSearch && docTrgSegment == null)
			{
				throw new ArgumentNullException("docTrgSegment", "Target segment cannot be null for duplicate search");
			}
			if (Settings.MinScore < SearchSettings.MinscoreLowerbound)
			{
				Settings.MinScore = SearchSettings.MinscoreLowerbound;
			}
			if (Settings.MaxResults <= 0)
			{
				Settings.MaxResults = 1;
			}
			ErrorCode errorCode = CheckLanguages(docSrcSegment, docTrgSegment);
			if (errorCode != 0)
			{
				throw new LanguagePlatformException(errorCode);
			}
			SearchResults searchResults = new SearchResults(Settings.SortSpecification);
			if (num)
			{
				TM.TargetTools.EnsureTokenizedSegment(docTrgSegment.Segment);
			}
			else
			{
				searchResults.SourceSegment = docSrcSegment.Segment;
				bool allowTokenBundles = !isConcordanceSearch && docTrgSegment != null;
				TM.SourceTools.EnsureTokenizedSegment(docSrcSegment.Segment, forceRetokenization: false, allowTokenBundles);
				if (docTrgSegment != null)
				{
					TM.TargetTools.EnsureTokenizedSegment(docTrgSegment.Segment, forceRetokenization: false, allowTokenBundles);
					searchResults.DocumentPlaceables = PlaceableComputer.ComputePlaceables(docSrcSegment.Segment, docTrgSegment.Segment);
				}
				else if (!isConcordanceSearch)
				{
					searchResults.DocumentPlaceables = PlaceableComputer.ComputePlaceables(docSrcSegment.Segment, null);
				}
				searchResults.SourceHash = GetSegmentHash(docSrcSegment);
				WordCountsOptions wordCountsOptions = new WordCountsOptions();
				wordCountsOptions.BreakOnTag = TM.Tm.WordCountFlags.HasFlag(WordCountFlags.BreakOnTag);
				wordCountsOptions.BreakOnHyphen = TM.Tm.WordCountFlags.HasFlag(WordCountFlags.BreakOnHyphen);
				wordCountsOptions.BreakOnDash = TM.Tm.WordCountFlags.HasFlag(WordCountFlags.BreakOnDash);
				wordCountsOptions.BreakOnApostrophe = TM.Tm.WordCountFlags.HasFlag(WordCountFlags.BreakOnApostrophe);
				wordCountsOptions.BreakAdvancedTokensByCharacter = Settings.AdvancedTokenizationLegacyScoring;
				searchResults.SourceWordCounts = new WordCounts(docSrcSegment.Segment.Tokens, wordCountsOptions, docSrcSegment.Segment.Culture);
			}
			SetSearchParameters(out int maxTuId, out bool descendingOrder, out DateTime lastChangeDate);
			int num2 = Math.Max(SearchSettings.MinscoreLowerbound, Settings.MinScore - 20);
			int num3 = Settings.MaxResults;
			if (num3 < 20 && Settings.Mode != SearchMode.DuplicateSearch)
			{
				num3 = 20;
			}
			int num4 = Math.Max(50, num3);
			if (_exactSearchTestPageSize > -1)
			{
				num4 = _exactSearchTestPageSize;
				num3 = _exactSearchTestPageSize;
			}
			List<int> list = new List<int>();
			int skipRows = 0;
			switch (Settings.Mode)
			{
			case SearchMode.ExactSearch:
			case SearchMode.NormalSearch:
			case SearchMode.FullSearch:
			case SearchMode.DuplicateSearch:
			{
				Translator translator = Settings.ComputeTranslationProposal ? new Translator(Settings, TM.Tm.TextContextMatchType) : null;
				if (translator != null)
				{
					translator.SourceTokenizationContext = TM.SourceTools.GetTokenizationContext();
					translator.TargetTokenizationContext = TM.TargetTools.GetTokenizationContext();
				}
				if (Settings.Mode != SearchMode.DuplicateSearch)
				{
					for (int i = 0; i < docSrcSegment.Segment.Tokens.Count; i++)
					{
						if (docSrcSegment.Segment.Tokens[i].Type == TokenType.Acronym)
						{
							list.Add(i);
						}
					}
				}
				List<int> list2 = new List<int>();
				bool flag = false;
				int num5;
				bool unmatchedAcronyms;
				int num6;
				do
				{
					List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list3;
					if (Settings.Mode == SearchMode.DuplicateSearch)
					{
						list3 = _Context.Storage.ExactSearch(TM.Tm.ResourceId.Id, GetSegmentHash(docSrcSegment), GetSegmentHash(docTrgSegment), num3, lastChangeDate, skipRows, tuContextData, descendingOrder, null);
					}
					else
					{
						List<int> tuIdsToSkip = null;
						if (list2.Count > 0 && string.IsNullOrEmpty(tuContextData.IdContext))
						{
							tuIdsToSkip = list2;
						}
						list3 = _Context.Storage.ExactSearch(TM.Tm.ResourceId.Id, GetSegmentHash(docSrcSegment), 0L, num4, lastChangeDate, skipRows, tuContextData, descendingOrder, tuIdsToSkip);
						if (!string.IsNullOrEmpty(tuContextData.IdContext))
						{
							list2.AddRange(list3.Select((Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit x) => x.Id));
						}
					}
					if (skipRows == 0)
					{
						skipRows = 1;
					}
					num5 = AddTUsToResult(docSrcSegment, docTrgSegment, searchResults, list3, FuzzyIndexes.SourceWordBased, translator, tuContextData, ref maxTuId, ref lastChangeDate, descendingOrder, out bool idContextMatchingCandidatesFound, out bool idContextMatchingCandidatesExhausted, list, out unmatchedAcronyms, ref skipRows);
					flag |= idContextMatchingCandidatesFound;
					if (list3.Count < num4)
					{
						idContextMatchingCandidatesExhausted = true;
					}
					if (!flag)
					{
						list2.Clear();
					}
					num6 = list3.Count;
					if (string.IsNullOrEmpty(tuContextData.IdContext))
					{
						continue;
					}
					if (idContextMatchingCandidatesExhausted)
					{
						tuContextData.IdContext = string.Empty;
						num6 = num4;
						if (flag)
						{
							SetSearchParameters(out maxTuId, out descendingOrder, out lastChangeDate);
							skipRows = 0;
						}
						list2 = new List<int>(new HashSet<int>(list2));
						flag = false;
					}
					else if (num5 > 0 && searchResults.Any((SearchResult x) => x.ScoringResult.IdContextMatch))
					{
						tuContextData.IdContext = string.Empty;
					}
				}
				while (num6 == num4 && ((num5 == 0 || !string.IsNullOrEmpty(tuContextData.IdContext)) | unmatchedAcronyms));
				searchResults.CheckForMultipleTranslations(Settings);
				if (((searchResults.Count == 0 && Settings.Mode == SearchMode.NormalSearch) || Settings.Mode == SearchMode.FullSearch) && (TM.Tm.FuzzyIndexes & FuzzyIndexes.SourceWordBased) != 0 && docSrcSegment.TmFeatureVector != null && docSrcSegment.TmFeatureVector.Count > 0 && Settings.MinScore < 100)
				{
					maxTuId = (descendingOrder ? int.MaxValue : (-1));
					List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list3 = _Context.Storage.FuzzySearch(TM.Tm.ResourceId.Id, docSrcSegment.TmFeatureVector, FuzzyIndexes.SourceWordBased, num2, num3, concordance: false, maxTuId, tuContextData, descendingOrder);
					int skipRows2 = 0;
					AddTUsToResult(docSrcSegment, docTrgSegment, searchResults, list3, FuzzyIndexes.SourceWordBased, translator, tuContextData, ref maxTuId, ref lastChangeDate, descendingOrder, out bool _, out bool _, null, out bool _, ref skipRows2);
				}
				if (searchResults.Count == 0 && translator != null && Settings.Mode != SearchMode.DuplicateSearch)
				{
					AttemptVirtualTranslation(docSrcSegment, searchResults, translator);
				}
				break;
			}
			case SearchMode.ConcordanceSearch:
				RunConcordanceSearch(docSrcSegment, isSource: true, num2, num3, ref maxTuId, searchResults, descendingOrder);
				break;
			case SearchMode.TargetConcordanceSearch:
				RunConcordanceSearch(docTrgSegment, isSource: false, num2, num3, ref maxTuId, searchResults, descendingOrder);
				break;
			}
			if (searchResults == null)
			{
				return searchResults;
			}
			searchResults.Results.RemoveAll((SearchResult r) => r.ScoringResult.Match < Settings.MinScore);
			if (Settings.SortSpecification != null && Settings.SortSpecification.Count > 0 && searchResults.Count > 1)
			{
				searchResults.Sort(Settings.SortSpecification);
			}
			else
			{
				searchResults.Sort(_DefaultSortOrder);
			}
			if (searchResults.Count > Settings.MaxResults)
			{
				searchResults.Cap(Settings.MaxResults);
			}
			return searchResults;
		}

		private void AttemptVirtualTranslation(AnnotatedSegment docSrcSegment, SearchResults results, Translator translator)
		{
			if (translator == null)
			{
				throw new ArgumentNullException("translator");
			}
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			if (docSrcSegment == null)
			{
				throw new ArgumentNullException("docSrcSegment");
			}
			SearchResult searchResult = translator.AttemptVirtualTranslation(docSrcSegment.Segment, TM.Tm);
			if (searchResult != null)
			{
				results.Add(searchResult);
			}
		}

		private void RunConcordanceSearch(AnnotatedSegment segment, bool isSource, int adjustedMinscore, int adjustedMaxResults, ref int maxTuId, SearchResults results, bool descendingOrder)
		{
			DateTime lastChangeDate = DateTime.MinValue;
			FuzzyIndexes fuzzyIndexes;
			FuzzyIndexes fuzzyIndexes2;
			if (isSource)
			{
				fuzzyIndexes = FuzzyIndexes.SourceCharacterBased;
				fuzzyIndexes2 = FuzzyIndexes.SourceWordBased;
			}
			else
			{
				fuzzyIndexes = FuzzyIndexes.TargetCharacterBased;
				fuzzyIndexes2 = FuzzyIndexes.TargetWordBased;
			}
			if ((TM.Tm.FuzzyIndexes & fuzzyIndexes) == 0 && (TM.Tm.FuzzyIndexes & fuzzyIndexes2) == 0)
			{
				throw new LanguagePlatformException(ErrorCode.TMSearchModeNotSupported);
			}
			bool idContextMatchingCandidatesFound;
			bool idContextMatchingCandidatesExhausted;
			bool unmatchedAcronyms;
			List<int> tmFeatureVector;
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list;
			if ((TM.Tm.FuzzyIndexes & fuzzyIndexes2) != 0)
			{
				tmFeatureVector = segment.TmFeatureVector;
				if (tmFeatureVector != null && tmFeatureVector.Count > 0)
				{
					do
					{
						list = _Context.Storage.FuzzySearch(TM.Tm.ResourceId.Id, tmFeatureVector, fuzzyIndexes2, adjustedMinscore, adjustedMaxResults, concordance: true, maxTuId, new TuContextData(), descendingOrder);
						if (list != null && list.Count > 0)
						{
							int skipRows = 0;
							if (isSource)
							{
								AddTUsToResult(segment, null, results, list, fuzzyIndexes2, null, new TuContextData(), ref maxTuId, ref lastChangeDate, descendingOrder, out idContextMatchingCandidatesFound, out idContextMatchingCandidatesExhausted, null, out unmatchedAcronyms, ref skipRows);
							}
							else
							{
								AddTUsToResult(null, segment, results, list, fuzzyIndexes2, null, new TuContextData(), ref maxTuId, ref lastChangeDate, descendingOrder, out unmatchedAcronyms, out idContextMatchingCandidatesExhausted, null, out idContextMatchingCandidatesFound, ref skipRows);
							}
						}
					}
					while (list != null && list.Count == adjustedMaxResults && results.Count < adjustedMaxResults);
				}
			}
			if ((TM.Tm.FuzzyIndexes & fuzzyIndexes) == 0)
			{
				return;
			}
			maxTuId = 0;
			tmFeatureVector = segment.ConcordanceFeatureVector;
			if (tmFeatureVector == null || tmFeatureVector.Count <= 0)
			{
				return;
			}
			do
			{
				list = _Context.Storage.FuzzySearch(TM.Tm.ResourceId.Id, tmFeatureVector, fuzzyIndexes, adjustedMinscore, adjustedMaxResults, concordance: true, maxTuId, new TuContextData(), descendingOrder: false);
				if (list != null && list.Count > 0)
				{
					int skipRows2 = 0;
					if (isSource)
					{
						AddTUsToResult(segment, null, results, list, fuzzyIndexes, null, new TuContextData(), ref maxTuId, ref lastChangeDate, orderDescending: false, out idContextMatchingCandidatesFound, out idContextMatchingCandidatesExhausted, null, out unmatchedAcronyms, ref skipRows2);
					}
					else
					{
						AddTUsToResult(null, segment, results, list, fuzzyIndexes, null, new TuContextData(), ref maxTuId, ref lastChangeDate, orderDescending: false, out unmatchedAcronyms, out idContextMatchingCandidatesExhausted, null, out idContextMatchingCandidatesFound, ref skipRows2);
					}
				}
			}
			while (list != null && list.Count == adjustedMaxResults && results.Count < adjustedMaxResults);
		}

		private static void SetLastTuid(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu, ref int lastTuId, ref DateTime lastChangeDate, bool orderDescending, ref int skipRows)
		{
			if (tu.Id > lastTuId && !orderDescending)
			{
				lastTuId = tu.Id;
			}
			if (tu.Id < lastTuId && orderDescending)
			{
				lastTuId = tu.Id;
			}
			if (tu.ChangeDate > lastChangeDate && !orderDescending)
			{
				lastChangeDate = tu.ChangeDate;
				skipRows = 1;
			}
			else if (tu.ChangeDate < lastChangeDate && orderDescending)
			{
				lastChangeDate = tu.ChangeDate;
				skipRows = 1;
			}
			else if (tu.ChangeDate == lastChangeDate)
			{
				skipRows++;
			}
		}

		internal bool AreTusDuplicates(AnnotatedTranslationUnit atu, List<Placeable> atuPlaceables, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memTu)
		{
			TM.SourceTools.EnsureTokenizedSegment(memTu.SourceSegment);
			TM.TargetTools.EnsureTokenizedSegment(memTu.TargetSegment);
			if (memTu.SourceSegment.Tokens.Count != atu.Source.Segment.Tokens.Count || memTu.TargetSegment.Tokens.Count != atu.Target.Segment.Tokens.Count)
			{
				return false;
			}
			SearchResult searchResult = new SearchResult(memTu);
			_Scorer.ComputeScores(searchResult, atu.Source, atu.Target, atuPlaceables, new TuContextData
			{
				IdContext = string.Empty
			}, isDuplicateSearch: true, FuzzyIndexes.SourceWordBased);
			if (searchResult.ScoringResult.IsExactMatch)
			{
				return !searchResult.ScoringResult.TargetSegmentDiffers;
			}
			return false;
		}

		private int AddTUsToResult(IAnnotatedSegment docSrcSegment, IAnnotatedSegment docTrgSegment, SearchResults results, IReadOnlyCollection<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tus, FuzzyIndexes usedIndex, Translator translator, TuContextData tuContextData, ref int lastTuId, ref DateTime lastChangeDate, bool orderDescending, out bool idContextMatchingCandidatesFound, out bool idContextMatchingCandidatesExhausted, IReadOnlyCollection<int> acronymTokenIndices, out bool unmatchedAcronyms, ref int skipRows)
		{
			idContextMatchingCandidatesExhausted = false;
			idContextMatchingCandidatesFound = false;
			unmatchedAcronyms = (acronymTokenIndices != null && acronymTokenIndices.Count > 0);
			if (tus == null || tus.Count == 0)
			{
				idContextMatchingCandidatesExhausted = true;
				return 0;
			}
			int num = 0;
			bool flag = false;
			SortedList<int, bool> sortedList = new SortedList<int, bool>();
			foreach (SearchResult item in (IEnumerable<SearchResult>)results)
			{
				sortedList.Add(item.MemoryTranslationUnit.ResourceId.Id, value: true);
			}
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu in tus)
			{
				SetLastTuid(tu, ref lastTuId, ref lastChangeDate, orderDescending, ref skipRows);
				if (!sortedList.ContainsKey(tu.Id))
				{
					Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = _Context.ResourceManager.GetTranslationUnit(tu, TM.Tm.FieldDeclarations, TM.Tm.LanguageDirection.SourceCulture, TM.Tm.LanguageDirection.TargetCulture);
					if (Settings.HardFilter == null || Settings.HardFilter.Evaluate(translationUnit))
					{
						SearchResult searchResult = new SearchResult(translationUnit);
						if (!Settings.IsConcordanceSearch)
						{
							TM.SourceTools.EnsureTokenizedSegment(searchResult.MemoryTranslationUnit.SourceSegment, forceRetokenization: false, allowTokenBundles: true);
							TM.TargetTools.EnsureTokenizedSegment(searchResult.MemoryTranslationUnit.TargetSegment, forceRetokenization: false, allowTokenBundles: true);
						}
						if (Settings.Mode != SearchMode.DuplicateSearch || (translationUnit.SourceSegment.Tokens.Count == docSrcSegment.Segment.Tokens.Count && translationUnit.TargetSegment.Tokens.Count == docTrgSegment.Segment.Tokens.Count))
						{
							if (!Settings.IsConcordanceSearch)
							{
								searchResult.MemoryPlaceables = PlaceableComputer.ComputePlaceables(translationUnit);
							}
							if (!Settings.IsDocumentSearch)
							{
								tuContextData.TextContext = null;
							}
							searchResult.ScoringResult = new ScoringResult();
							_Scorer.CheckContexts(searchResult, tuContextData);
							if (searchResult.ScoringResult.TextContextMatch != TextContextMatch.PrecedingAndFollowingSourceMatch)
							{
								_ = searchResult.ScoringResult.TextContextMatch;
								_ = 2;
							}
							if (!searchResult.ScoringResult.IdContextMatch)
							{
								idContextMatchingCandidatesExhausted = true;
							}
							else
							{
								idContextMatchingCandidatesFound = true;
							}
							_Scorer.ComputeScores(searchResult, docSrcSegment, docTrgSegment, results.DocumentPlaceables, tuContextData, Settings.Mode == SearchMode.DuplicateSearch, usedIndex);
							if (!searchResult.ScoringResult.IsExactMatch)
							{
								searchResult.ScoringResult.TextContextMatch = TextContextMatch.NoMatch;
								searchResult.ScoringResult.IsStructureContextMatch = false;
								searchResult.ScoringResult.IdContextMatch = false;
							}
							if (searchResult.ScoringResult.Match >= Settings.MinScore)
							{
								if (translator != null && docSrcSegment != null)
								{
									translator.CreateTranslationProposal(searchResult, docSrcSegment.Segment);
								}
								if (searchResult.ScoringResult.IsExactMatch & unmatchedAcronyms)
								{
									if (searchResult.ScoringResult.TextReplacements == 0)
									{
										unmatchedAcronyms = false;
									}
									else
									{
										bool flag2 = true;
										foreach (int acroIndex in acronymTokenIndices)
										{
											PlaceableAssociation placeableAssociation = searchResult.PlaceableAssociations.Find((PlaceableAssociation x) => x.Document.SourceTokenIndex == acroIndex);
											if (placeableAssociation?.Memory == null || placeableAssociation.Memory.SourceTokenIndex < 0 || string.CompareOrdinal(docSrcSegment.Segment.Tokens[acroIndex].Text, translationUnit.SourceSegment.Tokens[placeableAssociation.Memory.SourceTokenIndex].Text) != 0)
											{
												flag2 = false;
												break;
											}
										}
										unmatchedAcronyms = !flag2;
									}
								}
								if (searchResult.ScoringResult.Match >= Settings.MinScore && (Settings.Mode != 0 || searchResult.ScoringResult.IsExactMatch))
								{
									searchResult.ContextData = new TuContext(tu.Source.Hash, tu.Target.Hash);
									results.Add(searchResult);
									sortedList.Add(searchResult.MemoryTranslationUnit.ResourceId.Id, value: true);
									num++;
									if (searchResult.ScoringResult.IsExactMatch)
									{
										flag = true;
									}
								}
							}
						}
					}
				}
			}
			if (flag && Settings.Mode == SearchMode.NormalSearch)
			{
				results.RemoveAll((SearchResult i) => !i.ScoringResult.IsExactMatch && i.ScoringResult.Match < 95);
			}
			return num;
		}

		private void AddExactCandidateTUsToResults(IReadOnlyList<AnnotatedTranslationUnit> searchTUs, IReadOnlyList<bool> mask, int index, int batchsize, List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> candidateResultTUs, ICollection<SearchResults> batchResults, Translator translator, ICollection<AnnotatedTranslationUnit> notFoundTus)
		{
			List<Filter> filters = Settings.Filters;
			Settings.Filters = null;
			for (int i = index; i < searchTUs.Count && i < index + batchsize; i++)
			{
				AnnotatedTranslationUnit searchTu = searchTUs[i];
				SearchResults searchResults = new SearchResults();
				if (mask[i])
				{
					batchResults.Add(searchResults);
					List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list = candidateResultTUs.FindAll((Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit x) => x.Source.Hash == GetSegmentHash(searchTu.Source)).ToList();
					InitializeTuSearchResults(searchTu, searchResults);
					foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in list)
					{
						AddTuToSearchResults(translator, searchTu, searchResults, item);
					}
					searchResults.CheckForMultipleTranslations(Settings);
					if (!list.Any())
					{
						notFoundTus.Add(searchTUs[i]);
					}
				}
				else
				{
					batchResults.Add(null);
				}
			}
			Settings.Filters = filters;
		}

		private List<SearchResults> AddDuplicateCandidateTUsToResults(AnnotatedTranslationUnit[] searchTUs, bool[] mask, List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> candidateResultTUs)
		{
			List<SearchResults> list = new List<SearchResults>();
			for (int i = 0; i < searchTUs.Length; i++)
			{
				AnnotatedTranslationUnit searchTu = searchTUs[i];
				SearchResults searchResults = new SearchResults();
				if (mask == null || mask[i])
				{
					list.Add(searchResults);
					List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list2 = candidateResultTUs.FindAll((Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit x) => x.Source.Hash == GetSegmentHash(searchTu.Source) && x.Target.Hash == GetSegmentHash(searchTu.Target)).ToList();
					InitializeTuSearchResults(searchTu, searchResults);
					foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in list2)
					{
						AddDuplicateTuToSearchResults(searchTu, searchResults, item);
					}
				}
				else
				{
					list.Add(null);
				}
			}
			return list;
		}

		private void AddFuzzyCandidateTUsToResults(AnnotatedTranslationUnit[] searchTUs, int index, int batchsize, Dictionary<int, List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>> candidateResultTUs, List<SearchResults> searchResults, Translator translator)
		{
			List<Filter> filters = Settings.Filters;
			Settings.Filters = null;
			for (int i = index; i < searchTUs.Length && i < index + batchsize; i++)
			{
				AnnotatedTranslationUnit searchTu = searchTUs[i];
				if (candidateResultTUs.ContainsKey(i))
				{
					foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in candidateResultTUs[i])
					{
						AddTuToSearchResults(translator, searchTu, searchResults[i], item);
					}
				}
			}
			Settings.Filters = filters;
		}

		private void InitializeTuSearchResults(AnnotatedTranslationUnit searchTu, SearchResults results)
		{
			results.SourceSegment = searchTu.Source.Segment;
			results.SourceHash = GetSegmentHash(searchTu.Source);
			TM.SourceTools.EnsureTokenizedSegment(searchTu.Source.Segment, forceRetokenization: false, searchTu.Target.Segment != null);
			if (searchTu.Target != null)
			{
				TM.TargetTools.EnsureTokenizedSegment(searchTu.Target.Segment, forceRetokenization: false, allowTokenBundles: true);
				results.DocumentPlaceables = PlaceableComputer.ComputePlaceables(searchTu.Source.Segment, searchTu.Target.Segment);
			}
			else
			{
				results.DocumentPlaceables = PlaceableComputer.ComputePlaceables(searchTu.Source.Segment, null);
			}
		}

		private void AddTuToSearchResults(Translator translator, AnnotatedTranslationUnit searchTu, SearchResults results, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu)
		{
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit reducedTranslationUnit = _Context.ResourceManager.GetReducedTranslationUnit(tu, TM.Tm.LanguageDirection.SourceCulture, TM.Tm.LanguageDirection.TargetCulture);
			SearchResult searchResult = new SearchResult(reducedTranslationUnit);
			TM.SourceTools.EnsureTokenizedSegment(searchResult.MemoryTranslationUnit.SourceSegment, forceRetokenization: false, allowTokenBundles: true);
			TM.TargetTools.EnsureTokenizedSegment(searchResult.MemoryTranslationUnit.TargetSegment, forceRetokenization: false, allowTokenBundles: true);
			searchResult.MemoryPlaceables = PlaceableComputer.ComputePlaceables(reducedTranslationUnit);
			_Scorer.ComputeScores(searchResult, searchTu.Source, searchTu.Target, results.DocumentPlaceables, new TuContextData(), Settings.Mode == SearchMode.DuplicateSearch, FuzzyIndexes.SourceWordBased);
			if (searchResult.ScoringResult.Match >= Settings.MinScore)
			{
				if (translator != null && searchTu.Source.Segment != null)
				{
					translator.CreateTranslationProposal(searchResult, searchTu.Source.Segment);
				}
				results.Add(searchResult);
			}
		}

		private void AddDuplicateTuToSearchResults(AnnotatedTranslationUnit searchTu, SearchResults results, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu)
		{
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit reducedTranslationUnit = _Context.ResourceManager.GetReducedTranslationUnit(tu, TM.Tm.LanguageDirection.SourceCulture, TM.Tm.LanguageDirection.TargetCulture);
			SearchResult searchResult = new SearchResult(reducedTranslationUnit);
			TM.SourceTools.EnsureTokenizedSegment(reducedTranslationUnit.SourceSegment, forceRetokenization: false, reducedTranslationUnit.TargetSegment != null);
			TM.TargetTools.EnsureTokenizedSegment(reducedTranslationUnit.TargetSegment, forceRetokenization: false, reducedTranslationUnit.SourceSegment != null);
			if (reducedTranslationUnit.SourceSegment.Tokens.Count == searchTu.Source.Segment.Tokens.Count && reducedTranslationUnit.TargetSegment.Tokens.Count == searchTu.Target.Segment.Tokens.Count)
			{
				_Scorer.ComputeScores(searchResult, searchTu.Source, searchTu.Target, results.DocumentPlaceables, new TuContextData(), Settings.Mode == SearchMode.DuplicateSearch, FuzzyIndexes.SourceWordBased);
				if (searchResult.ScoringResult.Match >= Settings.MinScore)
				{
					results.Add(searchResult);
				}
			}
		}

		private void PopulateResults(List<SearchResults> allresults, FieldDefinitions fields, AnnotatedTranslationUnit[] searchTus)
		{
			List<Tuple<int, long, long>> list = new List<Tuple<int, long, long>>();
			Dictionary<int, List<SearchResult>> tuIndex = new Dictionary<int, List<SearchResult>>();
			foreach (SearchResults allresult in allresults)
			{
				if (allresult != null)
				{
					PrepareTUsSearchInput(list, tuIndex, allresult);
				}
			}
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> tus = _Context.Storage.GetTus(TM.Tm.ResourceId.Id, list, TM.Tm.IdContextMatch);
			Dictionary<int, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> dictionary = new Dictionary<int, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit>();
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit item in tus)
			{
				dictionary.Add(item.Id, item);
			}
			TuContext searchTuContext = new TuContext(0L, 0L);
			bool usePreviousTuForContext = false;
			foreach (SearchResults allresult2 in allresults)
			{
				if (allresult2 == null)
				{
					usePreviousTuForContext = true;
				}
				else
				{
					TuContext tuContext = null;
					foreach (SearchResult item2 in (IEnumerable<SearchResult>)allresult2)
					{
						Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu = dictionary[item2.MemoryTranslationUnit.ResourceId.Id];
						PopulateTuFromStorage(item2, storageTu, fields, searchTuContext, usePreviousTuForContext);
						if (tuContext == null)
						{
							long segmentHash = GetSegmentHash(new AnnotatedSegment(TM, item2.MemoryTranslationUnit.SourceSegment, isTargetSegment: true, keepTokens: false, keepPeripheralWhitespace: true));
							long segmentHash2 = GetSegmentHash(new AnnotatedSegment(TM, item2.MemoryTranslationUnit.TargetSegment, isTargetSegment: true, keepTokens: false, keepPeripheralWhitespace: true));
							tuContext = new TuContext(segmentHash, segmentHash2);
						}
					}
					if (tuContext != null)
					{
						searchTuContext = tuContext;
					}
					else
					{
						if (allresult2.Results.Count == 0)
						{
							searchTuContext = new TuContext(-1L, -1L);
							continue;
						}
						Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memoryTranslationUnit = allresult2.Results[0].MemoryTranslationUnit;
						long segmentHash3 = GetSegmentHash(new AnnotatedSegment(TM, memoryTranslationUnit.SourceSegment, isTargetSegment: true, keepTokens: false, keepPeripheralWhitespace: true));
						long segmentHash4 = GetSegmentHash(new AnnotatedSegment(TM, memoryTranslationUnit.TargetSegment, isTargetSegment: true, keepTokens: false, keepPeripheralWhitespace: true));
						searchTuContext = new TuContext(segmentHash3, segmentHash4);
					}
					usePreviousTuForContext = false;
				}
			}
			if (!TM.Tm.IdContextMatch)
			{
				return;
			}
			for (int i = 0; i < allresults.Count; i++)
			{
				if (allresults[i] == null)
				{
					continue;
				}
				AnnotatedTranslationUnit annotatedTranslationUnit = searchTus[i];
				if (annotatedTranslationUnit.TranslationUnit.IdContexts != null && annotatedTranslationUnit.TranslationUnit.IdContexts.Values.Count != 0)
				{
					string text = annotatedTranslationUnit.TranslationUnit.IdContexts.Values.First();
					if (!string.IsNullOrEmpty(text))
					{
						foreach (SearchResult result in allresults[i].Results)
						{
							Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memoryTranslationUnit2 = result.MemoryTranslationUnit;
							if (memoryTranslationUnit2.IdContexts != null && memoryTranslationUnit2.IdContexts.Values.Contains(text))
							{
								result.ScoringResult.IdContextMatch = true;
							}
						}
					}
				}
			}
		}

		private static void PrepareTUsSearchInput(List<Tuple<int, long, long>> searchInput, Dictionary<int, List<SearchResult>> tuIndex, SearchResults results)
		{
			foreach (SearchResult item2 in (IEnumerable<SearchResult>)results)
			{
				TuContext contextData = item2.ContextData;
				Tuple<int, long, long> item = new Tuple<int, long, long>(item2.MemoryTranslationUnit.ResourceId.Id, contextData?.Context1 ?? (-1), contextData?.Context2 ?? (-1));
				if (!searchInput.Contains(item))
				{
					searchInput.Add(item);
				}
				if (!tuIndex.ContainsKey(item2.MemoryTranslationUnit.ResourceId.Id))
				{
					tuIndex.Add(item2.MemoryTranslationUnit.ResourceId.Id, new List<SearchResult>
					{
						item2
					});
				}
				else
				{
					tuIndex[item2.MemoryTranslationUnit.ResourceId.Id].Add(item2);
				}
			}
		}

		private void PopulateTuFromStorage(SearchResult result, Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit storageTu, FieldDefinitions fields, TuContext searchTuContext, bool usePreviousTuForContext)
		{
			result.ScoringResult.TextContextMatch = TextContextMatch.NoMatch;
			_Context.ResourceManager.FillRemainingTranslationUnitDetails(result.MemoryTranslationUnit, storageTu, fields);
			if (result.ContextData == null && TM.Tm.TextContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
			{
				return;
			}
			switch (TM.Tm.TextContextMatchType)
			{
			case TextContextMatchType.PrecedingAndFollowingSource:
				searchTuContext = result.ContextData;
				break;
			case TextContextMatchType.PrecedingSourceAndTarget:
				if (result.ContextData != null && result.ContextData.Context2 != 0L)
				{
					searchTuContext.Context2 = result.ContextData.Context2;
				}
				break;
			}
			if (TM.Tm.TextContextMatchType == TextContextMatchType.PrecedingSourceAndTarget && usePreviousTuForContext)
			{
				searchTuContext = result.ContextData;
			}
			foreach (TuContext value in storageTu.Contexts.Values)
			{
				SearchResultScoreTuContext(result, searchTuContext, value, TM.Tm.TextContextMatchType);
			}
		}

		private static void SearchResultScoreTuContext(SearchResult result, TuContext searchingContext, TuContext dbcontext, TextContextMatchType textContextMatchType)
		{
			if (dbcontext.Context1 != searchingContext.Context1)
			{
				return;
			}
			if (dbcontext.Context2 == searchingContext.Context2)
			{
				result.ScoringResult.TextContextMatch = TextContextMatch.SourceTargetMatch;
				if (textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
				{
					result.ScoringResult.TextContextMatch = TextContextMatch.PrecedingAndFollowingSourceMatch;
				}
				result.ContextData = dbcontext;
			}
			else if (result.ScoringResult.TextContextMatch == TextContextMatch.NoMatch && textContextMatchType != TextContextMatchType.PrecedingAndFollowingSource)
			{
				result.ScoringResult.TextContextMatch = TextContextMatch.SourceMatch;
				result.ContextData = dbcontext;
			}
		}
	}
}
