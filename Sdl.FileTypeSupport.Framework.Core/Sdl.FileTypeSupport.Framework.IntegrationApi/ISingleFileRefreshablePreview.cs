using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface ISingleFileRefreshablePreview : ISingleFilePreviewControl, IAbstractPreviewControl, IAbstractPreviewController, ISingleFilePreviewController, INavigablePreview, IDisposable, IFileTypeDefinitionAware, IPreviewUpdatedViaSegmentFile, IAbstractUpdatablePreview, IPreviewUpdatedViaRefresh
	{
		bool RefreshPreview
		{
			get;
			set;
		}
	}
}
