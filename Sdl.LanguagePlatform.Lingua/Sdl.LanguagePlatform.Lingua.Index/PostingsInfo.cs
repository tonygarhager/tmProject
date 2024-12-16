namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class PostingsInfo
	{
		public int Feature;

		public int BlockCount;

		public int PostingsCount;

		public int FirstKey;

		public int LastKey;

		public PostingsInfo(int f, int bc, int pc, int firstKey, int lastKey)
		{
			Feature = f;
			BlockCount = bc;
			PostingsCount = pc;
			FirstKey = firstKey;
			LastKey = lastKey;
		}
	}
}
