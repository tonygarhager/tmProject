using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IAbstractPreviewApplication : IAbstractPreviewController
	{
		event EventHandler<PreviewClosedEventArgs> PreviewClosed;

		void Launch();

		void NotifyCanClose();
	}
}
