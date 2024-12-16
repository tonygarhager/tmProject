namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public class DeserializeFileInfo
	{
		private bool _isFileCreated;

		private bool _isDirectoryCreated;

		private string _fullPath;

		public bool IsFileCreated => _isFileCreated;

		public bool IsDirectoryCreated => _isDirectoryCreated;

		public string FullPath => _fullPath;

		public DeserializeFileInfo(bool isFileCreated, bool isDirectoryCreated, string fullPath)
		{
			_isFileCreated = isFileCreated;
			_isDirectoryCreated = isDirectoryCreated;
			_fullPath = fullPath;
		}
	}
}
