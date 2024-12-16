using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// A side by side realtime web browser preview controller
	/// </summary>
	public class SideBySideRealtimeWebBrowserPreviewController : SideBySideNavigableWebBrowserPreviewController, IPreviewUpdatedViaRefresh, IAbstractUpdatablePreview
	{
		/// <summary>
		/// The preview file TargetPath
		/// </summary>
		public TempFileManager TargetFilePath
		{
			get
			{
				return base.TargetPreviewFile;
			}
			set
			{
				base.TargetPreviewFile = value;
			}
		}

		/// <summary>
		/// Called after the preview file has been refreshed
		/// </summary>
		public void AfterFileRefresh()
		{
			RefreshTarget();
		}

		/// <summary>
		/// Called before the preview file has been refreshed
		/// </summary>
		public void BeforeFileRefresh()
		{
		}
	}
}
