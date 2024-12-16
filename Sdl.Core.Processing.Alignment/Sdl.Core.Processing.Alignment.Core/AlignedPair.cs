using Sdl.Core.Processing.Alignment.Common;

namespace Sdl.Core.Processing.Alignment.Core
{
	public class AlignedPair<T>
	{
		public T LeftObject
		{
			get;
			private set;
		}

		public T RightObject
		{
			get;
			private set;
		}

		public AlignmentType AlignmentType
		{
			get;
			private set;
		}

		public AlignedPair(T leftObject, T rightObject, AlignmentType alignmentType)
		{
			LeftObject = leftObject;
			RightObject = rightObject;
			AlignmentType = alignmentType;
		}

		public AlignedPair(T leftObject, T rightObject)
			: this(leftObject, rightObject, AlignmentType.Alignment11)
		{
		}

		public override bool Equals(object obj)
		{
			AlignedPair<T> alignedPair = obj as AlignedPair<T>;
			if (alignedPair != null && object.Equals(alignedPair.LeftObject, LeftObject) && object.Equals(alignedPair.RightObject, RightObject))
			{
				return AlignmentType == alignedPair.AlignmentType;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 13;
			if (!object.Equals(LeftObject, default(T)))
			{
				num += 157 * LeftObject.GetHashCode();
			}
			if (!object.Equals(RightObject, default(T)))
			{
				num += 9137 * RightObject.GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			return LeftObject + "|" + RightObject;
		}
	}
}
