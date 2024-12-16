using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class ImportParameters
	{
		internal bool IsBatch
		{
			get;
			set;
		}

		internal ImportType Type
		{
			get;
			set;
		}

		internal bool RetainFgaInfo
		{
			get;
			set;
		}

		internal TuContext TuContext
		{
			get;
			set;
		}

		internal int PreviousTranslationHash
		{
			get;
			set;
		}

		internal int IndexInBatch
		{
			get;
			set;
		}

		internal ImportSettings.TUUpdateMode UpdateMode
		{
			get;
			set;
		}
	}
}
