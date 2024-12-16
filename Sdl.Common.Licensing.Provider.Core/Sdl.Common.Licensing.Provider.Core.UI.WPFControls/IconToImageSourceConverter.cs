using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	public class IconToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Icon icon = value as Icon;
			if (icon != null)
			{
				if (int.TryParse(parameter as string, out int result))
				{
					return icon.ToImageSource(result, result);
				}
				return icon.ToImageSource();
			}
			return null;
		}

		public ImageSource ConvertFromBitmap(object value)
		{
			Bitmap bitmap = value as Bitmap;
			if (bitmap != null)
			{
				Icon value2 = ImageUtils.BitmapToIcon(bitmap);
				return Convert(value2, null, null, null) as ImageSource;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
