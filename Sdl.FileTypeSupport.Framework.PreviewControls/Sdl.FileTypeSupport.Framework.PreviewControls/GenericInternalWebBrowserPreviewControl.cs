using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.IO;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// This class can be used to preview a single source or target file in a WebBrowser control.
	/// <remarks>
	/// </remarks>
	/// The GenericSideBySideWebBrowserPreviewControl can be used to preview the source and target file in two System.Windows.Forms.WebBrowser controls.
	/// </summary>
	public class GenericInternalWebBrowserPreviewControl : AbstractPreviewControl, ISingleFilePreviewControl, IAbstractPreviewControl, IAbstractPreviewController, ISingleFilePreviewController, IDisposable
	{
		private bool disposed;

		private WebBrowser _browser;

		private string _initialHtmlPage;

		private TempFileManager _initialHtmlFile;

		private TempFileManager _previewFile;

		/// <summary>
		/// The actual Windows Forms control for the preview display.
		/// See: <see cref="P:Sdl.FileTypeSupport.Framework.IntegrationApi.IAbstractPreviewControl.Control" />.
		/// </summary>
		public override Control Control => _browser;

		/// <summary>
		/// If set then an initial HTML page will be displayed containg
		/// the content of this string in a tempoary .htm file.
		/// </summary>
		public string InitialHtmlPage
		{
			get
			{
				return _initialHtmlPage;
			}
			set
			{
				_initialHtmlPage = value;
				if (!string.IsNullOrEmpty(_initialHtmlPage))
				{
					_initialHtmlFile = new TempFileManager("InitialHtmlPage.htm");
					_initialHtmlFile.DeleteDirectoryIfEmpty = true;
					_initialHtmlFile.Locked = true;
					StreamWriter streamWriter = new StreamWriter(_initialHtmlFile.LockedFileStream);
					streamWriter.Write(_initialHtmlPage);
					streamWriter.Close();
					_browser.Navigate("file://" + _initialHtmlFile.FilePath);
				}
			}
		}

		/// <summary>
		/// Default implementation is accessor for member field.
		/// </summary>
		public TempFileManager PreviewFile
		{
			get
			{
				return _previewFile;
			}
			set
			{
				_previewFile = value;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public GenericInternalWebBrowserPreviewControl()
		{
			_browser = new WebBrowser();
			_browser.AllowWebBrowserDrop = false;
			_browser.IsWebBrowserContextMenuEnabled = false;
		}

		/// <summary>
		/// Used for a standard implementation of IDisposable.
		/// </summary>
		~GenericInternalWebBrowserPreviewControl()
		{
			Dispose(disposing: false);
		}

		/// <summary>
		/// Called to initially display or refresh the display of a single page in a System.Windows.Forms.WebBrowser control.
		/// If both source and target files exist then only the target file will be displayed.
		/// <remarks>The GenericSideBySideWebBrowserPreviewControl can be used to display both source and target files in two WebBrowser controls.</remarks>
		/// </summary>
		public override void Refresh()
		{
			if (_initialHtmlFile != null)
			{
				_initialHtmlFile.Dispose();
				_initialHtmlFile = null;
			}
			if (_previewFile != null && !string.IsNullOrEmpty(_previewFile.FilePath) && File.Exists(_previewFile.FilePath))
			{
				_browser.Navigate("file://" + _previewFile.FilePath);
			}
		}

		/// <summary>
		/// Implementation of the recommended dispose protocol.
		/// Deletes the managed WebBrowser control
		/// </summary>
		/// <param name="disposing">true if this method is called from IDisposable.Dispose() and false if called from Finalizer</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{
				if (_initialHtmlFile != null)
				{
					_initialHtmlFile.Dispose();
				}
				if (_browser != null)
				{
					_browser.Dispose();
				}
			}
			disposed = true;
		}

		/// <summary>
		/// Standard implementation of IDisposable, calls Dispose(true).
		/// </summary>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
