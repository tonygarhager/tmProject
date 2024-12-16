using Sdl.Core.Settings.Implementation;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public abstract class AbstractSingleFileRefreshablePreviewController : AbstractSettingsBundleAware, ISingleFileRefreshablePreview, ISingleFilePreviewControl, IAbstractPreviewControl, IAbstractPreviewController, ISingleFilePreviewController, INavigablePreview, IDisposable, IFileTypeDefinitionAware, IPreviewUpdatedViaSegmentFile, IAbstractUpdatablePreview, IPreviewUpdatedViaRefresh
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

		public virtual TempFileManager PreviewFile
		{
			get;
			set;
		}

		public virtual Color PreferredHighlightColor
		{
			get;
			set;
		}

		public IFileTypeDefinition FileTypeDefinition
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

		public virtual TempFileManager CreateSegmentFile(SegmentReference segment)
		{
			return null;
		}

		public virtual void UpdatePreviewFromSegmentFile(SegmentReference segment, TempFileManager translatedSegmentFile)
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
