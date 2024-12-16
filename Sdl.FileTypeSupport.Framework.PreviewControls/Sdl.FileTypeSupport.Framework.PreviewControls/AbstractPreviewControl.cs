using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// This is a simple generic externalpreview control that allow the creation of a 
	/// tempoary file for previewing and then launching of the associated or specified application for that file.
	/// Then when this object is disposed or the launched process exits the temp file is deleted.
	/// </summary>
	public abstract class AbstractPreviewControl : AbstractFileTypeDefinitionComponent, IAbstractPreviewControl, IAbstractPreviewController
	{
		/// <summary>
		/// The actual Windows Forms control for the preview display.
		/// See: <see cref="P:Sdl.FileTypeSupport.Framework.IntegrationApi.IAbstractPreviewControl.Control" />.
		/// </summary>
		public abstract Control Control
		{
			get;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public AbstractPreviewControl()
		{
		}

		/// <summary>
		/// Implementation should refresh the content of the control.
		/// </summary>
		public abstract void Refresh();
	}
}
