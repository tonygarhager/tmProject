using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public struct PreviewApplicationId
	{
		private string _Id;

		public string Id => _Id;

		public PreviewApplicationId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PreviewApplicationId))
			{
				return false;
			}
			return ((PreviewApplicationId)obj)._Id.Equals(_Id);
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(PreviewApplicationId first, PreviewApplicationId second)
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

		public static bool operator !=(PreviewApplicationId first, PreviewApplicationId second)
		{
			return !(first == second);
		}
	}
}
