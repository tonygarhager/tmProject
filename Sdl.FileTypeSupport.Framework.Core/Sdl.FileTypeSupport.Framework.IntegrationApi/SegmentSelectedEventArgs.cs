using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class SegmentSelectedEventArgs : EventArgs
	{
		private SegmentReference _selectedSegment;

		private INavigablePreview _originatingPreview;

		public SegmentReference SelectedSegment
		{
			get
			{
				return _selectedSegment;
			}
			set
			{
				_selectedSegment = value;
			}
		}

		public INavigablePreview OriginatingPreview
		{
			get
			{
				return _originatingPreview;
			}
			set
			{
				_originatingPreview = value;
			}
		}

		public SegmentSelectedEventArgs()
		{
		}

		public SegmentSelectedEventArgs(INavigablePreview originatingPreview, SegmentReference selectedSegment)
		{
			_selectedSegment = selectedSegment;
			_originatingPreview = originatingPreview;
		}
	}
}
