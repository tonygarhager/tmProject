using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class SubsegmentSearcher
	{
		private class FragmentDetails
		{
			public string FragmentIdString;

			public readonly SubsegmentSearchResults Results;

			public bool ResultsAddedToList;

			public short StartPos => Results.SourceFeatureStartIndex;

			public short Length => Results.SourceFeatureCount;

			public string Key => StartPos.ToString() + "-" + Length.ToString();

			public FragmentDetails(short sourceFeatureStartIndex, short sourceFeatureLength, short significantFeatureCount)
			{
				Results = new SubsegmentSearchResults(sourceFeatureStartIndex, sourceFeatureLength, significantFeatureCount);
			}
		}

		private class TUInfo
		{
			public int TuId;

			public List<string> SourceFeatures = new List<string>();

			public List<short> SourceFeatureTokenIndices = new List<short>();

			public List<string> TargetFeatures = new List<string>();

			public List<short> TargetFeatureTokenIndices = new List<short>();

			public Sdl.LanguagePlatform.Core.Segment SourceSegment;

			public Sdl.LanguagePlatform.Core.Segment TargetSegment;

			public Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memTU;

			public LiftAlignedSpanPairSet memTUFeatureBasedAlignmentData;

			public string memTUIdString;
		}

		private class FragmentSearchInfo
		{
			public readonly Dictionary<string, FragmentDetails> FragmentDetailsMap = new Dictionary<string, FragmentDetails>();

			public readonly Dictionary<string, List<FragmentDetails>> tm_tdb_idStringToFragmentDetailsListMap = new Dictionary<string, List<FragmentDetails>>();

			public readonly Dictionary<string, List<FragmentDetails>> dta_idStringToFragmentDetailsListMap = new Dictionary<string, List<FragmentDetails>>();

			public readonly HashSet<long> tm_tdb_queryHashes = new HashSet<long>();

			public readonly HashSet<long> dta_queryHashes = new HashSet<long>();

			public readonly Dictionary<long, List<string>> dtaHashToIdStringMap = new Dictionary<long, List<string>>();
		}

		public const int MaxSegmentLengthForSubsegmentRecall = 250;

		private const int NonExactMatchTreshold = 95;

		private readonly CallContext _Context;

		private readonly Scorer _Scorer;

		public SubsegmentSearchSettings Settings
		{
			get;
		}

		public AnnotatedTranslationMemory TM
		{
			get;
		}

		public SubsegmentSearcher(CallContext context, PersistentObjectToken tmId, SubsegmentSearchSettings settings)
		{
			if (settings.MinSignificantFeatures < 1)
			{
				throw new Exception("minFeatures < 1");
			}
			if (settings.MinTM_TDBSignificantFeatures < 1)
			{
				throw new Exception("MinTM_TDBSignificantFeatures < 1");
			}
			if (settings.MinSignificantFeatures > settings.MinFeatures)
			{
				throw new Exception("minSignificantTokens > minTokens");
			}
			if (settings.MinTM_TDBSignificantFeatures > settings.MinTM_TDBFeatures)
			{
				throw new Exception("minTM_TDBSignificantTokens > minTM_TDBTokens");
			}
			_Context = (context ?? throw new ArgumentNullException("context"));
			TM = _Context.GetAnnotatedTranslationMemory(tmId);
			Settings = settings;
			_Scorer = new Scorer(TM, Settings, TM.Tm.NormalizeCharWidths);
			settings.MinFeatures = Math.Max(settings.MinFeatures, SubsegmentUtilities.MinDTAFragmentLengthToIndex(TM.Tm.LanguageDirection.SourceCulture));
			settings.MinSignificantFeatures = Math.Max(settings.MinSignificantFeatures, SubsegmentUtilities.MinDTAFragmentSignificantFeatures(TM.Tm.LanguageDirection.SourceCulture));
		}

		public SubsegmentSearchResultsCollection[] Search(AnnotatedSegment[] segments, bool[] mask)
		{
			if (segments == null || segments.Length == 0)
			{
				throw new ArgumentNullException("segments");
			}
			if (mask != null && mask.Length != segments.Length)
			{
				throw new ArgumentException("If a mask is specified, its length must be equal to that of the segments collection");
			}
			SubsegmentSearchResultsCollection[] array = new SubsegmentSearchResultsCollection[segments.Length];
			List<Sdl.LanguagePlatform.Core.Segment> list = new List<Sdl.LanguagePlatform.Core.Segment>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < segments.Length; i++)
			{
				if (segments[i] == null)
				{
					array[i] = null;
					continue;
				}
				if (mask != null && !mask[i])
				{
					array[i] = null;
					continue;
				}
				TM.SourceTools.EnsureTokenizedSegment(segments[i].Segment, forceRetokenization: false, allowTokenBundles: false);
				if (segments[i].Segment.Tokens.Count > 250)
				{
					array[i] = null;
					continue;
				}
				TM.SourceTools.Stem(segments[i].Segment);
				list.Add(segments[i].Segment);
				list2.Add(i);
			}
			Dictionary<int, TUInfo> tusRetrieved = new Dictionary<int, TUInfo>();
			HashSet<long> hashSet = new HashSet<long>();
			HashSet<long> hashSet2 = new HashSet<long>();
			List<SubsegmentSearchResultsCollection> list3 = new List<SubsegmentSearchResultsCollection>();
			List<FragmentSearchInfo> list4 = new List<FragmentSearchInfo>();
			List<List<short>> list5 = new List<List<short>>();
			List<HashSet<string>> list6 = new List<HashSet<string>>();
			foreach (Sdl.LanguagePlatform.Core.Segment item3 in list)
			{
				SubsegmentSearchResultsCollection subsegmentSearchResultsCollection = new SubsegmentSearchResultsCollection();
				subsegmentSearchResultsCollection.ResultsPerFragment = new List<SubsegmentSearchResults>();
				subsegmentSearchResultsCollection.SourceSegment = item3.Duplicate();
				TM.SourceTools.EnsureTokenizedSegment(subsegmentSearchResultsCollection.SourceSegment);
				list3.Add(subsegmentSearchResultsCollection);
				List<short> list7 = new List<short>();
				List<string> features2 = subsegmentSearchResultsCollection.SourceSegmentFeatures = SubsegmentUtilities.GetFeatures(item3.Tokens, item3.Culture, list7);
				list5.Add(list7);
				FragmentSearchInfo item = GenerateFragments(features2, list7, item3, hashSet, hashSet2);
				list4.Add(item);
				HashSet<string> item2 = new HashSet<string>();
				list6.Add(item2);
			}
			int maxResults = Settings.MaxResults;
			Dictionary<int, HashSet<long>> hashesPerTu = new Dictionary<int, HashSet<long>>();
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list8 = _Context.Storage.ExactSearch(TM.Tm.ResourceId.Id, hashSet.ToList(), maxResults);
			FGASupport fGASupport = _Context.ResourceManager.GetTranslationMemory(TM.Tm.ResourceId).FGASupport;
			bool flag = !Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA) || (fGASupport != FGASupport.NonAutomatic && fGASupport != FGASupport.Automatic);
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> matchingTUs = null;
			if (list8 == null)
			{
				List<long> list9 = hashSet.ToList();
				list9.AddRange(hashSet2);
				matchingTUs = (list8 = _Context.Storage.SubsegmentSearch(TM.Tm.ResourceId.Id, list9, (byte)Settings.MinFeatures, (byte)Settings.MinSignificantFeatures, maxResults, hashesPerTu));
			}
			else if (!flag)
			{
				matchingTUs = _Context.Storage.SubsegmentSearch(TM.Tm.ResourceId.Id, hashSet2.ToList(), (byte)Settings.MinFeatures, (byte)Settings.MinSignificantFeatures, maxResults, hashesPerTu);
			}
			Translator translator = Settings.ComputeTranslationProposal ? new Translator(Settings, TM.Tm.TextContextMatchType) : null;
			if (translator != null)
			{
				translator.SourceTokenizationContext = TM.SourceTools.GetTokenizationContext();
				translator.TargetTokenizationContext = TM.TargetTools.GetTokenizationContext();
			}
			for (int j = 0; j < list.Count; j++)
			{
				Sdl.LanguagePlatform.Core.Segment docSegment = list[j];
				SubsegmentSearchResultsCollection subsegmentSearchResultsCollection2 = list3[j];
				array[list2[j]] = subsegmentSearchResultsCollection2;
				List<SubsegmentSearchResults> resultsList = subsegmentSearchResultsCollection2.ResultsPerFragment;
				List<short> featureTokenIndices = list5[j];
				FragmentSearchInfo fragmentSearchInfo = list4[j];
				HashSet<string> hashSet3 = list6[j];
				if (Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.TM_TDB))
				{
					ProcessTM_TDBMatches(list8, docSegment, featureTokenIndices, fragmentSearchInfo, tusRetrieved, translator, hashSet3);
				}
				if (flag)
				{
					foreach (string item4 in hashSet3)
					{
						FragmentDetails fragmentDetails = fragmentSearchInfo.FragmentDetailsMap[item4];
						resultsList.Add(fragmentDetails.Results);
					}
					continue;
				}
				ProcessDtaMatches(matchingTUs, hashesPerTu, docSegment, subsegmentSearchResultsCollection2.SourceSegmentFeatures, featureTokenIndices, fragmentSearchInfo, tusRetrieved, translator, hashSet3);
				List<FragmentDetails> list10 = new List<FragmentDetails>();
				foreach (string item5 in hashSet3)
				{
					list10.Add(fragmentSearchInfo.FragmentDetailsMap[item5]);
				}
				list10.Sort((FragmentDetails a, FragmentDetails b) => a.Length.CompareTo(b.Length));
				Dedup(list10, translator != null);
				list10.ForEach(delegate(FragmentDetails x)
				{
					resultsList.Add(x.Results);
				});
			}
			return array;
		}

		public SubsegmentSearchResultsCollection Search(AnnotatedSegment docSrcSegment)
		{
			TM.SourceTools.EnsureTokenizedSegment(docSrcSegment.Segment, forceRetokenization: false, allowTokenBundles: false);
			if (docSrcSegment.Segment.Tokens.Count > 250)
			{
				return null;
			}
			SubsegmentSearchResultsCollection subsegmentSearchResultsCollection = new SubsegmentSearchResultsCollection();
			subsegmentSearchResultsCollection.ResultsPerFragment = new List<SubsegmentSearchResults>();
			subsegmentSearchResultsCollection.SourceSegment = docSrcSegment.Segment.Duplicate();
			TM.SourceTools.EnsureTokenizedSegment(subsegmentSearchResultsCollection.SourceSegment);
			List<SubsegmentSearchResults> resultsList = subsegmentSearchResultsCollection.ResultsPerFragment;
			TM.SourceTools.Stem(docSrcSegment.Segment);
			List<short> list = new List<short>();
			List<string> features2 = subsegmentSearchResultsCollection.SourceSegmentFeatures = SubsegmentUtilities.GetFeatures(docSrcSegment.Segment.Tokens, docSrcSegment.Segment.Culture, list);
			FragmentSearchInfo fragmentSearchInfo = GenerateFragments(features2, list, docSrcSegment.Segment, null, null);
			Dictionary<int, TUInfo> tusRetrieved = new Dictionary<int, TUInfo>();
			Translator translator = Settings.ComputeTranslationProposal ? new Translator(Settings, TM.Tm.TextContextMatchType) : null;
			if (translator != null)
			{
				translator.SourceTokenizationContext = TM.SourceTools.GetTokenizationContext();
				translator.TargetTokenizationContext = TM.TargetTools.GetTokenizationContext();
			}
			HashSet<string> hashSet = new HashSet<string>();
			int maxResults = Settings.MaxResults;
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> list2 = _Context.Storage.ExactSearch(TM.Tm.ResourceId.Id, fragmentSearchInfo.tm_tdb_queryHashes.ToList(), maxResults);
			FGASupport fGASupport = _Context.ResourceManager.GetTranslationMemory(TM.Tm.ResourceId).FGASupport;
			bool flag = !Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA) || (fGASupport != FGASupport.NonAutomatic && fGASupport != FGASupport.Automatic);
			List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> matchingTUs = null;
			Dictionary<int, HashSet<long>> hashesPerTu = new Dictionary<int, HashSet<long>>();
			if (list2 == null)
			{
				List<long> list3 = fragmentSearchInfo.tm_tdb_queryHashes.ToList();
				list3.AddRange(fragmentSearchInfo.dta_queryHashes);
				matchingTUs = (list2 = _Context.Storage.SubsegmentSearch(TM.Tm.ResourceId.Id, list3, (byte)Settings.MinFeatures, (byte)Settings.MinSignificantFeatures, maxResults, hashesPerTu));
			}
			else if (!flag)
			{
				matchingTUs = _Context.Storage.SubsegmentSearch(TM.Tm.ResourceId.Id, fragmentSearchInfo.dta_queryHashes.ToList(), (byte)Settings.MinFeatures, (byte)Settings.MinSignificantFeatures, maxResults, hashesPerTu);
			}
			ProcessTM_TDBMatches(list2, docSrcSegment.Segment, list, fragmentSearchInfo, tusRetrieved, translator, hashSet);
			if (flag)
			{
				foreach (string item in hashSet)
				{
					FragmentDetails fragmentDetails = fragmentSearchInfo.FragmentDetailsMap[item];
					resultsList.Add(fragmentDetails.Results);
				}
				return subsegmentSearchResultsCollection;
			}
			ProcessDtaMatches(matchingTUs, hashesPerTu, docSrcSegment.Segment, features2, list, fragmentSearchInfo, tusRetrieved, translator, hashSet);
			List<FragmentDetails> list4 = new List<FragmentDetails>();
			foreach (string item2 in hashSet)
			{
				list4.Add(fragmentSearchInfo.FragmentDetailsMap[item2]);
			}
			list4.Sort((FragmentDetails a, FragmentDetails b) => a.Length.CompareTo(b.Length));
			Dedup(list4, translator != null);
			list4.ForEach(delegate(FragmentDetails x)
			{
				resultsList.Add(x.Results);
			});
			return subsegmentSearchResultsCollection;
		}

		private static void Dedup(IList<FragmentDetails> fragmentDetailsWithResultsInAscendingOrderOfLength, bool useTranslationProposal)
		{
			List<FragmentDetails> list = new List<FragmentDetails>();
			for (int i = 0; i < fragmentDetailsWithResultsInAscendingOrderOfLength.Count; i++)
			{
				for (int j = i + 1; j < fragmentDetailsWithResultsInAscendingOrderOfLength.Count; j++)
				{
					FragmentDetails fragmentDetails = fragmentDetailsWithResultsInAscendingOrderOfLength[i];
					FragmentDetails fragmentDetails2 = fragmentDetailsWithResultsInAscendingOrderOfLength[j];
					LiftSpan otherSpan = new LiftSpan(fragmentDetails.StartPos, fragmentDetails.Length);
					if (new LiftSpan(fragmentDetails2.StartPos, fragmentDetails2.Length).Covers(otherSpan, 0))
					{
						List<SubsegmentSearchResult> list2 = new List<SubsegmentSearchResult>();
						foreach (SubsegmentSearchResult item in (IEnumerable<SubsegmentSearchResult>)fragmentDetails.Results)
						{
							foreach (SubsegmentSearchResult item2 in (IEnumerable<SubsegmentSearchResult>)fragmentDetails2.Results)
							{
								SubsegmentSearchResult subsegmentSearchResult = item;
								SubsegmentSearchResult subsegmentSearchResult2 = item2;
								if (useTranslationProposal)
								{
									if (subsegmentSearchResult.CachedTranslationProposalString.Length == 0)
									{
										list2.Add(item);
										break;
									}
									if (subsegmentSearchResult2.CachedTranslationProposalString.IndexOf(subsegmentSearchResult.CachedTranslationProposalString, StringComparison.Ordinal) > -1)
									{
										if (subsegmentSearchResult.MatchType != SubsegmentMatchType.TM_TDB || subsegmentSearchResult2.MatchType == SubsegmentMatchType.TM_TDB)
										{
											list2.Add(item);
										}
										break;
									}
								}
								else if (subsegmentSearchResult2.TranslationFeatureString.IndexOf(subsegmentSearchResult.TranslationFeatureString, StringComparison.Ordinal) > -1)
								{
									if (subsegmentSearchResult.MatchType != SubsegmentMatchType.TM_TDB || subsegmentSearchResult2.MatchType == SubsegmentMatchType.TM_TDB)
									{
										list2.Add(item);
									}
									break;
								}
							}
						}
						foreach (SubsegmentSearchResult item3 in list2)
						{
							fragmentDetails.Results.Remove(item3);
						}
						if (fragmentDetails.Results.Count == 0)
						{
							list.Add(fragmentDetails);
							break;
						}
					}
				}
			}
			foreach (FragmentDetails item4 in list)
			{
				fragmentDetailsWithResultsInAscendingOrderOfLength.Remove(item4);
			}
		}

		private static void FindMatchingLeadingAndTrailingTokens(IReadOnlyList<short> queryFeatureTokenIndices, IReadOnlyList<short> memTuSourceFeatureTokenIndices, IReadOnlyList<short> memTuTargetFeatureTokenIndices, Sdl.LanguagePlatform.Core.Segment docSegment, Sdl.LanguagePlatform.Core.Segment memTuSourceSeg, Sdl.LanguagePlatform.Core.Segment memTuTargetSeg, out int matchingLeadingTokens, out int matchingTrailingTokens, int firstMatchingSourceFeature, int firstMatchingTargetFeature, int firstMatchingQueryFeature, int lastMatchingSourceFeature, int lastMatchingTargetFeature, int lastMatchingQueryFeature)
		{
			int num = queryFeatureTokenIndices[firstMatchingQueryFeature];
			int num2 = queryFeatureTokenIndices[lastMatchingQueryFeature];
			int num3 = 0;
			if (firstMatchingQueryFeature > 0)
			{
				num3 = queryFeatureTokenIndices[firstMatchingQueryFeature - 1] + 1;
			}
			int num4 = lastMatchingQueryFeature - firstMatchingQueryFeature + 1;
			int num5 = docSegment.Tokens.Count - 1;
			if (firstMatchingQueryFeature + num4 < queryFeatureTokenIndices.Count)
			{
				num5 = queryFeatureTokenIndices[firstMatchingQueryFeature + num4] - 1;
			}
			matchingLeadingTokens = 0;
			matchingTrailingTokens = 0;
			int num6 = memTuSourceFeatureTokenIndices[firstMatchingSourceFeature] - 1;
			int num7 = memTuTargetFeatureTokenIndices[firstMatchingTargetFeature] - 1;
			int num8 = num - 1;
			while (num6 >= 0 && num7 >= 0 && num8 >= num3)
			{
				Token docToken = docSegment.Tokens[num8];
				if (!LeadingOrTrailingTokensMatch(docToken, memTuSourceSeg.Tokens[num6], leading: true) || !LeadingOrTrailingTokensMatch(docToken, memTuTargetSeg.Tokens[num7], leading: true))
				{
					break;
				}
				matchingLeadingTokens++;
				num8--;
				num6--;
				num7--;
			}
			num6 = memTuSourceFeatureTokenIndices[lastMatchingSourceFeature] + 1;
			num7 = memTuTargetFeatureTokenIndices[lastMatchingTargetFeature] + 1;
			num8 = num2 + 1;
			while (num6 < memTuSourceSeg.Tokens.Count && num7 < memTuTargetSeg.Tokens.Count && num8 <= num5)
			{
				Token docToken2 = docSegment.Tokens[num8];
				if (!LeadingOrTrailingTokensMatch(docToken2, memTuSourceSeg.Tokens[num6], leading: false) || !LeadingOrTrailingTokensMatch(docToken2, memTuTargetSeg.Tokens[num7], leading: false))
				{
					break;
				}
				matchingTrailingTokens++;
				num8++;
				num6++;
				num7++;
			}
			while (matchingLeadingTokens > 0 && docSegment.Tokens[num - matchingLeadingTokens].Type == TokenType.Whitespace)
			{
				matchingLeadingTokens--;
			}
			while (matchingTrailingTokens > 0 && docSegment.Tokens[num2 + matchingTrailingTokens].Type == TokenType.Whitespace)
			{
				matchingTrailingTokens--;
			}
		}

		private void ProcessTM_TDBMatches(IEnumerable<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> matchingTUs, Sdl.LanguagePlatform.Core.Segment docSegment, IReadOnlyList<short> featureTokenIndices, FragmentSearchInfo fragmentSearchInfo, IDictionary<int, TUInfo> tusRetrieved, Translator translator, ISet<string> keysOfFragmentDetailsWithResults)
		{
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit matchingTU in matchingTUs)
			{
				TUInfo tUInfo;
				if (tusRetrieved.ContainsKey(matchingTU.Id))
				{
					tUInfo = tusRetrieved[matchingTU.Id];
				}
				else
				{
					tUInfo = TuInfoFromTu(matchingTU, forTM_TDB: true);
					tusRetrieved.Add(matchingTU.Id, tUInfo);
				}
				if (tUInfo != null && tUInfo.TargetFeatureTokenIndices.Count != 0)
				{
					string key = SubsegmentUtilities.FeaturesToIdString(tUInfo.SourceFeatures, 0, tUInfo.SourceFeatures.Count);
					if (fragmentSearchInfo.tm_tdb_idStringToFragmentDetailsListMap.ContainsKey(key))
					{
						List<short> sourceFeatureTokenIndices = tUInfo.SourceFeatureTokenIndices;
						List<short> targetFeatureTokenIndices = tUInfo.TargetFeatureTokenIndices;
						Sdl.LanguagePlatform.Core.Segment sourceSegment = tUInfo.SourceSegment;
						Sdl.LanguagePlatform.Core.Segment targetSegment = tUInfo.TargetSegment;
						foreach (FragmentDetails item in fragmentSearchInfo.tm_tdb_idStringToFragmentDetailsListMap[key])
						{
							int startPos = item.StartPos;
							int length = item.Length;
							_ = featureTokenIndices[startPos];
							_ = featureTokenIndices[startPos + length - 1];
							FindMatchingLeadingAndTrailingTokens(featureTokenIndices, sourceFeatureTokenIndices, targetFeatureTokenIndices, docSegment, sourceSegment, targetSegment, out int matchingLeadingTokens, out int matchingTrailingTokens, 0, 0, startPos, sourceFeatureTokenIndices.Count - 1, targetFeatureTokenIndices.Count - 1, startPos + length - 1);
							SubsegmentSearchResults results = item.Results;
							short num = (short)(sourceFeatureTokenIndices[0] - matchingLeadingTokens);
							short matchTokenCount = (short)((short)(sourceFeatureTokenIndices[sourceFeatureTokenIndices.Count - 1] + matchingTrailingTokens) - num + 1);
							short queryTokenIndex = (short)(featureTokenIndices[startPos] - matchingLeadingTokens);
							short queryTokenCount = (short)(featureTokenIndices[startPos + length - 1] - featureTokenIndices[startPos] + 1 + matchingLeadingTokens + matchingTrailingTokens);
							short translationTokenIndex = (short)(targetFeatureTokenIndices[0] - matchingLeadingTokens);
							short translationTokenCount = (short)(targetFeatureTokenIndices[targetFeatureTokenIndices.Count - 1] - targetFeatureTokenIndices[0] + 1 + matchingLeadingTokens + matchingTrailingTokens);
							CreateTranslationAndScoringSegments(docSegment, queryTokenIndex, queryTokenCount, tUInfo.memTU.SourceSegment, num, matchTokenCount, tUInfo.memTU.TargetSegment, translationTokenIndex, translationTokenCount, out AnnotatedSegment aDocSegmentForGeneratingTranslation, out AnnotatedSegment aDocSegmentForScoring, out Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memTUForScoring);
							AddMatchToResult(aDocSegmentForGeneratingTranslation, results, translator, num, matchTokenCount, SubsegmentMatchType.TM_TDB, queryTokenIndex, queryTokenCount, translationTokenIndex, translationTokenCount, tUInfo, 1f, scoreDiagonalOnly: false, aDocSegmentForScoring, memTUForScoring);
							if (results.Count > 0 && !item.ResultsAddedToList)
							{
								item.ResultsAddedToList = true;
								if (!keysOfFragmentDetailsWithResults.Contains(item.Key))
								{
									keysOfFragmentDetailsWithResults.Add(item.Key);
								}
							}
						}
					}
				}
			}
		}

		private void CreateTranslationAndScoringSegments(Sdl.LanguagePlatform.Core.Segment querySegment, int queryTokenIndex, int queryTokenCount, Sdl.LanguagePlatform.Core.Segment memTUSourceSegment, int matchTokenIndex, int matchTokenCount, Sdl.LanguagePlatform.Core.Segment memTUTargetSegment, int translationTokenIndex, int translationTokenCount, out AnnotatedSegment aDocSegmentForGeneratingTranslation, out AnnotatedSegment aDocSegmentForScoring, out Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memTUForScoring)
		{
			Sdl.LanguagePlatform.Core.Segment segment = new Sdl.LanguagePlatform.Core.Segment(TM.Tm.LanguageDirection.SourceCulture);
			segment.Tokens = new List<Token>();
			List<Token> tokensBefore = memTUSourceSegment.Tokens.GetRange(0, matchTokenIndex);
			int count = memTUSourceSegment.Tokens.Count - (matchTokenIndex + matchTokenCount);
			int index = matchTokenIndex + matchTokenCount;
			List<Token> tokensAfter = memTUSourceSegment.Tokens.GetRange(index, count);
			List<Token> matchingQueryTokens = querySegment.Tokens.GetRange(queryTokenIndex, queryTokenCount);
			List<Token> range = memTUSourceSegment.Tokens.GetRange(matchTokenIndex, matchTokenCount);
			List<TagToken> matchingQueryTagTokens = new List<TagToken>();
			matchingQueryTokens.ForEach(delegate(Token x)
			{
				TagToken tagToken8 = x as TagToken;
				if (tagToken8 != null)
				{
					matchingQueryTagTokens.Add(tagToken8);
				}
			});
			List<TagToken> matchingMemTUSourceTagTokens = new List<TagToken>();
			range.ForEach(delegate(Token x)
			{
				TagToken tagToken7 = x as TagToken;
				if (tagToken7 != null)
				{
					matchingMemTUSourceTagTokens.Add(tagToken7);
				}
			});
			List<TagToken> matchingQueryStartTokens = matchingQueryTagTokens.FindAll((TagToken x) => x.Tag.Type == TagType.Start);
			List<TagToken> matchingQueryEndTokens = matchingQueryTagTokens.FindAll((TagToken x) => x.Tag.Type == TagType.End);
			List<TagToken> matchingMemTUStartTokens = matchingMemTUSourceTagTokens.FindAll((TagToken x) => x.Tag.Type == TagType.Start);
			List<TagToken> matchingMemTUEndTokens = matchingMemTUSourceTagTokens.FindAll((TagToken x) => x.Tag.Type == TagType.End);
			List<TagToken> list = matchingQueryStartTokens.FindAll((TagToken x) => matchingQueryEndTokens.All((TagToken y) => y.Tag.Anchor != x.Tag.Anchor));
			List<TagToken> list2 = matchingQueryEndTokens.FindAll((TagToken x) => matchingQueryStartTokens.All((TagToken y) => y.Tag.Anchor != x.Tag.Anchor));
			List<TagToken> list3 = matchingMemTUStartTokens.FindAll((TagToken x) => matchingMemTUEndTokens.All((TagToken y) => y.Tag.Anchor != x.Tag.Anchor));
			List<TagToken> list4 = matchingMemTUEndTokens.FindAll((TagToken x) => matchingMemTUStartTokens.All((TagToken y) => y.Tag.Anchor != x.Tag.Anchor));
			list2.ForEach(delegate(TagToken x)
			{
				matchingQueryTokens.Remove(x);
			});
			list.ForEach(delegate(TagToken x)
			{
				matchingQueryTokens.Remove(x);
			});
			Action<TagToken> action = delegate(TagToken x)
			{
				Token token = tokensBefore.Find(delegate(Token y)
				{
					TagToken tagToken6 = y as TagToken;
					return tagToken6 != null && tagToken6.Tag.Anchor == x.Tag.Anchor;
				});
				if (token != null)
				{
					tokensBefore.Remove(token);
				}
				else
				{
					token = tokensAfter.Find(delegate(Token y)
					{
						TagToken tagToken5 = y as TagToken;
						return tagToken5 != null && tagToken5.Tag.Anchor == x.Tag.Anchor;
					});
					tokensAfter.Remove(token);
				}
			};
			list3.ForEach(action);
			list4.ForEach(action);
			List<TagToken> remainingQueryTags = new List<TagToken>();
			matchingQueryTokens.ForEach(delegate(Token x)
			{
				TagToken tagToken4 = x as TagToken;
				if (tagToken4 != null)
				{
					remainingQueryTags.Add(tagToken4);
				}
			});
			if (remainingQueryTags.Count > 0)
			{
				List<TagToken> remainingMemTuTags = new List<TagToken>();
				tokensBefore.ForEach(delegate(Token x)
				{
					TagToken tagToken3 = x as TagToken;
					if (tagToken3 != null)
					{
						remainingMemTuTags.Add(tagToken3);
					}
				});
				tokensAfter.ForEach(delegate(Token x)
				{
					TagToken tagToken2 = x as TagToken;
					if (tagToken2 != null)
					{
						remainingMemTuTags.Add(tagToken2);
					}
				});
				if (remainingMemTuTags.Count > 0)
				{
					List<Token> temp = new List<Token>();
					matchingQueryTokens.ForEach(delegate(Token x)
					{
						temp.Add(x.Duplicate() as Token);
					});
					matchingQueryTokens = temp;
					remainingQueryTags.Clear();
					matchingQueryTokens.ForEach(delegate(Token x)
					{
						TagToken tagToken = x as TagToken;
						if (tagToken != null)
						{
							remainingQueryTags.Add(tagToken);
						}
					});
					int highestAnchor = remainingMemTuTags.Max((TagToken x) => x.Tag.Anchor);
					Dictionary<int, int> anchorMap = new Dictionary<int, int>();
					remainingQueryTags.ForEach(delegate(TagToken x)
					{
						if (!anchorMap.ContainsKey(x.Tag.Anchor))
						{
							anchorMap.Add(x.Tag.Anchor, ++highestAnchor);
						}
					});
					remainingQueryTags.ForEach(delegate(TagToken x)
					{
						x.Tag.Anchor = anchorMap[x.Tag.Anchor];
					});
				}
			}
			SegmentEditor.AppendTokens(segment, tokensBefore);
			SegmentEditor.AppendTokens(segment, matchingQueryTokens);
			SegmentEditor.AppendTokens(segment, tokensAfter);
			List<Token> tokens = segment.Tokens;
			aDocSegmentForGeneratingTranslation = new AnnotatedSegment(TM, segment, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true);
			aDocSegmentForGeneratingTranslation.Segment.Tokens = tokens;
			Sdl.LanguagePlatform.Core.Segment segment2 = CreateFragmentSegment(querySegment, queryTokenIndex, queryTokenCount);
			tokens = segment2.Tokens;
			aDocSegmentForScoring = new AnnotatedSegment(TM, segment2, isTargetSegment: false, keepTokens: false, keepPeripheralWhitespace: true);
			aDocSegmentForScoring.Segment.Tokens = tokens;
			Sdl.LanguagePlatform.Core.Segment sourceSegment = CreateFragmentSegment(memTUSourceSegment, matchTokenIndex, matchTokenCount);
			Sdl.LanguagePlatform.Core.Segment targetSegment = CreateFragmentSegment(memTUTargetSegment, translationTokenIndex, translationTokenCount);
			memTUForScoring = new Sdl.LanguagePlatform.TranslationMemory.TranslationUnit(sourceSegment, targetSegment);
		}

		private void ProcessDtaMatches(List<Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit> matchingTUs, IReadOnlyDictionary<int, HashSet<long>> hashesPerTu, Sdl.LanguagePlatform.Core.Segment docSegment, IReadOnlyList<string> features, IReadOnlyList<short> featureTokenIndices, FragmentSearchInfo fragmentSearchInfo, IDictionary<int, TUInfo> tusRetrieved, Translator translator, ISet<string> keysOfFragmentDetailsWithResults)
		{
			foreach (Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit matchingTU in matchingTUs)
			{
				TUInfo tUInfo;
				if (tusRetrieved.ContainsKey(matchingTU.Id))
				{
					tUInfo = tusRetrieved[matchingTU.Id];
				}
				else
				{
					tUInfo = TuInfoFromTu(matchingTU, forTM_TDB: false);
					tusRetrieved.Add(matchingTU.Id, tUInfo);
				}
				if (tUInfo?.memTU.AlignmentData != null && !tUInfo.memTU.AlignmentData.IsEmpty)
				{
					SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(tUInfo.TargetFeatureTokenIndices);
					HashSet<long> hashSet = hashesPerTu[matchingTU.Id];
					List<LiftSpan> list = new List<LiftSpan>();
					List<FragmentDetails> list2 = new List<FragmentDetails>();
					Dictionary<string, List<short>> dictionary = new Dictionary<string, List<short>>();
					foreach (long item2 in hashSet)
					{
						if (fragmentSearchInfo.dtaHashToIdStringMap.ContainsKey(item2))
						{
							foreach (string item3 in fragmentSearchInfo.dtaHashToIdStringMap[item2])
							{
								int startIndex = 0;
								while (true)
								{
									int num = tUInfo.memTUIdString.IndexOf(item3, startIndex, StringComparison.Ordinal);
									if (num < 0)
									{
										break;
									}
									short item = (short)tUInfo.memTUIdString.Substring(0, num).Count((char c) => c == '|');
									if (!dictionary.TryGetValue(item3, out List<short> value))
									{
										dictionary.Add(item3, new List<short>
										{
											item
										});
									}
									else
									{
										value.Add(item);
									}
									startIndex = num + 1;
								}
								if (dictionary.ContainsKey(item3))
								{
									List<FragmentDetails> collection = fragmentSearchInfo.dta_idStringToFragmentDetailsListMap[item3];
									list2.AddRange(collection);
								}
							}
						}
					}
					list2.Sort((FragmentDetails a, FragmentDetails b) => b.Length.CompareTo(a.Length));
					foreach (FragmentDetails item4 in list2)
					{
						short num2 = dictionary[item4.FragmentIdString][0];
						LiftSpan memTUFeatureSpan = new LiftSpan();
						memTUFeatureSpan.StartIndex = num2;
						memTUFeatureSpan.Length = item4.Length;
						if ((memTUFeatureSpan.StartIndex != 0 || memTUFeatureSpan.Length != tUInfo.SourceFeatures.Count) && !list.Exists((LiftSpan s) => s.Covers(memTUFeatureSpan, 0)))
						{
							bool flag = true;
							for (int i = 0; i < item4.Length; i++)
							{
								if (string.CompareOrdinal(features[item4.StartPos + i], tUInfo.SourceFeatures[num2 + i]) != 0)
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								LiftSpan sourceSpan = new LiftSpan(num2, item4.Length);
								float confidence;
								LiftSpan translationSpan = SubsegmentUtilities.GetTranslationSpan(tUInfo.memTUFeatureBasedAlignmentData, sourceSpan, out confidence);
								if (translationSpan != null)
								{
									FindMatchingLeadingAndTrailingTokens(featureTokenIndices, tUInfo.SourceFeatureTokenIndices, tUInfo.TargetFeatureTokenIndices, docSegment, tUInfo.SourceSegment, tUInfo.TargetSegment, out int matchingLeadingTokens, out int matchingTrailingTokens, num2, translationSpan.StartIndex, item4.StartPos, num2 + item4.Length - 1, translationSpan.EndIndex, item4.StartPos + item4.Length - 1);
									short num3 = (short)(tUInfo.SourceFeatureTokenIndices[num2] - matchingLeadingTokens);
									short matchTokenCount = (short)((short)(tUInfo.SourceFeatureTokenIndices[num2 + item4.Length - 1] + matchingTrailingTokens) - num3 + 1);
									LiftSpan liftSpan = rawIndexVsSignificantIndexConverter.SignificantSpanToRawSpan(translationSpan);
									SubsegmentSearchResults results = item4.Results;
									short queryTokenCount = (short)(featureTokenIndices[item4.StartPos + item4.Length - 1] - featureTokenIndices[item4.StartPos] + 1 + matchingLeadingTokens + matchingTrailingTokens);
									short queryTokenIndex = (short)(featureTokenIndices[item4.StartPos] - matchingLeadingTokens);
									short translationTokenIndex = (short)(liftSpan.StartIndex - matchingLeadingTokens);
									short translationTokenCount = (short)(liftSpan.Length + matchingLeadingTokens + matchingTrailingTokens);
									CreateTranslationAndScoringSegments(docSegment, queryTokenIndex, queryTokenCount, tUInfo.memTU.SourceSegment, num3, matchTokenCount, tUInfo.memTU.TargetSegment, translationTokenIndex, translationTokenCount, out AnnotatedSegment aDocSegmentForGeneratingTranslation, out AnnotatedSegment aDocSegmentForScoring, out Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memTUForScoring);
									bool flag2 = aDocSegmentForScoring.Segment.Tokens.Count == memTUForScoring.SourceSegment.Tokens.Count;
									if (flag2)
									{
										for (int j = 0; j < item4.Length; j++)
										{
											if (featureTokenIndices[j + item4.StartPos] - featureTokenIndices[item4.StartPos] != tUInfo.SourceFeatureTokenIndices[j + num2] - tUInfo.SourceFeatureTokenIndices[num2])
											{
												flag2 = false;
												break;
											}
										}
									}
									AddMatchToResult(aDocSegmentForGeneratingTranslation, results, translator, num3, matchTokenCount, SubsegmentMatchType.DTA, queryTokenIndex, queryTokenCount, translationTokenIndex, translationTokenCount, tUInfo, confidence, flag2, aDocSegmentForScoring, memTUForScoring);
									if (results.Count > 0 && !item4.ResultsAddedToList)
									{
										item4.ResultsAddedToList = true;
										keysOfFragmentDetailsWithResults.Add(item4.Key);
									}
									list.Add(memTUFeatureSpan);
								}
							}
						}
					}
				}
			}
		}

		private TUInfo TuInfoFromTu(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationUnit tu, bool forTM_TDB)
		{
			Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit = _Context.ResourceManager.GetTranslationUnit(tu, TM.Tm.FieldDeclarations, TM.Tm.LanguageDirection.SourceCulture, TM.Tm.LanguageDirection.TargetCulture);
			if (!forTM_TDB)
			{
				List<Token> tokens = translationUnit.SourceSegment.Tokens;
				List<Token> tokens2 = translationUnit.TargetSegment.Tokens;
				LiftAlignedSpanPairSet alignmentData = translationUnit.AlignmentData;
				if (tokens == null || tokens2 == null || alignmentData == null)
				{
					return null;
				}
				if (alignmentData.IsEmpty)
				{
					return null;
				}
				LiftAlignedSpanPair liftAlignedSpanPair = alignmentData.Root();
				if (liftAlignedSpanPair.SourceLength != tokens.Count || liftAlignedSpanPair.TargetLength != tokens2.Count)
				{
					return null;
				}
			}
			Sdl.LanguagePlatform.Core.Segment sourceSegment = translationUnit.SourceSegment;
			Sdl.LanguagePlatform.Core.Segment targetSegment = translationUnit.TargetSegment;
			List<short> list = new List<short>();
			List<short> list2 = new List<short>();
			TM.SourceTools.EnsureTokenizedSegment(translationUnit.SourceSegment);
			TM.SourceTools.Stem(translationUnit.SourceSegment);
			List<string> features = SubsegmentUtilities.GetFeatures(sourceSegment.Tokens, sourceSegment.Culture, list);
			TM.TargetTools.EnsureTokenizedSegment(translationUnit.TargetSegment);
			TM.SourceTools.Stem(translationUnit.TargetSegment);
			List<string> features2 = SubsegmentUtilities.GetFeatures(targetSegment.Tokens, targetSegment.Culture, list2);
			TUInfo tUInfo = new TUInfo();
			tUInfo.TuId = tu.Id;
			tUInfo.SourceSegment = sourceSegment;
			tUInfo.TargetSegment = targetSegment;
			tUInfo.SourceFeatures = features;
			tUInfo.TargetFeatures = features2;
			tUInfo.SourceFeatureTokenIndices = list;
			tUInfo.TargetFeatureTokenIndices = list2;
			tUInfo.memTU = translationUnit;
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcFromConverter = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)tUInfo.SourceSegment.Tokens.Count);
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgFromConverter = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)tUInfo.TargetSegment.Tokens.Count);
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgToConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(tUInfo.TargetFeatureTokenIndices);
			SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcToConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(tUInfo.SourceFeatureTokenIndices);
			if (tUInfo.memTU.AlignmentData != null)
			{
				tUInfo.memTUFeatureBasedAlignmentData = SubsegmentUtilities.ConvertAlignmentSetIndices(tUInfo.memTU.AlignmentData, srcFromConverter, trgFromConverter, srcToConverter, trgToConverter);
			}
			tUInfo.memTUIdString = SubsegmentUtilities.FeaturesToIdString(tUInfo.SourceFeatures, 0, tUInfo.SourceFeatures.Count);
			return tUInfo;
		}

		private FragmentSearchInfo GenerateFragments(List<string> features, List<short> featureTokenIndices, Sdl.LanguagePlatform.Core.Segment docSegment, HashSet<long> collated_tm_tdb_hashes, HashSet<long> collated_dta_hashes)
		{
			FragmentSearchInfo fragmentSearchInfo = new FragmentSearchInfo();
			int num = SubsegmentUtilities.MaxTM_TDBFragmentLengthToSearchFor(features.Count, Settings.MinScore);
			int val = features.Count;
			if (!Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA))
			{
				val = num;
			}
			int num2 = Settings.MinTM_TDBFeatures;
			if (Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA) && Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.TM_TDB))
			{
				num2 = Math.Min(Settings.MinTM_TDBFeatures, Settings.MinFeatures);
			}
			else if (Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA))
			{
				num2 = Settings.MinFeatures;
			}
			int num3 = Settings.MinTM_TDBSignificantFeatures;
			if (Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA) && Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.TM_TDB))
			{
				num3 = Math.Min(Settings.MinTM_TDBSignificantFeatures, Settings.MinSignificantFeatures);
			}
			else if (Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA))
			{
				num3 = Settings.MinSignificantFeatures;
			}
			int num4 = features.Count - num2;
			for (int i = 0; i <= num4; i++)
			{
				int val2 = features.Count - i;
				int num5 = Math.Min(val, val2);
				for (int j = num2; j <= num5; j++)
				{
					short num6 = 0;
					for (int k = i; k < i + j; k++)
					{
						if (SubsegmentUtilities.IsSignificantForRecall(docSegment.Tokens[featureTokenIndices[k]]))
						{
							num6 = (short)(num6 + 1);
						}
					}
					if (num6 < num3)
					{
						continue;
					}
					string text = SubsegmentUtilities.FeaturesToIdString(features, i, j);
					long hashCodeLong = Hash.GetHashCodeLong(text);
					FragmentDetails fragmentDetails = new FragmentDetails((short)i, (short)j, num6);
					fragmentDetails.FragmentIdString = text;
					bool flag = false;
					if (Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.TM_TDB) && j >= Settings.MinTM_TDBFeatures && num6 >= Settings.MinTM_TDBSignificantFeatures && j <= num)
					{
						flag = true;
						if (!fragmentSearchInfo.tm_tdb_queryHashes.Contains(hashCodeLong))
						{
							fragmentSearchInfo.tm_tdb_queryHashes.Add(hashCodeLong);
						}
						collated_tm_tdb_hashes?.Add(hashCodeLong);
						if (fragmentSearchInfo.tm_tdb_idStringToFragmentDetailsListMap.ContainsKey(text))
						{
							fragmentSearchInfo.tm_tdb_idStringToFragmentDetailsListMap[text].Add(fragmentDetails);
						}
						else
						{
							fragmentSearchInfo.tm_tdb_idStringToFragmentDetailsListMap.Add(text, new List<FragmentDetails>
							{
								fragmentDetails
							});
						}
					}
					if (Settings.SubsegmentMatchTypes.Contains(SubsegmentMatchType.DTA) && j >= Settings.MinFeatures && num6 >= Settings.MinSignificantFeatures)
					{
						flag = true;
						if (!fragmentSearchInfo.dta_queryHashes.Contains(hashCodeLong))
						{
							fragmentSearchInfo.dta_queryHashes.Add(hashCodeLong);
						}
						collated_dta_hashes?.Add(hashCodeLong);
						if (fragmentSearchInfo.dta_idStringToFragmentDetailsListMap.ContainsKey(text))
						{
							fragmentSearchInfo.dta_idStringToFragmentDetailsListMap[text].Add(fragmentDetails);
						}
						else
						{
							fragmentSearchInfo.dta_idStringToFragmentDetailsListMap.Add(text, new List<FragmentDetails>
							{
								fragmentDetails
							});
							if (fragmentSearchInfo.dtaHashToIdStringMap.ContainsKey(hashCodeLong))
							{
								fragmentSearchInfo.dtaHashToIdStringMap[hashCodeLong].Add(text);
							}
							else
							{
								fragmentSearchInfo.dtaHashToIdStringMap.Add(hashCodeLong, new List<string>
								{
									text
								});
							}
						}
					}
					if (flag)
					{
						fragmentSearchInfo.FragmentDetailsMap.Add(fragmentDetails.Key, fragmentDetails);
					}
				}
			}
			return fragmentSearchInfo;
		}

		private static bool LeadingOrTrailingTokensMatch(Token docToken, Token memToken, bool leading)
		{
			if (!(docToken.GetType() == memToken.GetType()))
			{
				return false;
			}
			TagToken tagToken = docToken as TagToken;
			if (tagToken == null)
			{
				return true;
			}
			TagToken tagToken2 = memToken as TagToken;
			if (leading)
			{
				if (tagToken.Tag.Type == TagType.End)
				{
					return false;
				}
			}
			else if (tagToken.Tag.Type == TagType.Start)
			{
				return false;
			}
			return tagToken.Tag.Type == tagToken2.Tag.Type;
		}

		internal static Sdl.LanguagePlatform.Core.Segment CreateFragmentSegment(Sdl.LanguagePlatform.Core.Segment wholeSegment, int startTok, int tokenCount)
		{
			Sdl.LanguagePlatform.Core.Segment obj = new Sdl.LanguagePlatform.Core.Segment(wholeSegment.Culture)
			{
				Tokens = new List<Token>()
			};
			List<Token> tokens = wholeSegment.Tokens.GetRange(startTok, tokenCount);
			List<TagToken> tagTokens = new List<TagToken>();
			tokens.ForEach(delegate(Token x)
			{
				TagToken tagToken = x as TagToken;
				if (tagToken != null)
				{
					tagTokens.Add(tagToken);
				}
			});
			List<TagToken> startTokens = tagTokens.FindAll((TagToken x) => x.Tag.Type == TagType.Start);
			List<TagToken> endTokens = tagTokens.FindAll((TagToken x) => x.Tag.Type == TagType.End);
			List<TagToken> list = startTokens.FindAll((TagToken x) => endTokens.All((TagToken y) => y.Tag.Anchor != x.Tag.Anchor));
			List<TagToken> list2 = endTokens.FindAll((TagToken x) => startTokens.All((TagToken y) => y.Tag.Anchor != x.Tag.Anchor));
			list.ForEach(delegate(TagToken x)
			{
				tokens.Remove(x);
			});
			list2.ForEach(delegate(TagToken x)
			{
				tokens.Remove(x);
			});
			SegmentEditor.InsertTokens(obj, tokens, 0);
			return obj;
		}

		private static string TranslationFeaturesAndTagsToString(TUInfo tuInfo, short translationTokenIndex, short translationTokenCount, CultureInfo ci)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("|");
			for (int i = 0; i < translationTokenCount; i++)
			{
				Token token = tuInfo.memTU.TargetSegment.Tokens[translationTokenIndex + i];
				TagToken tagToken = token as TagToken;
				if (tagToken != null)
				{
					stringBuilder.Append(tagToken);
					stringBuilder.Append("|");
					continue;
				}
				List<short> tokenIndices = new List<short>();
				List<string> features = SubsegmentUtilities.GetFeatures(new List<Token>
				{
					token
				}, ci, tokenIndices);
				if (features != null && features.Count != 0)
				{
					stringBuilder.Append(features[0]);
					stringBuilder.Append("|");
				}
			}
			return stringBuilder.ToString();
		}

		private void AddMatchToResult(AnnotatedSegment docSrcSegment, SubsegmentSearchResults results, Translator translator, short matchTokenIndex, short matchTokenCount, SubsegmentMatchType matchType, short queryTokenIndex, short queryTokenCount, short translationTokenIndex, short translationTokenCount, TUInfo tuInfo, float confidence, bool scoreDiagonalOnly, AnnotatedSegment docFragmentForScoring, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit memTUFragmentForScoring)
		{
			if (Settings.HardFilter != null && !Settings.HardFilter.Evaluate(tuInfo.memTU))
			{
				return;
			}
			tuInfo.TargetFeatureTokenIndices.FindIndex((short x) => x >= translationTokenIndex);
			tuInfo.TargetFeatureTokenIndices.FindLastIndex((short x) => x <= translationTokenIndex + translationTokenCount - 1);
			string translationFeatureString = TranslationFeaturesAndTagsToString(tuInfo, translationTokenIndex, translationTokenCount, TM.Tm.LanguageDirection.TargetCulture);
			SubsegmentSearchResult res = new SubsegmentSearchResult(tuInfo.memTU, matchTokenIndex, matchTokenCount, queryTokenIndex, queryTokenCount, translationTokenIndex, translationTokenCount, matchType, translationFeatureString, confidence);
			TM.SourceTools.EnsureTokenizedSegment(res.MemoryTranslationUnit.SourceSegment, forceRetokenization: false, allowTokenBundles: false);
			TM.TargetTools.EnsureTokenizedSegment(res.MemoryTranslationUnit.TargetSegment, forceRetokenization: false, allowTokenBundles: false);
			res.MemoryPlaceables = PlaceableComputer.ComputePlaceables(memTUFragmentForScoring);
			res.MemoryTranslationUnit = memTUFragmentForScoring;
			List<Placeable> documentPlaceables = PlaceableComputer.ComputePlaceables(docFragmentForScoring.Segment, null);
			_Scorer.ComputeScores(res, docFragmentForScoring, null, documentPlaceables, new TuContextData(), isDuplicateSearch: false, FuzzyIndexes.SourceWordBased, scoreDiagonalOnly);
			res.PlaceableAssociations = null;
			List<Placeable> documentPlaceables2 = PlaceableComputer.ComputePlaceables(docSrcSegment.Segment, null);
			res.MemoryTranslationUnit = tuInfo.memTU;
			res.MemoryPlaceables = PlaceableComputer.ComputePlaceables(tuInfo.memTU);
			if (res.ScoringResult.Match >= Settings.MinScore)
			{
				scoreDiagonalOnly &= (docSrcSegment.Segment.Tokens.Count == res.MemoryTranslationUnit.SourceSegment.Tokens.Count);
				_Scorer.ComputeScores(res, docSrcSegment, null, documentPlaceables2, new TuContextData(), isDuplicateSearch: false, FuzzyIndexes.SourceWordBased, scoreDiagonalOnly);
				translator?.CreateTranslationProposal(res, docSrcSegment.Segment);
				if (res.ScoringResult.IsExactMatch && Settings.Mode == SearchMode.NormalSearch)
				{
					results.RemoveAll((SubsegmentSearchResult i) => !i.ScoringResult.IsExactMatch && i.ScoringResult.Match < 95 && i.MatchType == res.MatchType);
				}
				results.Add(res);
			}
		}
	}
}
