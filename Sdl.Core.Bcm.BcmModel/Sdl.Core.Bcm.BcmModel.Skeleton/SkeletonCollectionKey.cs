using System;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	public struct SkeletonCollectionKey : IEquatable<SkeletonCollectionKey>
	{
		public int Id
		{
			get;
			set;
		}

		public SkeletonCollectionKey(int id)
		{
			this = default(SkeletonCollectionKey);
			Id = id;
		}

		public static SkeletonCollectionKey From(int from)
		{
			return new SkeletonCollectionKey(from);
		}

		public bool Equals(SkeletonCollectionKey other)
		{
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is SkeletonCollectionKey)
			{
				return Equals((SkeletonCollectionKey)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id;
		}

		public static bool operator ==(SkeletonCollectionKey left, SkeletonCollectionKey right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SkeletonCollectionKey left, SkeletonCollectionKey right)
		{
			return !left.Equals(right);
		}
	}
}
