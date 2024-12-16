using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	[DataContract]
	public class TerminologyData : SkeletonItem, ICloneable<TerminologyData>, IEquatable<TerminologyData>
	{
		[Obsolete("Will be removed in a future version. Use the TermOrigin instead!")]
		[DataMember(Name = "origin")]
		public string Origin
		{
			get;
			set;
		}

		[DataMember(Name = "terms")]
		public List<Term> Terms
		{
			get;
			set;
		}

		public TerminologyData Clone()
		{
			TerminologyData result = (TerminologyData)MemberwiseClone();
			if (Terms != null)
			{
				result.Terms = new List<Term>();
				Terms.ForEach(delegate(Term x)
				{
					result.Terms.Add(x.Clone());
				});
			}
			return result;
		}

		public bool Equals(TerminologyData other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Equals((SkeletonItem)other) && string.Equals(Origin, other.Origin))
			{
				return Terms.IsSequenceEqual(other.Terms);
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
			return Equals((TerminologyData)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode = ((hashCode * 397) ^ ((Origin != null) ? Origin.GetHashCode() : 0));
			return (hashCode * 397) ^ ((Terms != null) ? Terms.GetHashCode() : 0);
		}
	}
}
