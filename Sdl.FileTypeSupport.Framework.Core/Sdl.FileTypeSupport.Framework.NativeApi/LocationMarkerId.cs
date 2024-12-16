using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public class LocationMarkerId : ICloneable
	{
		private string _Id;

		public string Id => _Id;

		public LocationMarkerId()
		{
			_Id = Guid.NewGuid().ToString();
		}

		public LocationMarkerId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			LocationMarkerId locationMarkerId = (LocationMarkerId)obj;
			if (locationMarkerId._Id != _Id)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _Id.GetHashCode();
		}

		public object Clone()
		{
			return new LocationMarkerId(_Id);
		}
	}
}
