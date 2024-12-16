using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class Context : SkeletonItem, ICloneable<Context>, IEquatable<Context>
	{
		[DataMember(Name = "contextDefinitionId")]
		public int ContextDefinitionId
		{
			get;
			set;
		}

		[DataMember(Name = "parentContextId", EmitDefaultValue = false)]
		public int ParentContextId
		{
			get;
			set;
		}

		public Context Clone()
		{
			return (Context)MemberwiseClone();
		}

		public bool Equals(Context other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other) && ContextDefinitionId == other.ContextDefinitionId)
			{
				return ParentContextId == other.ParentContextId;
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
			return Equals((Context)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ContextDefinitionId);
			return (hashCode * 397) ^ ParentContextId;
		}

		public override string ToString()
		{
			return $"id={base.Id} contextDefId={ContextDefinitionId} parentContextId={ParentContextId}";
		}
	}
}
