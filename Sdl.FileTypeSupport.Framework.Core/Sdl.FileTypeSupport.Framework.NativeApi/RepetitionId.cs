using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public struct RepetitionId : IComparable
	{
		private string _Id;

		public string Id => _Id;

		public RepetitionId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			RepetitionId repetitionId = (RepetitionId)obj;
			return _Id == repetitionId._Id;
		}

		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(_Id))
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(RepetitionId first, RepetitionId second)
		{
			return first._Id == second._Id;
		}

		public static bool operator !=(RepetitionId first, RepetitionId second)
		{
			return first._Id != second._Id;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is RepetitionId))
			{
				return -1;
			}
			RepetitionId repetitionId = (RepetitionId)obj;
			if (_Id != null)
			{
				return _Id.CompareTo(repetitionId._Id);
			}
			if (repetitionId._Id == null)
			{
				return 0;
			}
			return -1;
		}
	}
}
