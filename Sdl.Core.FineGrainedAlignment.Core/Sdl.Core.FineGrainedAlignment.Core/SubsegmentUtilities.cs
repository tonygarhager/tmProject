using Sdl.Core.LanguageProcessing;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.Core.FineGrainedAlignment.Core
{
	public static class SubsegmentUtilities
	{
		public class RawIndexVsSignificantIndexConverter
		{
			public enum SpanConversionOption
			{
				exactOnly,
				trimSpan,
				growSpan
			}

			private readonly List<short> _significantIndices;

			private bool _isNoopConverter;

			public ReadOnlyCollection<short> SignificantIndices => _significantIndices.AsReadOnly();

			public RawIndexVsSignificantIndexConverter(List<short> significantIndices)
			{
				_significantIndices = significantIndices;
			}

			public static RawIndexVsSignificantIndexConverter CreateNoopConverter(short sequenceLength)
			{
				List<short> list = new List<short>();
				for (short num = 0; num < sequenceLength; num = (short)(num + 1))
				{
					list.Add(num);
				}
				RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter = new RawIndexVsSignificantIndexConverter(list);
				rawIndexVsSignificantIndexConverter._isNoopConverter = true;
				return rawIndexVsSignificantIndexConverter;
			}

			public bool IsSignificant(short index)
			{
				return _significantIndices.Contains(index);
			}

			public int RawToSignificantIndex(short index)
			{
				if (_isNoopConverter)
				{
					return index;
				}
				return _significantIndices.IndexOf(index);
			}

			public short SignificantToRawIndex(short index)
			{
				return _significantIndices[index];
			}

			public LiftSpan RawSpanToSignificantSpan(LiftSpan rawSpan, SpanConversionOption option = SpanConversionOption.exactOnly)
			{
				if (_isNoopConverter)
				{
					return new LiftSpan(rawSpan.StartIndex, rawSpan.Length);
				}
				if (option == SpanConversionOption.growSpan)
				{
					throw new NotImplementedException();
				}
				short num = (short)_significantIndices.IndexOf(rawSpan.StartIndex);
				if (num == -1)
				{
					if (option == SpanConversionOption.exactOnly)
					{
						return null;
					}
					num = (short)_significantIndices.FindIndex((short x) => x > rawSpan.StartIndex);
					if (num == -1)
					{
						return null;
					}
				}
				int num2 = _significantIndices.IndexOf(rawSpan.EndIndex);
				if (num2 == -1)
				{
					if (option == SpanConversionOption.exactOnly)
					{
						return null;
					}
					num2 = (short)_significantIndices.FindLastIndex((short x) => x < rawSpan.EndIndex);
					if (num2 == -1)
					{
						return null;
					}
				}
				if (num2 < num)
				{
					return null;
				}
				return new LiftSpan(num, (short)(num2 - num + 1));
			}

			public LiftSpan RawSpanToTrimmedSignificantSpan(LiftSpan rawSpan)
			{
				if (_isNoopConverter)
				{
					return new LiftSpan(rawSpan.StartIndex, rawSpan.Length);
				}
				int num = _significantIndices.FindIndex((short x) => x >= rawSpan.StartIndex);
				if (num == -1)
				{
					return null;
				}
				int otherStartIndex = _significantIndices[num];
				if (!rawSpan.Covers(otherStartIndex))
				{
					return null;
				}
				int num2 = _significantIndices.FindLastIndex((short x) => x <= rawSpan.EndIndex);
				return new LiftSpan((short)num, (short)(num2 - num + 1));
			}

			public LiftSpan SignificantSpanToRawSpan(LiftSpan significantSpan)
			{
				if (_isNoopConverter)
				{
					return new LiftSpan(significantSpan.StartIndex, significantSpan.Length);
				}
				if (significantSpan.StartIndex >= _significantIndices.Count)
				{
					throw new Exception("Significant span StartIndex of " + significantSpan.StartIndex.ToString() + " exceeds sig. item count of " + _significantIndices.Count.ToString());
				}
				if (significantSpan.EndIndex >= _significantIndices.Count)
				{
					throw new Exception("Significant span EndIndex of " + significantSpan.EndIndex.ToString() + " exceeds sig. item count of " + _significantIndices.Count.ToString());
				}
				short num = _significantIndices[significantSpan.StartIndex];
				short num2 = _significantIndices[significantSpan.EndIndex];
				return new LiftSpan(num, (short)(num2 - num + 1));
			}
		}

		public static int MergeAlignmentSets(LiftAlignedSpanPairSet existingSet, LiftAlignedSpanPairSet incomingSet)
		{
			return MergeAlignmentSets(existingSet, incomingSet.GetTree());
		}

		private static int MergeAlignmentSets(LiftAlignedSpanPairSet existingSet, SimpleTree<LiftAlignedSpanPair> incomingNode)
		{
			int num = 0;
			if (!existingSet.Contradicts(incomingNode.Key, repetitionIsContradiction: true))
			{
				existingSet.Add(incomingNode.Key);
				num++;
			}
			foreach (SimpleTree<LiftAlignedSpanPair> item in incomingNode.Value)
			{
				num += MergeAlignmentSets(existingSet, item);
			}
			return num;
		}

		public static LiftAlignedSpanPairSet FeatureBasedAlignedSpanPairSetToRawAlignedSpanPairSet(LiftAlignedSpanPairSet featureBasedSet, RawIndexVsSignificantIndexConverter srcIndexConverter, RawIndexVsSignificantIndexConverter trgIndexConverter)
		{
			SimpleTree<LiftAlignedSpanPair> tree = featureBasedSet.GetTree();
			LiftAlignedSpanPairSet result = new LiftAlignedSpanPairSet((short)srcIndexConverter.SignificantIndices.Count, (short)trgIndexConverter.SignificantIndices.Count);
			FeatureBasedAlignedSpanPairSetToRawAlignedSpanPairSet(tree, result, srcIndexConverter, trgIndexConverter);
			return result;
		}

		private static void FeatureBasedAlignedSpanPairSetToRawAlignedSpanPairSet(SimpleTree<LiftAlignedSpanPair> node, LiftAlignedSpanPairSet result, RawIndexVsSignificantIndexConverter srcIndexConverter, RawIndexVsSignificantIndexConverter trgIndexConverter)
		{
			LiftSpan sourceSpan = srcIndexConverter.SignificantSpanToRawSpan(node.Key.SourceSpan);
			LiftSpan targetSpan = trgIndexConverter.SignificantSpanToRawSpan(node.Key.TargetSpan);
			LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair(sourceSpan, targetSpan);
			liftAlignedSpanPair.Provenance = node.Key.Provenance;
			liftAlignedSpanPair.Confidence = node.Key.Confidence;
			result.Add(liftAlignedSpanPair);
			foreach (SimpleTree<LiftAlignedSpanPair> item in node.Value)
			{
				FeatureBasedAlignedSpanPairSetToRawAlignedSpanPairSet(item, result, srcIndexConverter, trgIndexConverter);
			}
		}

		public static LiftAlignedSpanPairSet ConvertAlignmentSetIndices(LiftAlignedSpanPairSet set, RawIndexVsSignificantIndexConverter srcFromConverter, RawIndexVsSignificantIndexConverter trgFromConverter, short srcToSize, short trgToSize)
		{
			LiftAlignedSpanPairSet result = new LiftAlignedSpanPairSet(srcToSize, trgToSize);
			if (set.IsEmpty)
			{
				return result;
			}
			SimpleTree<LiftAlignedSpanPair> tree = set.GetTree();
			ConvertAlignmentSetIndices(tree, result, srcFromConverter, trgFromConverter, null, null);
			return result;
		}

		public static LiftAlignedSpanPairSet ConvertAlignmentSetIndices(LiftAlignedSpanPairSet set, RawIndexVsSignificantIndexConverter srcFromConverter, RawIndexVsSignificantIndexConverter trgFromConverter, RawIndexVsSignificantIndexConverter srcToConverter, RawIndexVsSignificantIndexConverter trgToConverter)
		{
			LiftAlignedSpanPairSet result = new LiftAlignedSpanPairSet((short)srcToConverter.SignificantIndices.Count, (short)trgToConverter.SignificantIndices.Count);
			if (set.IsEmpty)
			{
				return result;
			}
			SimpleTree<LiftAlignedSpanPair> tree = set.GetTree();
			ConvertAlignmentSetIndices(tree, result, srcFromConverter, trgFromConverter, srcToConverter, trgToConverter);
			return result;
		}

		private static void ConvertAlignmentSetIndices(SimpleTree<LiftAlignedSpanPair> node, LiftAlignedSpanPairSet result, RawIndexVsSignificantIndexConverter srcFromConverter, RawIndexVsSignificantIndexConverter trgFromConverter, RawIndexVsSignificantIndexConverter srcToConverter, RawIndexVsSignificantIndexConverter trgToConverter)
		{
			LiftSpan liftSpan = (srcFromConverter == null) ? new LiftSpan(node.Key.SourceSpan) : srcFromConverter.SignificantSpanToRawSpan(node.Key.SourceSpan);
			LiftSpan liftSpan2 = (trgFromConverter == null) ? new LiftSpan(node.Key.TargetSpan) : trgFromConverter.SignificantSpanToRawSpan(node.Key.TargetSpan);
			LiftSpan liftSpan3 = (srcToConverter == null) ? liftSpan : srcToConverter.RawSpanToTrimmedSignificantSpan(liftSpan);
			if (liftSpan3 == null)
			{
				return;
			}
			LiftSpan liftSpan4 = (trgToConverter == null) ? liftSpan2 : trgToConverter.RawSpanToTrimmedSignificantSpan(liftSpan2);
			if (liftSpan4 != null)
			{
				LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair(liftSpan3, liftSpan4);
				liftAlignedSpanPair.Confidence = node.Key.Confidence;
				liftAlignedSpanPair.Provenance = node.Key.Provenance;
				if (!result.Contradicts(liftAlignedSpanPair, repetitionIsContradiction: true))
				{
					result.Add(liftAlignedSpanPair);
				}
				foreach (SimpleTree<LiftAlignedSpanPair> item in node.Value)
				{
					ConvertAlignmentSetIndices(item, result, srcFromConverter, trgFromConverter, srcToConverter, trgToConverter);
				}
			}
		}

		public static bool IsSignificantForRecall(Token t)
		{
			switch (t.Type)
			{
			case TokenType.GeneralPunctuation:
			case TokenType.OpeningPunctuation:
			case TokenType.ClosingPunctuation:
			case TokenType.Date:
			case TokenType.Time:
			case TokenType.Variable:
			case TokenType.Number:
			case TokenType.Measurement:
			case TokenType.Whitespace:
			case TokenType.Uri:
			case TokenType.OtherTextPlaceable:
			case TokenType.Tag:
				return false;
			default:
			{
				SimpleToken simpleToken = t as SimpleToken;
				if (simpleToken != null)
				{
					return !simpleToken.IsStopword;
				}
				return false;
			}
			}
		}

		public static List<Token> GetNonWhitespaceTokens(List<Token> tokens, List<short> tokenIndices)
		{
			List<Token> list = new List<Token>();
			tokenIndices.Clear();
			for (short num = 0; num < tokens.Count; num = (short)(num + 1))
			{
				if (tokens[num].Type != TokenType.Whitespace)
				{
					list.Add(tokens[num]);
					tokenIndices.Add(num);
				}
			}
			return list;
		}

		public static List<short> GetFeatureIndicesOnly(List<Token> tokens, CultureInfo culture)
		{
			Segment segment = new Segment(culture);
			List<Token> list = new List<Token>();
			List<short> list2 = new List<short>();
			for (short num = 0; num < tokens.Count; num = (short)(num + 1))
			{
				Token item = tokens[num];
				list.Clear();
				list.Add(item);
				segment.Tokens = list;
				string text = InternalComputeSimplifiedIdentityString(segment, featureIdentificationOnly: true);
				if (text.Length > 0 && text[text.Length - 1] == '|')
				{
					text = text.Substring(0, text.Length - 1);
				}
				if (text.Length > 0 && text[0] == '|')
				{
					text = ((text.Length == 1) ? string.Empty : text.Substring(1));
				}
				if (text.Length > 0)
				{
					list2.Add(num);
				}
			}
			return list2;
		}

		public static List<string> GetFeatures(List<Token> tokens, CultureInfo culture, List<short> tokenIndices)
		{
			Segment segment = new Segment(culture);
			List<Token> list = new List<Token>();
			List<string> list2 = new List<string>();
			for (short num = 0; num < tokens.Count; num = (short)(num + 1))
			{
				Token item = tokens[num];
				list.Clear();
				list.Add(item);
				segment.Tokens = list;
				string text = ComputeSimplifiedIdentityString(segment);
				if (text.Length > 0 && text[text.Length - 1] == '|')
				{
					text = text.Substring(0, text.Length - 1);
				}
				if (text.Length > 0 && text[0] == '|')
				{
					text = ((text.Length == 1) ? string.Empty : text.Substring(1));
				}
				if (text.Length > 0)
				{
					list2.Add(text);
					tokenIndices?.Add(num);
				}
			}
			return list2;
		}

		public static string ComputeSimplifiedIdentityString(Segment segment)
		{
			return InternalComputeSimplifiedIdentityString(segment, featureIdentificationOnly: false);
		}

		private static string InternalComputeSimplifiedIdentityString(Segment segment, bool featureIdentificationOnly)
		{
			bool flag = AdvancedTokenization.TokenizesToWords(segment.Culture);
			if (segment.Tokens == null)
			{
				throw new Exception("ComputeSimplifiedIdentityString: segment must be tokenized");
			}
			StringBuilder stringBuilder = (!flag) ? new StringBuilder() : new StringBuilder("|");
			for (int i = 0; i < segment.Tokens.Count; i++)
			{
				Token token = segment.Tokens[i];
				string text = null;
				switch (token.Type)
				{
				case TokenType.Word:
				case TokenType.Uri:
				{
					SimpleToken simpleToken2 = token as SimpleToken;
					if (simpleToken2 != null)
					{
						text = ((simpleToken2.Stem != null) ? simpleToken2.Stem : simpleToken2.Text.ToLowerInvariant());
					}
					break;
				}
				case TokenType.OtherTextPlaceable:
				{
					SimpleToken simpleToken = token as SimpleToken;
					if (simpleToken != null)
					{
						text = ((!simpleToken.IsSubstitutable) ? ((simpleToken.Stem != null) ? simpleToken.Stem : simpleToken.Text.ToLowerInvariant()) : new string((char)(61696 + token.Type), 1));
					}
					break;
				}
				case TokenType.CharSequence:
					text = token.Text.ToLowerInvariant();
					break;
				case TokenType.Abbreviation:
					text = token.Text.ToLowerInvariant();
					break;
				case TokenType.Date:
				case TokenType.Time:
				case TokenType.Variable:
				case TokenType.Number:
				case TokenType.Measurement:
				case TokenType.Acronym:
				case TokenType.AlphaNumeric:
					text = new string((char)(61696 + token.Type), 1);
					break;
				case TokenType.UserDefined:
					text = new string((char)(61696 + token.Type), 1);
					break;
				case TokenType.GeneralPunctuation:
				case TokenType.OpeningPunctuation:
				case TokenType.ClosingPunctuation:
				case TokenType.Whitespace:
				case TokenType.Tag:
					continue;
				}
				if (text != null)
				{
					stringBuilder.Append(text);
					if (flag)
					{
						stringBuilder.Append("|");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static int MaxTM_TDBFragmentLengthToSearchFor(int queryFeatureCount, int minScore)
		{
			return (int)((double)queryFeatureCount * ((double)minScore / 100.0));
		}

		public static short MaxFragmentLengthToIndex(short sourceSegmentFeatureCount)
		{
			return sourceSegmentFeatureCount = (short)(sourceSegmentFeatureCount - 1);
		}

		public static short MinDTAFragmentLengthToIndex(CultureInfo culture)
		{
			return 1;
		}

		public static short MinDTAFragmentSignificantFeatures(CultureInfo culture)
		{
			return 1;
		}

		public static List<LiftSpan> GetTranslatableSpans(LiftAlignedSpanPairSet spanPairSet, List<string> features, List<short> featureTokenIndices, List<Token> sourceTokens, short minLength, int minSigWords, short maxLength, List<int> featureLengths, List<int> sigWordCounts, List<string> featureIdStrings)
		{
			List<LiftSpan> list = new List<LiftSpan>();
			for (short num = 0; num < featureTokenIndices.Count - minLength; num = (short)(num + 1))
			{
				short num2 = minLength;
				while (num + num2 <= featureTokenIndices.Count && num2 <= maxLength)
				{
					int num3 = featureTokenIndices[num];
					int num4 = featureTokenIndices[num + num2 - 1];
					int num5 = num4 - num3 + 1;
					LiftSpan liftSpan = new LiftSpan(num, num2);
					float confidence;
					LiftSpan translationSpan = GetTranslationSpan(spanPairSet, liftSpan, out confidence);
					if (translationSpan != null)
					{
						int num6 = 0;
						for (int i = 0; i < num2; i++)
						{
							if (IsSignificantForRecall(sourceTokens[featureTokenIndices[num + i]]))
							{
								num6++;
							}
						}
						if (num6 >= minSigWords)
						{
							featureLengths?.Add(num2);
							sigWordCounts?.Add(num6);
							list.Add(liftSpan);
							if (features != null)
							{
								string item = FeaturesToIdString(features, num, num2);
								featureIdStrings.Add(item);
							}
						}
					}
					num2 = (short)(num2 + 1);
				}
			}
			return list;
		}

		public static LiftSpan GetTranslationSpan(LiftAlignedSpanPairSet spanPairSet, LiftSpan sourceSpan, out float confidence)
		{
			confidence = 0f;
			List<LiftAlignedSpanPair> alignedSpanPairsCoveredBySpan = spanPairSet.GetAlignedSpanPairsCoveredBySpan(sourceSpan, searchTargetText: false, 0);
			if (alignedSpanPairsCoveredBySpan.Count == 0)
			{
				return null;
			}
			LiftSpanMerger sourceMerger = new LiftSpanMerger();
			LiftSpanMerger targetMerger = new LiftSpanMerger();
			alignedSpanPairsCoveredBySpan.ForEach(delegate(LiftAlignedSpanPair p)
			{
				double num4 = p.Confidence;
				if (num4 < 1E-05)
				{
					num4 = 1.0;
				}
				sourceMerger.AddSpan(p.SourceSpan, num4);
				targetMerger.AddSpan(p.TargetSpan, num4);
			});
			if (sourceMerger.GetInverseSpans(sourceSpan.StartIndex, sourceSpan.Length).Count > 0)
			{
				return null;
			}
			confidence = (float)sourceMerger.GetConfidence(sourceSpan.StartIndex, sourceSpan.Length);
			short num = alignedSpanPairsCoveredBySpan.Min((LiftAlignedSpanPair x) => x.TargetStartIndex);
			short num2 = alignedSpanPairsCoveredBySpan.Max((LiftAlignedSpanPair x) => x.TargetEndIndex);
			short num3 = (short)(num2 - num + 1);
			List<LiftSpan> inverseSpans = targetMerger.GetInverseSpans(num, num3);
			if (inverseSpans.Count > 0)
			{
				if (inverseSpans.Sum((LiftSpan x) => x.Length) > MaxTotalTargetGapLength(num3))
				{
					return null;
				}
				if (inverseSpans.Max((LiftSpan x) => x.Length) > MaxSingleTargetGapLength(num3))
				{
					return null;
				}
			}
			return new LiftSpan(num, num3);
		}

		private static int MaxTotalTargetGapLength(int totalTargetSpanLength)
		{
			if (totalTargetSpanLength < 5)
			{
				return 0;
			}
			if (totalTargetSpanLength < 10)
			{
				return 1;
			}
			return 2;
		}

		private static int MaxSingleTargetGapLength(int totalTargetSpanLength)
		{
			if (totalTargetSpanLength < 6)
			{
				return 0;
			}
			if (totalTargetSpanLength < 12)
			{
				return 1;
			}
			return 2;
		}

		public static string FeaturesToIdString(List<string> features, int startPos, int length)
		{
			string text = string.Join("|", features.ToArray(), startPos, length);
			if (!string.IsNullOrEmpty(text))
			{
				text = "|" + text + "|";
			}
			return text;
		}

		public static string GetTokenRendering(List<Token> tokens, LiftSpan span, bool addWhitespaceSeparation = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < span.Length; i++)
			{
				if (i > 0 && addWhitespaceSeparation)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(tokens[span.StartIndex + i].Text);
			}
			return stringBuilder.ToString();
		}
	}
}
