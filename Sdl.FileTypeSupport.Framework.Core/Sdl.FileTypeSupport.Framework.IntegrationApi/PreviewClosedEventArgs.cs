using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class PreviewClosedEventArgs : EventArgs
	{
		private IAbstractPreviewApplication _application;

		public IAbstractPreviewApplication Application
		{
			get
			{
				return _application;
			}
			set
			{
				_application = value;
			}
		}

		public PreviewClosedEventArgs(IAbstractPreviewApplication application)
		{
			_application = application;
		}
	}
}
