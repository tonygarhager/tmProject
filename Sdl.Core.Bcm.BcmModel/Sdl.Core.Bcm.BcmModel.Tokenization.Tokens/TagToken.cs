using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.Core.Bcm.BcmModel.Tokenization.Tokens
{
	[DataContract]
	public class TagToken : Token
	{
		[DataMember(Name = "type")]
		protected override string Type => "tag";

		[DataMember(Name = "tagType")]
		public TagType TagType
		{
			get;
			set;
		}

		[DataMember(Name = "anchor")]
		public int? Anchor
		{
			get;
			set;
		}

		[DataMember(Name = "alignmentAnchor")]
		public int? AlignmentAnchor
		{
			get;
		}

		[DataMember(Name = "tagId")]
		public string TagId
		{
			get;
			set;
		}

		[DataMember(Name = "textEquivalent")]
		public string TextEquivalent
		{
			get;
			set;
		}

		[DataMember(Name = "canHide")]
		public bool? CanHide
		{
			get;
			set;
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		public TagToken()
		{
		}

		public TagToken(TagToken other)
			: this(other.TagType, other.TagId, other.Anchor.GetValueOrDefault(), other.AlignmentAnchor.GetValueOrDefault(), other.TextEquivalent, other.CanHide.GetValueOrDefault())
		{
		}

		public TagToken(TagType type, string tagId, int anchor)
			: this(type, tagId, anchor, 0, null)
		{
		}

		public TagToken(TagType type, string tagId, int anchor, int alignmentAnchor, string textEquivalent)
			: this(type, tagId, anchor, alignmentAnchor, textEquivalent, canHide: false)
		{
		}

		public TagToken(TagType type, string tagId, int anchor, int alignmentAnchor, string textEquivalent, bool canHide)
		{
			Anchor = anchor;
			AlignmentAnchor = alignmentAnchor;
			TagId = tagId;
			TagType = type;
			CanHide = canHide;
			if (type == TagType.TextPlaceholder || type == TagType.LockedContent)
			{
				TextEquivalent = textEquivalent;
			}
		}

		public override Token Clone()
		{
			return new TagToken(this);
		}

		public override Similarity GetSimilarity(Token other)
		{
			TagToken tagToken = other as TagToken;
			if (tagToken == null || Type != tagToken.Type)
			{
				return Similarity.None;
			}
			if (TagId == null && tagToken.TagId == null)
			{
				if (Anchor != tagToken.Anchor)
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			if (TagId != null && tagToken.TagId != null)
			{
				if (!TagId.Equals(tagToken.TagId, StringComparison.Ordinal))
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			return Similarity.IdenticalType;
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
			TagToken tagToken = (TagToken)obj;
			if (Anchor != tagToken.Anchor || AlignmentAnchor != tagToken.AlignmentAnchor || Type != tagToken.Type || !string.Equals(TagId, tagToken.TagId, StringComparison.Ordinal) || !string.Equals(TextEquivalent, tagToken.TextEquivalent, StringComparison.Ordinal))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return (Anchor.GetValueOrDefault() << 11) ^ (AlignmentAnchor.GetValueOrDefault() << 7) ^ ((int)TagType << 5) ^ (((TagId != null) ? TagId.GetHashCode() : 0) << 3) ^ ((TextEquivalent != null) ? TextEquivalent.GetHashCode() : 0);
		}

		public override int GetWeakHashCode()
		{
			return (int)TagType << 5;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("<");
			if (TagType == TagType.End)
			{
				stringBuilder.Append("/");
			}
			stringBuilder.Append(Anchor?.ToString(CultureInfo.InvariantCulture));
			if (TagType != TagType.End)
			{
				if (AlignmentAnchor != 0)
				{
					stringBuilder.AppendFormat(" x={0}", AlignmentAnchor);
				}
				if (!string.IsNullOrEmpty(TagId))
				{
					stringBuilder.AppendFormat(" id={0}", TagId);
				}
			}
			if (TagType == TagType.Standalone || TagType == TagType.TextPlaceholder || TagType == TagType.LockedContent)
			{
				if (TagType == TagType.TextPlaceholder || TagType == TagType.LockedContent)
				{
					stringBuilder.Append(" text-equiv=\"");
					if (TextEquivalent != null)
					{
						stringBuilder.Append(TextEquivalent);
					}
					stringBuilder.Append("\"");
				}
				stringBuilder.Append("/");
			}
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}

		public string SeriliazedWihoutAnchor()
		{
			StringBuilder stringBuilder = new StringBuilder("<");
			if (TagType == TagType.End)
			{
				stringBuilder.Append("/");
			}
			stringBuilder.Append(Anchor?.ToString(CultureInfo.InvariantCulture));
			if (TagType != TagType.End && !string.IsNullOrEmpty(TagId))
			{
				stringBuilder.AppendFormat(" id={0}", TagId);
			}
			if (TagType == TagType.Standalone || TagType == TagType.TextPlaceholder || TagType == TagType.LockedContent)
			{
				if (TagType == TagType.TextPlaceholder || TagType == TagType.LockedContent)
				{
					stringBuilder.Append(" text-equiv=\"");
					if (TextEquivalent != null)
					{
						stringBuilder.Append(TextEquivalent);
					}
					stringBuilder.Append("\"");
				}
				stringBuilder.Append("/");
			}
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}
	}
}
