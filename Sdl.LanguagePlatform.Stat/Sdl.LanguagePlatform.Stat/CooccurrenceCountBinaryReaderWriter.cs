using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class CooccurrenceCountBinaryReaderWriter : IBinaryReaderWriter<CooccurrenceCount>
	{
		public bool Read(BinaryReader reader, out CooccurrenceCount item)
		{
			item = default(CooccurrenceCount);
			if (reader.BaseStream.Position >= reader.BaseStream.Length)
			{
				return false;
			}
			item.First = reader.ReadInt32();
			item.Second = reader.ReadInt32();
			item.Count = reader.ReadInt32();
			return true;
		}

		public void Write(BinaryWriter writer, CooccurrenceCount item)
		{
			writer.Write(item.First);
			writer.Write(item.Second);
			writer.Write(item.Count);
		}
	}
}
