using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public struct GeneratorId
	{
		private string _Id;

		public static GeneratorId Default = new GeneratorId(null);

		public string Id => _Id;

		public GeneratorId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GeneratorId))
			{
				return false;
			}
			return ((GeneratorId)obj)._Id.Equals(_Id);
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(GeneratorId first, GeneratorId second)
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

		public static bool operator !=(GeneratorId first, GeneratorId second)
		{
			return !(first == second);
		}
	}
}
