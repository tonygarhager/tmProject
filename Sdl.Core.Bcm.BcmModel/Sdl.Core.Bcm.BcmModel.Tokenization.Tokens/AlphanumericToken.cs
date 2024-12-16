using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class AlphanumericToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "alphanum";

		public override bool IsSubstitutable => true;

		public override bool IsPlaceable => true;

		public AlphanumericToken()
		{
		}

		public AlphanumericToken(string text)
			: base(text)
		{
		}

		public AlphanumericToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new AlphanumericToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			AlphanumericToken alphanumericToken = other as AlphanumericToken;
			if (alphanumericToken == null)
			{
				return Similarity.None;
			}
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			if (compareInfo.Compare(base.Text, alphanumericToken.Text, CompareOptions.IgnoreWidth) != 0)
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
			return GetSimilarity((AlphanumericToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
