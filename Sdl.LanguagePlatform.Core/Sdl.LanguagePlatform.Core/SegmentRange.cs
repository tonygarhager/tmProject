using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public class SegmentRange
	{
		[DataMember]
		public SegmentPosition From
		{
			get;
			set;
		}

		[DataMember]
		public SegmentPosition Into
		{
			get;
			set;
		}

		public int Length
		{
			get
			{
				if (From.Index != Into.Index)
				{
					return -1;
				}
				return Into.Position - From.Position + 1;
			}
		}

		public SegmentRange()
		{
		}

		public SegmentRange(SegmentPosition from, SegmentPosition into)
		{
			From = from;
			Into = into;
		}

		public SegmentRange(int run, int from, int into)
		{
			From = new SegmentPosition(run, from);
			Into = new SegmentPosition(run, into);
		}

		public SegmentRange Duplicate()
		{
			return new SegmentRange(From.Duplicate(), Into.Duplicate());
		}

		public bool OverlapsWith(SegmentRange other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			if (!IsInside(other.From))
			{
				return IsInside(other.Into);
			}
			return true;
		}

		public bool Contains(SegmentRange other)
		{
			if (IsInside(other.From))
			{
				return IsInside(other.Into);
			}
			return false;
		}

		public bool IsInside(SegmentPosition p)
		{
			if (p == null)
			{
				throw new ArgumentNullException();
			}
			if (p.Position >= From.Position && p.Position <= Into.Position && (p.Position != From.Position || p.Index >= From.Index))
			{
				if (p.Position == Into.Position)
				{
					return p.Index <= Into.Position;
				}
				return true;
			}
			return false;
		}

		public static int Compare(SegmentRange a, SegmentRange b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}
			return SegmentPosition.Compare(a.From, b.From);
		}

		public override string ToString()
		{
			return "[" + From?.ToString() + "-" + Into?.ToString() + "]";
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
			int num = 17;
			num = num * 31 + From.GetHashCode();
			return num * 31 + Into.GetHashCode();
		}
	}
}
