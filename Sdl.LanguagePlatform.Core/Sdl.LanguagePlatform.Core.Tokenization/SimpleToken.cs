using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	public class SimpleToken : Token
	{
		private TokenType _type;

		public override bool IsPlaceable
		{
			get
			{
				if (_type != TokenType.Acronym && _type != TokenType.Variable && _type != TokenType.Uri && _type != TokenType.AlphaNumeric)
				{
					return _type == TokenType.OtherTextPlaceable;
				}
				return true;
			}
		}

		public override bool IsSubstitutable
		{
			get
			{
				if (base.Type != TokenType.Acronym && _type != TokenType.Variable)
				{
					return _type == TokenType.AlphaNumeric;
				}
				return true;
			}
		}

		[DataMember]
		public string Stem
		{
			get;
			set;
		}

		public bool IsStopword
		{
			get;
			set;
		}

		public SimpleToken()
		{
		}

		public SimpleToken(string text)
			: this(text, TokenType.Word)
		{
		}

		public SimpleToken(SimpleToken other)
			: base(other)
		{
			Stem = ((other.Stem == null) ? null : string.Copy(other.Stem));
			_type = other._type;
			IsStopword = other.IsStopword;
		}

		public SimpleToken(string text, TokenType t)
			: base(text)
		{
			Text = text;
			_type = t;
			IsStopword = false;
		}

		public override SegmentElement Duplicate()
		{
			return new SimpleToken(this);
		}

		protected override void SetTokenType(TokenType tokenType)
		{
			_type = tokenType;
		}

		protected override TokenType GetTokenType()
		{
			return _type;
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			Similarity bundleSimilarity = GetBundleSimilarity(other);
			if (other == null || other.GetType() != GetType())
			{
				return bundleSimilarity;
			}
			SimpleToken simpleToken = other as SimpleToken;
			if (simpleToken == null || _type != simpleToken._type)
			{
				return Similarity.None;
			}
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			bool flag = compareInfo.Compare(Text, simpleToken.Text, CompareOptions.IgnoreWidth) == 0;
			if (_type == TokenType.Variable || _type == TokenType.Acronym || _type == TokenType.OtherTextPlaceable || _type == TokenType.AlphaNumeric)
			{
				if (!flag)
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			if (!flag)
			{
				return Similarity.None;
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
			return GetSimilarity((SimpleToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return Text;
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException();
			}
			visitor.VisitSimpleToken(this);
		}
	}
}
