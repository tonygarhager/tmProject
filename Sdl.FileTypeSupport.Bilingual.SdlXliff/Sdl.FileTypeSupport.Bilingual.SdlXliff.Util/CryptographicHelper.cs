using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.Util
{
	public static class CryptographicHelper
	{
		public const string SdlXliffEncryptionHeader = "SDL:SDLXLIFF-HEADER";

		public static byte[] GenerateRandomNumber(int length)
		{
			using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				byte[] array = new byte[length];
				rNGCryptoServiceProvider.GetBytes(array);
				return array;
			}
		}

		public static byte[] GetDecodedKey(string base64EncodedKey)
		{
			return Convert.FromBase64String(base64EncodedKey);
		}

		public static bool ReadHeader(FileStream fileStream, out byte[] IV)
		{
			IV = new byte[16];
			if (IsEncryptedSdlXliff(fileStream))
			{
				if (fileStream.Read(IV, 0, 16) != 16)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static void WriteHeader(Stream stream, byte[] iV)
		{
			stream.Write(Encoding.ASCII.GetBytes("SDL:SDLXLIFF-HEADER"), 0, "SDL:SDLXLIFF-HEADER".Length);
			foreach (byte value in iV)
			{
				stream.WriteByte(value);
			}
		}

		public static bool IsEncryptedSdlXliff(FileStream fileStream)
		{
			byte[] array = new byte["SDL:SDLXLIFF-HEADER".Length];
			if (fileStream.Read(array, 0, "SDL:SDLXLIFF-HEADER".Length) == "SDL:SDLXLIFF-HEADER".Length && Encoding.ASCII.GetString(array).Equals("SDL:SDLXLIFF-HEADER"))
			{
				return true;
			}
			return false;
		}
	}
}
