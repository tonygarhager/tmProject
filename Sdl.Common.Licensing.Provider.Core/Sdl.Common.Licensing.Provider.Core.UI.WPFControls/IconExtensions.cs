using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	public static class IconExtensions
	{
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		public static ImageSource ToImageSource(this Icon icon)
		{
			return icon.ToImageSource(BitmapSizeOptions.FromEmptyOptions());
		}

		public static ImageSource ToImageSource(this Icon icon, int width, int height)
		{
			return icon.ToImageSource(BitmapSizeOptions.FromWidthAndHeight(width, height));
		}

		private static ImageSource ToImageSource(this Icon icon, BitmapSizeOptions bitmapSizeOptions)
		{
			Bitmap bitmap = icon.ToBitmap();
			IntPtr hbitmap = bitmap.GetHbitmap();
			ImageSource imageSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, bitmapSizeOptions);
			imageSource.Freeze();
			if (!DeleteObject(hbitmap))
			{
				throw new Win32Exception();
			}
			return imageSource;
		}
	}
}
