using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public struct ParagraphUnitId : IComparable<ParagraphUnitId>
	{
		private string _Id;

		public string Id => _Id;

		public ParagraphUnitId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			ParagraphUnitId paragraphUnitId = (ParagraphUnitId)obj;
			if (_Id == null != (paragraphUnitId._Id == null))
			{
				return false;
			}
			if (_Id != null && !_Id.Equals(paragraphUnitId._Id, StringComparison.InvariantCulture))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(ParagraphUnitId first, ParagraphUnitId second)
		{
			if (first._Id == second._Id)
			{
				return true;
			}
			if (first._Id != null && first._Id.Equals(second._Id))
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(ParagraphUnitId first, ParagraphUnitId second)
		{
			if (first._Id == second._Id)
			{
				return false;
			}
			if (first._Id != null && first._Id.Equals(second._Id))
			{
				return false;
			}
			return true;
		}

		public int CompareTo(ParagraphUnitId other)
		{
			if (_Id != null)
			{
				return _Id.CompareTo(other._Id);
			}
			if (other._Id == null)
			{
				return 0;
			}
			return -1;
		}
	}
}
