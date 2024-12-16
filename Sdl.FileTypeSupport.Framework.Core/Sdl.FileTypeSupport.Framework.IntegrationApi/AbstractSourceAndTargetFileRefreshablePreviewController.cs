using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public abstract class AbstractSourceAndTargetFileRefreshablePreviewController : ISourceAndTargetFileRefreshablePreview, ISourceAndTargetFilePreviewControl, IAbstractPreviewControl, IAbstractPreviewController, ISourceAndTargetFilePreviewController, INavigablePreview, IDisposable, IAbstractUpdatablePreview, IPreviewUpdatedViaRefresh
	{
		public virtual Control Control
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool RefreshPreview
		{
			get;
			set;
		}

		public virtual TempFileManager SourcePreviewFile
		{
			get;
			set;
		}

		public virtual TempFileManager TargetPreviewFile
		{
			get;
			set;
		}

		public virtual Color PreferredHighlightColor
		{
			get;
			set;
		}

		public virtual TempFileManager TargetFilePath
		{
			get;
			set;
		}

		public event EventHandler<SegmentSelectedEventArgs> SegmentSelected;

		public virtual void Refresh()
		{
		}

		public virtual void NavigateToSegment(SegmentReference segment)
		{
		}

		public virtual void Dispose()
		{
		}

		public virtual void BeforeFileRefresh()
		{
		}

		public virtual void AfterFileRefresh()
		{
		}

		public void OnSegmentSelected(SegmentSelectedEventArgs args)
		{
			if (this.SegmentSelected != null)
			{
				this.SegmentSelected(this, args);
			}
		}
	}
}
