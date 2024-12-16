using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public class WordBcmTokenConverter : BcmTokenConverter<WordToken, SimpleToken>
	{
		public override Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaTokenSpecific(WordToken token)
		{
			return new SimpleToken(token.Text, TokenType.Word)
			{
				IsStopword = token.IsStopword,
				Stem = token.Stem,
				Span = BcmTokenConverter<WordToken, SimpleToken>.BcmToLinguaSegmentRange(token.Span)
			};
		}

		public override Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmTokenSpecific(SimpleToken lingua)
		{
			return new WordToken(lingua.Text)
			{
				IsStopword = lingua.IsStopword,
				Stem = lingua.Stem,
				Span = BcmTokenConverter<WordToken, SimpleToken>.LinguatoBcmSegmentRange(lingua.Span)
			};
		}
	}
}
