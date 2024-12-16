using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class DateTimeToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "datetime";

		[DataMember(Name = "value")]
		public DateTime Value
		{
			get;
			set;
		}

		[DataMember(Name = "dateTimeType")]
		public DateTimePatternType DateTimeType
		{
			get;
			set;
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		public bool IsDateToken
		{
			get
			{
				if (DateTimeType != DateTimePatternType.LongDate)
				{
					return DateTimeType == DateTimePatternType.ShortDate;
				}
				return true;
			}
		}

		public bool IsTimeToken
		{
			get
			{
				if (DateTimeType != DateTimePatternType.LongTime)
				{
					return DateTimeType == DateTimePatternType.ShortTime;
				}
				return true;
			}
		}

		public DateTimeToken()
		{
		}

		public DateTimeToken(DateTimeToken other)
			: base(other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Value = other.Value;
			DateTimeType = other.DateTimeType;
		}

		public DateTimeToken(string text, DateTime dateTime, DateTimePatternType type)
			: base(text)
		{
			DateTimeType = type;
			Value = dateTime;
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
			return GetSimilarity((DateTimeToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override Similarity GetSimilarity(Token other)
		{
			if (other == null || other.GetType() != GetType())
			{
				return Similarity.None;
			}
			DateTimeToken dateTimeToken = other as DateTimeToken;
			if (dateTimeToken == null)
			{
				return Similarity.None;
			}
			if ((!IsDateToken || !dateTimeToken.IsDateToken) && (!IsTimeToken || !dateTimeToken.IsTimeToken))
			{
				return Similarity.None;
			}
			if (!(Math.Abs((Value - dateTimeToken.Value).TotalSeconds) < 10.0))
			{
				return Similarity.IdenticalType;
			}
			return Similarity.IdenticalValueAndType;
		}

		public override Token Clone()
		{
			return new DateTimeToken(this);
		}
	}
}
