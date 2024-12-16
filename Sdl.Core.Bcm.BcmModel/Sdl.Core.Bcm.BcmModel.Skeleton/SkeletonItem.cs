using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public abstract class SkeletonItem : MetadataContainer, IEquatable<SkeletonItem>
	{
		[JsonProperty(PropertyName = "id", Order = int.MinValue)]
		public int Id
		{
			get;
			set;
		}

		public bool Equals(SkeletonItem other)
		{
			return Equals((MetadataContainer)other);
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
			return Equals((SkeletonItem)obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
