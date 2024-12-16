using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class NumberToken : Token
	{
		private static readonly string[] StandardDigits;

		[DataMember(Name = "type")]
		protected override string Type => "number";

		private string CanonicalNumber
		{
			get;
			set;
		}

		[DataMember(Name = "value")]
		public double Value
		{
			get;
			set;
		}

		[DataMember(Name = "valueValid")]
		public bool ValueValid
		{
			get;
			set;
		}

		[DataMember(Name = "sign")]
		public Sign Sign
		{
			get;
			set;
		}

		[DataMember(Name = "rawSign")]
		public string RawSign
		{
			get;
			set;
		}

		[DataMember(Name = "decimalSeparator")]
		public NumericSeparator DecimalSeparator
		{
			get;
			set;
		}

		[DataMember(Name = "groupSeparator")]
		public NumericSeparator GroupSeparator
		{
			get;
			set;
		}

		[DataMember(Name = "alternateGroupSeparator")]
		public char AlternateGroupSeparator
		{
			get;
			set;
		}

		[DataMember(Name = "alternateDecimalSeparator")]
		public char AlternateDecimalSeparator
		{
			get;
			set;
		}

		[DataMember(Name = "rawFractionalDigits")]
		public string RawFractionalDigits
		{
			get;
			set;
		}

		[DataMember(Name = "rawDecimalDigits")]
		public string RawDecimalDigits
		{
			get;
			set;
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		public NumberToken()
		{
		}

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
			GroupSeparator = groupSeparator;
			AlternateGroupSeparator = ((groupSeparator == NumericSeparator.Alternate) ? alternateGroupSeparator : '\0');
			DecimalSeparator = decimalSeparator;
			AlternateDecimalSeparator = ((decimalSeparator == NumericSeparator.Alternate) ? alternateDecimalSeparator : '\0');
			Sign = sign;
			RawSign = rawSign;
			RawDecimalDigits = rawDecimalDigits;
			RawFractionalDigits = rawFractionalDigits;
			UpdateCanonicalFormAndValue();
		}

		public NumberToken(NumberToken other)
			: base(other)
		{
			SetValue(other, keepNumericSeparators: false);
		}

		public override Token Clone()
		{
			return new NumberToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			if (other == null || other.GetType() != GetType())
			{
				return Similarity.None;
			}
			NumberToken numberToken = other as NumberToken;
			if (numberToken == null)
			{
				return Similarity.None;
			}
			if (ValueValid)
			{
				if (Math.Abs(Value - numberToken.Value) < 0.01)
				{
					return Similarity.IdenticalValueAndType;
				}
				return Similarity.IdenticalType;
			}
			if (string.Equals(CanonicalNumber, numberToken.CanonicalNumber))
			{
				return Similarity.IdenticalValueAndType;
			}
			return Similarity.IdenticalType;
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
			if (ValueValid && numberToken.ValueValid)
			{
				return Math.Abs(Value - numberToken.Value) < 1E-06;
			}
			return string.Equals(CanonicalNumber, numberToken.CanonicalNumber);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool SetValue(Token blueprint, bool keepNumericSeparators)
		{
			NumberToken numberToken = blueprint as NumberToken;
			if (numberToken == null)
			{
				throw new ArgumentException("blueprint token does not have the expected type");
			}
			if (GroupSeparator != numberToken.GroupSeparator || DecimalSeparator != numberToken.DecimalSeparator || RawDecimalDigits != numberToken.RawDecimalDigits || RawFractionalDigits != numberToken.RawFractionalDigits || RawSign != numberToken.RawSign || Sign != numberToken.Sign || Math.Abs(Value - numberToken.Value) > 1E-06 || ValueValid != numberToken.ValueValid || !string.Equals(CanonicalNumber, numberToken.CanonicalNumber))
			{
				if (!keepNumericSeparators || RawDecimalDigits.Length <= 3)
				{
					GroupSeparator = numberToken.GroupSeparator;
					AlternateGroupSeparator = numberToken.AlternateGroupSeparator;
				}
				if (!keepNumericSeparators || string.IsNullOrEmpty(RawFractionalDigits))
				{
					DecimalSeparator = numberToken.DecimalSeparator;
					AlternateDecimalSeparator = numberToken.AlternateDecimalSeparator;
				}
				RawFractionalDigits = numberToken.RawFractionalDigits;
				RawDecimalDigits = numberToken.RawDecimalDigits;
				RawSign = numberToken.RawSign;
				Sign = numberToken.Sign;
				Value = numberToken.Value;
				ValueValid = numberToken.ValueValid;
				CanonicalNumber = numberToken.CanonicalNumber;
				return true;
			}
			return false;
		}

		public void SetValue(string sign, string decimalPart, string fractionalPart, NumberFormatInfo format)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (string.IsNullOrEmpty(sign))
			{
				RawSign = null;
				Sign = Sign.None;
			}
			else
			{
				char c = sign[0];
				if (c == '+' || c == '＋')
				{
					Sign = Sign.Plus;
				}
				else
				{
					Sign = Sign.Minus;
				}
				RawSign = sign;
			}
			char c2 = (!string.IsNullOrEmpty(format.NumberDecimalSeparator)) ? format.NumberDecimalSeparator[0] : '\0';
			char c3 = (!string.IsNullOrEmpty(format.NumberGroupSeparator)) ? format.NumberGroupSeparator[0] : '\0';
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < decimalPart.Length; i++)
			{
				int num = (int)char.GetNumericValue(decimalPart, i);
				if (num < 0)
				{
					if (GroupSeparator == NumericSeparator.None)
					{
						if (decimalPart[i] == c3)
						{
							GroupSeparator = NumericSeparator.Primary;
							continue;
						}
						AlternateGroupSeparator = decimalPart[i];
						GroupSeparator = NumericSeparator.Alternate;
					}
				}
				else
				{
					stringBuilder.Append("0123456789"[num]);
				}
			}
			RawDecimalDigits = stringBuilder.ToString();
			if (string.IsNullOrEmpty(fractionalPart))
			{
				RawFractionalDigits = string.Empty;
				DecimalSeparator = NumericSeparator.None;
			}
			else
			{
				if (fractionalPart[0] == c2)
				{
					DecimalSeparator = NumericSeparator.Primary;
				}
				else
				{
					AlternateDecimalSeparator = fractionalPart[0];
					DecimalSeparator = NumericSeparator.Alternate;
				}
				stringBuilder = new StringBuilder();
				for (int j = 1; j < fractionalPart.Length; j++)
				{
					int num2 = (int)char.GetNumericValue(fractionalPart, j);
					if (num2 < 0)
					{
						throw new Exception("TokenizerInvalidNumericFormat");
					}
					stringBuilder.Append("0123456789"[num2]);
				}
				RawFractionalDigits = stringBuilder.ToString();
			}
			UpdateCanonicalFormAndValue();
		}

		private void UpdateCanonicalFormAndValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Sign == Sign.Minus)
			{
				stringBuilder.Append("-");
			}
			stringBuilder.Append(RawDecimalDigits);
			if (RawFractionalDigits != null)
			{
				stringBuilder.Append(".");
				stringBuilder.Append(RawFractionalDigits);
			}
			CanonicalNumber = stringBuilder.ToString();
			try
			{
				Value = double.Parse(CanonicalNumber, CultureInfo.InvariantCulture);
			}
			catch
			{
				Value = 0.0;
				ValueValid = false;
			}
		}

		public string FormatNumber(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			return FormatNumber(culture.NumberFormat);
		}

		public string FormatNumber(NumberFormatInfo nfi)
		{
			if (nfi == null)
			{
				throw new ArgumentNullException();
			}
			StringBuilder stringBuilder = new StringBuilder();
			switch (Sign)
			{
			case Sign.Plus:
				stringBuilder.Append(nfi.PositiveSign);
				break;
			case Sign.Minus:
				stringBuilder.Append(nfi.NegativeSign);
				break;
			}
			if (!string.IsNullOrEmpty(RawDecimalDigits))
			{
				string separator = null;
				if (GroupSeparator == NumericSeparator.Alternate && AlternateGroupSeparator != 0)
				{
					separator = char.ToString(AlternateGroupSeparator);
				}
				else if (GroupSeparator == NumericSeparator.Primary)
				{
					separator = nfi.NumberGroupSeparator;
				}
				AppendDigits(stringBuilder, RawDecimalDigits, nfi, useNativeDigits: false, nfi.NumberGroupSizes, separator);
			}
			if (!string.IsNullOrEmpty(RawFractionalDigits))
			{
				if (DecimalSeparator == NumericSeparator.Alternate && AlternateDecimalSeparator != 0)
				{
					stringBuilder.Append(AlternateDecimalSeparator);
				}
				else if (DecimalSeparator == NumericSeparator.Primary)
				{
					stringBuilder.Append(nfi.NumberDecimalSeparator);
				}
				AppendDigits(stringBuilder, RawFractionalDigits, nfi, useNativeDigits: false, null, null);
			}
			return stringBuilder.ToString();
		}

		private static void AppendDigits(StringBuilder sb, string rawDigits, NumberFormatInfo nfi, bool useNativeDigits, int[] grouping, string separator)
		{
			if (rawDigits == null)
			{
				return;
			}
			if (grouping == null || grouping.Length == 0 || string.IsNullOrEmpty(separator))
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
					if (num + 1 < grouping.Length)
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
	}
}
