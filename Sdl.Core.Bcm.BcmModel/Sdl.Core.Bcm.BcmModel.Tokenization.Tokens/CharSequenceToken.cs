using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class CharSequenceToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "charseq";

		public CharSequenceToken()
		{
		}

		public CharSequenceToken(string text)
			: base(text)
		{
		}

		public CharSequenceToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new CharSequenceToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			CharSequenceToken charSequenceToken = other as CharSequenceToken;
			if (charSequenceToken == null)
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
			return GetSimilarity((CharSequenceToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
