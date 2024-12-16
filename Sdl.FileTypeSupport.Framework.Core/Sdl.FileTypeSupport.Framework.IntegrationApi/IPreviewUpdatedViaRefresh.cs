namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IPreviewUpdatedViaRefresh : IAbstractUpdatablePreview
	{
		TempFileManager TargetFilePath
		{
			get;
			set;
		}

		void BeforeFileRefresh();

		void AfterFileRefresh();
	}
}
