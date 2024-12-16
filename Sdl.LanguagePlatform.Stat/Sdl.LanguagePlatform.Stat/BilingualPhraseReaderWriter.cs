using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class BilingualPhraseReaderWriter : IBinaryReaderWriter<BilingualPhrase>
	{
		public bool Read(BinaryReader reader, out BilingualPhrase item)
		{
			item = null;
			if (reader.BaseStream.Position >= reader.BaseStream.Length)
			{
				return false;
			}
			item = BilingualPhrase.Read(reader);
			return true;
		}

		public void Write(BinaryWriter writer, BilingualPhrase item)
		{
			item.Write(writer);
		}
	}
}
