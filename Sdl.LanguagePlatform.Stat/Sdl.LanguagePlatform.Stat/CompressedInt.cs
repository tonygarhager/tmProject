using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.Stat
{
	internal static class CompressedInt
	{
		private class CompressedIntReader : BinaryReader
		{
			public CompressedIntReader(Stream input)
				: base(input)
			{
			}

			public override int ReadInt32()
			{
				return Read7BitEncodedInt();
			}
		}

		private class CompressedIntWriter : BinaryWriter
		{
			public CompressedIntWriter(Stream output)
				: base(output)
			{
			}

			public override void Write(int value)
			{
				Write7BitEncodedInt(value);
			}
		}

		public static byte[] GetBytes(IList<int> list)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CompressedIntWriter compressedIntWriter = new CompressedIntWriter(memoryStream))
				{
					foreach (int item in list)
					{
						compressedIntWriter.Write(item);
					}
					compressedIntWriter.Flush();
				}
				return memoryStream.ToArray();
			}
		}

		public static List<int> GetInts(IList<byte> list)
		{
			using (MemoryStream input = new MemoryStream(list.ToArray()))
			{
				using (CompressedIntReader compressedIntReader = new CompressedIntReader(input))
				{
					List<int> list2 = new List<int>();
					while (compressedIntReader.PeekChar() >= 0)
					{
						int item = compressedIntReader.ReadInt32();
						list2.Add(item);
					}
					return list2;
				}
			}
		}
	}
}
