using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class AcronymToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "acronym";

		public override bool IsSubstitutable => true;

		public override bool IsPlaceable => true;

		public AcronymToken()
		{
		}

		public AcronymToken(string text)
			: base(text)
		{
		}

		public AcronymToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new AcronymToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			AcronymToken acronymToken = other as AcronymToken;
			if (acronymToken == null)
			{
				return Similarity.None;
			}
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			if (compareInfo.Compare(base.Text, acronymToken.Text, CompareOptions.IgnoreWidth) != 0)
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
			return GetSimilarity((AcronymToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
