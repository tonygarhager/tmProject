using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class NumberFormatData
	{
		public List<string> Digits;

		public List<SeparatorCombination> SeparatorCombinations;

		public string PositiveSigns;

		public string NegativeSigns;

		public int[] NumberGroupSizes;

		public int NumberNegativePattern;

		public NumberFormatData()
		{
			Digits = new List<string>();
			SeparatorCombinations = new List<SeparatorCombination>();
		}

		public bool AddSeparatorCombination(string groupSeparator, string decimalSeparator, bool augmentGroupSeparators)
		{
			SeparatorCombination item = new SeparatorCombination(groupSeparator, decimalSeparator, augmentGroupSeparators);
			if (SeparatorCombinations.Contains(item))
			{
				return false;
			}
			SeparatorCombinations.Add(item);
			return true;
		}

		public string GetCombinedDecimalSeparators()
		{
			HashSet<char> hashSet = new HashSet<char>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SeparatorCombination separatorCombination in SeparatorCombinations)
			{
				if (!string.IsNullOrEmpty(separatorCombination.DecimalSeparators))
				{
					string decimalSeparators = separatorCombination.DecimalSeparators;
					foreach (char c in decimalSeparators)
					{
						if (!hashSet.Contains(c))
						{
							stringBuilder.Append(c);
							hashSet.Add(c);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
