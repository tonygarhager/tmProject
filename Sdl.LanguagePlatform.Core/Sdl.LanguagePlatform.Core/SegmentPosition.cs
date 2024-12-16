using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public class SegmentPosition
	{
		private int _runIndex;

		private int _positionInRun;

		[DataMember]
		public int Index
		{
			get
			{
				return _runIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_runIndex = value;
			}
		}

		[DataMember]
		public int Position
		{
			get
			{
				return _positionInRun;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_positionInRun = value;
			}
		}

		public SegmentPosition()
		{
		}

		public SegmentPosition(int runIndex, int positionInRun)
		{
			_runIndex = runIndex;
			_positionInRun = positionInRun;
		}

		public SegmentPosition Duplicate()
		{
			return new SegmentPosition(_runIndex, _positionInRun);
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
			if (_positionInRun == segmentPosition._positionInRun)
			{
				return _runIndex == segmentPosition._runIndex;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _positionInRun.GetHashCode() ^ _runIndex.GetHashCode();
		}

		public static int Compare(SegmentPosition a, SegmentPosition b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}
			int num = a._runIndex - b._runIndex;
			if (num == 0)
			{
				num = a._positionInRun - b._positionInRun;
			}
			return num;
		}

		public override string ToString()
		{
			return "{" + _runIndex.ToString() + "," + _positionInRun.ToString() + "}";
		}
	}
}
