using ManagedCompactEncodingDetector;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities
{
	public static class FileEncoding
	{
		public static int GetDefaultAnsiCodepage(CultureInfo cultureInfo)
		{
			if (cultureInfo != null && cultureInfo.TextInfo != null)
			{
				return cultureInfo.TextInfo.ANSICodePage;
			}
			return 0;
		}

		public static Pair<Codepage, DetectionLevel> Detect(string filePath, Encoding suggestedEncoding, out string lineBreakType)
		{
			bool hasUTF8BOM;
			return Detect(filePath, suggestedEncoding, out lineBreakType, out hasUTF8BOM);
		}

		public static Pair<Codepage, DetectionLevel> Detect(string filePath, Encoding suggestedEncoding, out string lineBreakType, out bool hasUTF8BOM)
		{
			Pair<Codepage, DetectionLevel> pair = new Pair<Codepage, DetectionLevel>(null, DetectionLevel.Unknown);
			lineBreakType = "\r\n";
			hasUTF8BOM = false;
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				if (!fileStream.CanSeek)
				{
					return pair;
				}
				byte[] array = new byte[4];
				fileStream.Read(array, 0, 4);
				fileStream.Seek(0L, SeekOrigin.Begin);
				bool flag = true;
				byte[] array2 = new byte[1024];
				int num = 0;
				for (int num2 = fileStream.Read(array2, 0, 1024); num2 != 0; num2 = fileStream.Read(array2, 0, 1024))
				{
					for (int i = 0; i < num2; i++)
					{
						if (array2[i] == 13)
						{
							fileStream.Seek(num + i + 1, SeekOrigin.Begin);
							byte[] array3 = new byte[2];
							fileStream.Read(array3, 0, 2);
							if (array3[0] == 10 || (array3[0] == 0 && array3[1] == 10))
							{
								lineBreakType = "\r\n";
								flag = false;
							}
							else
							{
								lineBreakType = "\r";
								flag = false;
							}
							break;
						}
						if (array2[i] == 10)
						{
							lineBreakType = "\n";
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
					num += 1024;
				}
				fileStream.Close();
				if (array[0] == 239 && array[1] == 187 && array[2] == 191)
				{
					pair.First = new Codepage(Encoding.GetEncoding(65001));
					pair.Second = DetectionLevel.Certain;
					hasUTF8BOM = true;
					return pair;
				}
				if (array[0] == 0 && array[1] == 0 && array[2] == 254 && array[3] == byte.MaxValue)
				{
					pair.First = new Codepage(Encoding.GetEncoding(12001));
					pair.Second = DetectionLevel.Certain;
					return pair;
				}
				if (array[0] == byte.MaxValue && array[1] == 254 && array[2] == 0 && array[3] == 0)
				{
					pair.First = new Codepage(Encoding.GetEncoding(12000));
					pair.Second = DetectionLevel.Certain;
					return pair;
				}
				if (array[0] == byte.MaxValue && array[1] == 254)
				{
					pair.First = new Codepage(Encoding.GetEncoding(1200));
					pair.Second = DetectionLevel.Certain;
					return pair;
				}
				if (array[0] == 254 && array[1] == byte.MaxValue)
				{
					pair.First = new Codepage(Encoding.GetEncoding(1201));
					pair.Second = DetectionLevel.Certain;
					return pair;
				}
				EncodingDetector encodingDetector = new EncodingDetector();
				Encoding encoding;
				using (FileStream fileStream2 = File.OpenRead(filePath))
				{
					encoding = ((fileStream2.Length != 0L) ? encodingDetector.Detect(fileStream2) : null);
					if (SameEncodings(suggestedEncoding, encoding))
					{
						pair.First = new Codepage(suggestedEncoding);
						pair.Second = DetectionLevel.Likely;
					}
				}
				if (pair.First != null)
				{
					return pair;
				}
				if (encoding == null)
				{
					return pair;
				}
				pair.First = new Codepage(encoding);
				pair.Second = DetectionLevel.Guess;
				return pair;
			}
		}

		private static bool SameEncodings(Encoding suggestedEncoding, Encoding potentialEncoding)
		{
			if (potentialEncoding == null || suggestedEncoding == null)
			{
				return false;
			}
			return potentialEncoding.CodePage == suggestedEncoding.CodePage;
		}
	}
}
