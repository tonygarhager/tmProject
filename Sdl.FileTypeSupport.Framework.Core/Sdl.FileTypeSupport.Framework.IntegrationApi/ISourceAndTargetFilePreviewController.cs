namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface ISourceAndTargetFilePreviewController
	{
		TempFileManager SourcePreviewFile
		{
			get;
			set;
		}

		TempFileManager TargetPreviewFile
		{
			get;
			set;
		}
	}
}
