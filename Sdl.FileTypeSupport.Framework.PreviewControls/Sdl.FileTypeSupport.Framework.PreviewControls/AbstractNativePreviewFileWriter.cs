using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// Abstract base class that can be used to build native preview file writers.
	///
	/// This class is derived from <see cref="T:Sdl.FileTypeSupport.Framework.NativeApi.AbstractNativeFileWriter" /> but also adds a property <see cref="P:Sdl.FileTypeSupport.Framework.PreviewControls.AbstractNativePreviewFileWriter.ExtensionMaps" /> that can be set
	/// in the filter definition's preview writer section to automatically map file extensions so that the preview file can be viewed in varions preview applications.
	/// </summary>
	public abstract class AbstractNativePreviewFileWriter : AbstractNativeFileWriter
	{
		private string[] _extensionMaps;

		/// <summary>
		/// A list of filter extensions that need to be mapped to other extensions before previewing.
		/// For example mappping .csv to .csv.txt files so they can be previewed in an IE browser control.
		/// </summary>
		public string[] ExtensionMaps
		{
			get
			{
				return _extensionMaps;
			}
			set
			{
				_extensionMaps = value;
			}
		}

		/// <summary>
		/// Propose a filename based on origianl file path but with the file extension mapped to a new file extension if
		/// a map is found in <see cref="P:Sdl.FileTypeSupport.Framework.PreviewControls.AbstractNativePreviewFileWriter.ExtensionMaps" />.
		/// </summary>
		/// <param name="fileProperties"></param>
		/// <param name="proposedFileInfo"></param>
		/// <returns></returns>
		public override void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
			if (string.IsNullOrEmpty(proposedFileInfo.Filename))
			{
				string modifiedTempFilePath = TempFileUtils.GetModifiedTempFilePath(_extensionMaps, fileProperties.OriginalFilePath);
				if (!string.IsNullOrEmpty(modifiedTempFilePath))
				{
					proposedFileInfo.Filename = modifiedTempFilePath;
				}
			}
		}
	}
}
