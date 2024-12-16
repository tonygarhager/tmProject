using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	public class FilePathProvider
	{
		private List<string> _FilePaths = new List<string>();

		private int _NextFilePath;

		public List<string> FilePaths
		{
			get
			{
				return _FilePaths;
			}
			set
			{
				_FilePaths = value;
			}
		}

		public OutputPropertiesProvider Provider => InitializeOutputSettings;

		public FilePathProvider(params string[] filePaths)
		{
			_FilePaths.AddRange(filePaths);
		}

		public void InitializeOutputSettings(INativeOutputFileProperties outputProperties, IPersistentFileConversionProperties conversionProperties, IOutputFileInfo suggestedFileInfo)
		{
			if (_NextFilePath >= _FilePaths.Count)
			{
				throw new FileTypeSupportException(string.Format(CultureInfo.CurrentCulture, "Only {0} filename(s) were specified, no file name available for file number {1}", _FilePaths.Count, _NextFilePath));
			}
			outputProperties.OutputFilePath = _FilePaths[_NextFilePath++];
		}
	}
}
