using System;
using System.Globalization;
using System.Text;

namespace Sdl.LanguagePlatform.Core
{
	public static class StringUtilities
	{
		public enum Casing
		{
			AllUpper,
			AllLower,
			InitialUpper,
			Mixed
		}

		public static Casing DetermineCasing(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return Casing.Mixed;
			}
			if (!char.IsUpper(s[0]))
			{
				if (!CharacterProperties.IsAll(s, 1, char.IsLower))
				{
					return Casing.Mixed;
				}
				return Casing.AllLower;
			}
			if (CharacterProperties.IsAll(s, 1, char.IsUpper))
			{
				return Casing.AllUpper;
			}
			if (!CharacterProperties.IsAll(s, 1, char.IsLower))
			{
				return Casing.Mixed;
			}
			return Casing.InitialUpper;
		}

		public static bool StartWithAny(string s, char[] characterList)
		{
			if (characterList == null)
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			return s.IndexOfAny(characterList) == 0;
		}

		public static bool EndsWithAny(string s, char[] characterList)
		{
			if (characterList == null)
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			return s.LastIndexOfAny(characterList) == s.Length - 1;
		}

		public static bool IsAllWhitespace(string t)
		{
			if (!string.IsNullOrEmpty(t))
			{
				return CharacterProperties.IsAll(t, CharacterProperties.IsWhitespace);
			}
			return false;
		}

		public static string EscapeString(string s)
		{
			return EscapeString(s, "\"\\");
		}

		public static string RemoveAll(string s, Predicate<char> property)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in s)
			{
				if (!property(c))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static string EscapeString(string s, string charactersToEscape)
		{
			if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(charactersToEscape))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char value in s)
			{
				if (charactersToEscape.IndexOf(value) >= 0)
				{
					stringBuilder.Append("\\");
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public static int GetPrefixLength(string s, char[] prefixChars)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0;
			}
			if (prefixChars == null || prefixChars.Length == 0)
			{
				return 0;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (Array.IndexOf(prefixChars, s[i]) < 0)
				{
					return i;
				}
			}
			return s.Length;
		}

		public static int GetSuffixLength(string s, char[] suffixChars)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0;
			}
			if (suffixChars == null || suffixChars.Length == 0)
			{
				return 0;
			}
			int num = s.Length - 1;
			for (int num2 = num; num2 >= 0; num2--)
			{
				if (Array.IndexOf(suffixChars, s[num2]) < 0)
				{
					return num - num2;
				}
			}
			return s.Length;
		}

		public static string TrimStart(string s, char[] trimCharacters, out string trimmedPrefix)
		{
			trimmedPrefix = null;
			int prefixLength = GetPrefixLength(s, trimCharacters);
			if (prefixLength <= 0)
			{
				return s;
			}
			trimmedPrefix = s.Substring(0, prefixLength);
			return s.Substring(prefixLength);
		}

		public static string TrimEnd(string s, char[] trimCharacters, out string trimmedSuffix)
		{
			trimmedSuffix = null;
			int suffixLength = GetSuffixLength(s, trimCharacters);
			if (suffixLength <= 0)
			{
				return s;
			}
			int length = s.Length;
			trimmedSuffix = s.Substring(length - suffixLength);
			return s.Substring(0, length - suffixLength);
		}

		public static string MergeStrings(string a, string b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			StringBuilder stringBuilder = new StringBuilder(a);
			foreach (char value in b)
			{
				if (a.IndexOf(value) < 0)
				{
					stringBuilder.Append(value);
				}
			}
			return stringBuilder.ToString();
		}

		public static string GenerateRandomWord(int length, bool initialUpper)
		{
			if (length < 0)
			{
				return null;
			}
			if (length == 0)
			{
				return string.Empty;
			}
			Random random = new Random();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				int index = random.Next("abcdefghijklmnopqrstuvwxyz".Length);
				char c = "abcdefghijklmnopqrstuvwxyz"[index];
				if (i == 0 && initialUpper)
				{
					c = char.ToUpper(c, CultureInfo.InvariantCulture);
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public static string HalfWidthToFullWidth(string input)
		{
			char[] array = input.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == ' ')
				{
					array[i] = '\u3000';
				}
				else if (array[i] < '\u007f')
				{
					array[i] = (char)(array[i] + 65248);
				}
			}
			return new string(array);
		}

		public static string HalfWidthToFullWidth2(string input)
		{
			if (input.Length != 0)
			{
				return MapStringExWrapper.MapString(input, MapStringExWrapper.MapFlag.FullWidth);
			}
			return string.Empty;
		}
	}
}
