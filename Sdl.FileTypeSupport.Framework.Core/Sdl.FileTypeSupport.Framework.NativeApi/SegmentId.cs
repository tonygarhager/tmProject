using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public struct SegmentId
	{
		private string _Id;

		public string Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		public SegmentId(string id)
		{
			_Id = id;
		}

		public SegmentId(long number)
		{
			_Id = number.ToString();
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is SegmentId)
			{
				SegmentId segmentId = (SegmentId)obj;
				return _Id == segmentId._Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(SegmentId first, SegmentId second)
		{
			return first._Id == second._Id;
		}

		public static bool operator !=(SegmentId first, SegmentId second)
		{
			return first._Id != second._Id;
		}
	}
}
