using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class StructureTagDefinition : SkeletonItem, ICloneable<StructureTagDefinition>, IEquatable<StructureTagDefinition>
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

		[DataMember(Name = "subContent", EmitDefaultValue = false)]
		public SubContentCollection SubContent
		{
			get;
			set;
		}

		public StructureTagDefinition Clone()
		{
			StructureTagDefinition result = (StructureTagDefinition)MemberwiseClone();
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

		public bool Equals(StructureTagDefinition other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other) && string.Equals(DisplayText, other.DisplayText) && SubContent.IsSequenceEqual(other.SubContent))
			{
				return string.Equals(TagContent, other.TagContent);
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
			return Equals((StructureTagDefinition)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((DisplayText != null) ? DisplayText.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((SubContent != null) ? SubContent.GetHashCode() : 0));
			return (hashCode * 397) ^ ((TagContent != null) ? TagContent.GetHashCode() : 0);
		}
	}
}
