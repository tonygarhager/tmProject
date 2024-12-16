using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class GeneralPunctuationToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "punctuation";

		public GeneralPunctuationToken()
		{
		}

		public GeneralPunctuationToken(string text)
			: base(text)
		{
		}

		public GeneralPunctuationToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new GeneralPunctuationToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			GeneralPunctuationToken generalPunctuationToken = other as GeneralPunctuationToken;
			if (generalPunctuationToken == null)
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
			return GetSimilarity((GeneralPunctuationToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
