using Sdl.Core.Bcm.BcmModel.Tokenization;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public class MeasureBcmTokenConverter : BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken, Sdl.LanguagePlatform.Core.Tokenization.MeasureToken>
	{
		public override Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaTokenSpecific(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken token)
		{
			Sdl.LanguagePlatform.Core.Tokenization.NumberToken linguaNumber = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken, Sdl.LanguagePlatform.Core.Tokenization.MeasureToken>.GetLinguaNumber(token);
			return new Sdl.LanguagePlatform.Core.Tokenization.MeasureToken(token.Text, linguaNumber, (Sdl.LanguagePlatform.Core.Tokenization.Unit)token.Unit, token.UnitString, token.UnitSeparator)
			{
				Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken, Sdl.LanguagePlatform.Core.Tokenization.MeasureToken>.BcmToLinguaSegmentRange(token.Span)
			};
		}

		public override Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmTokenSpecific(Sdl.LanguagePlatform.Core.Tokenization.MeasureToken lingua)
		{
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken numericPart = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken, Sdl.LanguagePlatform.Core.Tokenization.MeasureToken>.LinguatoBcmNumber(lingua);
			return new Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken(lingua.Text, numericPart, (Sdl.Core.Bcm.BcmModel.Tokenization.Unit)lingua.Unit, lingua.UnitString, lingua.UnitSeparator)
			{
				Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken, Sdl.LanguagePlatform.Core.Tokenization.MeasureToken>.LinguatoBcmSegmentRange(lingua.Span)
			};
		}
	}
}
