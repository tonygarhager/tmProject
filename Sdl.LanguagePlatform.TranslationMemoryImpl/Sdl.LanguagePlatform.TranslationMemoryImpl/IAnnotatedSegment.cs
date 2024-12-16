using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal interface IAnnotatedSegment
	{
		Segment Segment
		{
			get;
		}

		long Hash
		{
			get;
		}
	}
}
