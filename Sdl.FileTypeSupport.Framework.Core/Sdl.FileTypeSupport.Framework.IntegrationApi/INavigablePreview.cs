using System;
using System.Drawing;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface INavigablePreview
	{
		Color PreferredHighlightColor
		{
			get;
			set;
		}

		event EventHandler<SegmentSelectedEventArgs> SegmentSelected;

		void NavigateToSegment(SegmentReference segment);
	}
}
