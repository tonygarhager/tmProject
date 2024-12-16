using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.Core.LanguageProcessing
{
	public class CurrencyFSTEx
	{
		private const int CurrentVersion = 1;

		public int Version
		{
			get;
			set;
		}

		public List<CurrencyFormat> CurrencyFormats
		{
			get;
			set;
		}

		public CurrencyFSTEx()
		{
			Version = 1;
			CurrencyFormats = new List<CurrencyFormat>();
		}

		public static CurrencyFSTEx GetDefaults(CultureInfo culture, LanguageMetadata lm, IResourceDataAccessor accessor)
		{
			if (accessor == null)
			{
				accessor = new ResourceFileResourceAccessor();
			}
			Wordlist wordlist = new Wordlist();
			using (Stream stream = accessor.ReadResourceData(culture, LanguageResourceType.CurrencySymbols, fallback: true))
			{
				if (stream != null)
				{
					wordlist.Load(stream);
				}
			}
			if (wordlist.Count == 0)
			{
				return null;
			}
			CurrencyFSTEx currencyFSTEx = new CurrencyFSTEx();
			foreach (string item in wordlist.Items)
			{
				CurrencyFormat currencyFormat = new CurrencyFormat
				{
					Symbol = item,
					CurrencySymbolPositions = new List<CurrencySymbolPosition>
					{
						CurrencySymbolPosition.beforeAmount,
						CurrencySymbolPosition.afterAmount
					}
				};
				if (!lm.CurrencyPrecedesNumber)
				{
					currencyFormat.CurrencySymbolPositions.Reverse();
				}
				currencyFSTEx.CurrencyFormats.Add(currencyFormat);
			}
			return currencyFSTEx;
		}

		public static CurrencyFSTEx FromBinary(byte[] data)
		{
			string @string = Encoding.UTF8.GetString(data);
			List<string> list = new List<string>(@string.Split(new char[1]
			{
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries));
			if (list.Count == 0)
			{
				return new CurrencyFSTEx
				{
					Version = 1
				};
			}
			if (!int.TryParse(list[0], out int result))
			{
				throw new Exception("Unexpected data during CurrencyFSTEx deserialization");
			}
			if (result > 1)
			{
				throw new Exception("Unexpected CurrencyFSTEx version: " + result.ToString());
			}
			list.RemoveAt(0);
			List<CurrencyFormat> list2 = new List<CurrencyFormat>();
			string text = null;
			foreach (string item in list)
			{
				CurrencyFormat currencyFormat = new CurrencyFormat();
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				while (num < item.Length && !char.IsWhiteSpace(item[num]))
				{
					stringBuilder.Append(item[num++]);
				}
				if (stringBuilder.Length == 0)
				{
					text = item;
					break;
				}
				num++;
				currencyFormat.Symbol = stringBuilder.ToString();
				currencyFormat.Separators = new HashSet<char>();
				for (; num < item.Length && (item[num] == 'Z' || char.IsWhiteSpace(item[num])); num++)
				{
					char c = item[num];
					if (c == 'Z')
					{
						c = '\0';
					}
					currencyFormat.Separators.Add(c);
				}
				if (num >= item.Length)
				{
					text = item;
					break;
				}
				string text2 = item.Substring(num);
				int num2 = text2.IndexOf('|');
				if (num2 == -1)
				{
					text = item;
					break;
				}
				string category = text2.Substring(num2 + 1);
				text2 = text2.Substring(0, num2);
				string[] array = text2.Split(new char[1]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				currencyFormat.CurrencySymbolPositions = new List<CurrencySymbolPosition>();
				string[] array2 = array;
				foreach (string s in array2)
				{
					if (!int.TryParse(s, out int result2))
					{
						text = item;
						break;
					}
					if (!Enum.IsDefined(typeof(CurrencySymbolPosition), result2))
					{
						text = item;
						break;
					}
					currencyFormat.CurrencySymbolPositions.Add((CurrencySymbolPosition)result2);
				}
				if (text != null)
				{
					break;
				}
				currencyFormat.Category = category;
				list2.Add(currencyFormat);
			}
			if (text != null)
			{
				throw new Exception("Invalid currency format line: " + text);
			}
			return new CurrencyFSTEx
			{
				CurrencyFormats = list2,
				Version = result
			};
		}

		public byte[] ToBinary()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (CurrencyFormats != null && CurrencyFormats.Count > 0)
			{
				stringBuilder.Append(1.ToString());
				foreach (CurrencyFormat currencyFormat in CurrencyFormats)
				{
					stringBuilder.Append('\r');
					stringBuilder.Append(currencyFormat.Symbol);
					stringBuilder.Append(' ');
					if (currencyFormat.Separators != null)
					{
						foreach (char separator in currencyFormat.Separators)
						{
							char value = separator;
							if (separator == '\0')
							{
								value = 'Z';
							}
							stringBuilder.Append(value);
						}
					}
					if (currencyFormat.CurrencySymbolPositions != null)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						foreach (CurrencySymbolPosition currencySymbolPosition in currencyFormat.CurrencySymbolPositions)
						{
							if (stringBuilder2.Length > 0)
							{
								stringBuilder2.Append(",");
							}
							int num = (int)currencySymbolPosition;
							stringBuilder2.Append(num.ToString());
						}
						stringBuilder.Append(stringBuilder2);
					}
					stringBuilder.Append('|');
					stringBuilder.Append(currencyFormat.Category);
				}
			}
			string s = stringBuilder.ToString();
			return Encoding.UTF8.GetBytes(s);
		}
	}
}
