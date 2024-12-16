using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Sdl.Common.Licensing.Provider.Core.UI.WPFControls
{
	internal class ImageUtils
	{
		private static byte[] pngiconheader = new byte[22]
		{
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			24,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		public static Icon BitmapToIcon(Image image)
		{
			return BitmapToIcon(image, image.Width, image.Height);
		}

		public static Icon BitmapToIcon(Image image, int width, int height)
		{
			Size newSize = new Size(width, height);
			using (Bitmap bitmap = new Bitmap(image, newSize))
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					bitmap.Save(memoryStream, ImageFormat.Png);
					memoryStream.Position = 0L;
					array = memoryStream.ToArray();
				}
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					if (height >= 256)
					{
						height = 0;
					}
					pngiconheader[6] = (byte)height;
					pngiconheader[7] = (byte)height;
					pngiconheader[14] = (byte)(array.Length & 0xFF);
					pngiconheader[15] = (byte)(array.Length / 256);
					pngiconheader[18] = (byte)pngiconheader.Length;
					memoryStream2.Write(pngiconheader, 0, pngiconheader.Length);
					memoryStream2.Write(array, 0, array.Length);
					memoryStream2.Position = 0L;
					return new Icon(memoryStream2);
				}
			}
		}
	}
}
