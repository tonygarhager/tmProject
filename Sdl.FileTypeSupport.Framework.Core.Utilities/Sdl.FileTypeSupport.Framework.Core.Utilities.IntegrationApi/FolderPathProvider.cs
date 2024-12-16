using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.IO;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	public class FolderPathProvider
	{
		private string _OutputFolder;

		private bool _ignoreProposedFilename = true;

		public string OutputFolder
		{
			get
			{
				return _OutputFolder;
			}
			set
			{
				_OutputFolder = value;
			}
		}

		public bool IgnoreProposedFilename
		{
			get
			{
				return _ignoreProposedFilename;
			}
			set
			{
				_ignoreProposedFilename = value;
			}
		}

		public OutputPropertiesProvider Provider => InitializeOutputSettings;

		public FolderPathProvider(string outputFolder)
		{
			_OutputFolder = outputFolder;
		}

		public void InitializeOutputSettings(INativeOutputFileProperties outputProperties, IPersistentFileConversionProperties conversionProperties, IOutputFileInfo suggestedFileInfo)
		{
			string text = suggestedFileInfo?.Filename;
			if (_ignoreProposedFilename || string.IsNullOrEmpty(text))
			{
				text = Path.GetFileName(conversionProperties.OriginalFilePath);
			}
			outputProperties.OutputFilePath = Path.Combine(_OutputFolder, text);
		}
	}
}
