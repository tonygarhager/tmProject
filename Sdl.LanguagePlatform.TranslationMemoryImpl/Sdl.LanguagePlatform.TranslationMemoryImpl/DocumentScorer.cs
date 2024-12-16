using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class DocumentScorer : AbstractScorer
	{
		private readonly LanguageTools _SourceLanguageTools;

		private readonly LanguageTools _TargetLanguageTools;

		public DocumentScorer(SearchSettings settings, LanguageTools sourceLanguageTools, LanguageTools targetLanguageTools, TextContextMatchType textContextMatchType)
			: base(settings, textContextMatchType)
		{
			_SourceLanguageTools = sourceLanguageTools;
			_TargetLanguageTools = targetLanguageTools;
		}

		protected override LanguageTools GetSourceTools()
		{
			return _SourceLanguageTools;
		}

		protected override LanguageTools GetTargetTools()
		{
			return _TargetLanguageTools;
		}

		protected override IAnnotatedSegment GetAnnotatedSegment(Segment segment, bool isTargetSegment, bool keepTokens, bool keepPeripheralWhitespace)
		{
			LanguageTools languageTools = isTargetSegment ? _TargetLanguageTools : _SourceLanguageTools;
			return new AnnotatedDocumentSegment(segment, languageTools, keepTokens, keepPeripheralWhitespace);
		}

		protected override BuiltinRecognizers Recognizers()
		{
			return BuiltinRecognizers.RecognizeNone;
		}
	}
}
