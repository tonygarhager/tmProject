using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization
{
	[DataContract]
	public class SegmentRange : ExtensionDataContainer
	{
		[DataMember(Name = "from")]
		public SegmentPosition From
		{
			get;
			set;
		}

		[DataMember(Name = "into")]
		public SegmentPosition Into
		{
			get;
			set;
		}

		public SegmentRange()
		{
			From = new SegmentPosition();
			Into = new SegmentPosition();
		}

		public SegmentRange(SegmentPosition from, SegmentPosition into)
		{
			From = from;
			Into = into;
		}

		public SegmentRange(int runIndex, int fromIndex, int intoIndex)
		{
			From = new SegmentPosition(runIndex, fromIndex);
			Into = new SegmentPosition(runIndex, intoIndex);
		}

		public bool Equals(SegmentRange other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			if (SegmentPosition.Compare(From, other.From) == 0)
			{
				return SegmentPosition.Compare(Into, other.Into) == 0;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			SegmentRange other = obj as SegmentRange;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static int Compare(SegmentRange a, SegmentRange b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}
			return SegmentPosition.Compare(a.From, b.From);
		}

		public SegmentRange Clone()
		{
			return new SegmentRange(From.Clone(), Into.Clone());
		}

		public override string ToString()
		{
			return $"[{From}-{Into}]";
		}
	}
}
