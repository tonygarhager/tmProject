using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public class NumberBcmTokenConverter : BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken, Sdl.LanguagePlatform.Core.Tokenization.NumberToken>
	{
		public override Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaTokenSpecific(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken token)
		{
			return BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken, Sdl.LanguagePlatform.Core.Tokenization.NumberToken>.GetLinguaNumber(token);
		}

		public override Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmTokenSpecific(Sdl.LanguagePlatform.Core.Tokenization.NumberToken lingua)
		{
			return BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken, Sdl.LanguagePlatform.Core.Tokenization.NumberToken>.LinguatoBcmNumber(lingua);
		}
	}
}
