using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public class TuContextData
	{
		public TuContext TextContext
		{
			get;
			set;
		}

		public string IdContext
		{
			get;
			set;
		}

		public string CurrentStructureContextOverride
		{
			get;
			set;
		}

		public TuContextData()
		{
			TextContext = null;
			IdContext = string.Empty;
			CurrentStructureContextOverride = null;
		}
	}
}
