using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class Chunk : Match
	{
		public ChunkType ChunkType
		{
			get;
		}

		public SegmentationMethod Reason
		{
			get;
		}

		public Chunk(int start, int length, ChunkType type, SegmentationMethod method)
			: base(start, length)
		{
			ChunkType = type;
			Reason = method;
		}
	}
}
