using System.Windows;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	public class NumericUpDownChangedRoutedEventArgs : RoutedEventArgs
	{
		public double Interval
		{
			get;
			set;
		}

		public NumericUpDownChangedRoutedEventArgs(RoutedEvent routedEvent, double interval)
			: base(routedEvent)
		{
			Interval = interval;
		}
	}
}
