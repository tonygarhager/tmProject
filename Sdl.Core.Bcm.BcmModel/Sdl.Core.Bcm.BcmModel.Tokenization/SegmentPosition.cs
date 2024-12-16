using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel.Tokenization
{
	[DataContract]
	public class SegmentPosition : ExtensionDataContainer
	{
		[DataMember(Name = "runIndex")]
		public int RunIndex
		{
			get;
			set;
		}

		[DataMember(Name = "positionInRun")]
		public int PositionInRun
		{
			get;
			set;
		}

		public SegmentPosition()
		{
		}

		public SegmentPosition(int runIndex, int positionInRun)
		{
			RunIndex = runIndex;
			PositionInRun = positionInRun;
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
			SegmentPosition segmentPosition = (SegmentPosition)obj;
			if (PositionInRun == segmentPosition.PositionInRun)
			{
				return RunIndex == segmentPosition.RunIndex;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return PositionInRun.GetHashCode() ^ RunIndex.GetHashCode();
		}

		public static int Compare(SegmentPosition a, SegmentPosition b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}
			int num = a.RunIndex - b.RunIndex;
			if (num == 0)
			{
				num = a.PositionInRun - b.PositionInRun;
			}
			return num;
		}

		public SegmentPosition Clone()
		{
			return new SegmentPosition(RunIndex, PositionInRun);
		}

		public override string ToString()
		{
			return $"{{{RunIndex},{PositionInRun}}}";
		}
	}
}
