using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class URIToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "uri";

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => false;

		public URIToken()
		{
		}

		public URIToken(string text)
			: base(text)
		{
		}

		public URIToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new URIToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			URIToken uRIToken = other as URIToken;
			if (uRIToken == null)
			{
				return Similarity.None;
			}
			return base.GetSimilarity(other);
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
			return GetSimilarity((URIToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
