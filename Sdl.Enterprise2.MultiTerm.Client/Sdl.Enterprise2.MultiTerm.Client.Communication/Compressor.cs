using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	internal class Compressor
	{
		private readonly WSCompressionMode _compressionMode;

		public Compressor(WSCompressionMode mode)
		{
			_compressionMode = mode;
		}

		public byte[] Compress(byte[] uncompressedData)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (Stream stream = GetOutputStream(memoryStream))
				{
					stream.Write(uncompressedData, 0, uncompressedData.Length);
					stream.Close();
					return memoryStream.ToArray();
				}
			}
		}

		public string DecompressElement(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			byte[] array = new byte[2048];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int count;
				while ((count = reader.ReadElementContentAsBase64(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				memoryStream.Position = 0L;
				using (Stream stream = GetInputStream(memoryStream))
				{
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						while (true)
						{
							count = stream.Read(array, 0, array.Length);
							if (count < 1)
							{
								break;
							}
							memoryStream2.Write(array, 0, count);
						}
						stream.Close();
						byte[] array2 = memoryStream2.ToArray();
						return Encoding.UTF8.GetString(array2, 0, array2.Length);
					}
				}
			}
		}

		public byte[] CompressString(string data)
		{
			return Compress(Encoding.UTF8.GetBytes(data));
		}

		private Stream GetOutputStream(Stream targetStream)
		{
			switch (_compressionMode)
			{
			case WSCompressionMode.GZip:
				return new GZipOutputStream(targetStream);
			case WSCompressionMode.Deflate:
				return new DeflaterOutputStream(targetStream);
			default:
				throw new InvalidOperationException("Unsupported compression mode.");
			}
		}

		private Stream GetInputStream(Stream sourceStream)
		{
			switch (_compressionMode)
			{
			case WSCompressionMode.GZip:
				return new GZipInputStream(sourceStream);
			case WSCompressionMode.Deflate:
				return new InflaterInputStream(sourceStream);
			default:
				throw new InvalidOperationException("Unsupported compression mode.");
			}
		}
	}
}
