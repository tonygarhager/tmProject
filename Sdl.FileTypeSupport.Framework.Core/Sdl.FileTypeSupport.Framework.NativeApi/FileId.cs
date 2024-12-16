using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public struct FileId
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

		public FileId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FileId))
			{
				return false;
			}
			return ((FileId)obj)._Id == _Id;
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(FileId first, FileId second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(FileId first, FileId second)
		{
			return !first.Equals(second);
		}
	}
}
