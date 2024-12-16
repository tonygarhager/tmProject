using System;

namespace Sdl.FileTypeSupport.Framework
{
	[Serializable]
	public struct FileTypeDefinitionId
	{
		private string _Id;

		public string Id => _Id;

		public FileTypeDefinitionId(string id)
		{
			_Id = id;
		}

		public override string ToString()
		{
			return _Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FileTypeDefinitionId))
			{
				return false;
			}
			return ((FileTypeDefinitionId)obj)._Id.Equals(_Id);
		}

		public override int GetHashCode()
		{
			if (_Id == null)
			{
				return 0;
			}
			return _Id.GetHashCode();
		}

		public static bool operator ==(FileTypeDefinitionId first, FileTypeDefinitionId second)
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

		public static bool operator !=(FileTypeDefinitionId first, FileTypeDefinitionId second)
		{
			return !(first == second);
		}
	}
}
