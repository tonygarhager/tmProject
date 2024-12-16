using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class DoubleBufferedTableLayoutPanel : TableLayoutPanel
	{
		public DoubleBufferedTableLayoutPanel()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		}

		public DoubleBufferedTableLayoutPanel(IContainer container)
		{
			container.Add(this);
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		}
	}
}
