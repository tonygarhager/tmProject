using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public struct TagId
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

		public TagId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is TagId))
			{
				return false;
			}
			return ((TagId)obj)._Id == _Id;
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(TagId first, TagId second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(TagId first, TagId second)
		{
			return !first.Equals(second);
		}
	}
}
