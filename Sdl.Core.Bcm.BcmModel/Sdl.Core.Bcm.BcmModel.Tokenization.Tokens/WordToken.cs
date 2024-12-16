using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class WordToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "word";

		[DataMember(Name = "stem")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Stem
		{
			get;
			set;
		}

		[DataMember(Name = "isStopWord")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool IsStopword
		{
			get;
			set;
		}

		public WordToken()
		{
		}

		public WordToken(string text)
			: base(text)
		{
		}

		public WordToken(WordToken other)
			: base(other)
		{
			Stem = ((other.Stem == null) ? null : string.Copy(other.Stem));
			IsStopword = other.IsStopword;
		}

		public override Token Clone()
		{
			return new WordToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			WordToken wordToken = other as WordToken;
			if (wordToken == null || other.GetType() != wordToken.GetType())
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
			return GetSimilarity((WordToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
