using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IAbstractPreviewControl : IAbstractPreviewController
	{
		Control Control
		{
			get;
		}

		void Refresh();
	}
}
