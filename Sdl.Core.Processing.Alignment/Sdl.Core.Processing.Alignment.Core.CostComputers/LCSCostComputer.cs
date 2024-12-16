using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class LCSCostComputer : IAlignmentCostComputer
	{
		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			if (!sourceElements.Any() || !targetElements.Any())
			{
				return AlignmentCost.MaxValue;
			}
			string mergedString = getMergedString(sourceElements);
			string mergedString2 = getMergedString(targetElements);
			if (mergedString.Length == 0 || mergedString2.Length == 0)
			{
				return AlignmentCost.MaxValue;
			}
			int num = Math.Max(mergedString.Length, mergedString2.Length);
			int num2 = lcsMemoization(mergedString, mergedString2);
			double value = 1.0 - (double)num2 / (double)num;
			return (AlignmentCost)value;
		}

		private int lcsMemoization(string a, string b)
		{
			int[,] array = new int[a.Length + 1, b.Length + 1];
			for (int i = 0; i < a.Length; i++)
			{
				array[i, 0] = 0;
			}
			for (int j = 0; j < b.Length; j++)
			{
				array[0, j] = 0;
			}
			for (int k = 0; k < a.Length; k++)
			{
				for (int l = 0; l < b.Length; l++)
				{
					if (a[k] == b[l])
					{
						array[k + 1, l + 1] = array[k, l] + 1;
					}
					else
					{
						array[k + 1, l + 1] = Math.Max(array[k + 1, l], array[k, l + 1]);
					}
				}
			}
			return array[a.Length, b.Length];
		}

		private static string GetMergedTextItems(IEnumerable<string> textElements)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string textElement in textElements)
			{
				stringBuilder.Append(textElement);
			}
			return stringBuilder.ToString();
		}

		private string getMergedString(IEnumerable<AlignmentElement> elements)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AlignmentElement element in elements)
			{
				if (element.TextBetweenSegment != string.Empty)
				{
					stringBuilder.Append(element.TextBetweenSegment);
				}
				stringBuilder.Append(element.TextContent);
			}
			return stringBuilder.ToString().Trim();
		}
	}
}
