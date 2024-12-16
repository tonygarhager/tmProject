namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeOutputSettingsAware
	{
		void SetOutputProperties(INativeOutputFileProperties properties);

		void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo);
	}
}
