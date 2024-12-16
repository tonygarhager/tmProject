using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal interface IBinaryReaderWriter<T>
	{
		bool Read(BinaryReader reader, out T item);

		void Write(BinaryWriter writer, T item);
	}
}
