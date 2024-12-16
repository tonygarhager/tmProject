using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.IO;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// This class is used to preview the source and target file in two System.Windows.Forms.WebBrowser controls.
	/// <remarks>The GenericInternalWebBrowserPreviewControl can be used to preview a single source or target file in a WebBrowser control.</remarks>
	/// </summary>
	public class GenericSideBySideWebBrowserPreviewControl : AbstractPreviewControl, ISourceAndTargetFilePreviewControl, IAbstractPreviewControl, IAbstractPreviewController, ISourceAndTargetFilePreviewController, IDisposable
	{
		private bool disposed;

		private DualBrowserControl _dualBrowserControl;

		private WebBrowser _sourceBrowser;

		private WebBrowser _targetBrowser;

		private string _initialHtmlPage;

		private TempFileManager _initialHtmlFile;

		private TempFileManager _sourcePreviewFile;

		private TempFileManager _targetPreviewFile;

		/// <summary>
		/// The actual Windows Forms control for the preview display.
		/// See: <see cref="P:Sdl.FileTypeSupport.Framework.IntegrationApi.IAbstractPreviewControl.Control" />.
		/// </summary>
		public override Control Control => _dualBrowserControl;

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
					_sourceBrowser.Navigate("file://" + _initialHtmlFile.FilePath);
					_targetBrowser.Navigate("file://" + _initialHtmlFile.FilePath);
				}
			}
		}

		/// <summary>
		/// Default implementation is accessor for member field.
		/// </summary>
		public TempFileManager SourcePreviewFile
		{
			get
			{
				return _sourcePreviewFile;
			}
			set
			{
				_sourcePreviewFile = value;
			}
		}

		/// <summary>
		/// Default implementation is accessor for member field.
		/// </summary>
		public TempFileManager TargetPreviewFile
		{
			get
			{
				return _targetPreviewFile;
			}
			set
			{
				_targetPreviewFile = value;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public GenericSideBySideWebBrowserPreviewControl()
		{
			_dualBrowserControl = new DualBrowserControl();
			_sourceBrowser = _dualBrowserControl.WebBrowserSrc;
			_targetBrowser = _dualBrowserControl.WebBrowserTrg;
		}

		/// <summary>
		/// Used for a standard implementation of IDisposable.
		/// </summary>
		~GenericSideBySideWebBrowserPreviewControl()
		{
			Dispose(disposing: false);
		}

		/// <summary>
		/// Called to initially display or refresh the display of a the source and target pages in two System.Windows.Forms.WebBrowser controls.
		/// If either the source and target files do not exist then the InitialHtmlPage page will be displayed.
		/// <remarks>The GenericInternalWebBrowserPreviewControl can be used to display a single source or target file in a WebBrowser control.</remarks>
		/// </summary>
		public override void Refresh()
		{
			NavigateToFile(_sourcePreviewFile, _sourceBrowser);
			NavigateToFile(_targetPreviewFile, _targetBrowser);
		}

		private void NavigateToFile(TempFileManager previewFile, WebBrowser browser)
		{
			if (previewFile != null && !string.IsNullOrEmpty(previewFile.FilePath) && File.Exists(previewFile.FilePath))
			{
				browser.Navigate("file://" + previewFile.FilePath);
			}
			else if (_initialHtmlFile != null && !string.IsNullOrEmpty(_initialHtmlFile.FilePath) && File.Exists(_initialHtmlFile.FilePath))
			{
				browser.Navigate("file://" + _initialHtmlFile.FilePath);
			}
			else
			{
				browser.Navigate("");
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
				if (_sourceBrowser != null)
				{
					_sourceBrowser.Dispose();
				}
				if (_targetBrowser != null)
				{
					_targetBrowser.Dispose();
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
