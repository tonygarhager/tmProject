using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class NGramReaderWriter : IBinaryReaderWriter<NGram>
	{
		public bool Read(BinaryReader reader, out NGram item)
		{
			item = null;
			if (reader.BaseStream.Position >= reader.BaseStream.Length)
			{
				return false;
			}
			int num = reader.ReadInt32();
			int count = reader.ReadInt32();
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = reader.ReadInt32();
			}
			item = new NGram(array)
			{
				Count = count
			};
			return true;
		}

		public void Write(BinaryWriter writer, NGram item)
		{
			writer.Write(item.Data.Length);
			writer.Write(item.Count);
			int[] data = item.Data;
			foreach (int value in data)
			{
				writer.Write(value);
			}
		}
	}
}
