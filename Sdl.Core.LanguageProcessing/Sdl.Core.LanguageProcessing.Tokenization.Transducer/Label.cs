using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.Tokenization.Transducer
{
	public class Label
	{
		public const int SpecialSymbolEpsilon = -1;

		public const int SpecialSymbolBeginningOfWord = -3;

		public const int SpecialSymbolEndOfWord = -4;

		public const int SpecialSymbolBeginningOfLine = -5;

		public const int SpecialSymbolEndOfLine = -6;

		public const int SpecialSymbolWhitespace = -7;

		public const int SpecialSymbolDigit = -8;

		public const int FirstUserDefinedSymbol = -1000;

		public bool IsCharLabel => Symbol >= 0;

		public int Symbol
		{
			get;
			set;
		}

		public bool IsConsuming
		{
			get
			{
				if (Symbol < 0 && Symbol != -7 && Symbol != -8)
				{
					return Symbol <= -1000;
				}
				return true;
			}
		}

		public bool IsEpsilon => Symbol == -1;

		public Label(int symbol)
		{
			Symbol = symbol;
		}

		public Label(Label other)
		{
			Symbol = other.Symbol;
		}

		public bool Matches(string s, int position, bool ignoreCase)
		{
			char c;
			if (Symbol >= 0 || Symbol <= -1000)
			{
				if (position >= s.Length)
				{
					return false;
				}
				c = s[position];
				bool flag = c == Symbol;
				if (!flag && ignoreCase)
				{
					flag = (char.ToLowerInvariant(c) == char.ToLowerInvariant((char)Symbol));
				}
				return flag;
			}
			if (Symbol == -1)
			{
				return true;
			}
			if (position == 0 && (Symbol == -5 || Symbol == -3))
			{
				return true;
			}
			if (position >= s.Length)
			{
				if (Symbol != -6)
				{
					return Symbol == -4;
				}
				return true;
			}
			c = s[position];
			switch (Symbol)
			{
			case -7:
				return char.IsWhiteSpace(c);
			case -8:
				return char.IsDigit(c);
			case -3:
			{
				char c2 = (position > 0) ? s[position - 1] : '\0';
				char c3 = c;
				if (char.IsLetterOrDigit(c3))
				{
					return !char.IsLetterOrDigit(c2);
				}
				return false;
			}
			default:
			{
				char c2 = c;
				return !char.IsLetterOrDigit(c2);
			}
			}
		}

		public static bool Matches(char c, List<int> symbols, bool ignoreCase)
		{
			if (symbols == null || symbols.Count == 0)
			{
				return false;
			}
			int num = symbols[0];
			bool flag = true;
			for (int i = 1; i < symbols.Count; i++)
			{
				if (!flag)
				{
					break;
				}
				if (symbols[i] < num)
				{
					flag = false;
				}
				num = symbols[i];
			}
			if (!flag)
			{
				symbols.Sort();
			}
			bool flag2 = symbols.BinarySearch(c) >= 0;
			if (!flag2 && ignoreCase)
			{
				if (char.IsUpper(c))
				{
					flag2 = (symbols.BinarySearch(char.ToLowerInvariant(c)) >= 0);
				}
				else if (char.IsLower(c))
				{
					flag2 = (symbols.BinarySearch(char.ToUpperInvariant(c)) >= 0);
				}
			}
			if (!flag2 && symbols.BinarySearch(-7) >= 0)
			{
				flag2 = char.IsWhiteSpace(c);
			}
			if (!flag2 && symbols.BinarySearch(-8) >= 0)
			{
				flag2 = char.IsDigit(c);
			}
			return flag2;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			Label label = obj as Label;
			if (label != null)
			{
				return Symbol == label.Symbol;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Symbol;
		}

		public override string ToString()
		{
			if (Symbol >= 0)
			{
				return char.ToString((char)Symbol);
			}
			switch (Symbol)
			{
			case -5:
				return "^";
			case -3:
				return "#<";
			case -6:
				return "$";
			case -4:
				return "#>";
			case -1:
				return string.Empty;
			case -7:
				return "\\s";
			case -8:
				return "\\d";
			default:
				return $"[Invalid symbol code ={Symbol}]";
			}
		}
	}
}
