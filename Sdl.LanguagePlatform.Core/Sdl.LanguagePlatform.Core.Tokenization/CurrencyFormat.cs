using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public class CurrencyFormat : ICloneable
	{
		private string _symbol;

		private HashSet<char> _separators;

		public string Symbol
		{
			get
			{
				return _symbol;
			}
			set
			{
				if (value != null && value.Any(char.IsWhiteSpace))
				{
					throw new Exception("Whitespace is not permitted in currency symbols");
				}
				_symbol = value;
			}
		}

		public string Category
		{
			get;
			set;
		}

		public List<CurrencySymbolPosition> CurrencySymbolPositions
		{
			get;
			set;
		}

		public HashSet<char> Separators
		{
			get
			{
				return _separators;
			}
			set
			{
				if (value != null && value.Any((char x) => x != 0 && !char.IsWhiteSpace(x)))
				{
					throw new Exception("Separator characters must be whitespace or the null character");
				}
				_separators = value;
			}
		}

		public object Clone()
		{
			CurrencyFormat currencyFormat = new CurrencyFormat();
			currencyFormat.Symbol = ((Symbol == null) ? Symbol : new string(Symbol.ToArray()));
			currencyFormat.Category = ((Category == null) ? Category : new string(Category.ToArray()));
			currencyFormat.CurrencySymbolPositions = ((CurrencySymbolPositions == null) ? CurrencySymbolPositions : new List<CurrencySymbolPosition>(CurrencySymbolPositions));
			currencyFormat.Separators = ((Separators == null) ? Separators : new HashSet<char>(Separators));
			return currencyFormat;
		}
	}
}
