using Sdl.Core.Globalization;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.Lingua.TermRecognition;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal abstract class AbstractScorer
	{
		protected static readonly float WordDeleteOrInsertPenalty = 6f;

		protected static readonly float MinorChangePenalty = 1f;

		protected static readonly float MediumChangePenalty = 3f;

		protected SearchSettings Settings;

		private readonly TextContextMatchType _textContextMatchType;

		private readonly bool _normalizeCharWidths;

		protected Tokenizer LegacySourceTokenizer;

		protected Tokenizer LegacyTargetTokenizer;

		protected LanguageTools SourceTools => GetSourceTools();

		protected LanguageTools TargetTools => GetTargetTools();

		internal AbstractScorer(SearchSettings settings, TextContextMatchType textContextMatchType, bool normalizeCharWidths)
			: this(settings, textContextMatchType)
		{
			_normalizeCharWidths = normalizeCharWidths;
		}

		public AbstractScorer(SearchSettings settings, TextContextMatchType textContextMatchType)
		{
			Settings = settings;
			_textContextMatchType = textContextMatchType;
		}

		protected abstract BuiltinRecognizers Recognizers();

		protected abstract LanguageTools GetSourceTools();

		protected abstract LanguageTools GetTargetTools();

		protected abstract IAnnotatedSegment GetAnnotatedSegment(Segment segment, bool isTargetSegment, bool keepTokens, bool keepPeripheralWhitespace);

		private static long GetSegmentHash(IAnnotatedSegment s)
		{
			return s.Hash;
		}

		private static bool IsWhitespaceOrPunctuation(Token t)
		{
			if (!t.IsWhitespace)
			{
				return t.IsPunctuation;
			}
			return true;
		}

		private static float GetLengthBasedChangeScore(float l)
		{
			if (l <= 3f)
			{
				return MediumChangePenalty;
			}
			if (l >= 6f)
			{
				return WordDeleteOrInsertPenalty;
			}
			float num = MinorChangePenalty + (l - 3f) * (WordDeleteOrInsertPenalty - MinorChangePenalty) / 3f;
			if (!(num < MinorChangePenalty) && !(num > WordDeleteOrInsertPenalty))
			{
				return num;
			}
			return WordDeleteOrInsertPenalty;
		}

		private static float CountWords(IList<Token> tokens, out int totalWords, out int stopWords)
		{
			float num = 0f;
			totalWords = 0;
			stopWords = 0;
			foreach (Token token in tokens)
			{
				if (token.IsWhitespace || token.IsPunctuation)
				{
					num += 0.1f;
				}
				else if (!(token is TagToken))
				{
					num += 1f;
				}
				if (token.IsWord)
				{
					totalWords++;
					if (IsStopword(token))
					{
						stopWords++;
					}
				}
			}
			return num;
		}

		private static bool IsStopword(Token t)
		{
			return (t as SimpleToken)?.IsStopword ?? false;
		}

		private int ComputeSimplifiedMatchScore(EditDistance ed, IList<Token> docSrcTokens, IList<Token> memSrcTokens, out int matchingPlaceholdersByValue, bool applySmallChangeAdjustment, bool normalizeCharWidths, out bool charWidthDifference)
		{
			float num = 0f;
			matchingPlaceholdersByValue = 0;
			charWidthDifference = false;
			int totalWords;
			int stopWords;
			int totalWords2;
			int stopWords2;
			float num2 = CountWords(docSrcTokens, out totalWords, out stopWords) + CountWords(memSrcTokens, out totalWords2, out stopWords2);
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			foreach (EditDistanceItem item in ed.Items)
			{
				switch (item.Operation)
				{
				case EditOperation.Change:
					switch (item.Resolution)
					{
					case EditDistanceResolution.None:
						if (normalizeCharWidths && string.CompareOrdinal(StringUtilities.HalfWidthToFullWidth2(docSrcTokens[item.Source].Text), StringUtilities.HalfWidthToFullWidth2(memSrcTokens[item.Target].Text)) == 0)
						{
							charWidthDifference = true;
						}
						else if (IsWhitespaceOrPunctuation(docSrcTokens[item.Source]) && IsWhitespaceOrPunctuation(memSrcTokens[item.Target]))
						{
							num += 0.1f;
						}
						else if (!(docSrcTokens[item.Source] is TagToken) || !(memSrcTokens[item.Target] is TagToken))
						{
							num = ((!(item.Costs < 0.06)) ? ((!(item.Costs < 0.3)) ? (num + 1.5f) : (num + 0.7f)) : (num + 0.2f));
						}
						else if (item.Resolution != 0)
						{
							num += 0.5f;
						}
						break;
					case EditDistanceResolution.Substitution:
						num5++;
						break;
					}
					break;
				case EditOperation.Move:
					num = ((item.Resolution != 0) ? (num + 0.5f) : (num + 0.8f));
					break;
				case EditOperation.Delete:
					if (item.Resolution == EditDistanceResolution.None)
					{
						num = ((!docSrcTokens[item.Source].IsWhitespace && !docSrcTokens[item.Source].IsPunctuation) ? ((!applySmallChangeAdjustment || docSrcTokens[item.Source].Text.Length > 2) ? (num + 1.5f) : (num + 0.7f)) : (num + 0.1f));
					}
					break;
				case EditOperation.Insert:
					if (item.Resolution == EditDistanceResolution.None)
					{
						num = ((!memSrcTokens[item.Target].IsWhitespace && !memSrcTokens[item.Target].IsPunctuation) ? ((!applySmallChangeAdjustment || memSrcTokens[item.Target].Text.Length > 2) ? (num + 1.5f) : (num + 0.7f)) : (num + 0.1f));
					}
					break;
				case EditOperation.Identity:
					if (item.Resolution == EditDistanceResolution.Substitution && docSrcTokens[item.Source].IsPlaceable)
					{
						matchingPlaceholdersByValue++;
					}
					if (docSrcTokens[item.Source].IsWord)
					{
						if (IsStopword(docSrcTokens[item.Source]))
						{
							num3++;
						}
						else
						{
							num4++;
						}
					}
					break;
				}
			}
			if (num2 <= 0f)
			{
				num2 = 1f;
			}
			int num6 = (int)(num * 100f / num2);
			if (totalWords > 5 && totalWords2 > 5 && num3 + num4 > 0)
			{
				float srcStopRatio = (float)stopWords / (float)totalWords;
				float tgtStopRatio = (float)stopWords2 / (float)totalWords2;
				float identicalStopRatio = (float)num3 / (float)(num3 + num4 + num5);
				num6 += ComputeStopwordRatioMalus(srcStopRatio, tgtStopRatio, identicalStopRatio, totalWords, totalWords2, 7);
			}
			else if (num6 > 0 && num3 > 0)
			{
				float num7 = (float)num3 / (float)(num3 + num4);
				int num8 = (int)(150f / num2);
				num6 += (int)(num7 * (float)num8);
			}
			if (num6 > 100)
			{
				return 0;
			}
			if (num6 == 0 && num > 0f)
			{
				num6++;
			}
			return 100 - num6;
		}

		private static int ComputeStopwordRatioMalus(float srcStopRatio, float tgtStopRatio, float identicalStopRatio, int srcWords, int tgtWords, int maxPenalty)
		{
			if (srcWords <= 5 || tgtWords <= 5)
			{
				return 0;
			}
			float num = (srcStopRatio + tgtStopRatio) / 2f;
			if (num >= identicalStopRatio)
			{
				return 0;
			}
			if (num <= identicalStopRatio * 0.65f)
			{
				return maxPenalty;
			}
			if (num <= identicalStopRatio * 0.8f)
			{
				return maxPenalty / 2;
			}
			return 0;
		}

		private static bool ApplySmallChangeAdjustment(CultureInfo culture)
		{
			switch (culture.TwoLetterISOLanguageName)
			{
			case "ko":
			case "ja":
			case "cz":
				return false;
			default:
				return true;
			}
		}

		public void ComputeScores(SearchResult searchResult, IAnnotatedSegment docSrcSegment, IAnnotatedSegment docTrgSegment, List<Placeable> documentPlaceables, TuContextData tuContextData, bool isDuplicateSearch, FuzzyIndexes usedIndex, bool scoreDiagonalOnly = false)
		{
			bool charWidthDifference = false;
			ScoringResult scoringResult = new ScoringResult();
			if (searchResult.ScoringResult == null)
			{
				searchResult.ScoringResult = scoringResult;
			}
			else
			{
				scoringResult = searchResult.ScoringResult;
			}
			int num;
			if (Settings.IsConcordanceSearch)
			{
				num = GetConcordanceScore(searchResult, docSrcSegment, docTrgSegment);
			}
			else
			{
				bool charactersNormalizeSafely = string.CompareOrdinal(docSrcSegment.Segment.Culture.TwoLetterISOLanguageName, "ko") != 0;
				bool applySmallChangeAdjustment = ApplySmallChangeAdjustment(docSrcSegment.Segment.Culture);
				if (docSrcSegment.Segment.Tokens == null)
				{
					SourceTools.EnsureTokenizedSegment(docSrcSegment.Segment);
				}
				SourceTools.Stem(docSrcSegment.Segment);
				SourceTools.Stem(searchResult.MemoryTranslationUnit.SourceSegment);
				num = 0;
				if (docSrcSegment.Segment.Tokens != null)
				{
					if (SegmentEditDistanceComputer.CanComputeEditDistance(docSrcSegment.Segment.Tokens.Count, searchResult.MemoryTranslationUnit.SourceSegment.Tokens.Count))
					{
						SegmentEditDistanceComputer segmentEditDistanceComputer = new SegmentEditDistanceComputer();
						BuiltinRecognizers disabledAutoSubstitutions = Settings.AutoLocalizationSettings?.DisableAutoSubstitution ?? BuiltinRecognizers.RecognizeNone;
						scoringResult.EditDistance = segmentEditDistanceComputer.ComputeEditDistance(docSrcSegment.Segment.Tokens, searchResult.MemoryTranslationUnit.SourceSegment.Tokens, isDuplicateSearch, disabledAutoSubstitutions, out TagAssociations _, charactersNormalizeSafely, applySmallChangeAdjustment, isDuplicateSearch | scoreDiagonalOnly);
						scoringResult.ResolvedPlaceables = 0;
						TargetTools.Stem(searchResult.MemoryTranslationUnit.TargetSegment);
						if (searchResult.MemoryPlaceables == null)
						{
							searchResult.MemoryPlaceables = searchResult.MemoryTranslationUnit.ComputePlaceables();
						}
						int num2 = 0;
						if (documentPlaceables != null)
						{
							num2 = documentPlaceables.Count((Placeable plc) => plc.IsTag);
						}
						ComputePlaceableAssociations(searchResult, docSrcSegment.Segment, documentPlaceables);
						ResolvePlaceables(searchResult, docSrcSegment.Segment, documentPlaceables);
						CheckPlaceableFormat(docSrcSegment.Segment, searchResult, documentPlaceables);
						if (!isDuplicateSearch)
						{
							if (num2 == 0)
							{
								CheckAndDeleteMemoryTags(searchResult);
							}
							else
							{
								scoringResult.TagMismatch = CheckTagMismatchPenalty(searchResult, documentPlaceables);
							}
						}
						bool normalizeCharWidths = _normalizeCharWidths && CultureInfoExtensions.UseFullWidth(docSrcSegment.Segment.Culture);
						num = ComputeSimplifiedMatchScore(scoringResult.EditDistance, docSrcSegment.Segment.Tokens, searchResult.MemoryTranslationUnit.SourceSegment.Tokens, out int matchingPlaceholdersByValue, applySmallChangeAdjustment, normalizeCharWidths, out charWidthDifference);
						searchResult.MatchingPlaceholderTokens = matchingPlaceholdersByValue;
						if (!isDuplicateSearch && LegacySourceTokenizer != null)
						{
							num = CalculateLegacyScore(docSrcSegment, docTrgSegment, searchResult, disabledAutoSubstitutions, charactersNormalizeSafely, num2, normalizeCharWidths);
						}
					}
					else if (docSrcSegment.Segment.Tokens.Count == searchResult.MemoryTranslationUnit.SourceSegment.Tokens.Count)
					{
						num = ((string.CompareOrdinal(docSrcSegment.Segment.ToString(), searchResult.MemoryTranslationUnit.SourceSegment.ToString()) == 0) ? 100 : 0);
					}
				}
			}
			if (num > 100)
			{
				num = 100;
			}
			else if (num < 0)
			{
				num = 0;
			}
			scoringResult.BaseScore = num;
			searchResult.ScoringResult.TextContextMatch = TextContextMatch.NoMatch;
			if (num <= 0)
			{
				return;
			}
			if (scoringResult.IsExactMatch && !Settings.IsConcordanceSearch)
			{
				CheckDifferentTarget(searchResult, documentPlaceables, docTrgSegment);
				CheckContexts(searchResult, tuContextData);
				if (isDuplicateSearch && scoringResult.TargetSegmentDiffers)
				{
					scoringResult.BaseScore = 0;
					return;
				}
			}
			ApplyPenalties(searchResult, charWidthDifference);
		}

		private void ApplyPenalties(SearchResult searchResult, bool charWidthDifference)
		{
			if (charWidthDifference)
			{
				ApplyCharWidthPenalty(searchResult);
			}
			ApplyFilterPenalties(searchResult);
			ApplyProviderPenalty(searchResult);
			ApplyAlignmentPenatly(searchResult);
			ApplyConfirmationLevelPenalties(searchResult);
		}

		private int CalculateLegacyScore(IAnnotatedSegment docSrcSegment, IAnnotatedSegment docTrgSegment, SearchResult searchResult, BuiltinRecognizers disabledAutoSubstitutions, bool charactersNormalizeSafely, int srcTagCount, bool normalizeCharWidths)
		{
			TranslationUnit translationUnit = new TranslationUnit(searchResult.MemoryTranslationUnit.SourceSegment.Duplicate(), searchResult.MemoryTranslationUnit.TargetSegment.Duplicate());
			List<Token> tokens = docSrcSegment.Segment.Tokens;
			tokens = WordsToCharSequences(tokens, LegacySourceTokenizer, null);
			List<Token> tokens2 = translationUnit.SourceSegment.Tokens;
			tokens2 = WordsToCharSequences(tokens2, LegacySourceTokenizer, null);
			List<Token> tokens3 = translationUnit.TargetSegment.Tokens;
			if (LegacyTargetTokenizer != null)
			{
				tokens3 = WordsToCharSequences(tokens3, LegacyTargetTokenizer, null);
			}
			translationUnit.SourceSegment.Tokens = tokens2;
			translationUnit.TargetSegment.Tokens = tokens3;
			SearchResult searchResult2 = new SearchResult(translationUnit);
			searchResult2.MemoryPlaceables = translationUnit.ComputePlaceables();
			Segment segment = docSrcSegment.Segment.Duplicate();
			segment.Tokens = tokens;
			Segment segment2 = null;
			if (docTrgSegment?.Segment != null)
			{
				segment2 = docTrgSegment.Segment.Duplicate();
			}
			List<Placeable> list = new List<Placeable>();
			if (segment2 != null)
			{
				list = PlaceableComputer.ComputePlaceables(segment, segment2);
			}
			else if (!Settings.IsConcordanceSearch)
			{
				list = PlaceableComputer.ComputePlaceables(segment, null);
			}
			SegmentEditDistanceComputer segmentEditDistanceComputer = new SegmentEditDistanceComputer();
			TagAssociations alignedTags = null;
			searchResult2.ScoringResult = new ScoringResult();
			searchResult2.ScoringResult.EditDistance = segmentEditDistanceComputer.ComputeEditDistance(tokens, tokens2, computeDiagonalOnly: false, disabledAutoSubstitutions, out alignedTags, charactersNormalizeSafely, applySmallChangeAdjustment: true, diagonalOnly: false);
			searchResult2.MemoryPlaceables = PlaceableComputer.ComputePlaceables(translationUnit);
			ComputePlaceableAssociations(searchResult2, segment, list);
			ResolvePlaceables(searchResult2, segment, list);
			if (srcTagCount == 0)
			{
				CheckAndDeleteMemoryTags(searchResult2);
			}
			else
			{
				searchResult2.ScoringResult.TagMismatch = CheckTagMismatchPenalty(searchResult2, list);
			}
			int matchingPlaceholdersByValue;
			bool charWidthDifference;
			return ComputeSimplifiedMatchScore(searchResult2.ScoringResult.EditDistance, tokens, tokens2, out matchingPlaceholdersByValue, applySmallChangeAdjustment: true, normalizeCharWidths, out charWidthDifference);
		}

		private List<Token> WordsToCharSequences(List<Token> tokens, Tokenizer tokenizer, IDictionary<int, int> tokenIndexMap)
		{
			List<Token> list = new List<Token>();
			int num = 0;
			int num2 = 0;
			foreach (Token token in tokens)
			{
				if (token.Type != TokenType.Word)
				{
					list.Add(token);
					tokenIndexMap?.Add(num2, num);
					num2++;
					num++;
				}
				else
				{
					string s = token.Text;
					if (_normalizeCharWidths)
					{
						SimpleToken simpleToken = token as SimpleToken;
						if (simpleToken != null && simpleToken.Type == TokenType.Word && !string.IsNullOrEmpty(simpleToken.Stem) && simpleToken.Stem.Length != simpleToken.Text.Length)
						{
							s = simpleToken.Stem;
						}
					}
					List<Token> list2 = tokenizer.GetTokens(s, enhancedAsian: false);
					foreach (Token item in list2)
					{
						if (item.Text != null && item.Text.Length == 1 && item.Text[0] == '\uff70')
						{
							item.Type = TokenType.GeneralPunctuation;
						}
						if (item.Type == TokenType.Acronym)
						{
							item.Type = TokenType.Word;
						}
					}
					if (_normalizeCharWidths)
					{
						List<Token> list3 = new List<Token>();
						foreach (Token item2 in list2)
						{
							if (item2.Type != TokenType.Word)
							{
								list3.Add(item2);
							}
							else
							{
								string text = item2.Text;
								for (int i = 0; i < text.Length; i++)
								{
									list3.Add(new SimpleToken(string.Concat(str1: text[i].ToString(), str0: string.Empty), TokenType.CharSequence));
								}
							}
						}
						list2 = list3;
					}
					foreach (Token item3 in list2)
					{
						_ = item3;
						tokenIndexMap?.Add(num2, num);
						num2++;
					}
					num++;
					list.AddRange(list2);
				}
			}
			return list;
		}

		private int GetConcordanceScore(SearchResult searchResult, IAnnotatedSegment docSrcSegment, IAnnotatedSegment docTrgSegment)
		{
			IAnnotatedSegment annotatedSegment;
			IAnnotatedSegment annotatedSegment2;
			if (Settings.Mode == SearchMode.ConcordanceSearch)
			{
				annotatedSegment = docSrcSegment;
				annotatedSegment2 = GetAnnotatedSegment(searchResult.MemoryTranslationUnit.SourceSegment, isTargetSegment: false, keepTokens: true, keepPeripheralWhitespace: false);
				SourceTools.EnsureTokenizedSegment(annotatedSegment.Segment);
				SourceTools.EnsureTokenizedSegment(annotatedSegment2.Segment);
			}
			else
			{
				annotatedSegment = docTrgSegment;
				annotatedSegment2 = GetAnnotatedSegment(searchResult.MemoryTranslationUnit.TargetSegment, isTargetSegment: true, keepTokens: true, keepPeripheralWhitespace: false);
				TargetTools.EnsureTokenizedSegment(annotatedSegment.Segment);
				TargetTools.EnsureTokenizedSegment(annotatedSegment2.Segment);
			}
			bool useWidthNormalization = _normalizeCharWidths && CultureInfoExtensions.UseFullWidth(annotatedSegment2.Segment.Culture);
			TermFinderResult termFinderResult = TermFinder.FindTerms(annotatedSegment.Segment, annotatedSegment2.Segment, expectContinuousMatch: true, useWidthNormalization);
			if (termFinderResult?.MatchingRanges == null || termFinderResult.MatchingRanges.Count == 0)
			{
				return 0;
			}
			searchResult.ScoringResult.MatchingConcordanceRanges = termFinderResult.MatchingRanges;
			return termFinderResult.Score;
		}

		private void CheckPlaceableFormat(Segment docSegment, SearchResult searchResult, List<Placeable> documentPlaceables)
		{
			if (documentPlaceables != null)
			{
				foreach (Placeable plc in documentPlaceables)
				{
					PlaceableAssociation placeableAssociation = searchResult.PlaceableAssociations?.Find((PlaceableAssociation element) => element.Document.Equals(plc));
					if (placeableAssociation != null && placeableAssociation.Memory != null && placeableAssociation.Memory.SourceTokenIndex >= 0)
					{
						ILocalizableToken localizableToken = docSegment.Tokens[placeableAssociation.Document.SourceTokenIndex] as ILocalizableToken;
						Token token = searchResult.MemoryTranslationUnit.SourceSegment.Tokens[placeableAssociation.Memory.SourceTokenIndex];
						if (localizableToken != null && !localizableToken.DoesFormatMatch(token as ILocalizableToken))
						{
							searchResult.ScoringResult.PlaceableFormatChanges++;
						}
					}
				}
			}
		}

		private bool CheckTagMismatchPenalty(SearchResult searchResult, List<Placeable> documentPlaceables)
		{
			ScoringResult scoringResult = searchResult.ScoringResult;
			bool tagMismatch = false;
			if (searchResult.MemoryPlaceables != null)
			{
				foreach (Placeable plc2 in searchResult.MemoryPlaceables)
				{
					if (searchResult.PlaceableAssociations?.Find((PlaceableAssociation element) => element.Memory.Equals(plc2)) == null && plc2.IsTag)
					{
						int edItem = scoringResult.EditDistance.FindTargetItemIndex(plc2.SourceTokenIndex);
						ApplyTagMismatchPenalty(scoringResult, searchResult, edItem, plc2, ref tagMismatch);
					}
				}
			}
			if (documentPlaceables == null)
			{
				return tagMismatch;
			}
			foreach (Placeable plc in documentPlaceables)
			{
				if (searchResult.PlaceableAssociations?.Find((PlaceableAssociation element) => element.Document.Equals(plc)) == null && plc.IsTag)
				{
					int edItem2 = scoringResult.EditDistance.FindSourceItemIndex(plc.SourceTokenIndex);
					ApplyTagMismatchPenalty(scoringResult, searchResult, edItem2, plc, ref tagMismatch);
				}
			}
			return tagMismatch;
		}

		private void ApplyTagMismatchPenalty(ScoringResult result, SearchResult searchResult, int edItem, Placeable plc, ref bool tagMismatch)
		{
			if (edItem >= 0 && result.EditDistance.Items[edItem].Resolution == EditDistanceResolution.None)
			{
				searchResult.ScoringResult.EditDistance.SetResolutionAt(edItem, EditDistanceResolution.Other);
				Penalty pt;
				if (plc.Type != PlaceableType.PairedTagEnd && (pt = Settings.FindPenalty(PenaltyType.TagMismatch)) != null)
				{
					result.ApplyPenalty(pt);
				}
				tagMismatch = true;
			}
		}

		private void CheckAndDeleteMemoryTags(SearchResult searchResult)
		{
			if (searchResult.MemoryPlaceables != null && searchResult.MemoryPlaceables.Count != 0)
			{
				ScoringResult scoringResult = searchResult.ScoringResult;
				foreach (Placeable memoryPlaceable in searchResult.MemoryPlaceables)
				{
					if ((memoryPlaceable.Type == PlaceableType.PairedTagStart || memoryPlaceable.Type == PlaceableType.PairedTagEnd || memoryPlaceable.Type == PlaceableType.StandaloneTag) && memoryPlaceable.SourceTokenIndex >= 0)
					{
						int num = scoringResult.EditDistance.FindTargetItemIndex(memoryPlaceable.SourceTokenIndex);
						if (num >= 0)
						{
							_ = scoringResult.EditDistance[num].Operation;
							_ = 3;
							searchResult.ScoringResult.EditDistance.SetResolutionAt(num, EditDistanceResolution.Deletion);
							int num2 = ++scoringResult.ResolvedPlaceables;
							scoringResult.MemoryTagsDeleted = true;
						}
					}
				}
				Penalty pt;
				if (scoringResult.MemoryTagsDeleted && (pt = Settings.FindPenalty(PenaltyType.MemoryTagsDeleted)) != null)
				{
					scoringResult.ApplyPenalty(pt);
				}
			}
		}

		private void CheckDifferentTarget(SearchResult searchResult, List<Placeable> documentPlaceables, IAnnotatedSegment docTrgSegment)
		{
			ScoringResult scoringResult = searchResult.ScoringResult;
			if (docTrgSegment == null)
			{
				return;
			}
			IAnnotatedSegment annotatedSegment = GetAnnotatedSegment(searchResult.MemoryTranslationUnit.TargetSegment, isTargetSegment: true, keepTokens: true, keepPeripheralWhitespace: true);
			if (annotatedSegment.Segment.Tokens == null)
			{
				TargetTools.EnsureTokenizedSegment(annotatedSegment.Segment);
			}
			if (docTrgSegment.Segment.Tokens == null)
			{
				TargetTools.EnsureTokenizedSegment(docTrgSegment.Segment);
			}
			bool flag = docTrgSegment.Segment.Tokens.Count == annotatedSegment.Segment.Tokens.Count && GetSegmentHash(docTrgSegment) == GetSegmentHash(annotatedSegment);
			if (flag)
			{
				TagAssociations alignedTags;
				EditDistance editDistance = new SegmentEditDistanceComputer(computeMoves: false).ComputeEditDistance(disabledAutoSubstitutions: Settings.AutoLocalizationSettings?.DisableAutoSubstitution ?? BuiltinRecognizers.RecognizeNone, sourceTokens: docTrgSegment.Segment.Tokens, targetTokens: annotatedSegment.Segment.Tokens, computeDiagonalOnly: true, alignedTags: out alignedTags);
				if (editDistance.Distance > 0.0)
				{
					for (int i = 0; i < editDistance.Items.Count; i++)
					{
						if (!flag)
						{
							break;
						}
						EditDistanceItem item = editDistance.Items[i];
						switch (item.Operation)
						{
						case EditOperation.Change:
							if (searchResult.ScoringResult.ResolvedPlaceables == 0 || searchResult.PlaceableAssociations == null || searchResult.PlaceableAssociations.Count == 0)
							{
								flag = false;
							}
							else
							{
								flag = searchResult.PlaceableAssociations.Any((PlaceableAssociation x) => x.Document.TargetTokenIndex == item.Source && x.Memory.TargetTokenIndex == item.Target);
								if (!flag)
								{
									flag = (searchResult.PlaceableAssociations.Any((PlaceableAssociation x) => x.Document.TargetTokenIndex == item.Source) && searchResult.PlaceableAssociations.Any((PlaceableAssociation x) => x.Memory.TargetTokenIndex == item.Target));
								}
							}
							if (!flag && docTrgSegment.Segment.Tokens[item.Source] is TagToken && annotatedSegment.Segment.Tokens[item.Target] is TagToken)
							{
								bool num = documentPlaceables?.Exists((Placeable x) => x.SourceTokenIndex == -1 && x.TargetTokenIndex == item.Source) ?? false;
								bool flag2 = searchResult.MemoryPlaceables != null && searchResult.MemoryPlaceables.Exists((Placeable x) => x.SourceTokenIndex == -1 && x.TargetTokenIndex == item.Target);
								flag = (num & flag2);
							}
							break;
						case EditOperation.Move:
						case EditOperation.Insert:
						case EditOperation.Delete:
							flag = false;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				searchResult.ScoringResult.TargetSegmentDiffers = true;
				Penalty penalty = Settings.FindPenalty(PenaltyType.TargetSegmentMismatch);
				if (penalty != null)
				{
					scoringResult.ApplyPenalty(penalty);
				}
			}
		}

		public void CheckContexts(SearchResult searchResult, TuContextData tuContextData)
		{
			if (tuContextData != null)
			{
				CheckTextContext(searchResult, tuContextData.TextContext);
				CheckStructureContext(searchResult, tuContextData.CurrentStructureContextOverride);
				CheckIdContext(searchResult, tuContextData.IdContext);
			}
		}

		private void CheckTextContext(SearchResult searchResult, TuContext leftContext)
		{
			searchResult.ScoringResult.TextContextMatch = TextContextMatch.NoMatch;
			if (leftContext != null && searchResult.MemoryTranslationUnit.Contexts != null)
			{
				TextContextMatch textContextMatch = TextContextMatch.NoMatch;
				foreach (TuContext value in searchResult.MemoryTranslationUnit.Contexts.Values)
				{
					if (_textContextMatchType == TextContextMatchType.PrecedingSourceAndTarget)
					{
						textContextMatch = TextContextMatch.SourceMatch;
					}
					if (leftContext.Context2 == value.Context2)
					{
						textContextMatch = TextContextMatch.SourceTargetMatch;
						if (_textContextMatchType == TextContextMatchType.PrecedingAndFollowingSource)
						{
							textContextMatch = TextContextMatch.PrecedingAndFollowingSourceMatch;
						}
						break;
					}
				}
				searchResult.ScoringResult.TextContextMatch = textContextMatch;
			}
		}

		private void CheckStructureContext(SearchResult searchResult, string currentStructureContextOverride)
		{
			ScoringResult scoringResult = searchResult.ScoringResult;
			scoringResult.IsStructureContextMatch = false;
			string text = currentStructureContextOverride ?? Settings.CurrentStructureContext;
			if (!string.IsNullOrEmpty(text) && searchResult.MemoryTranslationUnit.FieldValues != null)
			{
				MultipleStringFieldValue multipleStringFieldValue = searchResult.MemoryTranslationUnit.FieldValues[Field.StructureContextFieldName] as MultipleStringFieldValue;
				if (multipleStringFieldValue != null)
				{
					scoringResult.IsStructureContextMatch = multipleStringFieldValue.HasValue(text);
				}
			}
		}

		private static void CheckIdContext(SearchResult searchResult, string idContext)
		{
			if (!(idContext == string.Empty) && searchResult.MemoryTranslationUnit.IdContexts != null)
			{
				searchResult.ScoringResult.IdContextMatch = searchResult.MemoryTranslationUnit.IdContexts.HasValue(idContext);
			}
		}

		private void ApplyConfirmationLevelPenalty(SearchResult searchResult, ConfirmationLevel confirmationLevel, PenaltyType penalty)
		{
			Penalty penalty2;
			if ((penalty2 = Settings.FindPenalty(penalty)) != null && penalty2.Malus > 0 && searchResult.MemoryTranslationUnit.ConfirmationLevel == confirmationLevel)
			{
				searchResult.ScoringResult.ApplyPenalty(penalty2);
			}
		}

		private void ApplyConfirmationLevelPenalties(SearchResult searchResult)
		{
			ApplyConfirmationLevelPenalty(searchResult, ConfirmationLevel.Unspecified, PenaltyType.NotTranslated);
			ApplyConfirmationLevelPenalty(searchResult, ConfirmationLevel.Draft, PenaltyType.Draft);
			ApplyConfirmationLevelPenalty(searchResult, ConfirmationLevel.Translated, PenaltyType.Translated);
			ApplyConfirmationLevelPenalty(searchResult, ConfirmationLevel.ApprovedSignOff, PenaltyType.ApprovedSignOff);
			ApplyConfirmationLevelPenalty(searchResult, ConfirmationLevel.ApprovedTranslation, PenaltyType.ApprovedTranslation);
			ApplyConfirmationLevelPenalty(searchResult, ConfirmationLevel.RejectedTranslation, PenaltyType.RejectedTranslation);
			ApplyConfirmationLevelPenalty(searchResult, ConfirmationLevel.RejectedSignOff, PenaltyType.RejectedSignOff);
		}

		private void ApplyProviderPenalty(SearchResult searchResult)
		{
			Penalty penalty;
			if ((penalty = Settings.FindPenalty(PenaltyType.ProviderPenalty)) != null && penalty.Malus > 0)
			{
				searchResult.ScoringResult.ApplyPenalty(penalty);
			}
		}

		private void ApplyAlignmentPenatly(SearchResult searchResult)
		{
			Penalty penalty;
			if ((penalty = Settings.FindPenalty(PenaltyType.Alignment)) != null && searchResult.MemoryTranslationUnit.Origin == TranslationUnitOrigin.Alignment && searchResult.MemoryTranslationUnit.SystemFields.CreationDate.Equals(searchResult.MemoryTranslationUnit.SystemFields.ChangeDate) && penalty.Malus > 0)
			{
				searchResult.ScoringResult.ApplyPenalty(penalty);
			}
		}

		private void ApplyCharWidthPenalty(SearchResult searchResult)
		{
			Penalty penalty;
			if ((penalty = Settings.FindPenalty(PenaltyType.CharacterWidthDifference)) != null && penalty.Malus > 0)
			{
				searchResult.ScoringResult.ApplyPenalty(penalty);
			}
		}

		private void ApplyFilterPenalties(SearchResult searchResult)
		{
			ScoringResult scoringResult = searchResult.ScoringResult;
			if (Settings.Filters != null)
			{
				foreach (Filter filter in Settings.Filters)
				{
					if (filter.FilterExpression != null && !filter.FilterExpression.Evaluate(searchResult.MemoryTranslationUnit))
					{
						scoringResult.ApplyFilter(filter.Name, filter.Penalty);
					}
				}
			}
		}

		public void ApplySoftFilters(SearchResult searchResult)
		{
			ApplyFilterPenalties(searchResult);
		}

		public static bool ArePlaceablesAssignable(Placeable srcPlc, Placeable memPlc, Segment docSegment, Segment memSegment)
		{
			if (!PlaceableAssociation.AreAssociable(srcPlc, memPlc))
			{
				return false;
			}
			if (docSegment.Tokens[srcPlc.SourceTokenIndex].GetSimilarity(memSegment.Tokens[memPlc.SourceTokenIndex], allowCompatibility: true) >= SegmentElement.Similarity.IdenticalType)
			{
				return true;
			}
			if ((srcPlc.Type == PlaceableType.StandaloneTag && memPlc.Type == PlaceableType.TextPlaceholder) || (srcPlc.Type == PlaceableType.TextPlaceholder && memPlc.Type == PlaceableType.StandaloneTag))
			{
				if (docSegment.Tokens[srcPlc.SourceTokenIndex] != null && memSegment.Tokens[memPlc.SourceTokenIndex] != null && docSegment.Tokens[srcPlc.SourceTokenIndex].Type == TokenType.Tag)
				{
					return memSegment.Tokens[memPlc.SourceTokenIndex].Type == TokenType.Tag;
				}
				return false;
			}
			return false;
		}

		private static void ComputePlaceableAssociations(SearchResult searchResult, Segment docSrcSegment, IReadOnlyCollection<Placeable> sourcePlaceables)
		{
			if (searchResult.PlaceableAssociations == null && searchResult.MemoryPlaceables != null && searchResult.MemoryPlaceables.Count != 0 && sourcePlaceables != null && sourcePlaceables.Count != 0)
			{
				ScoringResult scoringResult = searchResult.ScoringResult;
				List<PlaceableAssociation> list = new List<PlaceableAssociation>();
				bool[] array = new bool[searchResult.MemoryPlaceables.Count];
				foreach (Placeable sourcePlaceable in sourcePlaceables)
				{
					if (sourcePlaceable.SourceTokenIndex >= 0)
					{
						int num = scoringResult.EditDistance.FindSourceItemIndex(sourcePlaceable.SourceTokenIndex);
						if (num < 0)
						{
							throw new LanguagePlatformException(ErrorCode.InternalError);
						}
						switch (scoringResult.EditDistance[num].Operation)
						{
						case EditOperation.Identity:
						case EditOperation.Change:
						{
							for (int j = 0; j < searchResult.MemoryPlaceables.Count; j++)
							{
								if (!array[j])
								{
									Placeable placeable2 = searchResult.MemoryPlaceables[j];
									if (placeable2.SourceTokenIndex == scoringResult.EditDistance[num].Target && ArePlaceablesAssignable(sourcePlaceable, placeable2, docSrcSegment, searchResult.MemoryTranslationUnit.SourceSegment))
									{
										list.Add(new PlaceableAssociation(sourcePlaceable, placeable2));
										array[j] = true;
										break;
									}
								}
							}
							break;
						}
						case EditOperation.Move:
						{
							for (int i = 0; i < searchResult.MemoryPlaceables.Count; i++)
							{
								if (!array[i])
								{
									Placeable placeable = searchResult.MemoryPlaceables[i];
									if (placeable.SourceTokenIndex == scoringResult.EditDistance[num].Target && placeable.Type == sourcePlaceable.Type)
									{
										list.Add(new PlaceableAssociation(sourcePlaceable, placeable));
										array[i] = true;
										break;
									}
								}
							}
							break;
						}
						}
					}
				}
				if (list.Count > 0)
				{
					searchResult.PlaceableAssociations = list;
				}
			}
		}

		private void ResolvePlaceables(SearchResult searchResult, Segment docSrcSegment, List<Placeable> sourcePlaceables)
		{
			if (searchResult.PlaceableAssociations != null && searchResult.PlaceableAssociations.Count != 0)
			{
				Dictionary<string, List<PlaceableAssociation>> dictionary = new Dictionary<string, List<PlaceableAssociation>>();
				foreach (Placeable sourcePlaceable in sourcePlaceables)
				{
					Translator.SetDuplicateAssociations(searchResult, docSrcSegment, sourcePlaceable.SourceTokenIndex, dictionary, isDocument: true);
				}
				Dictionary<string, List<PlaceableAssociation>> dictionary2 = new Dictionary<string, List<PlaceableAssociation>>();
				foreach (Placeable memoryPlaceable in searchResult.MemoryPlaceables)
				{
					Translator.SetDuplicateAssociations(searchResult, searchResult.MemoryTranslationUnit.SourceSegment, memoryPlaceable.SourceTokenIndex, dictionary2, isDocument: false);
				}
				ScoringResult scoringResult = searchResult.ScoringResult;
				foreach (PlaceableAssociation placeableAssociation in searchResult.PlaceableAssociations)
				{
					Placeable document = placeableAssociation.Document;
					Placeable memory = placeableAssociation.Memory;
					int index = scoringResult.EditDistance.FindSourceItemIndex(document.SourceTokenIndex);
					Token token = docSrcSegment.Tokens[document.SourceTokenIndex];
					bool flag = token.IsSubstitutable && searchResult.MemoryTranslationUnit.SourceSegment.Tokens[memory.SourceTokenIndex].IsSubstitutable;
					switch (scoringResult.EditDistance[index].Operation)
					{
					case EditOperation.Identity:
					case EditOperation.Change:
						if (flag)
						{
							string placeholderKey = Translator.GetPlaceholderKey(searchResult.MemoryTranslationUnit.SourceSegment.Tokens[memory.SourceTokenIndex]);
							if (dictionary2.ContainsKey(placeholderKey) && dictionary2[placeholderKey].Count > 1)
							{
								string placeholderKey2 = Translator.GetPlaceholderKey(docSrcSegment.Tokens[document.SourceTokenIndex]);
								if (dictionary.ContainsKey(placeholderKey2) && (dictionary[placeholderKey2].Count != dictionary2[placeholderKey].Count || !Translator.ArePlaceableAssociationListsEqual(dictionary[placeholderKey2], dictionary2[placeholderKey])))
								{
									flag = false;
								}
							}
						}
						if (flag && !memory.IsTag)
						{
							if (memory.TargetTokenIndex < 0 || !searchResult.MemoryTranslationUnit.TargetSegment.Tokens[memory.TargetTokenIndex].IsSubstitutable)
							{
								flag = false;
							}
							if (flag && Settings.AutoLocalizationSettings != null)
							{
								flag = Settings.AutoLocalizationSettings.AttemptAutoSubstitution(token);
							}
						}
						if (flag)
						{
							scoringResult.EditDistance.SetResolutionAt(index, EditDistanceResolution.Substitution);
							int num = ++scoringResult.ResolvedPlaceables;
						}
						break;
					case EditOperation.Move:
						if (flag && Settings.AutoLocalizationSettings != null)
						{
							flag = Settings.AutoLocalizationSettings.AttemptAutoSubstitution(token);
						}
						if (flag)
						{
							scoringResult.EditDistance.SetResolutionAt(index, EditDistanceResolution.Move);
						}
						break;
					}
				}
			}
		}
	}
}
