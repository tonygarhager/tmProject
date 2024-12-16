using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class DateTimePattern : FSTRecognizer
	{
		public DateTimePatternType PatternType
		{
			get;
		}

		public string FormatString
		{
			get;
		}

		public DateTimePattern(DateTimePatternType patternType, CultureInfo culture, string formatString, FST fst)
			: base(fst, culture)
		{
			PatternType = patternType;
			FormatString = formatString;
		}

		private DateTime ParseOutput(string fstOutput)
		{
			if (string.CompareOrdinal(fstOutput, "X") == 0)
			{
				return default(DateTime);
			}
			char c = '\0';
			int length = fstOutput.Length;
			int num = 0;
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				char c2 = fstOutput[i];
				if (flag)
				{
					if (c2 == '\'')
					{
						flag = false;
						stringBuilder.Append("'\\''");
						for (int j = 0; j < num; j++)
						{
							stringBuilder.Append(c);
						}
						stringBuilder.Append("'\\''");
					}
				}
				else if (char.IsLetter(c2))
				{
					num = ((c != c2) ? 1 : (num + 1));
					c = c2;
					stringBuilder.Append('\'');
					stringBuilder.Append(c);
					stringBuilder.Append('\'');
				}
				else if (c2 == '\'')
				{
					flag = true;
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (!DateTime.TryParseExact(fstOutput, stringBuilder.ToString(), base.Culture, DateTimeStyles.None, out DateTime result))
			{
				result = default(DateTime);
			}
			return result;
		}

		public Match Match(string s, int startOffset, out DateTime output)
		{
			output = default(DateTime);
			List<FSTMatch> list = ComputeMatches(s, startOffset, ignoreCase: true, 1, keepLongestMatchesOnly: false);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			output = ParseOutput(list[0].Output);
			return new Match(startOffset, list[0].Length);
		}
	}
}
