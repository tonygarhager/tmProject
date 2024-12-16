using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.Anchoring
{
	internal class CognateAnchorSearch : AbstractAnchoringStrategy
	{
		private class Cognate
		{
			public Token Token
			{
				get;
				private set;
			}

			public DocumentSegmentId LeftId
			{
				get;
				private set;
			}

			public DocumentSegmentId RightId
			{
				get;
				private set;
			}

			public Cognate(Token token, DocumentSegmentId leftId, DocumentSegmentId rightId)
			{
				Token = token;
				LeftId = leftId;
				RightId = rightId;
			}
		}

		public const int MinimumSegmentsPerAnchor = 8;

		public const int MinimumCognateLength = 4;

		private readonly CultureInfo _leftCulture;

		private readonly CultureInfo _rightCulture;

		public static bool SupportsCulture(CultureInfo culture)
		{
			return CultureInfoExtensions.UseBlankAsWordSeparator(culture);
		}

		public static bool SupportsAlignmentAlgorithm(AlignmentAlgorithmType alignmentAlgorithmType)
		{
			if ((uint)alignmentAlgorithmType <= 3u || (uint)(alignmentAlgorithmType - 5) <= 1u)
			{
				return true;
			}
			return false;
		}

		public CognateAnchorSearch(CultureInfo leftCulture, CultureInfo rightCulture)
		{
			if (leftCulture == null)
			{
				throw new ArgumentNullException("leftCulture");
			}
			if (rightCulture == null)
			{
				throw new ArgumentNullException("rightCulture");
			}
			if (!CultureInfoExtensions.UseBlankAsWordSeparator(leftCulture))
			{
				throw new ArgumentException("CognateAnchorSearch: culture not supported");
			}
			if (!CultureInfoExtensions.UseBlankAsWordSeparator(rightCulture))
			{
				throw new ArgumentException("CognateAnchorSearch: culture not supported");
			}
			_leftCulture = leftCulture;
			_rightCulture = rightCulture;
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
			int num = Math.Min(leftElements.Count, rightElements.Count) / 8;
			if (num == 0)
			{
				return new List<AlignmentData>();
			}
			IList<AlignmentData> list = new List<AlignmentData>();
			InvertedIndex invertedIndex = new InvertedIndex(base.TokensContainer);
			invertedIndex.AddElements(leftElements);
			InvertedIndex invertedIndex2 = new InvertedIndex(base.TokensContainer);
			invertedIndex2.AddElements(rightElements);
			foreach (AlignmentElement rightElement in rightElements)
			{
				AlignmentData anchor = GetAnchor(invertedIndex, invertedIndex2, rightElement);
				if (anchor != null)
				{
					list.Add(anchor);
				}
			}
			return ((IEnumerable<AlignmentData>)list).OrderBy((Func<AlignmentData, double>)((AlignmentData alignmentData) => alignmentData.Cost)).Take(num).ToList();
		}

		private AlignmentData GetAnchor(InvertedIndex leftIndex, InvertedIndex rightIndex, AlignmentElement rightElement)
		{
			IEnumerable<Token> tokens = base.TokensContainer[rightElement.Content];
			IEnumerable<Token> uniqueTokens = GetUniqueTokens(tokens);
			IEnumerable<Cognate> cognates = GetCognates(leftIndex, rightIndex, uniqueTokens);
			if (cognates.Count() > 0)
			{
				IEnumerable<DocumentSegmentId> source = cognates.Select((Cognate cognate) => cognate.LeftId).Distinct();
				IEnumerable<DocumentSegmentId> source2 = cognates.Select((Cognate cognate) => cognate.RightId).Distinct();
				if (source.Count() == 1 && source2.Count() == 1)
				{
					AlignmentCost cost = (AlignmentCost)(1.0 / ((double)cognates.Count() + 1.0));
					return new AlignmentData(new List<DocumentSegmentId>
					{
						source.First()
					}, new List<DocumentSegmentId>
					{
						source2.First()
					}, cost);
				}
			}
			return null;
		}

		private IEnumerable<Token> GetUniqueTokens(IEnumerable<Token> tokens)
		{
			Dictionary<string, Token> dictionary = new Dictionary<string, Token>();
			foreach (Token token in tokens)
			{
				string text = token.Text;
				if (!dictionary.ContainsKey(text))
				{
					dictionary.Add(text, token);
				}
			}
			return dictionary.Values;
		}

		private IEnumerable<Cognate> GetCognates(InvertedIndex leftIndex, InvertedIndex rightIndex, IEnumerable<Token> tokens)
		{
			return (from token in tokens
				where !token.IsWhitespace && !token.IsPunctuation && token.Text.Length >= 4
				let leftElementsWithToken = leftIndex.GetElements(token).ToList()
				where leftElementsWithToken.Count() == 1
				let rightElementsWithToken = rightIndex.GetElements(token).ToList()
				where rightElementsWithToken.Count() == 1
				select new Cognate(token, leftElementsWithToken.First().Id, rightElementsWithToken.First().Id)).ToList();
		}
	}
}
