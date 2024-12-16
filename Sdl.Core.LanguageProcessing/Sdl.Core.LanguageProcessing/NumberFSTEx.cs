using Sdl.Core.LanguageProcessing.Tokenization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing
{
	public class NumberFSTEx
	{
		private const int CurrentVersion = 1;

		public List<SeparatorCombination> SeparatorCombinations
		{
			get;
			set;
		}

		public int Version
		{
			get;
			set;
		}

		public NumberFSTEx()
		{
			Version = 1;
		}

		public static NumberFSTEx FromBinary(byte[] data)
		{
			string @string = Encoding.UTF8.GetString(data);
			List<SeparatorCombination> list = new List<SeparatorCombination>();
			char[] separator = new char[1]
			{
				'\t'
			};
			char[] separator2 = new char[1]
			{
				'\r'
			};
			List<string> list2 = new List<string>(@string.Split(separator2, StringSplitOptions.RemoveEmptyEntries));
			if (list2.Count == 0)
			{
				return new NumberFSTEx
				{
					Version = 1
				};
			}
			if (!int.TryParse(list2[0], out int result))
			{
				throw new Exception("Unexpected data during NumberFSTEx deserialization");
			}
			if (result > 1)
			{
				throw new Exception("Unexpected NumberFSTEx version: " + result.ToString());
			}
			list2.RemoveAt(0);
			foreach (string item in list2)
			{
				string[] array = item.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 2)
				{
					list.Add(new SeparatorCombination(array[0], array[1], augmentGroupSeparators: false));
				}
			}
			return new NumberFSTEx
			{
				SeparatorCombinations = list,
				Version = result
			};
		}

		public byte[] ToBinary()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (SeparatorCombinations == null || SeparatorCombinations.Count <= 0)
			{
				return Encoding.UTF8.GetBytes(stringBuilder.ToString());
			}
			stringBuilder.Append(1.ToString());
			foreach (SeparatorCombination separatorCombination in SeparatorCombinations)
			{
				stringBuilder.Append("\r");
				stringBuilder.Append(separatorCombination.GroupSeparators + "\t" + separatorCombination.DecimalSeparators);
			}
			return Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}
	}
}
