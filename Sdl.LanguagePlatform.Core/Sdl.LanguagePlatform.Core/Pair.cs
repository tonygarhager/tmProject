namespace Sdl.LanguagePlatform.Core
{
	public class Pair<T>
	{
		public T Left;

		public T Right;

		public Pair()
			: this(default(T), default(T))
		{
		}

		public Pair(T left, T right)
		{
			Left = left;
			Right = right;
		}
	}
}
