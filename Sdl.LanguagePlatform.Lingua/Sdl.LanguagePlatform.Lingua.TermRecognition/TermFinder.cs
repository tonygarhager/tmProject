using Sdl.Core.LanguageProcessing;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua.Alignment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	public static class TermFinder
	{
		private const string TokenSeparator = "~";

		private const string UnmatchedToken = "#";

		private const float DiceThreshold = 0.7f;

		public static TermFinderResult FindTerms(string search, string text, CultureInfo culture, bool expectContinuousMatch)
		{
			if (string.IsNullOrEmpty(search))
			{
				throw new ArgumentNullException("search");
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			Segment segment = new Segment(culture);
			Segment segment2 = new Segment(culture);
			segment.Add(search);
			segment2.Add(text);
			Tokenizer tokenizer = new Tokenizer(TokenizerSetupFactory.Create(culture));
			segment.Tokens = tokenizer.Tokenize(segment);
			segment2.Tokens = tokenizer.Tokenize(segment2);
			return FindTerms(segment, segment2, expectContinuousMatch);
		}

		public static TermFinderResult FindTerms(Segment searchSegment, Segment textSegment, bool expectContinuousMatch)
		{
			return FindTerms(searchSegment, textSegment, expectContinuousMatch, useWidthNormalization: false);
		}

		public static TermFinderResult FindTerms(Segment searchSegment, Segment textSegment, bool expectContinuousMatch, bool useWidthNormalization)
		{
			if (searchSegment == null)
			{
				throw new ArgumentNullException("searchSegment");
			}
			if (textSegment == null)
			{
				throw new ArgumentNullException("textSegment");
			}
			if (searchSegment.Tokens == null || textSegment.Tokens == null)
			{
				throw new ArgumentException("Segments are expected to be tokenized");
			}
			if (searchSegment.Culture == null || textSegment.Culture == null)
			{
				throw new ArgumentException("At least one segment culture is null");
			}
			if (!CultureInfoExtensions.AreCompatible(searchSegment.Culture, textSegment.Culture))
			{
				throw new ArgumentException("Segment cultures are incompatible");
			}
			if (searchSegment.Tokens.Count == 0 || textSegment.Tokens.Count == 0)
			{
				return null;
			}
			if (!AdvancedTokenization.TokenizesToWords(searchSegment.Culture))
			{
				return FindTermsCharBased(searchSegment, textSegment);
			}
			bool useCharacterDecomposition = string.CompareOrdinal(searchSegment.Culture.TwoLetterISOLanguageName, "ko") != 0;
			return FindTermsWordBased(searchSegment, textSegment, expectContinuousMatch, useCharacterDecomposition, useWidthNormalization);
		}

		private static TermFinderResult FindTermsCharBased(Segment searchSegment, Segment textSegment)
		{
			List<SegmentPosition> ranges;
			string text = searchSegment.ToPlain(tolower: true, tobase: true, out ranges);
			List<SegmentPosition> ranges2;
			string text2 = textSegment.ToPlain(tolower: true, tobase: true, out ranges2);
			if (text.Length == 0)
			{
				return null;
			}
			char[] source = text.ToCharArray();
			char[] target = text2.ToCharArray();
			int length = text.Length;
			int length2 = text2.Length;
			SubstringAlignmentDisambiguator picker = new SubstringAlignmentDisambiguator();
			List<AlignedSubstring> list = SequenceAlignmentComputer<char>.ComputeCoverage(source, target, new CharSubstringScoreProvider(), picker);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			TermFinderResult termFinderResult = new TermFinderResult
			{
				MatchingRanges = new List<SegmentRange>()
			};
			List<SegmentPosition> list2 = new List<SegmentPosition>();
			foreach (AlignedSubstring item in list)
			{
				if (item.Source.Length != item.Target.Length)
				{
					return null;
				}
				for (int i = 0; i < item.Source.Length; i++)
				{
					list2.Add(ranges2[item.Target.Start + i]);
				}
			}
			if (list2.Count == 0)
			{
				return null;
			}
			termFinderResult.MatchingRanges = SortAndMelt(list2);
			float num = (float)list2.Count / (float)length;
			termFinderResult.Score = (int)(100f * num);
			if (termFinderResult.Score < 0)
			{
				termFinderResult.Score = 0;
			}
			else if (termFinderResult.Score > 100)
			{
				termFinderResult.Score = 100;
			}
			return termFinderResult;
		}

		private static TermFinderResult FindTermsWordBased(Segment searchSegment, Segment textSegment, bool expectContinuousMatch, bool useCharacterDecomposition, bool useWidthNormalization)
		{
			bool normalizeWidths = useWidthNormalization && CultureInfoExtensions.UseFullWidth(textSegment.Culture);
			bool flag = false;
			int[,] array = ComputeTokenAssociationScores(searchSegment, textSegment, useCharacterDecomposition, normalizeWidths);
			int[] array2 = new int[searchSegment.Tokens.Count];
			TermFinderResult termFinderResult = new TermFinderResult
			{
				MatchingRanges = new List<SegmentRange>()
			};
			BitArray bitArray = new BitArray(textSegment.Tokens.Count);
			IList<string> list = new List<string>();
			IList<string> list2 = new List<string>();
			int num = 0;
			for (int i = 0; i < searchSegment.Tokens.Count; i++)
			{
				if (!searchSegment.Tokens[i].IsWhitespace)
				{
					num++;
					list.Add(searchSegment.Tokens[i].Text.ToLowerInvariant());
				}
				for (int j = 0; j < textSegment.Tokens.Count; j++)
				{
					if (array[i, j] > 0)
					{
						if (!bitArray[j])
						{
							termFinderResult.MatchingRanges.Add(textSegment.Tokens[j].Span);
							bitArray[j] = true;
						}
						if (array[i, j] > array2[i])
						{
							array2[i] = array[i, j];
						}
					}
				}
			}
			if (num == 0)
			{
				return null;
			}
			int num2 = (int)((float)array2.Sum() / (float)num);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int k = 0; k < textSegment.Tokens.Count; k++)
			{
				if (!bitArray[k])
				{
					continue;
				}
				if (list2.Count > 0)
				{
					int previousWordTokenIndex = GetPreviousWordTokenIndex(textSegment.Tokens, k);
					if (previousWordTokenIndex > -1 && !bitArray[previousWordTokenIndex])
					{
						list2.Add("#");
					}
				}
				string text = textSegment.Tokens[k].Text.ToLowerInvariant();
				if (!dictionary.ContainsKey(text))
				{
					dictionary.Add(text, 0);
				}
				list2.Add(text);
			}
			string text2 = string.Join("~", list.ToArray());
			string text3 = string.Join("~", list2.ToArray());
			int num3 = 0;
			if ((expectContinuousMatch || num2 < 100) && text2.Length > 0 && text3.Length > 0)
			{
				CaseAwareCharSubsequenceScoreProvider scorer = new CaseAwareCharSubsequenceScoreProvider(useCharacterDecomposition, normalizeWidths);
				CaseAwareCharSubsequenceScoreProvider scorer2 = new CaseAwareCharSubsequenceScoreProvider(useCharacterDecomposition, normalizeWidths: false);
				char[] source = text2.ToCharArray();
				char[] target = text3.ToCharArray();
				List<AlignedSubstring> list3 = SequenceAlignmentComputer<char>.ComputeLongestCommonSubsequence(source, target, 1, scorer, null);
				int num4 = list3.Sum((AlignedSubstring x) => x.Length);
				double num5 = 1.0;
				if (flag && num4 > 0)
				{
					List<AlignedSubstring> source2 = SequenceAlignmentComputer<char>.ComputeLongestCommonSubsequence(source, target, 1, scorer2, null);
					int num6 = list3.Sum((AlignedSubstring x) => x.Score);
					int num7 = source2.Sum((AlignedSubstring x) => x.Score);
					double num8 = (double)num6 + (double)(num6 - num7) * 0.6;
					num5 = num8 / (double)(num4 * 2);
					if (num5 > 1.0)
					{
						flag = false;
					}
				}
				int num9 = RemoveTokenDuplicates(text3, list3);
				float num10 = 2f * (float)Math.Min(num, dictionary.Count) / (float)(num + dictionary.Count);
				num3 = (int)((75f + 25f * num10) * 2f * (float)num4 / (float)(text2.Length + num9));
				if (flag)
				{
					num3 = (int)(num5 * (double)num3);
				}
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num3 > 100)
				{
					num3 = 100;
				}
			}
			if (num2 == 100 && num3 > 0)
			{
				termFinderResult.Score = (200 + num3) / 3;
			}
			else
			{
				termFinderResult.Score = Math.Max(num2, num3);
			}
			return termFinderResult;
		}

		private static int RemoveTokenDuplicates(string trgConcat, List<AlignedSubstring> lcs)
		{
			if (lcs == null)
			{
				throw new ArgumentNullException("lcs");
			}
			if (lcs.Count == 0)
			{
				throw new ArgumentException("lcs");
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			string str = trgConcat.Substring(0, lcs[0].Target.Start);
			string text = trgConcat.Substring(lcs[0].Target.Start, lcs[0].Target.Length);
			string str2 = trgConcat.Substring(lcs[0].Target.Start + lcs[0].Target.Length);
			string[] array = (str + str2).Split("~".ToCharArray());
			foreach (string text2 in array)
			{
				if (!text.Contains(text2) && !dictionary.ContainsKey(text2) && text2 != "#")
				{
					dictionary.Add(text2, 0);
				}
			}
			if (dictionary.Count > 0)
			{
				return text.Length + dictionary.Sum((KeyValuePair<string, int> x) => x.Key.Length) + dictionary.Count;
			}
			return text.Length;
		}

		private static int GetPreviousWordTokenIndex(IList<Token> tokens, int currentTokenIndex)
		{
			for (int num = currentTokenIndex - 1; num >= 0; num--)
			{
				if (tokens[num].IsWord)
				{
					return num;
				}
			}
			return -1;
		}

		private static int[,] ComputeTokenAssociationScores(Segment searchSegment, Segment textSegment, bool useCharacterDecomposition, bool normalizeWidths)
		{
			int[,] array = new int[searchSegment.Tokens.Count, textSegment.Tokens.Count];
			int[] array2 = new int[textSegment.Tokens.Count];
			int num = 0;
			bool flag = false;
			CaseAwareCharSubsequenceScoreProvider scorer = new CaseAwareCharSubsequenceScoreProvider(useCharacterDecomposition, normalizeWidths);
			for (int i = 0; i < searchSegment.Tokens.Count; i++)
			{
				Token token = searchSegment.Tokens[i];
				if (token.IsWhitespace || token is TagToken)
				{
					continue;
				}
				for (int j = 0; j < textSegment.Tokens.Count; j++)
				{
					Token token2 = textSegment.Tokens[j];
					if (token2.IsWhitespace || token2 is TagToken)
					{
						continue;
					}
					array[i, j] = 0;
					List<AlignedSubstring> list = SequenceAlignmentComputer<char>.ComputeLongestCommonSubsequence(token.Text.ToCharArray(), token2.Text.ToCharArray(), 0, scorer, null);
					if (list == null || list.Count == 0)
					{
						continue;
					}
					int num2 = list.Sum((AlignedSubstring x) => x.Length);
					if (num2 == 0)
					{
						continue;
					}
					float num3 = 2f * (float)num2 / (float)(token.Text.Length + token2.Text.Length);
					if (num3 >= 0.7f)
					{
						array[i, j] = (int)(num3 * 100f);
						if (array[i, j] < 100)
						{
							array2[num++] = j;
						}
						else
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					for (int k = 0; k < num; k++)
					{
						array[i, array2[k]] = 0;
					}
					num = 0;
					flag = false;
				}
			}
			return array;
		}

		private static bool VerifyRanges(IEnumerable<SegmentRange> ranges, Segment segment)
		{
			if (ranges == null)
			{
				return true;
			}
			foreach (SegmentRange range in ranges)
			{
				if (range?.From == null || range.Into == null)
				{
					return false;
				}
				if (range.From.Index != range.Into.Index)
				{
					return false;
				}
				if (range.From.Index < 0 || range.From.Index >= segment.Elements.Count)
				{
					return false;
				}
				if (range.From.Position > range.Into.Position)
				{
					return false;
				}
				Text text = segment.Elements[range.From.Index] as Text;
				if (text == null)
				{
					return false;
				}
				if (range.From.Position >= text.Value.Length || range.Into.Position >= text.Value.Length)
				{
					return false;
				}
			}
			return true;
		}

		private static List<SegmentRange> SortAndMelt(List<SegmentPosition> positions)
		{
			List<SegmentRange> list = new List<SegmentRange>();
			positions.Sort(delegate(SegmentPosition a, SegmentPosition b)
			{
				int num = a.Index - b.Index;
				if (num == 0)
				{
					num = a.Position - b.Position;
				}
				return num;
			});
			SegmentRange segmentRange = new SegmentRange(positions[0], positions[0].Duplicate());
			list.Add(segmentRange);
			for (int i = 1; i < positions.Count; i++)
			{
				if (positions[i].Index == segmentRange.From.Index && positions[i].Position == segmentRange.Into.Position + 1)
				{
					segmentRange.Into.Position = positions[i].Position;
					continue;
				}
				segmentRange = new SegmentRange(positions[i], positions[i].Duplicate());
				list.Add(segmentRange);
			}
			return list;
		}
	}
}
