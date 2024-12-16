using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class PhraseReaderWriter : IBinaryReaderWriter<Phrase>
	{
		public bool Read(BinaryReader reader, out Phrase item)
		{
			item = null;
			if (reader.BaseStream.Position >= reader.BaseStream.Length)
			{
				return false;
			}
			item = Phrase.Read(reader);
			return true;
		}

		public void Write(BinaryWriter writer, Phrase item)
		{
			item.Write(writer);
		}
	}
}
