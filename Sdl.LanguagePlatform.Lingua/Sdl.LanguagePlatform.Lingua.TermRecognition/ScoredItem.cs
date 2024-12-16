namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	public struct ScoredItem
	{
		public int Item;

		public double Score;

		public ScoredItem(int i, double s)
		{
			Item = i;
			Score = s;
		}
	}
}
