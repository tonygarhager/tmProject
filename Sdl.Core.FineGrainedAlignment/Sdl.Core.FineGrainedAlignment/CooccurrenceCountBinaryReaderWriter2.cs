using Sdl.LanguagePlatform.Stat;
using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class CooccurrenceCountBinaryReaderWriter2 : IBinaryReaderWriter2<CooccurrenceCount>
	{
		public bool Read(BinaryReader reader, out CooccurrenceCount item)
		{
			item = default(CooccurrenceCount);
			if (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				item.First = reader.ReadInt32();
				item.Second = reader.ReadInt32();
				item.Count = reader.ReadInt32();
				return true;
			}
			return false;
		}

		public void Write(BinaryWriter writer, CooccurrenceCount item)
		{
			writer.Write(item.First);
			writer.Write(item.Second);
			writer.Write(item.Count);
		}
	}
}
