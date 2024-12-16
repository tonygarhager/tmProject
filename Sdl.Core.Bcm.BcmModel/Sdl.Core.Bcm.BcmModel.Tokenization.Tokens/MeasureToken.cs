using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class MeasureToken : NumberToken
	{
		[DataMember(Name = "type")]
		protected override string Type => "measure";

		[DataMember(Name = "unit")]
		public Unit Unit
		{
			get;
			set;
		}

		[DataMember(Name = "unitSeparator")]
		public char UnitSeparator
		{
			get;
			set;
		}

		[DataMember(Name = "unitString")]
		public string UnitString
		{
			get;
			set;
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		public bool IsValid => UnitString != null;

		public MeasureToken()
		{
		}

		public MeasureToken(string text, string sign, string decimalPart, string fractionalPart, Unit unit, string unitString, char unitSeparator, NumberFormatInfo format)
			: base(text, sign, decimalPart, fractionalPart, format)
		{
			Unit = unit;
			UnitString = unitString;
			UnitSeparator = unitSeparator;
		}

		public MeasureToken(string text, NumberToken numericPart, Unit unit, string unitString, char unitSeparator)
			: base(text, numericPart.GroupSeparator, numericPart.DecimalSeparator, numericPart.AlternateGroupSeparator, numericPart.AlternateDecimalSeparator, numericPart.Sign, numericPart.RawSign, numericPart.RawDecimalDigits, numericPart.RawFractionalDigits)
		{
			Unit = unit;
			UnitString = unitString;
			UnitSeparator = unitSeparator;
		}

		public MeasureToken(MeasureToken other)
			: base(other)
		{
			Unit = other.Unit;
			UnitSeparator = other.UnitSeparator;
			UnitString = other.UnitString;
		}

		public override Token Clone()
		{
			return new MeasureToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			if (other == null || other.GetType() != GetType())
			{
				return Similarity.None;
			}
			MeasureToken measureToken = other as MeasureToken;
			if (measureToken == null)
			{
				return Similarity.None;
			}
			bool flag = base.GetSimilarity((Token)measureToken) == Similarity.IdenticalValueAndType;
			if (Unit == Unit.NoUnit && measureToken.Unit == Unit.NoUnit)
			{
				if (PhysicalUnit.AreUnitsSameCategory(UnitString, measureToken.UnitString))
				{
					if (!flag)
					{
						return Similarity.IdenticalType;
					}
					return Similarity.IdenticalValueAndType;
				}
				return Similarity.None;
			}
			if (PhysicalUnit.AreUnitTypesCompatible(Unit, measureToken.Unit))
			{
				if (!flag)
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			return Similarity.None;
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
			if (!base.Equals(obj))
			{
				return false;
			}
			MeasureToken measureToken = (MeasureToken)obj;
			if (Unit != measureToken.Unit)
			{
				return false;
			}
			if (Unit == Unit.NoUnit)
			{
				return string.Equals(UnitString, measureToken.UnitString, StringComparison.Ordinal);
			}
			return true;
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 3) | (int)Unit;
		}

		public new bool SetValue(Token blueprint, bool keepNumericSeparators)
		{
			bool flag = base.SetValue(blueprint, keepNumericSeparators);
			MeasureToken measureToken = blueprint as MeasureToken;
			if (measureToken != null && (flag || Unit != measureToken.Unit || UnitSeparator != measureToken.UnitSeparator || !string.Equals(UnitString, measureToken.UnitString, StringComparison.Ordinal)))
			{
				Unit = measureToken.Unit;
				UnitSeparator = measureToken.UnitSeparator;
				UnitString = measureToken.UnitString;
				flag = true;
			}
			return flag;
		}
	}
}
