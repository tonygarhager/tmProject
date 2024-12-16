using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Lingua;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class AnnotatedSegment : AbstractAnnotatedSegment
	{
		private readonly LanguageTools _tools;

		public AnnotatedSegment(AnnotatedTranslationMemory tm, Segment s, bool isTargetSegment, bool keepTokens, bool keepPeripheralWhitespace)
			: base(s, keepTokens, keepPeripheralWhitespace)
		{
			_tools = (isTargetSegment ? tm.TargetTools : tm.SourceTools);
		}

		protected override LanguageTools GetLinguaLanguageTools()
		{
			return _tools;
		}

		public override string ToString()
		{
			return _Segment?.ToPlain() ?? "null";
		}
	}
}
