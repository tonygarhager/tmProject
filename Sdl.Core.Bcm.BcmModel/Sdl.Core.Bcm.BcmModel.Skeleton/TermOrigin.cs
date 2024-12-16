using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class TermOrigin : ExtensionDataContainer, ICloneable<TermOrigin>, IEquatable<TermOrigin>
	{
		[DataMember(Name = "systemId")]
		public string SystemId
		{
			get;
			set;
		}

		[DataMember(Name = "systemName")]
		public string SystemName
		{
			get;
			set;
		}

		[DataMember(Name = "resourceId")]
		public string ResourceId
		{
			get;
			set;
		}

		[DataMember(Name = "resourceName")]
		public string ResourceName
		{
			get;
			set;
		}

		public TermOrigin Clone()
		{
			return (TermOrigin)MemberwiseClone();
		}

		public bool Equals(TermOrigin other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (string.Equals(SystemId, other.SystemId) && string.Equals(SystemName, other.SystemName) && string.Equals(ResourceId, other.ResourceId))
			{
				return string.Equals(ResourceName, other.ResourceName);
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
			return Equals((TermOrigin)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((SystemName != null) ? SystemName.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((SystemId != null) ? SystemId.GetHashCode() : 0));
			hashCode = ((hashCode * 397) ^ ((ResourceName != null) ? ResourceName.GetHashCode() : 0));
			return (hashCode * 397) ^ ((ResourceId != null) ? ResourceId.GetHashCode() : 0);
		}
	}
}
