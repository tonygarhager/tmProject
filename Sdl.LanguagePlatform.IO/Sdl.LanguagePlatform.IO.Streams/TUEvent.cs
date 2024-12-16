using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.IO.Streams
{
	public class TUEvent : Event
	{
		private TranslationUnit _TU;

		public TranslationUnit TranslationUnit => _TU;

		public TUEvent(TranslationUnit tu)
		{
			_TU = tu;
		}
	}
}
