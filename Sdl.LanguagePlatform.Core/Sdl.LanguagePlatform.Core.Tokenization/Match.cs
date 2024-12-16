namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public class Match
	{
		public int Index
		{
			get;
			set;
		}

		public int Length
		{
			get;
			set;
		}

		public Match(int index, int length)
		{
			Index = index;
			Length = length;
		}
	}
}
