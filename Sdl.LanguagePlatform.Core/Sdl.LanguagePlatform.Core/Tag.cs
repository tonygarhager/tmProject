using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public class Tag : SegmentElement
	{
		[DataMember]
		public TagType Type
		{
			get;
			set;
		}

		[DataMember]
		public int Anchor
		{
			get;
			set;
		}

		[DataMember]
		public int AlignmentAnchor
		{
			get;
			set;
		}

		[DataMember]
		public string TagID
		{
			get;
			set;
		}

		[DataMember]
		public string TextEquivalent
		{
			get;
			set;
		}

		[DataMember]
		public bool CanHide
		{
			get;
			set;
		}

		public Tag()
		{
		}

		public Tag(Tag other)
			: this(other.Type, other.TagID, other.Anchor, other.AlignmentAnchor, other.TextEquivalent, other.CanHide)
		{
		}

		public Tag(TagType type, string tagId, int anchor)
			: this(type, tagId, anchor, 0, null)
		{
		}

		public Tag(TagType type, string tagId, int anchor, int alignmentAnchor, string textEquivalent)
			: this(type, tagId, anchor, alignmentAnchor, textEquivalent, canHide: false)
		{
		}

		public Tag(TagType type, string tagId, int anchor, int alignmentAnchor, string textEquivalent, bool canHide)
		{
			Anchor = anchor;
			AlignmentAnchor = alignmentAnchor;
			TagID = tagId;
			Type = type;
			CanHide = canHide;
			if (type == TagType.TextPlaceholder || type == TagType.LockedContent)
			{
				TextEquivalent = textEquivalent;
			}
		}

		public override SegmentElement Duplicate()
		{
			return new Tag(this);
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			Tag tag = other as Tag;
			if (tag == null || Type != tag.Type)
			{
				return Similarity.None;
			}
			if (TagID == null && tag.TagID == null)
			{
				if (Anchor != tag.Anchor)
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			if (TagID != null && tag.TagID != null)
			{
				if (!TagID.Equals(tag.TagID, StringComparison.Ordinal))
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			return Similarity.IdenticalType;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("<");
			if (Type == TagType.End)
			{
				stringBuilder.Append("/");
			}
			stringBuilder.Append(Anchor.ToString(CultureInfo.InvariantCulture));
			if (Type != TagType.End)
			{
				if (AlignmentAnchor != 0)
				{
					stringBuilder.AppendFormat(" x={0}", AlignmentAnchor);
				}
				if (!string.IsNullOrEmpty(TagID))
				{
					stringBuilder.AppendFormat(" id={0}", TagID);
				}
			}
			if (Type == TagType.Standalone || Type == TagType.TextPlaceholder || Type == TagType.LockedContent)
			{
				if (Type == TagType.TextPlaceholder || Type == TagType.LockedContent)
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
			if (Type == TagType.End)
			{
				stringBuilder.Append("/");
			}
			stringBuilder.Append(Anchor.ToString(CultureInfo.InvariantCulture));
			if (Type != TagType.End && !string.IsNullOrEmpty(TagID))
			{
				stringBuilder.AppendFormat(" id={0}", TagID);
			}
			if (Type == TagType.Standalone || Type == TagType.TextPlaceholder || Type == TagType.LockedContent)
			{
				if (Type == TagType.TextPlaceholder || Type == TagType.LockedContent)
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
			Tag tag = (Tag)obj;
			if (Anchor == tag.Anchor && AlignmentAnchor == tag.AlignmentAnchor && Type == tag.Type && string.Equals(TagID, tag.TagID, StringComparison.Ordinal))
			{
				return string.Equals(TextEquivalent, tag.TextEquivalent, StringComparison.Ordinal);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (Anchor << 11) ^ (AlignmentAnchor << 7) ^ ((int)Type << 5) ^ (((TagID != null) ? TagID.GetHashCode() : 0) << 3) ^ ((TextEquivalent != null) ? TextEquivalent.GetHashCode() : 0);
		}

		public override int GetWeakHashCode()
		{
			return (int)Type << 5;
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException();
			}
			visitor.VisitTag(this);
		}
	}
}
