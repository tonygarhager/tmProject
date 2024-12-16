namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataRange
	{
		public MarkupDataPosition From
		{
			get;
		}

		public MarkupDataPosition Into
		{
			get;
		}

		public MarkupDataRange(MarkupDataPosition from, MarkupDataPosition into)
		{
			From = from;
			Into = into;
		}
	}
}
