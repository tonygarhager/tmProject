using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.Anchoring
{
	internal class ExactMatchAnchorSearch : AbstractAnchoringStrategy
	{
		internal readonly CultureInfo LeftCulture;

		internal readonly CultureInfo RightCulture;

		public static bool SupportsAlignmentAlgorithm(AlignmentAlgorithmType alignmentAlgorithmType)
		{
			if ((uint)alignmentAlgorithmType <= 2u)
			{
				return true;
			}
			return false;
		}

		public ExactMatchAnchorSearch(CultureInfo leftCulture, CultureInfo rightCulture)
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
			int num = 2;
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < leftElements.Count; j++)
			{
				int num4 = (j / 2 - num >= 0) ? (j / 2 - num) : 0;
				if (j > 0 && num4 <= num3)
				{
					num4 = num3;
				}
				int num5 = (j + num4 + num <= rightElements.Count) ? (j + num4 + num) : rightElements.Count;
				int i;
				for (i = num4; i < num5; i++)
				{
					if (list.Any((AlignmentData x) => x.RightIds.Contains(rightElements[i].Id)))
					{
						continue;
					}
					AlignmentData anchor = GetAnchor(leftElements[j], rightElements[i]);
					if (anchor != null && IsValidAnchorPosition(j, i, num2, num3))
					{
						AlignmentData firstRevertedAnchor = GetFirstRevertedAnchor(leftElements, rightElements, num2, num3);
						if (firstRevertedAnchor.Equals(anchor))
						{
							num2 = j + 1;
							num3 = i + 1;
							list.Add(anchor);
							break;
						}
					}
				}
			}
			return list;
		}

		private AlignmentData GetFirstRevertedAnchor(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, int leftIndex, int rightIndex)
		{
			for (int i = rightIndex; i < rightElements.Count; i++)
			{
				for (int j = leftIndex; j < leftElements.Count; j++)
				{
					AlignmentData anchor = GetAnchor(leftElements[j], rightElements[i]);
					if (anchor != null)
					{
						return anchor;
					}
				}
			}
			return null;
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

		private AlignmentData GetAnchor(AlignmentElement leftElement, AlignmentElement rightElement)
		{
			string text = leftElement.TextContent.Trim();
			string text2 = rightElement.TextContent.Trim();
			if (text.Length > 0 && text2.Length > 0 && text.ToUpperInvariant().Equals(text2.ToUpperInvariant()))
			{
				return new AlignmentData(new List<DocumentSegmentId>
				{
					leftElement.Id
				}, new List<DocumentSegmentId>
				{
					rightElement.Id
				}, AlignmentCost.MinValue);
			}
			return null;
		}
	}
}
