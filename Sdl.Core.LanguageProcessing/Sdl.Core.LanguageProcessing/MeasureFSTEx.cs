using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.LanguageProcessing
{
	public class MeasureFSTEx
	{
		private const int CurrentVersion = 1;

		public int Version
		{
			get;
			set;
		}

		public Dictionary<string, CustomUnitDefinition> UnitDefinitions
		{
			get;
			set;
		}

		public MeasureFSTEx()
		{
			Version = 1;
		}

		public static MeasureFSTEx FromBinary(byte[] data)
		{
			string @string = Encoding.UTF8.GetString(data);
			List<string> list = new List<string>(@string.Split(new char[1]
			{
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries));
			if (list.Count == 0)
			{
				return new MeasureFSTEx
				{
					Version = 1
				};
			}
			if (!int.TryParse(list[0], out int result))
			{
				throw new Exception("Unexpected data during MeasureFSTEx deserialization");
			}
			if (result > 1)
			{
				throw new Exception("Unexpected MeasureFSTEx version: " + result.ToString());
			}
			list.RemoveAt(0);
			char[] separator = new char[1]
			{
				'\t'
			};
			Dictionary<string, CustomUnitDefinition> dictionary = new Dictionary<string, CustomUnitDefinition>();
			foreach (string item in list)
			{
				string[] array = item.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 0 || array.Length >= 4)
				{
					throw new Exception("Unexpected custom unit serialization format");
				}
				string text = null;
				string text2 = null;
				if (array.Length > 1)
				{
					text2 = array[1];
				}
				if (array.Length > 2)
				{
					text = array[2];
				}
				CustomUnitDefinition customUnitDefinition = null;
				if (text2 != null)
				{
					if (!Enum.TryParse(text2, out Unit result2))
					{
						throw new Exception("Unexpected custom unit serialization type: " + array[1]);
					}
					customUnitDefinition = new CustomUnitDefinition
					{
						Unit = result2,
						CategoryName = (string.IsNullOrEmpty(text) ? null : text)
					};
					if (customUnitDefinition.CategoryName != null && customUnitDefinition.Unit != Unit.NoUnit)
					{
						throw new Exception("Unexpected category name in custom unit data for unit " + array[0] + " of type " + array[1]);
					}
				}
				dictionary.Add(array[0], customUnitDefinition);
			}
			return new MeasureFSTEx
			{
				UnitDefinitions = dictionary,
				Version = result
			};
		}

		public byte[] ToBinary()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (UnitDefinitions != null && UnitDefinitions.Count > 0)
			{
				stringBuilder.Append(1.ToString());
				foreach (KeyValuePair<string, CustomUnitDefinition> unitDefinition in UnitDefinitions)
				{
					stringBuilder.Append('\r');
					string text = string.Empty;
					string text2 = string.Empty;
					if (unitDefinition.Value != null)
					{
						text = unitDefinition.Value.Unit.ToString();
						text2 = (string.IsNullOrEmpty(unitDefinition.Value.CategoryName) ? string.Empty : unitDefinition.Value.CategoryName);
					}
					stringBuilder.Append(unitDefinition.Key + "\t" + text + "\t" + text2);
				}
			}
			string s = stringBuilder.ToString();
			return Encoding.UTF8.GetBytes(s);
		}
	}
}
