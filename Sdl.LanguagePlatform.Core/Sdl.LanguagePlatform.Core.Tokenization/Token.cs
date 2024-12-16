using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	[KnownType(typeof(DateTimeToken))]
	[KnownType(typeof(NumberToken))]
	[KnownType(typeof(MeasureToken))]
	[KnownType(typeof(SimpleToken))]
	[KnownType(typeof(GenericPlaceableToken))]
	[KnownType(typeof(TagToken))]
	public abstract class Token : SegmentElement
	{
		private string _text;

		private SegmentRange _span;

		[XmlIgnore]
		public TokenizationContext TokenizationContext
		{
			get;
			set;
		}

		[DataMember]
		public virtual string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		[DataMember]
		public SegmentRange Span
		{
			get
			{
				return _span;
			}
			set
			{
				_span = value;
			}
		}

		public virtual bool IsPlaceable => false;

		public virtual bool IsSubstitutable => false;

		[DataMember]
		public TokenType Type
		{
			get
			{
				return GetTokenType();
			}
			set
			{
				SetTokenType(value);
			}
		}

		public bool IsWord
		{
			get
			{
				TokenType type = Type;
				if (type != TokenType.Word && type != TokenType.Abbreviation && type != TokenType.Acronym && type != TokenType.Uri)
				{
					return type == TokenType.OtherTextPlaceable;
				}
				return true;
			}
		}

		public bool IsPunctuation
		{
			get
			{
				TokenType type = Type;
				if (type != TokenType.GeneralPunctuation && type != TokenType.OpeningPunctuation)
				{
					return type == TokenType.ClosingPunctuation;
				}
				return true;
			}
		}

		public bool IsWhitespace
		{
			get
			{
				TokenType type = Type;
				return type == TokenType.Whitespace;
			}
		}

		[XmlIgnore]
		public CultureInfo Culture
		{
			get;
			set;
		}

		public Token()
		{
			_text = null;
		}

		public Token(Token other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			_text = ((other._text == null) ? null : string.Copy(other._text));
			_span = other._span?.Duplicate();
			Culture = other.Culture;
		}

		public Token(string text)
		{
			_text = text;
		}

		protected abstract TokenType GetTokenType();

		protected virtual void SetTokenType(TokenType tokenType)
		{
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
			Token token = (Token)obj;
			if (string.Equals(_text, token._text, StringComparison.Ordinal))
			{
				return _span.Equals(token._span);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 31 + _text.GetHashCode();
			return num * 31 + _span.GetHashCode();
		}

		public override int GetWeakHashCode()
		{
			return GetHashCode();
		}

		protected Similarity GetBundleSimilarity(SegmentElement other)
		{
			return (other as TokenBundle)?.GetSimilarity(this) ?? Similarity.None;
		}
	}
}
