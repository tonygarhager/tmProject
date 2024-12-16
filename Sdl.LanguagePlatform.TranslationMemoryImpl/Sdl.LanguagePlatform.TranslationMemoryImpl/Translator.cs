using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class Translator
	{
		private readonly SearchSettings _settings;

		private readonly TextContextMatchType _textContextMatchType = TextContextMatchType.PrecedingSourceAndTarget;

		public TokenizationContext SourceTokenizationContext
		{
			get;
			set;
		}

		public TokenizationContext TargetTokenizationContext
		{
			get;
			set;
		}

		public Translator(SearchSettings settings)
		{
			_settings = settings;
		}

		internal Translator(SearchSettings settings, TextContextMatchType textContextMatchType)
		{
			_settings = settings;
			_textContextMatchType = textContextMatchType;
		}

		public void CreateTranslationProposal(SearchResult searchResult, Segment docSourceSegment)
		{
			CreateTranslationProposal(searchResult, docSourceSegment, panicMode: false);
		}

		private static SearchResult CreateVirtualTranslationSearchResult(TranslationUnit tu, int score)
		{
			tu.Origin = TranslationUnitOrigin.MachineTranslation;
			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
			return new SearchResult(tu)
			{
				TranslationProposal = new TranslationUnit(tu),
				ScoringResult = new ScoringResult
				{
					BaseScore = score,
					EditDistance = new EditDistance(1, 1, 0.0)
				}
			};
		}

		public static Segment CopySourceWithAutoLocalization(Segment docSourceSegment, CultureInfo targetCulture, AutoLocalizationSettings autoLocalizationSettings)
		{
			if (targetCulture == null)
			{
				throw new ArgumentNullException("targetCulture");
			}
			if (docSourceSegment?.Elements == null || docSourceSegment.Elements.Count == 0 || docSourceSegment.Tokens == null)
			{
				return null;
			}
			HashSet<int> hashSet = new HashSet<int>();
			Segment segment = docSourceSegment.Duplicate();
			segment.Culture = targetCulture;
			for (int i = 0; i < segment.Tokens.Count; i++)
			{
				Token token = segment.Tokens[i];
				ILocalizableToken localizableToken = token as ILocalizableToken;
				if (DoAttemptAutoSubstitution(token, autoLocalizationSettings))
				{
					try
					{
						localizableToken?.Localize(targetCulture, autoLocalizationSettings);
						hashSet.Add(i);
					}
					catch
					{
					}
				}
			}
			segment.UpdateFromTokenIndices(hashSet);
			return segment;
		}

		private static bool IsAutoTranslatable(Token t, BuiltinRecognizers disabledAutoSubs)
		{
			switch (t.Type)
			{
			case TokenType.AlphaNumeric:
				return (disabledAutoSubs & BuiltinRecognizers.RecognizeAlphaNumeric) == 0;
			case TokenType.Date:
				return (disabledAutoSubs & BuiltinRecognizers.RecognizeDates) == 0;
			case TokenType.Measurement:
				return (disabledAutoSubs & BuiltinRecognizers.RecognizeMeasurements) == 0;
			case TokenType.Number:
				return (disabledAutoSubs & BuiltinRecognizers.RecognizeNumbers) == 0;
			case TokenType.Time:
				return (disabledAutoSubs & BuiltinRecognizers.RecognizeTimes) == 0;
			case TokenType.GeneralPunctuation:
			case TokenType.OpeningPunctuation:
			case TokenType.ClosingPunctuation:
			case TokenType.Variable:
			case TokenType.Whitespace:
			case TokenType.Uri:
			case TokenType.Tag:
				return true;
			case TokenType.OtherTextPlaceable:
			{
				SimpleToken simpleToken = t as SimpleToken;
				if (simpleToken == null)
				{
					return simpleToken.IsSubstitutable;
				}
				return true;
			}
			default:
				return false;
			}
		}

		private void NumberTokenVirtualTranslationPrep(NumberToken translatedNumber)
		{
			if (translatedNumber.GroupSeparator != 0)
			{
				translatedNumber.GroupSeparator = NumericSeparator.Primary;
			}
			if (translatedNumber.DecimalSeparator != 0)
			{
				translatedNumber.DecimalSeparator = NumericSeparator.Primary;
			}
		}

		private SearchResult LegacyVirtualTranslation(Segment docSourceSegment, TranslationMemorySetup tm)
		{
			if (docSourceSegment.Tokens.Count != 1)
			{
				return null;
			}
			if (docSourceSegment.Tokens[0].GetType() == typeof(NumberToken))
			{
				NumberToken numberToken = docSourceSegment.Tokens[0] as NumberToken;
				NumberToken numberToken2;
				if (_settings.AutoLocalizationSettings != null && !_settings.AutoLocalizationSettings.AttemptAutoSubstitution(numberToken))
				{
					numberToken2 = numberToken;
				}
				else
				{
					numberToken2 = new NumberToken(numberToken);
					NumberTokenVirtualTranslationPrep(numberToken2);
					numberToken2.TokenizationContext = TargetTokenizationContext;
					numberToken2.Localize(tm.LanguageDirection.TargetCulture, _settings.AutoLocalizationSettings);
				}
				TranslationUnit translationUnit = new TranslationUnit();
				translationUnit.SourceSegment = docSourceSegment.Duplicate();
				translationUnit.TargetSegment = new Segment(tm.LanguageDirection.TargetCulture);
				translationUnit.TargetSegment.Add(numberToken2?.Text);
				return CreateVirtualTranslationSearchResult(translationUnit, 100);
			}
			if (docSourceSegment.Tokens[0].GetType() == typeof(MeasureToken))
			{
				MeasureToken measureToken = docSourceSegment.Tokens[0] as MeasureToken;
				MeasureToken measureToken2;
				if (_settings.AutoLocalizationSettings != null && !_settings.AutoLocalizationSettings.AttemptAutoSubstitution(measureToken))
				{
					measureToken2 = measureToken;
				}
				else
				{
					measureToken2 = new MeasureToken(measureToken);
					measureToken2.TokenizationContext = TargetTokenizationContext;
					measureToken2.Localize(tm.LanguageDirection.TargetCulture, _settings.AutoLocalizationSettings);
				}
				TranslationUnit translationUnit2 = new TranslationUnit();
				translationUnit2.SourceSegment = docSourceSegment.Duplicate();
				translationUnit2.TargetSegment = new Segment(tm.LanguageDirection.TargetCulture);
				translationUnit2.TargetSegment.Add(measureToken2?.Text);
				return CreateVirtualTranslationSearchResult(translationUnit2, 95);
			}
			SimpleToken simpleToken = docSourceSegment.Tokens[0] as SimpleToken;
			if (simpleToken == null || docSourceSegment.Tokens[0].Type != TokenType.Uri)
			{
				return null;
			}
			SimpleToken simpleToken2 = new SimpleToken(simpleToken);
			TranslationUnit translationUnit3 = new TranslationUnit();
			translationUnit3.SourceSegment = docSourceSegment.Duplicate();
			translationUnit3.TargetSegment = new Segment(tm.LanguageDirection.TargetCulture);
			translationUnit3.TargetSegment.Add(simpleToken2.Text);
			return CreateVirtualTranslationSearchResult(translationUnit3, 95);
		}

		public SearchResult AttemptVirtualTranslation(Segment docSourceSegment, TranslationMemorySetup tm)
		{
			if (tm == null)
			{
				throw new ArgumentNullException("tm");
			}
			BuiltinRecognizers disabledAutoSubs = BuiltinRecognizers.RecognizeNone;
			if (_settings?.AutoLocalizationSettings != null)
			{
				disabledAutoSubs = _settings.AutoLocalizationSettings.DisableAutoSubstitution;
			}
			if (docSourceSegment?.Elements == null || docSourceSegment.Elements.Count == 0 || docSourceSegment.Tokens == null)
			{
				return null;
			}
			SearchResult searchResult = LegacyVirtualTranslation(docSourceSegment, tm);
			if (searchResult != null)
			{
				return searchResult;
			}
			HashSet<TokenType> hashSet = new HashSet<TokenType>
			{
				TokenType.Number,
				TokenType.Measurement
			};
			for (int i = 0; i < docSourceSegment.Tokens.Count; i++)
			{
				Token token = docSourceSegment.Tokens[i];
				if (!IsAutoTranslatable(token, disabledAutoSubs))
				{
					if (i == 0 || i == docSourceSegment.Tokens.Count - 1)
					{
						return null;
					}
					if (token.Type != TokenType.Word)
					{
						return null;
					}
					if (string.CompareOrdinal(token.Text, "x") != 0)
					{
						return null;
					}
					int num = i - 1;
					int j = i + 1;
					while (num > 0 && docSourceSegment.Tokens[num].Type == TokenType.Whitespace)
					{
						num--;
					}
					for (; j < docSourceSegment.Tokens.Count - 1 && docSourceSegment.Tokens[j].Type == TokenType.Whitespace; j++)
					{
					}
					TokenType type = docSourceSegment.Tokens[num].Type;
					TokenType type2 = docSourceSegment.Tokens[j].Type;
					if (!hashSet.Contains(type))
					{
						return null;
					}
					if (!hashSet.Contains(type2))
					{
						return null;
					}
				}
			}
			TranslationUnit translationUnit = new TranslationUnit();
			translationUnit.SourceSegment = docSourceSegment.Duplicate();
			Segment segment = docSourceSegment.Duplicate();
			foreach (Token token2 in segment.Tokens)
			{
				ILocalizableToken localizableToken = token2 as ILocalizableToken;
				if (localizableToken != null)
				{
					NumberToken numberToken = token2 as NumberToken;
					if (numberToken != null)
					{
						NumberTokenVirtualTranslationPrep(numberToken);
					}
					localizableToken.TokenizationContext = TargetTokenizationContext;
					localizableToken.Localize(tm.LanguageDirection.TargetCulture, _settings?.AutoLocalizationSettings);
				}
			}
			translationUnit.TargetSegment = new Segment(tm.LanguageDirection.TargetCulture)
			{
				Tokens = new List<Token>()
			};
			SegmentEditor.AppendTokens(translationUnit.TargetSegment, segment.Tokens);
			return CreateVirtualTranslationSearchResult(translationUnit, 95);
		}

		internal static Token SafeGetToken(Segment segment, int index)
		{
			if (segment.Tokens == null || index < 0 || index >= segment.Tokens.Count)
			{
				return null;
			}
			return segment.Tokens[index];
		}

		internal static string GetPlaceholderKey(Token tok)
		{
			string text;
			switch (tok.Type)
			{
			case TokenType.Abbreviation:
			case TokenType.Variable:
			case TokenType.Acronym:
			case TokenType.Uri:
			case TokenType.OtherTextPlaceable:
			case TokenType.AlphaNumeric:
				text = tok.Text;
				break;
			case TokenType.Date:
			case TokenType.Number:
			case TokenType.Measurement:
				text = tok.Text;
				break;
			default:
				return string.Empty;
			}
			return text + "-" + tok.Type.ToString();
		}

		private static bool WrittenWithoutGroupSeparator(NumberToken nt, NumberFormatInfo nfi)
		{
			int num = (nfi?.NumberGroupSizes == null || nfi.NumberGroupSizes.Length < 1) ? 3 : nfi.NumberGroupSizes[0];
			if (nt.GroupSeparator != 0)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(nt.RawDecimalDigits))
			{
				return nt.RawDecimalDigits.Length > num;
			}
			return false;
		}

		private void CreateTranslationProposal(SearchResult searchResult, Segment docSourceSegment, bool panicMode)
		{
			if (searchResult == null)
			{
				throw new ArgumentNullException("searchResult");
			}
			if (searchResult.MemoryTranslationUnit == null)
			{
				throw new ArgumentNullException("MemoryTranslationUnit");
			}
			if (docSourceSegment == null)
			{
				throw new ArgumentNullException("docSourceSegment");
			}
			if (panicMode && searchResult.PlaceableAssociations != null)
			{
				RemoveAll(searchResult.PlaceableAssociations, delegate(PlaceableAssociation pa)
				{
					bool num = pa.Type == PlaceableType.PairedTagStart || pa.Type == PlaceableType.PairedTagEnd || pa.Type == PlaceableType.StandaloneTag;
					if (num)
					{
						int num2 = --searchResult.ScoringResult.ResolvedPlaceables;
					}
					return num;
				});
				if (searchResult.ScoringResult.ResolvedPlaceables < 0)
				{
					searchResult.ScoringResult.ResolvedPlaceables = 0;
				}
				ApplyPenaltyIfSet(searchResult, PenaltyType.TagMismatch);
			}
			searchResult.TranslationProposal = new TranslationUnit(searchResult.MemoryTranslationUnit);
			MapAnchors(searchResult.TranslationProposal);
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> hashSet2 = new HashSet<int>();
			bool flag = false;
			bool flag2 = false;
			if (searchResult.PlaceableAssociations != null)
			{
				Dictionary<string, List<PlaceableAssociation>> dictionary = new Dictionary<string, List<PlaceableAssociation>>();
				Placeable[] array = searchResult.PlaceableAssociations.Select((PlaceableAssociation x) => x.Document).ToArray();
				foreach (Placeable placeable in array)
				{
					SetDuplicateAssociations(searchResult, docSourceSegment, placeable.SourceTokenIndex, dictionary, isDocument: true);
				}
				Dictionary<string, List<PlaceableAssociation>> dictionary2 = new Dictionary<string, List<PlaceableAssociation>>();
				foreach (Placeable memoryPlaceable in searchResult.MemoryPlaceables)
				{
					SetDuplicateAssociations(searchResult, searchResult.MemoryTranslationUnit.SourceSegment, memoryPlaceable.SourceTokenIndex, dictionary2, isDocument: false);
				}
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				HashSet<Placeable> hashSet3 = new HashSet<Placeable>();
				foreach (PlaceableAssociation placeableAssociation in searchResult.PlaceableAssociations)
				{
					if (placeableAssociation.Memory != null && placeableAssociation.Document != null)
					{
						Token token = SafeGetToken(docSourceSegment, placeableAssociation.Document.SourceTokenIndex);
						Token token2 = SafeGetToken(searchResult.TranslationProposal.SourceSegment, placeableAssociation.Memory.SourceTokenIndex);
						Token token3 = SafeGetToken(searchResult.TranslationProposal.TargetSegment, placeableAssociation.Memory.TargetTokenIndex);
						ILocalizableToken originalMemoryToken = SafeGetToken(searchResult.MemoryTranslationUnit.SourceSegment, placeableAssociation.Memory.SourceTokenIndex) as ILocalizableToken;
						ILocalizableToken localizableToken = SafeGetToken(searchResult.MemoryTranslationUnit.TargetSegment, placeableAssociation.Memory.TargetTokenIndex) as ILocalizableToken;
						if (token2 != null || token3 != null)
						{
							if (placeableAssociation.Type == PlaceableType.Text || placeableAssociation.Type == PlaceableType.LockedContent)
							{
								if (token2 == null || token3 == null)
								{
									continue;
								}
								string text = token.Text;
								string value = token2.Text;
								if (placeableAssociation.Type == PlaceableType.LockedContent)
								{
									TagToken tagToken = token as TagToken;
									if (tagToken != null)
									{
										TagToken tagToken2 = token2 as TagToken;
										if (tagToken2 != null)
										{
											text = tagToken.Tag.SeriliazedWihoutAnchor();
											value = tagToken2.Tag.SeriliazedWihoutAnchor();
										}
									}
								}
								if (text.Equals(value, StringComparison.Ordinal))
								{
									hashSet3.Add(placeableAssociation.Memory);
									if (placeableAssociation.Type == PlaceableType.Text)
									{
										continue;
									}
								}
							}
							if (token2 != null)
							{
								string placeholderKey = GetPlaceholderKey(token2);
								if (dictionary2.ContainsKey(placeholderKey) && dictionary2[placeholderKey].Count > 1)
								{
									string placeholderKey2 = GetPlaceholderKey(token);
									if ((dictionary.ContainsKey(placeholderKey2) && dictionary[placeholderKey2].Count != dictionary2[placeholderKey].Count) || !ArePlaceableAssociationListsEqual(dictionary[placeholderKey2], dictionary2[placeholderKey]))
									{
										continue;
									}
								}
							}
							hashSet3.Add(placeableAssociation.Memory);
							if (token2 != null)
							{
								RecordTagIdMapping(token, token2, dictionary3);
								bool flag3 = (token2 as ILocalizableToken).DoesFormatMatch(token as ILocalizableToken);
								if (UpdateTokenValue(token2, token, originalMemoryToken, searchResult.MemoryTranslationUnit.SourceSegment.Culture, null, placeableAssociation.Memory.SourceTokenIndex == 0, SourceTokenizationContext))
								{
									hashSet.Add(placeableAssociation.Memory.SourceTokenIndex);
									if (!flag3)
									{
										searchResult.ScoringResult.PlaceableFormatChanges++;
									}
								}
							}
							if (token3 != null)
							{
								RecordTagIdMapping(token, token3, dictionary3);
								Token blueprint = token;
								NumberToken numberToken = token as NumberToken;
								if (numberToken != null)
								{
									NumberToken numberToken2 = token.Duplicate() as NumberToken;
									NumberToken numberToken3 = localizableToken as NumberToken;
									numberToken2.DecimalSeparator = numberToken3.DecimalSeparator;
									numberToken2.AlternateDecimalSeparator = numberToken3.AlternateDecimalSeparator;
									numberToken2.GroupSeparator = numberToken3.GroupSeparator;
									numberToken2.AlternateGroupSeparator = numberToken3.AlternateGroupSeparator;
									if (numberToken2.DecimalSeparator == NumericSeparator.None)
									{
										numberToken2.DecimalSeparator = NumericSeparator.Primary;
									}
									if (numberToken2.GroupSeparator == NumericSeparator.None && !WrittenWithoutGroupSeparator(numberToken3, LanguageMetadata.GetOrCreateMetadata(searchResult.MemoryTranslationUnit.TargetSegment.CultureName)?.NumberFormat))
									{
										numberToken2.GroupSeparator = NumericSeparator.Primary;
									}
									if (WrittenWithoutGroupSeparator(numberToken, LanguageMetadata.GetOrCreateMetadata(searchResult.MemoryTranslationUnit.SourceSegment.CultureName)?.NumberFormat))
									{
										numberToken2.GroupSeparator = NumericSeparator.None;
									}
									blueprint = numberToken2;
								}
								if (UpdateTokenValue(token3, blueprint, localizableToken, searchResult.MemoryTranslationUnit.TargetSegment.Culture, _settings.AutoLocalizationSettings, placeableAssociation.Memory.TargetTokenIndex == 0, TargetTokenizationContext))
								{
									hashSet2.Add(placeableAssociation.Memory.TargetTokenIndex);
									flag = RecordAutoLocalizationSubstitution(token3);
									bool flag4 = RecordTextSubstitutions(token3);
									flag2 |= flag4;
									if (flag4)
									{
										searchResult.ScoringResult.TextReplacements++;
									}
								}
							}
						}
					}
				}
				if (dictionary3.Count > 0)
				{
					foreach (Placeable memoryPlaceable2 in searchResult.MemoryPlaceables)
					{
						if (!hashSet3.Contains(memoryPlaceable2) && memoryPlaceable2.IsTag)
						{
							Token obj = (memoryPlaceable2.SourceTokenIndex >= 0) ? searchResult.TranslationProposal.SourceSegment.Tokens[memoryPlaceable2.SourceTokenIndex] : null;
							Token token4 = (memoryPlaceable2.TargetTokenIndex >= 0) ? searchResult.TranslationProposal.TargetSegment.Tokens[memoryPlaceable2.TargetTokenIndex] : null;
							TagToken tagToken3 = obj as TagToken;
							if (tagToken3 != null && tagToken3.Tag != null && !string.IsNullOrEmpty(tagToken3.Tag.TagID) && dictionary3.TryGetValue(tagToken3.Tag.TagID, out string value2))
							{
								tagToken3.Tag.TagID = value2;
								hashSet.Add(memoryPlaceable2.SourceTokenIndex);
							}
							TagToken tagToken4 = token4 as TagToken;
							if (tagToken4 != null && tagToken4.Tag != null && !string.IsNullOrEmpty(tagToken4.Tag.TagID) && dictionary3.TryGetValue(tagToken4.Tag.TagID, out string value3))
							{
								tagToken4.Tag.TagID = value3;
								hashSet2.Add(memoryPlaceable2.TargetTokenIndex);
							}
							hashSet3.Add(memoryPlaceable2);
						}
					}
				}
			}
			if (searchResult.ScoringResult.MemoryTagsDeleted)
			{
				foreach (Placeable memoryPlaceable3 in searchResult.MemoryPlaceables)
				{
					if ((memoryPlaceable3.Type == PlaceableType.PairedTagStart || memoryPlaceable3.Type == PlaceableType.PairedTagEnd || memoryPlaceable3.Type == PlaceableType.StandaloneTag) && memoryPlaceable3.SourceTokenIndex >= 0)
					{
						TagToken blueprint2 = new TagToken();
						Token token5 = (memoryPlaceable3.SourceTokenIndex >= 0) ? searchResult.TranslationProposal.SourceSegment.Tokens[memoryPlaceable3.SourceTokenIndex] : null;
						Token token6 = (memoryPlaceable3.TargetTokenIndex >= 0) ? searchResult.TranslationProposal.TargetSegment.Tokens[memoryPlaceable3.TargetTokenIndex] : null;
						if (token5 != null && UpdateTokenValue(token5, blueprint2, null, null, null, isFirstSegmentToken: false, SourceTokenizationContext))
						{
							hashSet.Add(memoryPlaceable3.SourceTokenIndex);
						}
						if (token6 != null && UpdateTokenValue(token6, blueprint2, null, null, null, isFirstSegmentToken: false, TargetTokenizationContext))
						{
							hashSet2.Add(memoryPlaceable3.TargetTokenIndex);
						}
					}
				}
			}
			if (flag)
			{
				ApplyPenaltyIfSet(searchResult, PenaltyType.AutoLocalization);
			}
			if (flag2)
			{
				ApplyPenaltyIfSet(searchResult, PenaltyType.TextReplacement);
			}
			if (hashSet.Count > 0)
			{
				searchResult.TranslationProposal.SourceSegment.UpdateFromTokenIndices(hashSet);
			}
			if (hashSet2.Count > 0)
			{
				searchResult.TranslationProposal.TargetSegment.UpdateFromTokenIndices(hashSet2);
			}
			RestoreAnchors(searchResult.TranslationProposal);
			if (!searchResult.TranslationProposal.IsValid() && !panicMode)
			{
				CreateTranslationProposal(searchResult, docSourceSegment, panicMode: true);
			}
		}

		private static void RemoveAll<T>(List<T> elements, Func<T, bool> d)
		{
			int num = 0;
			while (num < elements.Count)
			{
				if (d(elements[num]))
				{
					elements.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}

		private void ApplyPenaltyIfSet(SearchResult result, PenaltyType type)
		{
			Penalty penalty = _settings.FindPenalty(type);
			if (penalty != null)
			{
				result.ScoringResult.ApplyPenalty(penalty);
			}
		}

		private static bool RecordAutoLocalizationSubstitution(Token t)
		{
			return t is ILocalizableToken;
		}

		private static bool RecordTextSubstitutions(Token t)
		{
			if (t.Type != TokenType.Tag)
			{
				if (t.Type != TokenType.AlphaNumeric && t.Type != TokenType.Acronym)
				{
					return t.Type == TokenType.Variable;
				}
				return true;
			}
			TagToken tagToken = t as TagToken;
			if (tagToken != null && tagToken.Tag != null)
			{
				if (tagToken.Tag.Type != TagType.TextPlaceholder)
				{
					return tagToken.Tag.Type == TagType.LockedContent;
				}
				return true;
			}
			return false;
		}

		private static void RecordTagIdMapping(Token docToken, Token memToken, IDictionary<string, string> mapping)
		{
			if (docToken.Type != TokenType.Tag || memToken.Type != TokenType.Tag)
			{
				return;
			}
			TagToken tagToken = docToken as TagToken;
			if (tagToken == null)
			{
				return;
			}
			TagToken tagToken2 = memToken as TagToken;
			if (tagToken2 != null && tagToken.Tag != null && tagToken2.Tag != null)
			{
				string tagID = tagToken.Tag.TagID;
				string tagID2 = tagToken2.Tag.TagID;
				if (!string.IsNullOrEmpty(tagID) && !string.IsNullOrEmpty(tagID2) && !mapping.ContainsKey(tagID2))
				{
					mapping.Add(tagID2, tagID);
				}
			}
		}

		private void MapAnchors(TranslationUnit tu)
		{
			MapAnchorsToNegativeValues(tu.SourceSegment);
			MapAnchorsToNegativeValues(tu.TargetSegment);
		}

		private void MapAnchorsToNegativeValues(Segment s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			foreach (SegmentElement element in s.Elements)
			{
				Tag tag = element as Tag;
				if (tag != null)
				{
					if (tag.AlignmentAnchor > 0)
					{
						tag.AlignmentAnchor = -tag.AlignmentAnchor;
					}
					if (tag.Anchor > 0)
					{
						tag.Anchor = -tag.Anchor;
					}
				}
			}
		}

		private void GetMaxAnchors(Segment s, out int maxAnchor, out int maxAlignmentAnchor)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			maxAnchor = 0;
			maxAlignmentAnchor = 0;
			foreach (SegmentElement element in s.Elements)
			{
				Tag tag = element as Tag;
				if (tag != null)
				{
					if (tag.AlignmentAnchor > 0 && tag.AlignmentAnchor > maxAlignmentAnchor)
					{
						maxAlignmentAnchor = tag.AlignmentAnchor;
					}
					if (tag.Anchor > 0 && tag.Anchor > maxAnchor)
					{
						maxAnchor = tag.Anchor;
					}
				}
			}
		}

		private void RestoreAnchors(TranslationUnit tu)
		{
			GetMaxAnchors(tu.SourceSegment, out int maxAnchor, out int maxAlignmentAnchor);
			GetMaxAnchors(tu.TargetSegment, out int maxAnchor2, out int maxAlignmentAnchor2);
			int nextAlignmentAnchor = Math.Max(maxAlignmentAnchor, maxAlignmentAnchor2) + 1;
			int nextAnchor = maxAnchor + 1;
			int nextAnchor2 = maxAnchor2 + 1;
			Dictionary<int, int> alignmentAnchorMapping = new Dictionary<int, int>();
			RestoreAnchors(tu.SourceSegment, ref nextAnchor, ref nextAlignmentAnchor, alignmentAnchorMapping);
			RestoreAnchors(tu.TargetSegment, ref nextAnchor2, ref nextAlignmentAnchor, alignmentAnchorMapping);
		}

		private static void RestoreAnchors(Segment s, ref int nextAnchor, ref int nextAlignmentAnchor, IDictionary<int, int> alignmentAnchorMapping)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (SegmentElement element in s.Elements)
			{
				Tag tag = element as Tag;
				if (tag != null)
				{
					int value;
					if (tag.AlignmentAnchor < 0)
					{
						if (alignmentAnchorMapping.TryGetValue(tag.AlignmentAnchor, out value))
						{
							tag.AlignmentAnchor = value;
						}
						else
						{
							alignmentAnchorMapping.Add(tag.AlignmentAnchor, nextAlignmentAnchor);
							tag.AlignmentAnchor = nextAlignmentAnchor;
							nextAlignmentAnchor++;
						}
					}
					if (tag.Anchor < 0)
					{
						if (dictionary.TryGetValue(tag.Anchor, out value))
						{
							tag.Anchor = value;
						}
						else
						{
							dictionary.Add(tag.Anchor, nextAnchor);
							tag.Anchor = nextAnchor;
							nextAnchor++;
						}
					}
				}
			}
		}

		private static bool DoAttemptAutoSubstitution(Token t, AutoLocalizationSettings autoLocSettings)
		{
			return autoLocSettings?.AttemptAutoSubstitution(t) ?? t.IsSubstitutable;
		}

		private static bool KeepNumberSeparators(AutoLocalizationSettings autoLocSettings, ILocalizableToken originalMemoryToken, Token blueprint)
		{
			NumberToken numberToken = blueprint as NumberToken;
			if (numberToken != null && numberToken.GroupSeparator == NumericSeparator.None)
			{
				return false;
			}
			bool flag = autoLocSettings != null && autoLocSettings.LocalizationParametersSource == LocalizationParametersSource.FromMemory;
			if (flag || originalMemoryToken == null)
			{
				return flag;
			}
			NumberToken numberToken2 = originalMemoryToken as NumberToken;
			if (numberToken2 != null && (numberToken2.GroupSeparator == NumericSeparator.Alternate || numberToken2.DecimalSeparator == NumericSeparator.Alternate))
			{
				flag = true;
			}
			return flag;
		}

		private bool UpdateTokenValue(Token victim, Token blueprint, ILocalizableToken originalMemoryToken, CultureInfo localizeIntoCulture, AutoLocalizationSettings autoLocSettings, bool isFirstSegmentToken, TokenizationContext tokenizationContext)
		{
			bool flag = false;
			try
			{
				if (victim.Type != blueprint.Type || victim.GetType() != blueprint.GetType())
				{
					return false;
				}
				ILocalizableToken localizableToken = victim as ILocalizableToken;
				if (localizableToken == null)
				{
					switch (victim.Type)
					{
					case TokenType.Variable:
					case TokenType.Acronym:
					case TokenType.OtherTextPlaceable:
					case TokenType.AlphaNumeric:
						if (!DoAttemptAutoSubstitution(victim, autoLocSettings))
						{
							return flag;
						}
						flag = !string.Equals(victim.Text, blueprint.Text, StringComparison.Ordinal);
						if (!flag)
						{
							return flag;
						}
						victim.Text = blueprint.Text;
						return flag;
					case TokenType.Tag:
					{
						TagToken obj = victim as TagToken;
						TagToken blueprint2 = blueprint as TagToken;
						obj?.UpdateValue(blueprint2);
						flag = true;
						return flag;
					}
					default:
						return flag;
					case TokenType.Uri:
					case TokenType.UserDefined:
						return flag;
					}
				}
				localizableToken.TokenizationContext = tokenizationContext;
				bool keepNumericSeparators = KeepNumberSeparators(autoLocSettings, originalMemoryToken, blueprint);
				if (!DoAttemptAutoSubstitution(victim, autoLocSettings))
				{
					return flag;
				}
				if (!localizableToken.SetValue(blueprint, keepNumericSeparators))
				{
					return flag;
				}
				localizableToken.Localize(localizeIntoCulture, autoLocSettings, originalMemoryToken, isFirstSegmentToken);
				flag = true;
				return flag;
			}
			catch
			{
				return flag;
			}
		}

		public SearchResult CreateTranslationProposal(TranslationUnit translatedSegment, Segment docSourceSegment, LanguageTools sourceLanguageTools, LanguageTools targetLanguageTools)
		{
			AnnotatedDocumentSegment annotatedDocumentSegment = new AnnotatedDocumentSegment(docSourceSegment, sourceLanguageTools, keepTokens: true, keepPeripheralWhitespace: false);
			sourceLanguageTools.EnsureTokenizedSegment(annotatedDocumentSegment.Segment, forceRetokenization: false, allowTokenBundles: false);
			List<Placeable> documentPlaceables = PlaceableComputer.ComputePlaceables(annotatedDocumentSegment.Segment, null);
			SearchResult searchResult = new SearchResult(translatedSegment);
			sourceLanguageTools.EnsureTokenizedSegment(searchResult.MemoryTranslationUnit.SourceSegment);
			targetLanguageTools.EnsureTokenizedSegment(searchResult.MemoryTranslationUnit.TargetSegment);
			searchResult.MemoryPlaceables = PlaceableComputer.ComputePlaceables(translatedSegment);
			new DocumentScorer(_settings, sourceLanguageTools, targetLanguageTools, _textContextMatchType).ComputeScores(searchResult, annotatedDocumentSegment, null, documentPlaceables, new TuContextData(), isDuplicateSearch: false, FuzzyIndexes.SourceWordBased);
			CreateTranslationProposal(searchResult, annotatedDocumentSegment.Segment);
			return searchResult;
		}

		internal static void SetDuplicateAssociations(SearchResult searchResult, Segment segment, int placeableIndex, Dictionary<string, List<PlaceableAssociation>> uniquePlaceables, bool isDocument)
		{
			if (placeableIndex >= 0)
			{
				foreach (PlaceableAssociation placeableAssociation in searchResult.PlaceableAssociations)
				{
					Token tok = SafeGetToken(segment, placeableIndex);
					Token tok2 = SafeGetToken(segment, isDocument ? placeableAssociation.Document.SourceTokenIndex : placeableAssociation.Memory.SourceTokenIndex);
					string placeholderKey = GetPlaceholderKey(tok);
					string placeholderKey2 = GetPlaceholderKey(tok2);
					if (!string.IsNullOrEmpty(placeholderKey) && placeholderKey.Equals(placeholderKey2))
					{
						if (uniquePlaceables.ContainsKey(placeholderKey) && !uniquePlaceables[placeholderKey].Contains(placeableAssociation))
						{
							uniquePlaceables[placeholderKey].Add(placeableAssociation);
						}
						else
						{
							uniquePlaceables[placeholderKey] = new List<PlaceableAssociation>
							{
								placeableAssociation
							};
						}
					}
				}
			}
		}

		internal static bool ArePlaceableAssociationListsEqual(List<PlaceableAssociation> list1, List<PlaceableAssociation> list2)
		{
			List<PlaceableAssociation> source = list1.Except(list2).ToList();
			List<PlaceableAssociation> source2 = list2.Except(list1).ToList();
			if (!source.Any())
			{
				return !source2.Any();
			}
			return false;
		}
	}
}
