using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public struct PreviewControlId
	{
		private string _Id;

		public string Id => _Id;

		public PreviewControlId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PreviewControlId))
			{
				return false;
			}
			return ((PreviewControlId)obj)._Id.Equals(_Id);
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(PreviewControlId first, PreviewControlId second)
		{
			if (first.Id == second.Id)
			{
				return true;
			}
			if (first.Id == null)
			{
				return false;
			}
			return first._Id.Equals(second._Id);
		}

		public static bool operator !=(PreviewControlId first, PreviewControlId second)
		{
			return !(first == second);
		}
	}
}
