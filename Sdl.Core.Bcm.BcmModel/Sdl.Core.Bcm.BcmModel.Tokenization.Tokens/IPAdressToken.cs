using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class IPAdressToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "ipadress";

		public override bool IsSubstitutable => true;

		public override bool IsPlaceable => false;

		public IPAdressToken()
		{
		}

		public IPAdressToken(string text)
			: base(text)
		{
		}

		public IPAdressToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new IPAdressToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			IPAdressToken iPAdressToken = other as IPAdressToken;
			if (iPAdressToken == null)
			{
				return Similarity.None;
			}
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			if (compareInfo.Compare(base.Text, iPAdressToken.Text, CompareOptions.IgnoreWidth) != 0)
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
			return GetSimilarity((IPAdressToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
