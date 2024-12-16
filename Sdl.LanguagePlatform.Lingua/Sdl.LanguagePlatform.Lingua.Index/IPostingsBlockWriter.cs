namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface IPostingsBlockWriter
	{
		void WriteBlock(PostingsBlock postingsBlock);

		void DeleteBlock(PostingsBlock postingsBlock);

		void DeleteBlock(int blockId);
	}
}
