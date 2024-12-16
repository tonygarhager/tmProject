using System.IO;

namespace Sdl.Core.FineGrainedAlignment
{
	internal interface IBinaryReaderWriter2<T>
	{
		bool Read(BinaryReader reader, out T item);

		void Write(BinaryWriter writer, T item);
	}
}
