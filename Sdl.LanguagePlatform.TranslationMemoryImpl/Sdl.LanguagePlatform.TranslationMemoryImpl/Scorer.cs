using Sdl.Core.LanguageProcessing;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class Scorer : AbstractScorer
	{
		private readonly AnnotatedTranslationMemory _tm;

		internal Scorer(AnnotatedTranslationMemory tm, SearchSettings settings, bool normalizeCharWidths)
			: base(settings, tm.Tm.TextContextMatchType, normalizeCharWidths)
		{
			_tm = tm;
			SetupLegacyTokenizers();
		}

		public Scorer(AnnotatedTranslationMemory tm, SearchSettings settings)
			: base(settings, tm.Tm.TextContextMatchType, tm.Tm.NormalizeCharWidths)
		{
			_tm = tm;
			SetupLegacyTokenizers();
		}

		private void SetupLegacyTokenizers()
		{
			if (Settings.AdvancedTokenizationLegacyScoring && !CultureInfoExtensions.UseBlankAsWordSeparator(_tm.Tm.LanguageDirection.SourceCulture) && AdvancedTokenization.TokenizesToWords(_tm.Tm.LanguageDirection.SourceCulture))
			{
				TokenizerSetup setup = TokenizerSetupFactory.Create(_tm.Tm.LanguageDirection.SourceCulture, Recognizers());
				LegacySourceTokenizer = new Tokenizer(setup);
			}
			if (Settings.AdvancedTokenizationLegacyScoring && !CultureInfoExtensions.UseBlankAsWordSeparator(_tm.Tm.LanguageDirection.TargetCulture) && AdvancedTokenization.TokenizesToWords(_tm.Tm.LanguageDirection.TargetCulture))
			{
				TokenizerSetup setup = TokenizerSetupFactory.Create(_tm.Tm.LanguageDirection.TargetCulture, Recognizers());
				LegacyTargetTokenizer = new Tokenizer(setup);
			}
		}

		protected override LanguageTools GetSourceTools()
		{
			return _tm.SourceTools;
		}

		protected override LanguageTools GetTargetTools()
		{
			return _tm.TargetTools;
		}

		protected override IAnnotatedSegment GetAnnotatedSegment(Segment segment, bool isTargetSegment, bool keepTokens, bool keepPeripheralWhitespace)
		{
			return new AnnotatedSegment(_tm, segment, isTargetSegment, keepTokens, keepPeripheralWhitespace);
		}

		protected override BuiltinRecognizers Recognizers()
		{
			return _tm.Tm.Recognizers;
		}
	}
}
