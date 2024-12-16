using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class ContextDefinition : SkeletonItem, ICloneable<ContextDefinition>, IEquatable<ContextDefinition>
	{
		[DataMember(Name = "isTmContext", EmitDefaultValue = false)]
		public bool IsTmContext
		{
			get;
			set;
		}

		[DataMember(Name = "isStructureContext", EmitDefaultValue = false)]
		public bool IsStructureContext
		{
			get;
			set;
		}

		[DataMember(Name = "typeId", EmitDefaultValue = false)]
		public string TypeId
		{
			get;
			set;
		}

		[DataMember(Name = "displayName", EmitDefaultValue = false)]
		public string DisplayName
		{
			get;
			set;
		}

		[DataMember(Name = "displayCode", EmitDefaultValue = false)]
		public string DisplayCode
		{
			get;
			set;
		}

		[DataMember(Name = "displayColor", EmitDefaultValue = false)]
		public string DisplayColor
		{
			get;
			set;
		}

		[DataMember(Name = "description", EmitDefaultValue = false)]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Name = "formattingGroupId", EmitDefaultValue = false)]
		public int FormattingGroupId
		{
			get;
			set;
		}

		public bool IsSidContext => TypeId == "sdl:sid";

		public bool IsTmStructureContext
		{
			get
			{
				if (IsTmContext)
				{
					return !IsSidContext;
				}
				return false;
			}
		}

		public ContextDefinition Clone()
		{
			return (ContextDefinition)MemberwiseClone();
		}

		public override string ToString()
		{
			return $"ContextDefinition Id:{base.Id} Description:{Description}";
		}

		public bool Equals(ContextDefinition other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other) && IsTmContext.Equals(other.IsTmContext) && IsStructureContext.Equals(other.IsStructureContext) && string.Equals(TypeId, other.TypeId) && string.Equals(DisplayName, other.DisplayName) && string.Equals(DisplayCode, other.DisplayCode) && string.Equals(DisplayColor, other.DisplayColor) && string.Equals(Description, other.Description))
			{
				return FormattingGroupId == other.FormattingGroupId;
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
			return Equals((ContextDefinition)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ IsTmContext.GetHashCode());
			hashCode = ((hashCode * 397) ^ IsStructureContext.GetHashCode());
			hashCode = ((hashCode * 397) ^ ((TypeId != null) ? TypeId.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((DisplayName != null) ? DisplayName.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((DisplayCode != null) ? DisplayCode.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((DisplayColor != null) ? DisplayColor.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((Description != null) ? Description.GetHashCode() : 0));
			return (hashCode * 397) ^ FormattingGroupId;
		}
	}
}
