using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Serialization;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	[JsonConverter(typeof(TokenCreator))]
	public abstract class Token : ExtensionDataContainer, ICloneable<Token>, IEquatable<Token>
	{
		[DataMember(Name = "text")]
		public string Text
		{
			get;
			set;
		}

		[DataMember(Name = "span")]
		public SegmentRange Span
		{
			get;
			set;
		}

		public virtual bool IsPlaceable => false;

		public virtual bool IsSubstitutable => false;

		[DataMember(Name = "type")]
		protected abstract string Type
		{
			get;
		}

		protected Token()
		{
			Text = null;
		}

		protected Token(Token other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Text = ((other.Text == null) ? null : string.Copy(other.Text));
			Span = other.Span?.Clone();
		}

		protected Token(string text)
		{
			Text = text;
		}

		public virtual Token Clone()
		{
			return (Token)MemberwiseClone();
		}

		public virtual bool Equals(Token other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (string.Equals(Text, other.Text, StringComparison.Ordinal))
			{
				return Span.Equals(other.Span);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual int GetWeakHashCode()
		{
			return GetHashCode();
		}

		public override string ToString()
		{
			return Text;
		}

		public virtual Similarity GetSimilarity(Token other)
		{
			if (other == null)
			{
				return Similarity.None;
			}
			CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			if (compareInfo.Compare(Text, other.Text, CompareOptions.IgnoreWidth) != 0)
			{
				return Similarity.None;
			}
			return Similarity.IdenticalValueAndType;
		}
	}
}
