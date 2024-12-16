using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class TagPairDefinition : SkeletonItem, ICloneable<TagPairDefinition>, IEquatable<TagPairDefinition>
	{
		[DataMember(Name = "startTagDisplayText", EmitDefaultValue = false)]
		public string StartTagDisplayText
		{
			get;
			set;
		}

		[DataMember(Name = "startTagContent", EmitDefaultValue = false)]
		public string StartTagContent
		{
			get;
			set;
		}

		[DataMember(Name = "endTagDisplayText", EmitDefaultValue = false)]
		public string EndTagDisplayText
		{
			get;
			set;
		}

		[DataMember(Name = "endTagContent", EmitDefaultValue = false)]
		public string EndTagContent
		{
			get;
			set;
		}

		[DefaultValue(false)]
		[DataMember(Name = "canHide")]
		public bool CanHide
		{
			get;
			set;
		}

		[DataMember(Name = "segmentationHint", EmitDefaultValue = false)]
		[DefaultValue(SegmentationHint.MayExclude)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
		public SegmentationHint SegmentationHint
		{
			get;
			set;
		}

		[DataMember(Name = "formattingGroupId")]
		public int FormattingGroupId
		{
			get;
			set;
		}

		[DataMember(Name = "subContent", EmitDefaultValue = false)]
		public SubContentCollection SubContent
		{
			get;
			set;
		}

		[DataMember(Name = "quickInsertId", EmitDefaultValue = false)]
		public string QuickInsertId
		{
			get;
			set;
		}

		public TagPairDefinition()
		{
			CanHide = false;
			SegmentationHint = SegmentationHint.MayExclude;
		}

		public TagPairDefinition Clone()
		{
			TagPairDefinition result = (TagPairDefinition)MemberwiseClone();
			result.ReplaceMetadataWith(base.Metadata);
			if (SubContent != null)
			{
				result.SubContent = new SubContentCollection();
				SubContent.ForEach(delegate(LocalizableSubContent x)
				{
					result.SubContent.Add(x.Clone());
				});
			}
			return result;
		}

		public bool Equals(TagPairDefinition other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other) && string.Equals(StartTagDisplayText, other.StartTagDisplayText) && string.Equals(StartTagContent, other.StartTagContent) && string.Equals(EndTagDisplayText, other.EndTagDisplayText) && string.Equals(EndTagContent, other.EndTagContent) && CanHide.Equals(other.CanHide) && SegmentationHint == other.SegmentationHint && FormattingGroupId == other.FormattingGroupId && SubContent.IsSequenceEqual(other.SubContent))
			{
				return string.Equals(QuickInsertId, other.QuickInsertId);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((TagPairDefinition)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((StartTagDisplayText != null) ? StartTagDisplayText.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((StartTagContent != null) ? StartTagContent.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((EndTagDisplayText != null) ? EndTagDisplayText.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((EndTagContent != null) ? EndTagContent.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ CanHide.GetHashCode());
			hashCode = ((hashCode * 397) ^ (int)SegmentationHint);
			hashCode = ((hashCode * 397) ^ FormattingGroupId);
			hashCode = ((hashCode * 397) ^ ((SubContent != null) ? SubContent.GetHashCode() : 0));
			return (hashCode * 397) ^ ((QuickInsertId != null) ? QuickInsertId.GetHashCode() : 0);
		}
	}
}
