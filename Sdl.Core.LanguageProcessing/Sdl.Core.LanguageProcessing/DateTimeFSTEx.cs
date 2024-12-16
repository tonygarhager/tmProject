using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing
{
	public class DateTimeFSTEx
	{
		private const int CurrentVersion = 1;

		public int Version
		{
			get;
			set;
		}

		public List<string> Patterns
		{
			get;
			set;
		}

		public DateTimeFSTEx()
		{
			Version = 1;
			Patterns = new List<string>();
		}

		public static DateTimeFSTEx FromBinary(byte[] data)
		{
			string @string = Encoding.UTF8.GetString(data);
			List<string> list = new List<string>(@string.Split(new char[1]
			{
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries));
			if (list.Count == 0)
			{
				return new DateTimeFSTEx
				{
					Version = 1
				};
			}
			if (!int.TryParse(list[0], out int result))
			{
				throw new Exception("Unexpected data during DateTimeFSTEx deserialization");
			}
			if (result > 1)
			{
				throw new Exception("Unexpected DateTimeFSTEx version: " + result.ToString());
			}
			list.RemoveAt(0);
			DateTimeFSTEx dateTimeFSTEx = new DateTimeFSTEx
			{
				Version = result
			};
			foreach (string item in list)
			{
				dateTimeFSTEx.Patterns.Add(item);
			}
			return dateTimeFSTEx;
		}

		public byte[] ToBinary()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Patterns == null || Patterns.Count <= 0)
			{
				return Encoding.UTF8.GetBytes(stringBuilder.ToString());
			}
			stringBuilder.Append(1.ToString());
			foreach (string pattern in Patterns)
			{
				stringBuilder.Append('\r');
				stringBuilder.Append(pattern);
			}
			return Encoding.UTF8.GetBytes(stringBuilder.ToString());
		}
	}
}
