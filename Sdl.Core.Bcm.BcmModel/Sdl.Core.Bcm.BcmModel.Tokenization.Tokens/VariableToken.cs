using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class VariableToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "variable";

		public override bool IsSubstitutable => true;

		public override bool IsPlaceable => true;

		public VariableToken()
		{
		}

		public VariableToken(string text)
			: base(text)
		{
		}

		public VariableToken(Token other)
			: base(other)
		{
		}

		public override Token Clone()
		{
			return new VariableToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			VariableToken variableToken = other as VariableToken;
			if (variableToken == null)
			{
				return Similarity.None;
			}
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			if (compareInfo.Compare(base.Text, variableToken.Text, CompareOptions.IgnoreWidth) != 0)
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
			return GetSimilarity((VariableToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
