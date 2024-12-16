using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Lingua;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class AnnotatedDocumentSegment : AbstractAnnotatedSegment
	{
		private readonly LanguageTools _LanguageTools;

		public AnnotatedDocumentSegment(Segment s, LanguageTools languageTools, bool keepTokens, bool keepPeripheralWhitespace)
			: base(s, keepTokens, keepPeripheralWhitespace)
		{
			_LanguageTools = languageTools;
		}

		protected override LanguageTools GetLinguaLanguageTools()
		{
			return _LanguageTools;
		}
	}
}
