using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface ISourceAndTargetFileRefreshablePreview : ISourceAndTargetFilePreviewControl, IAbstractPreviewControl, IAbstractPreviewController, ISourceAndTargetFilePreviewController, INavigablePreview, IDisposable, IAbstractUpdatablePreview, IPreviewUpdatedViaRefresh
	{
		bool RefreshPreview
		{
			get;
			set;
		}
	}
}
