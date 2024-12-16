namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class Hit
	{
		public int Key;

		public int Score;

		public Hit(int k, int s)
		{
			Key = k;
			Score = s;
		}

		public override string ToString()
		{
			return $"key {Key}; score {Score}";
		}
	}
}
