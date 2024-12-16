using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.Anchoring
{
	internal class TokenComparisonAnchorSearch : AbstractAnchoringStrategy
	{
		internal readonly CultureInfo LeftCulture;

		internal readonly CultureInfo RightCulture;

		internal const int MatchThresholdPercentage = 85;

		public static bool SupportsAlignmentAlgorithm(AlignmentAlgorithmType alignmentAlgorithmType)
		{
			if ((uint)alignmentAlgorithmType <= 2u)
			{
				return true;
			}
			return false;
		}

		public TokenComparisonAnchorSearch(CultureInfo leftCulture, CultureInfo rightCulture)
		{
			if (leftCulture == null)
			{
				throw new ArgumentNullException("leftCulture");
			}
			if (rightCulture == null)
			{
				throw new ArgumentNullException("rightCulture");
			}
			LeftCulture = leftCulture;
			RightCulture = rightCulture;
		}

		public override IList<AlignmentData> GetAnchors(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements)
		{
			if (leftElements == null)
			{
				throw new ArgumentNullException("leftElements");
			}
			if (rightElements == null)
			{
				throw new ArgumentNullException("rightElements");
			}
			IList<AlignmentData> list = new List<AlignmentData>();
			int lastLeftMatch = 0;
			int num = 0;
			int j;
			for (j = 0; j < leftElements.Count; j++)
			{
				if (list.Any((AlignmentData x) => x.LeftIds.Contains(leftElements[j].Id)))
				{
					continue;
				}
				int num2 = (j / 2 - 2 >= 0) ? (j / 2 - 2) : 0;
				if (j > 0 && num2 <= num)
				{
					num2 = num;
				}
				int num3 = (j + num2 + 2 <= rightElements.Count) ? (j + num2 + 2) : rightElements.Count;
				int i;
				for (i = num2; i < num3; i++)
				{
					if (!list.Any((AlignmentData x) => x.RightIds.Contains(rightElements[i].Id)))
					{
						AlignmentData anchor = GetAnchor(leftElements[j], (j + 1 < leftElements.Count) ? leftElements[j + 1] : null, rightElements[i], (i + 1 < rightElements.Count) ? rightElements[i + 1] : null);
						if (anchor != null && IsValidAnchorPosition(j, i, lastLeftMatch, num))
						{
							lastLeftMatch = j + 1;
							num = i + 1;
							list.Add(anchor);
							break;
						}
					}
				}
			}
			return list;
		}

		private bool IsValidAnchorPosition(int leftIndex, int rightIndex, int lastLeftMatch, int lastRightMatch)
		{
			if (leftIndex == lastLeftMatch && Math.Abs(rightIndex - leftIndex) > leftIndex / 2 + 1)
			{
				return false;
			}
			if (rightIndex == lastRightMatch && Math.Abs(rightIndex - leftIndex) > rightIndex / 2 + 1)
			{
				return false;
			}
			if (leftIndex != lastLeftMatch && rightIndex != lastRightMatch && ((rightIndex - lastRightMatch) * 2 < leftIndex - lastLeftMatch || (leftIndex - lastLeftMatch) * 2 < rightIndex - lastRightMatch))
			{
				return false;
			}
			return true;
		}

		private AlignmentData GetAnchor(AlignmentElement leftElement, AlignmentElement nextLeftElement, AlignmentElement rightElement, AlignmentElement nextRightElement)
		{
			List<Token> significantTokens = GetSignificantTokens(base.TokensContainer[leftElement.Content]);
			List<Token> significantTokens2 = GetSignificantTokens(base.TokensContainer[rightElement.Content]);
			int num = CompareTokens(significantTokens, significantTokens2);
			AlignmentData result = new AlignmentData(new List<DocumentSegmentId>
			{
				leftElement.Id
			}, new List<DocumentSegmentId>
			{
				rightElement.Id
			}, AlignmentCost.MinValue);
			if (nextLeftElement != null && nextRightElement != null)
			{
				int num2 = CompareTokens(GetSignificantTokens(base.TokensContainer[nextLeftElement.Content]), GetSignificantTokens(base.TokensContainer[nextRightElement.Content]));
				if (num2 > 85)
				{
					if (num <= 85)
					{
						return null;
					}
					return result;
				}
			}
			if (nextRightElement != null)
			{
				int num3 = CompareTokens(significantTokens, GetSignificantTokens(base.TokensContainer[nextRightElement.Content]));
				if (num3 > 85)
				{
					if (num <= 85)
					{
						return new AlignmentData(new List<DocumentSegmentId>
						{
							leftElement.Id
						}, new List<DocumentSegmentId>
						{
							nextRightElement.Id
						}, AlignmentCost.MinValue);
					}
					return result;
				}
			}
			if (nextLeftElement != null)
			{
				int num4 = CompareTokens(GetSignificantTokens(base.TokensContainer[nextLeftElement.Content]), significantTokens2);
				if (num4 > 85)
				{
					if (num <= 85)
					{
						return new AlignmentData(new List<DocumentSegmentId>
						{
							nextLeftElement.Id
						}, new List<DocumentSegmentId>
						{
							rightElement.Id
						}, AlignmentCost.MinValue);
					}
					return result;
				}
			}
			if (nextRightElement != null)
			{
				int num5 = CompareTokens(significantTokens, significantTokens2, null, GetSignificantTokens(base.TokensContainer[nextRightElement.Content]));
				if (num5 > num)
				{
					num = num5;
					result = new AlignmentData(new List<DocumentSegmentId>
					{
						leftElement.Id
					}, new List<DocumentSegmentId>
					{
						rightElement.Id,
						nextRightElement.Id
					}, AlignmentCost.MinValue);
				}
			}
			if (nextLeftElement != null)
			{
				int num6 = CompareTokens(significantTokens, significantTokens2, GetSignificantTokens(base.TokensContainer[nextLeftElement.Content]));
				if (num6 > num)
				{
					num = num6;
					result = new AlignmentData(new List<DocumentSegmentId>
					{
						leftElement.Id,
						nextLeftElement.Id
					}, new List<DocumentSegmentId>
					{
						rightElement.Id
					}, AlignmentCost.MinValue);
				}
			}
			if (nextLeftElement != null && nextRightElement != null)
			{
				int num7 = CompareTokens(significantTokens, significantTokens2, GetSignificantTokens(base.TokensContainer[nextLeftElement.Content]), GetSignificantTokens(base.TokensContainer[nextRightElement.Content]));
				if (num7 > num)
				{
					num = num7;
					result = new AlignmentData(new List<DocumentSegmentId>
					{
						leftElement.Id,
						nextLeftElement.Id
					}, new List<DocumentSegmentId>
					{
						rightElement.Id,
						nextRightElement.Id
					}, AlignmentCost.MinValue);
				}
			}
			if (num >= 85)
			{
				return result;
			}
			return null;
		}

		private List<Token> GetSignificantTokens(List<Token> tokens)
		{
			return tokens.Where((Token x) => x.Type == TokenType.Abbreviation || x.Type == TokenType.Acronym || x.Type == TokenType.Date || x.Type == TokenType.Measurement || x.Type == TokenType.Number || x.Type == TokenType.OtherTextPlaceable || x.Type == TokenType.Tag || x.Type == TokenType.Time || x.Type == TokenType.Uri).ToList();
		}

		private int CompareTokens(List<Token> leftTokens, List<Token> rightTokens, List<Token> nextLeftTokens = null, List<Token> nextRightTokens = null)
		{
			List<Token> list = new List<Token>();
			list.AddRange(leftTokens);
			List<Token> list2 = new List<Token>();
			list2.AddRange(rightTokens);
			int num = list.Count;
			int num2 = rightTokens.Count;
			if (nextLeftTokens != null)
			{
				num += nextLeftTokens.Count;
				list.AddRange(nextLeftTokens);
			}
			if (nextRightTokens != null)
			{
				num2 += nextRightTokens.Count;
				list2.AddRange(nextRightTokens);
			}
			if (num == 0 || num2 == 0)
			{
				return 0;
			}
			int num3 = 0;
			foreach (Token token in list)
			{
				Token token2 = list2.FirstOrDefault((Token x) => token.GetSimilarity(x) == SegmentElement.Similarity.IdenticalValueAndType);
				if (token2 != null)
				{
					num3++;
					list2.Remove(token2);
				}
			}
			return (int)((double)num3 / (double)Math.Max(num, num2) * 100.0);
		}
	}
}
