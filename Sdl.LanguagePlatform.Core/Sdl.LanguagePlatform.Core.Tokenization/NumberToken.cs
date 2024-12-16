using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	[KnownType(typeof(MeasureToken))]
	public class NumberToken : Token, ILocalizableToken
	{
		private NumericSeparator _groupSeparator;

		private NumericSeparator _decimalSeparator;

		private char _alternateGroupSeparator;

		private char _alternateDecimalSeparator;

		private Sign _sign;

		private string _rawFractionalDigits;

		private string _rawDecimalDigits;

		private string _rawSign;

		private string _canonicalNumber;

		private bool _valueValid;

		private static readonly string[] StandardDigits;

		[DataMember]
		public double Value
		{
			get;
			set;
		}

		[DataMember]
		public bool ValueValid
		{
			get
			{
				return _valueValid;
			}
			set
			{
				_valueValid = false;
			}
		}

		[DataMember]
		public Sign Sign
		{
			get
			{
				return _sign;
			}
			set
			{
				_sign = value;
			}
		}

		[DataMember]
		public string RawSign
		{
			get
			{
				return _rawSign;
			}
			set
			{
				_rawSign = value;
			}
		}

		[DataMember]
		public NumericSeparator DecimalSeparator
		{
			get
			{
				return _decimalSeparator;
			}
			set
			{
				_decimalSeparator = value;
			}
		}

		[DataMember]
		public NumericSeparator GroupSeparator
		{
			get
			{
				return _groupSeparator;
			}
			set
			{
				_groupSeparator = value;
			}
		}

		[DataMember]
		public char AlternateGroupSeparator
		{
			get
			{
				return _alternateGroupSeparator;
			}
			set
			{
				_alternateGroupSeparator = value;
			}
		}

		[DataMember]
		public char AlternateDecimalSeparator
		{
			get
			{
				return _alternateDecimalSeparator;
			}
			set
			{
				_alternateDecimalSeparator = value;
			}
		}

		[DataMember]
		public string RawFractionalDigits
		{
			get
			{
				return _rawFractionalDigits;
			}
			set
			{
				_rawFractionalDigits = value;
			}
		}

		[DataMember]
		public string RawDecimalDigits
		{
			get
			{
				return _rawDecimalDigits;
			}
			set
			{
				_rawDecimalDigits = value;
			}
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		static NumberToken()
		{
			StandardDigits = new string[10];
			for (int i = 0; i < 10; i++)
			{
				StandardDigits[i] = char.ToString("0123456789"[i]);
			}
		}

		public NumberToken(string text, string sign, string decimalPart, string fractionalPart, NumberFormatInfo format)
			: base(text)
		{
			SetValue(sign, decimalPart, fractionalPart, format);
		}

		public NumberToken(string text, NumericSeparator groupSeparator, NumericSeparator decimalSeparator, char alternateGroupSeparator, char alternateDecimalSeparator, Sign sign, string rawSign, string rawDecimalDigits, string rawFractionalDigits)
			: base(text)
		{
			_groupSeparator = groupSeparator;
			_alternateGroupSeparator = ((groupSeparator == NumericSeparator.Alternate) ? alternateGroupSeparator : '\0');
			_decimalSeparator = decimalSeparator;
			_alternateDecimalSeparator = ((decimalSeparator == NumericSeparator.Alternate) ? alternateDecimalSeparator : '\0');
			_sign = sign;
			_rawSign = rawSign;
			_rawDecimalDigits = rawDecimalDigits;
			_rawFractionalDigits = rawFractionalDigits;
			UpdateCanonicalFormAndValue();
		}

		public NumberToken(NumberToken other)
			: base(other)
		{
			SetValue(other, keepNumericSeparators: false);
		}

		public virtual bool SetValue(Token blueprint, bool keepNumericSeparators)
		{
			NumberToken numberToken = blueprint as NumberToken;
			if (numberToken == null)
			{
				throw new ArgumentException("blueprint token does not have the expected type");
			}
			if (_groupSeparator == numberToken._groupSeparator && _decimalSeparator == numberToken._decimalSeparator && _rawDecimalDigits == numberToken._rawDecimalDigits && _rawFractionalDigits == numberToken._rawFractionalDigits && _rawSign == numberToken._rawSign && _sign == numberToken._sign && Value == numberToken.Value && _valueValid == numberToken._valueValid && string.Equals(_canonicalNumber, numberToken._canonicalNumber))
			{
				return false;
			}
			if (!keepNumericSeparators || _rawDecimalDigits.Length <= 3)
			{
				_groupSeparator = numberToken._groupSeparator;
				_alternateGroupSeparator = numberToken._alternateGroupSeparator;
			}
			if (!keepNumericSeparators || string.IsNullOrEmpty(_rawFractionalDigits))
			{
				_decimalSeparator = numberToken._decimalSeparator;
				_alternateDecimalSeparator = numberToken._alternateDecimalSeparator;
			}
			_rawFractionalDigits = numberToken._rawFractionalDigits;
			_rawDecimalDigits = numberToken._rawDecimalDigits;
			_rawSign = numberToken._rawSign;
			_sign = numberToken._sign;
			Value = numberToken.Value;
			_valueValid = numberToken._valueValid;
			_canonicalNumber = numberToken._canonicalNumber;
			return true;
		}

		public virtual void SetValue(string sign, string decimalPart, string fractionalPart, NumberFormatInfo format)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (string.IsNullOrEmpty(decimalPart))
			{
				throw new LanguagePlatformException(ErrorCode.TokenizerInvalidNumericFormat);
			}
			if (string.IsNullOrEmpty(sign))
			{
				_rawSign = null;
				_sign = Sign.None;
			}
			else
			{
				char c = sign[0];
				if (c == '+' || c == '＋')
				{
					_sign = Sign.Plus;
				}
				else
				{
					_sign = Sign.Minus;
				}
				_rawSign = sign;
			}
			char c2 = (!string.IsNullOrEmpty(format.NumberDecimalSeparator)) ? format.NumberDecimalSeparator[0] : '\0';
			char c3 = (!string.IsNullOrEmpty(format.NumberGroupSeparator)) ? format.NumberGroupSeparator[0] : '\0';
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < decimalPart.Length; i++)
			{
				int num = (int)char.GetNumericValue(decimalPart, i);
				if (num < 0)
				{
					if (_groupSeparator == NumericSeparator.None)
					{
						if (decimalPart[i] == c3)
						{
							_groupSeparator = NumericSeparator.Primary;
							continue;
						}
						_alternateGroupSeparator = decimalPart[i];
						_groupSeparator = NumericSeparator.Alternate;
					}
				}
				else
				{
					stringBuilder.Append("0123456789"[num]);
				}
			}
			_rawDecimalDigits = stringBuilder.ToString();
			if (string.IsNullOrEmpty(fractionalPart))
			{
				_rawFractionalDigits = string.Empty;
				_decimalSeparator = NumericSeparator.None;
			}
			else
			{
				if (fractionalPart[0] == c2)
				{
					_decimalSeparator = NumericSeparator.Primary;
				}
				else
				{
					_alternateDecimalSeparator = fractionalPart[0];
					_decimalSeparator = NumericSeparator.Alternate;
				}
				stringBuilder = new StringBuilder();
				for (int j = 1; j < fractionalPart.Length; j++)
				{
					int num2 = (int)char.GetNumericValue(fractionalPart, j);
					if (num2 < 0)
					{
						throw new LanguagePlatformException(ErrorCode.TokenizerInvalidNumericFormat);
					}
					stringBuilder.Append("0123456789"[num2]);
				}
				_rawFractionalDigits = stringBuilder.ToString();
			}
			UpdateCanonicalFormAndValue();
		}

		private void UpdateCanonicalFormAndValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (_sign == Sign.Minus)
			{
				stringBuilder.Append("-");
			}
			stringBuilder.Append(_rawDecimalDigits);
			if (_rawFractionalDigits != null)
			{
				stringBuilder.Append(".");
				stringBuilder.Append(_rawFractionalDigits);
			}
			_canonicalNumber = stringBuilder.ToString();
			try
			{
				Value = double.Parse(_canonicalNumber, CultureInfo.InvariantCulture);
			}
			catch
			{
				Value = 0.0;
				_valueValid = false;
			}
		}

		public override SegmentElement Duplicate()
		{
			return new NumberToken(this);
		}

		public virtual bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings)
		{
			return Localize(culture, autoLocalizationSettings, null, adjustCasing: false);
		}

		public virtual bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings, ILocalizableToken originalMemoryToken, bool adjustCasing)
		{
			return Localize(culture, autoLocalizationSettings, originalMemoryToken, adjustCasing, currency: false);
		}

		internal bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings, ILocalizableToken originalMemoryToken, bool adjustCasing, bool currency)
		{
			NumberToken numberToken = originalMemoryToken as NumberToken;
			if (numberToken == null)
			{
				if (DecimalSeparator == NumericSeparator.Alternate)
				{
					DecimalSeparator = NumericSeparator.Primary;
				}
				if (GroupSeparator == NumericSeparator.Alternate)
				{
					GroupSeparator = NumericSeparator.Primary;
				}
			}
			string text = FormatNumber(culture, autoLocalizationSettings, currency, numberToken);
			if (string.Equals(text, Text, StringComparison.Ordinal))
			{
				return false;
			}
			Text = text;
			base.Culture = culture;
			return true;
		}

		public string FormatNumber(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			return FormatNumber(LanguageMetadata.GetOrCreateMetadata(culture.Name).NumberFormat);
		}

		private string FormatNumber(CultureInfo culture, AutoLocalizationSettings settings, bool currency, NumberToken originalMemoryToken)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			return FormatNumber(LanguageMetadata.GetOrCreateMetadata(culture.Name).NumberFormat, settings, currency, originalMemoryToken);
		}

		public string FormatNumber(NumberFormatInfo nfi)
		{
			return FormatNumber(nfi, null, currency: false, null);
		}

		private string FormatNumber(NumberFormatInfo nfi, AutoLocalizationSettings settings, bool currency, NumberToken originalMemoryToken)
		{
			if (nfi == null)
			{
				throw new ArgumentNullException();
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = !string.IsNullOrEmpty(originalMemoryToken?.RawSign) && originalMemoryToken.Sign == Sign;
			bool flag2 = true;
			if (settings != null)
			{
				flag2 = !(currency ? settings.FormatCurrencySeparatorsUniformly : settings.FormatNumberSeparatorsUniformly);
			}
			if (originalMemoryToken == null)
			{
				flag2 = false;
			}
			_ = nfi.NumberNegativePattern;
			_ = 2;
			switch (_sign)
			{
			case Sign.Plus:
				stringBuilder.Append(flag ? originalMemoryToken.RawSign : nfi.PositiveSign);
				break;
			case Sign.Minus:
				stringBuilder.Append(flag ? originalMemoryToken.RawSign : nfi.NegativeSign);
				break;
			}
			if (!string.IsNullOrEmpty(_rawDecimalDigits))
			{
				string separator = null;
				switch (_groupSeparator)
				{
				case NumericSeparator.Alternate:
					if (_alternateGroupSeparator != 0)
					{
						separator = char.ToString(_alternateGroupSeparator);
					}
					break;
				case NumericSeparator.Primary:
					separator = nfi.NumberGroupSeparator;
					if (base.TokenizationContext?.SeparatorCombinations != null && base.TokenizationContext.SeparatorCombinations.Count > 0)
					{
						separator = base.TokenizationContext.SeparatorCombinations[0].GroupSeparators[0].ToString();
					}
					break;
				}
				string text = (!currency) ? settings?.NumberGroupSeparator : settings?.CurrencyGroupSeparator;
				if (text != null && !WrittenWithoutGroupSeparator(nfi) && !flag2)
				{
					separator = text;
				}
				AppendDigits(stringBuilder, _rawDecimalDigits, nfi, useNativeDigits: false, nfi.NumberGroupSizes, separator);
			}
			if (!string.IsNullOrEmpty(_rawFractionalDigits))
			{
				string value = null;
				switch (_decimalSeparator)
				{
				case NumericSeparator.Alternate:
					if (_alternateDecimalSeparator != 0)
					{
						value = char.ToString(_alternateDecimalSeparator);
					}
					break;
				case NumericSeparator.Primary:
					value = nfi.NumberDecimalSeparator;
					if (base.TokenizationContext?.SeparatorCombinations != null && base.TokenizationContext.SeparatorCombinations.Count > 0)
					{
						value = base.TokenizationContext.SeparatorCombinations[0].DecimalSeparators[0].ToString();
					}
					break;
				}
				string text2 = (!currency) ? settings?.NumberDecimalSeparator : settings?.CurrencyDecimalSeparator;
				if (text2 != null && !flag2)
				{
					value = text2;
				}
				stringBuilder.Append(value);
				AppendDigits(stringBuilder, _rawFractionalDigits, nfi, useNativeDigits: false, null, null);
			}
			return stringBuilder.ToString();
		}

		private bool WrittenWithoutGroupSeparator(NumberFormatInfo nfi)
		{
			int num = (nfi?.NumberGroupSizes == null || nfi.NumberGroupSizes.Length < 1) ? 3 : nfi.NumberGroupSizes[0];
			if (GroupSeparator != 0)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(RawDecimalDigits))
			{
				return RawDecimalDigits.Length > num;
			}
			return false;
		}

		private static void AppendDigits(StringBuilder sb, string rawDigits, NumberFormatInfo nfi, bool useNativeDigits, IReadOnlyList<int> grouping, string separator)
		{
			if (rawDigits == null)
			{
				return;
			}
			if (grouping == null || grouping.Count == 0 || string.IsNullOrEmpty(separator))
			{
				foreach (char c in rawDigits)
				{
					sb.Append(useNativeDigits ? nfi.NativeDigits[c - 48] : StandardDigits[c - 48]);
				}
				return;
			}
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			for (int num3 = rawDigits.Length - 1; num3 >= 0; num3--)
			{
				int num4 = grouping[num];
				if (num4 > 0 && num2 == num4)
				{
					stringBuilder.Insert(0, separator);
					num2 = 0;
					if (num + 1 < grouping.Count)
					{
						num++;
					}
				}
				char c2 = rawDigits[num3];
				stringBuilder.Insert(0, useNativeDigits ? nfi.NativeDigits[c2 - 48] : StandardDigits[c2 - 48]);
				num2++;
			}
			sb.Append(stringBuilder);
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			Similarity bundleSimilarity = GetBundleSimilarity(other);
			if (other == null || other.GetType() != GetType())
			{
				return bundleSimilarity;
			}
			NumberToken numberToken = other as NumberToken;
			if (numberToken == null)
			{
				return Similarity.None;
			}
			if (_valueValid)
			{
				if (!(Math.Abs(Value - numberToken.Value) < 0.01))
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			if (!string.Equals(_canonicalNumber, numberToken._canonicalNumber))
			{
				return Similarity.IdenticalType;
			}
			return Similarity.IdenticalValueAndType;
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
			NumberToken numberToken = (NumberToken)obj;
			if (_valueValid && numberToken._valueValid)
			{
				return Value == numberToken.Value;
			}
			return string.Equals(_canonicalNumber, numberToken._canonicalNumber);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected override TokenType GetTokenType()
		{
			return TokenType.Number;
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException();
			}
			visitor.VisitNumberToken(this);
		}
	}
}
