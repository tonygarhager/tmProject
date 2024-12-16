using Sdl.Core.Bcm.BcmModel.Tokenization;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public class DateTimeBcmTokenConverter : BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.DateTimeToken, Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken>
	{
		public override Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaTokenSpecific(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.DateTimeToken token)
		{
			return new Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken(token.Text, token.Value, (Sdl.LanguagePlatform.Core.Tokenization.DateTimePatternType)token.DateTimeType)
			{
				Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.DateTimeToken, Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken>.BcmToLinguaSegmentRange(token.Span)
			};
		}

		public override Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmTokenSpecific(Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken lingua)
		{
			return new Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.DateTimeToken(lingua.Text, lingua.Value, (Sdl.Core.Bcm.BcmModel.Tokenization.DateTimePatternType)lingua.DateTimePatternType)
			{
				Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.DateTimeToken, Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken>.LinguatoBcmSegmentRange(lingua.Span)
			};
		}
	}
}
