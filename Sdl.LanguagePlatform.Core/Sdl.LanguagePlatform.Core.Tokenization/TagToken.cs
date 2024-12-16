using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	public class TagToken : Token
	{
		[DataMember]
		public Tag Tag
		{
			get;
			set;
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		public TagToken()
		{
		}

		public TagToken(Tag tag)
			: base(tag.ToString())
		{
			Tag = (tag ?? throw new ArgumentNullException());
		}

		public TagToken(TagToken other)
			: base(other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Tag = ((other.Tag == null) ? null : new Tag(other.Tag));
		}

		public override SegmentElement Duplicate()
		{
			return new TagToken(this);
		}

		public void UpdateValue(TagToken blueprint)
		{
			UpdateValue(blueprint, updateValuesOnly: false);
		}

		public void UpdateValue(TagToken blueprint, bool updateValuesOnly)
		{
			if (blueprint.Tag == null)
			{
				Tag = null;
				Text = null;
			}
			else if (updateValuesOnly)
			{
				Tag.TagID = blueprint.Tag.TagID;
				Tag.TextEquivalent = blueprint.Tag.TextEquivalent;
				Text = Tag.ToString();
			}
			else
			{
				Tag = (Tag)blueprint.Tag.Duplicate();
				Text = Tag.ToString();
			}
		}

		protected override TokenType GetTokenType()
		{
			return TokenType.Tag;
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			TagToken tagToken = other as TagToken;
			if (tagToken == null || Tag == null || tagToken.Tag == null)
			{
				return Similarity.None;
			}
			return Tag.GetSimilarity(tagToken.Tag);
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
			if (!base.Equals(obj))
			{
				return false;
			}
			TagToken tagToken = (TagToken)obj;
			if (Tag == null && tagToken.Tag == null)
			{
				return true;
			}
			if (Tag == null || tagToken.Tag == null)
			{
				return false;
			}
			return Tag.Equals(tagToken.Tag);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			if (Tag != null)
			{
				return Tag.ToString();
			}
			return "(null)";
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException();
			}
			visitor.VisitTagToken(this);
		}
	}
}
