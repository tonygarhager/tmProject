using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public static class PlaceableComputer
	{
		public static List<Placeable> ComputePlaceables(TranslationUnit tu)
		{
			return ComputePlaceables(tu.SourceSegment, tu.TargetSegment);
		}

		public static List<Placeable> ComputePlaceables(Segment srcSegment, Segment trgSegment)
		{
			if (srcSegment == null)
			{
				throw new ArgumentNullException("srcSegment");
			}
			if (srcSegment.Tokens == null)
			{
				throw new ArgumentNullException("Tokens");
			}
			bool srcHasBundles = srcSegment.HasTokenBundles();
			bool trgHasBundles = trgSegment?.HasTokenBundles() ?? false;
			List<Placeable> list = ComputeTagAlignments(srcSegment, trgSegment);
			List<Placeable> list2 = ComputeNontagAlignments(srcSegment, trgSegment, srcHasBundles, trgHasBundles);
			if (list2 != null)
			{
				if (list == null)
				{
					list = list2;
				}
				else
				{
					list.AddRange(list2);
				}
			}
			return list;
		}

		public static void ConvertPlaceablesToAlignments(List<Placeable> placeables, LiftAlignedSpanPairSet alignmentData, List<Token> sourceTokens, List<Token> targetTokens)
		{
			if (placeables != null)
			{
				foreach (Placeable placeable in placeables)
				{
					if (placeable.Type != 0 && placeable.SourceTokenIndex != -1 && placeable.TargetTokenIndex != -1)
					{
						LiftSpan sourceSpan = new LiftSpan((short)placeable.SourceTokenIndex, 1);
						LiftSpan targetSpan = new LiftSpan((short)placeable.TargetTokenIndex, 1);
						LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair
						{
							SourceSpan = sourceSpan,
							TargetSpan = targetSpan,
							Provenance = 3,
							Confidence = 1f
						};
						liftAlignedSpanPair.Validate();
						if (!alignmentData.Contradicts(liftAlignedSpanPair, repetitionIsContradiction: true))
						{
							alignmentData.Add(liftAlignedSpanPair);
						}
					}
				}
				AddLegacyAcronymAlignments(alignmentData, sourceTokens, targetTokens);
			}
		}

		private static void AddLegacyAcronymAlignments(LiftAlignedSpanPairSet alignmentData, IReadOnlyList<Token> sourceTokens, IReadOnlyList<Token> targetTokens)
		{
			Dictionary<string, List<int>> dictionary = new Dictionary<string, List<int>>();
			Dictionary<string, List<int>> dictionary2 = new Dictionary<string, List<int>>();
			for (int i = 0; i < sourceTokens.Count; i++)
			{
				Token token = sourceTokens[i];
				if (token.Type == TokenType.Word && token.Text.Length > 3 && token.Text.All(char.IsUpper))
				{
					if (dictionary.ContainsKey(token.Text))
					{
						dictionary[token.Text].Add(i);
					}
					else
					{
						dictionary.Add(token.Text, new List<int>
						{
							i
						});
					}
				}
			}
			for (int j = 0; j < targetTokens.Count; j++)
			{
				Token token2 = targetTokens[j];
				if (token2.Type == TokenType.Word && token2.Text.Length > 3 && token2.Text.All(char.IsUpper))
				{
					if (dictionary2.ContainsKey(token2.Text))
					{
						dictionary2[token2.Text].Add(j);
					}
					else
					{
						dictionary2.Add(token2.Text, new List<int>
						{
							j
						});
					}
				}
			}
			foreach (KeyValuePair<string, List<int>> item in dictionary.Where((KeyValuePair<string, List<int>> x) => x.Value.Count > 1).ToList())
			{
				dictionary.Remove(item.Key);
			}
			foreach (KeyValuePair<string, List<int>> item2 in dictionary2.Where((KeyValuePair<string, List<int>> x) => x.Value.Count > 1).ToList())
			{
				dictionary2.Remove(item2.Key);
			}
			foreach (KeyValuePair<string, List<int>> item3 in dictionary)
			{
				if (dictionary2.ContainsKey(item3.Key))
				{
					LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair((short)item3.Value[0], 1, (short)dictionary2[item3.Key][0], 1);
					if (!alignmentData.Contradicts(liftAlignedSpanPair, repetitionIsContradiction: true))
					{
						liftAlignedSpanPair.Provenance = 3;
						liftAlignedSpanPair.Confidence = 1f;
						alignmentData.Add(liftAlignedSpanPair);
					}
				}
			}
		}

		private static List<Placeable> ComputeTagAlignments(Segment srcSegment, Segment trgSegment)
		{
			if (srcSegment == null)
			{
				throw new ArgumentNullException("srcSegment");
			}
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			Dictionary<int, bool> dictionary2 = new Dictionary<int, bool>();
			for (int i = 0; i < srcSegment.Tokens.Count; i++)
			{
				if (srcSegment.Tokens[i] is TagToken)
				{
					dictionary.Add(i, value: false);
				}
			}
			bool flag = trgSegment?.Tokens != null;
			if (flag)
			{
				for (int j = 0; j < trgSegment.Tokens.Count; j++)
				{
					if (trgSegment.Tokens[j] is TagToken)
					{
						dictionary2.Add(j, value: false);
					}
				}
			}
			if (dictionary.Count == 0 && dictionary2.Count == 0)
			{
				return null;
			}
			List<Placeable> list = new List<Placeable>();
			if (flag)
			{
				for (int k = 0; k < srcSegment.Tokens.Count; k++)
				{
					if (!dictionary.ContainsKey(k) || dictionary[k])
					{
						continue;
					}
					TagToken tagToken = srcSegment.Tokens[k] as TagToken;
					if (tagToken == null)
					{
						continue;
					}
					Tag tag = tagToken.Tag;
					if (tag.AlignmentAnchor <= 0 || (tag.Type != TagType.Standalone && tag.Type != TagType.Start && tag.Type != TagType.TextPlaceholder && tag.Type != TagType.LockedContent))
					{
						continue;
					}
					int num = -1;
					int num2 = -1;
					for (int l = 0; l < trgSegment.Tokens.Count; l++)
					{
						if (!dictionary2.ContainsKey(l) || dictionary2[l])
						{
							continue;
						}
						TagToken tagToken2 = trgSegment.Tokens[l] as TagToken;
						if (tagToken2 == null)
						{
							continue;
						}
						Tag tag2 = tagToken2.Tag;
						if (tag2.AlignmentAnchor == tag.AlignmentAnchor)
						{
							dictionary[k] = true;
							dictionary2[l] = true;
							if (tag2.Type == TagType.Standalone)
							{
								list.Add(new Placeable(PlaceableType.StandaloneTag, k, l));
								break;
							}
							if (tag2.Type == TagType.TextPlaceholder)
							{
								list.Add(new Placeable(PlaceableType.TextPlaceholder, k, l));
								break;
							}
							if (tag2.Type == TagType.LockedContent)
							{
								list.Add(new Placeable(PlaceableType.LockedContent, k, l));
								break;
							}
							list.Add(new Placeable(PlaceableType.PairedTagStart, k, l));
							num = tag2.Anchor;
						}
						else if (tag2.Type == TagType.End && num >= 0 && tag2.Anchor == num)
						{
							num2 = l;
							break;
						}
					}
					if (tag.Type != TagType.Start || num2 < 0)
					{
						continue;
					}
					for (int m = k + 1; m < srcSegment.Tokens.Count; m++)
					{
						TagToken tagToken3 = srcSegment.Tokens[m] as TagToken;
						if (tagToken3 != null && !dictionary[m] && tagToken3.Tag.Type == TagType.End && tagToken3.Tag.Anchor == tag.Anchor)
						{
							dictionary[m] = true;
							dictionary2[num2] = true;
							list.Add(new Placeable(PlaceableType.PairedTagEnd, m, num2));
							break;
						}
					}
				}
			}
			foreach (KeyValuePair<int, bool> item in dictionary)
			{
				if (!item.Value)
				{
					list.Add(new Placeable(GetPlaceableType(srcSegment.Tokens[item.Key]), item.Key, -1));
				}
			}
			if (trgSegment?.Tokens == null)
			{
				return list;
			}
			foreach (KeyValuePair<int, bool> item2 in dictionary2)
			{
				if (!item2.Value)
				{
					list.Add(new Placeable(GetPlaceableType(trgSegment.Tokens[item2.Key]), -1, item2.Key));
				}
			}
			return list;
		}

		private static List<Placeable> ComputeNontagAlignments(Segment srcSegment, Segment trgSegment, bool srcHasBundles, bool trgHasBundles)
		{
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			Dictionary<int, bool> dictionary2 = new Dictionary<int, bool>();
			for (int i = 0; i < srcSegment.Tokens.Count; i++)
			{
				if (srcSegment.Tokens[i].IsPlaceable && !(srcSegment.Tokens[i] is TagToken))
				{
					dictionary.Add(i, value: false);
				}
			}
			bool flag = trgSegment?.Tokens != null;
			if (flag)
			{
				for (int j = 0; j < trgSegment.Tokens.Count; j++)
				{
					if (trgSegment.Tokens[j].IsPlaceable && !(trgSegment.Tokens[j] is TagToken))
					{
						dictionary2.Add(j, value: false);
					}
				}
			}
			if (dictionary.Count == 0 && dictionary2.Count == 0)
			{
				if (srcHasBundles)
				{
					srcSegment.RemoveTokenBundles();
				}
				if (trgHasBundles)
				{
					trgSegment?.RemoveTokenBundles();
				}
				return null;
			}
			List<Placeable> list = new List<Placeable>();
			if (flag)
			{
				for (int k = 0; k < srcSegment.Tokens.Count; k++)
				{
					Token srcToken = srcSegment.Tokens[k];
					if (!dictionary.ContainsKey(k) || dictionary[k])
					{
						continue;
					}
					int num = -1;
					SegmentElement.Similarity similarity = SegmentElement.Similarity.None;
					for (int l = 0; l < trgSegment.Tokens.Count; l++)
					{
						Token other = trgSegment.Tokens[l];
						if (dictionary2.ContainsKey(l) && !dictionary2[l])
						{
							SegmentElement.Similarity similarity2 = srcToken.GetSimilarity(other);
							if (similarity2 > similarity)
							{
								similarity = similarity2;
								num = l;
							}
						}
					}
					if (similarity > SegmentElement.Similarity.IdenticalType)
					{
						dictionary[k] = true;
						dictionary2[num] = true;
						bool num2 = srcToken is TokenBundle;
						bool flag2 = trgSegment.Tokens[num] is TokenBundle;
						if (num2 | flag2)
						{
							Token trgToken = trgSegment.Tokens[num];
							GetBestBundleCombination(ref srcToken, ref trgToken);
							srcSegment.Tokens[k] = srcToken;
							trgSegment.Tokens[num] = trgToken;
						}
						list.Add(new Placeable(GetPlaceableType(srcToken), k, num));
					}
				}
			}
			if (srcHasBundles)
			{
				srcSegment.RemoveTokenBundles();
			}
			if (trgHasBundles)
			{
				trgSegment?.RemoveTokenBundles();
			}
			foreach (KeyValuePair<int, bool> item in dictionary)
			{
				if (!item.Value)
				{
					list.Add(new Placeable(GetPlaceableType(srcSegment.Tokens[item.Key]), item.Key, -1));
				}
			}
			if (trgSegment?.Tokens == null)
			{
				return list;
			}
			foreach (KeyValuePair<int, bool> item2 in dictionary2)
			{
				if (!item2.Value)
				{
					list.Add(new Placeable(GetPlaceableType(trgSegment.Tokens[item2.Key]), -1, item2.Key));
				}
			}
			return list;
		}

		private static void GetBestBundleCombination(ref Token srcToken, ref Token trgToken)
		{
			TokenBundle tokenBundle = srcToken as TokenBundle;
			TokenBundle tokenBundle2 = trgToken as TokenBundle;
			if (tokenBundle == null && tokenBundle2 == null)
			{
				return;
			}
			tokenBundle?.SortByDecreasingPriority();
			tokenBundle2?.SortByDecreasingPriority();
			int num = -1;
			int num2 = -1;
			SegmentElement.Similarity similarity = SegmentElement.Similarity.None;
			if (tokenBundle != null)
			{
				for (int i = 0; i < tokenBundle.Count; i++)
				{
					PrioritizedToken prioritizedToken = tokenBundle[i];
					if (tokenBundle2 != null)
					{
						for (int j = 0; j < tokenBundle2.Count; j++)
						{
							SegmentElement.Similarity similarity2 = tokenBundle2[j].Token.GetSimilarity(prioritizedToken.Token);
							if (similarity2 > similarity)
							{
								similarity = similarity2;
								num = i;
								num2 = j;
							}
						}
					}
					else
					{
						SegmentElement.Similarity similarity2 = trgToken.GetSimilarity(prioritizedToken.Token);
						if (similarity2 > similarity)
						{
							similarity = similarity2;
							num = i;
							num2 = -1;
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < tokenBundle2.Count; k++)
				{
					SegmentElement.Similarity similarity2 = tokenBundle2[k].Token.GetSimilarity(srcToken);
					if (similarity2 > similarity)
					{
						similarity = similarity2;
						num = -1;
						num2 = k;
					}
				}
			}
			if (num >= 0 && tokenBundle != null)
			{
				srcToken = tokenBundle[num].Token;
				srcToken.Span = tokenBundle.Span;
			}
			if (num2 >= 0 && tokenBundle2 != null)
			{
				trgToken = tokenBundle2[num2].Token;
				trgToken.Span = tokenBundle2.Span;
			}
		}

		private static PlaceableType GetPlaceableType(Token t)
		{
			if (!t.IsPlaceable)
			{
				return PlaceableType.None;
			}
			TagToken tagToken = t as TagToken;
			if (tagToken == null)
			{
				return PlaceableType.Text;
			}
			switch (tagToken.Tag.Type)
			{
			case TagType.End:
				return PlaceableType.PairedTagEnd;
			case TagType.Start:
				return PlaceableType.PairedTagStart;
			case TagType.Standalone:
				return PlaceableType.StandaloneTag;
			case TagType.TextPlaceholder:
				return PlaceableType.TextPlaceholder;
			case TagType.LockedContent:
				return PlaceableType.LockedContent;
			default:
				return PlaceableType.None;
			}
		}
	}
}
