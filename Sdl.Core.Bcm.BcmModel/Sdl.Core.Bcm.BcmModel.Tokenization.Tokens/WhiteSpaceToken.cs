using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class WhiteSpaceToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "whitespace";

		public WhiteSpaceToken()
		{
		}

		public WhiteSpaceToken(string text)
			: base(text)
		{
		}

		public WhiteSpaceToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new WhiteSpaceToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			WhiteSpaceToken whiteSpaceToken = other as WhiteSpaceToken;
			if (whiteSpaceToken == null)
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
			return GetSimilarity((WhiteSpaceToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
