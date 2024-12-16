using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	public class GenericPlaceableToken : SimpleToken
	{
		[DataMember]
		internal bool _IsSubstitutable;

		[DataMember]
		internal string _TokenClass;

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => _IsSubstitutable;

		public string TokenClass => _TokenClass;

		public GenericPlaceableToken(string text, string tokenClass, bool isSubstitutable)
			: base(text, TokenType.OtherTextPlaceable)
		{
			_TokenClass = tokenClass;
			_IsSubstitutable = isSubstitutable;
		}

		public GenericPlaceableToken(GenericPlaceableToken other)
			: base(other)
		{
			_IsSubstitutable = other._IsSubstitutable;
			_TokenClass = other._TokenClass;
		}

		public override SegmentElement Duplicate()
		{
			return new GenericPlaceableToken(this);
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			Similarity bundleSimilarity = GetBundleSimilarity(other);
			if (other == null || other.GetType() != GetType())
			{
				return bundleSimilarity;
			}
			GenericPlaceableToken genericPlaceableToken = other as GenericPlaceableToken;
			if (genericPlaceableToken == null || base.Type != genericPlaceableToken.Type)
			{
				return Similarity.None;
			}
			if (!string.Equals(_TokenClass, genericPlaceableToken._TokenClass, StringComparison.Ordinal))
			{
				return Similarity.None;
			}
			if (!string.Equals(Text, genericPlaceableToken.Text, StringComparison.Ordinal))
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
			return GetSimilarity((GenericPlaceableToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 31 + base.GetHashCode();
			return num * 31 + _TokenClass.GetHashCode();
		}
	}
}
