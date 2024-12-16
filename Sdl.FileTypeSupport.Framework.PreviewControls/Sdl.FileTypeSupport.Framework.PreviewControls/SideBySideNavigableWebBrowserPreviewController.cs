using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// A side by side navigable web browser preview controller
	/// </summary>
	public class SideBySideNavigableWebBrowserPreviewController : AbstractPreviewControl, ISourceAndTargetFilePreviewControl, IAbstractPreviewControl, IAbstractPreviewController, ISourceAndTargetFilePreviewController, INavigablePreview, IDisposable
	{
		private bool disposed;

		private NavigableDualBrowserControl _navigableDualBrowserControl;

		private WebBrowser _sourceBrowser;

		private WebBrowser _targetBrowser;

		private string _initialHtmlPage;

		private TempFileManager _initialHtmlFile;

		private TempFileManager _sourcePreviewFile;

		private TempFileManager _targetPreviewFile;

		private Color _preferredHighlightColor = Color.Yellow;

		private FileId _fileId;

		/// <summary>
		/// Property to access the Preview Control
		/// </summary>
		public override Control Control => _navigableDualBrowserControl;

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
		/// Property for getting/setting preferred highlight color
		/// </summary>
		public Color PreferredHighlightColor
		{
			get
			{
				return _preferredHighlightColor;
			}
			set
			{
				_preferredHighlightColor = value;
				_navigableDualBrowserControl.SetBestMatchingHighlightColor(value);
			}
		}

		/// <summary>
		/// Segment selected event
		/// </summary>
		public event EventHandler<SegmentSelectedEventArgs> SegmentSelected;

		/// <summary>
		/// SideBySideNavigableWebBroserPreviewController constructor
		/// </summary>
		public SideBySideNavigableWebBrowserPreviewController()
		{
			_navigableDualBrowserControl = new NavigableDualBrowserControl();
			_sourceBrowser = _navigableDualBrowserControl.WebBrowserSrc;
			_targetBrowser = _navigableDualBrowserControl.WebBrowserTrg;
			_navigableDualBrowserControl.WindowSelectionChanged += _navigableDualBrowserControl_WindowSelectionChanged;
		}

		/// <summary>
		/// SideBySideNavigableWebBrowserPreviewController destructor
		/// </summary>
		~SideBySideNavigableWebBrowserPreviewController()
		{
			Dispose(disposing: false);
		}

		/// <summary>
		/// Handler for the WindowSelectionChanged event from the word preview control,
		/// raises the corresponding event on the INavigablePreview interface.
		/// </summary>
		/// <param name="component"></param>
		private void _navigableDualBrowserControl_WindowSelectionChanged(IInteractivePreviewComponent component)
		{
			SegmentReference selectedSegment = _navigableDualBrowserControl.GetSelectedSegment();
			SegmentReference selectedSegment2 = new SegmentReference(_fileId, selectedSegment.ParagraphUnitId, selectedSegment.SegmentId);
			OnSegmentSelected(this, new SegmentSelectedEventArgs(this, selectedSegment2));
		}

		/// <summary>
		/// Raise the <see cref="E:Sdl.FileTypeSupport.Framework.PreviewControls.SideBySideNavigableWebBrowserPreviewController.SegmentSelected" /> event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public virtual void OnSegmentSelected(object sender, SegmentSelectedEventArgs args)
		{
			if (this.SegmentSelected != null)
			{
				this.SegmentSelected(sender, args);
			}
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

		/// <summary>
		/// Called to refresh the Target portion of the control
		/// </summary>
		protected void RefreshTarget()
		{
			_navigableDualBrowserControl.RefreshTargetBrowser();
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
		/// Dispose mechanism
		/// </summary>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Called when navigating to a specific segment is required
		/// </summary>
		/// <param name="segment"></param>
		public void NavigateToSegment(SegmentReference segment)
		{
			_fileId = segment.FileId;
			_navigableDualBrowserControl.ScrollToSegment(segment);
		}
	}
}
