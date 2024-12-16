using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class PlaceholderTagDefinition : SkeletonItem, ICloneable<PlaceholderTagDefinition>, IEquatable<PlaceholderTagDefinition>
	{
		[DataMember(Name = "displayText", EmitDefaultValue = false)]
		public string DisplayText
		{
			get;
			set;
		}

		[DataMember(Name = "tagContent", EmitDefaultValue = false)]
		public string TagContent
		{
			get;
			set;
		}

		[DataMember(Name = "textEquivalent", EmitDefaultValue = false)]
		public string TextEquivalent
		{
			get;
			set;
		}

		[DataMember(Name = "segmentationHint", EmitDefaultValue = false)]
		[DefaultValue(SegmentationHint.MayExclude)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public SegmentationHint SegmentationHint
		{
			get;
			set;
		}

		[DataMember(Name = "subContent")]
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

		public PlaceholderTagDefinition()
		{
			SegmentationHint = SegmentationHint.MayExclude;
		}

		public PlaceholderTagDefinition Clone()
		{
			PlaceholderTagDefinition result = (PlaceholderTagDefinition)MemberwiseClone();
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

		public bool Equals(PlaceholderTagDefinition other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other) && string.Equals(DisplayText, other.DisplayText) && string.Equals(TagContent, other.TagContent) && string.Equals(TextEquivalent, other.TextEquivalent) && SegmentationHint == other.SegmentationHint && SubContent.IsSequenceEqual(other.SubContent))
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
			return Equals((PlaceholderTagDefinition)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((DisplayText != null) ? DisplayText.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((TagContent != null) ? TagContent.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((TextEquivalent != null) ? TextEquivalent.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ (int)SegmentationHint);
			hashCode = ((hashCode * 397) ^ ((SubContent != null) ? SubContent.GetHashCode() : 0));
			return (hashCode * 397) ^ ((QuickInsertId != null) ? QuickInsertId.GetHashCode() : 0);
		}
	}
}
