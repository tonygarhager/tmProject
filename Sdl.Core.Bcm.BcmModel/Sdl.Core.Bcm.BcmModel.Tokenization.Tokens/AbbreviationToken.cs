using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class AbbreviationToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "abbreviation";

		public AbbreviationToken()
		{
		}

		public AbbreviationToken(string text)
			: base(text)
		{
		}

		public AbbreviationToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new AbbreviationToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			AbbreviationToken abbreviationToken = other as AbbreviationToken;
			if (abbreviationToken == null)
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
			return GetSimilarity((AbbreviationToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
