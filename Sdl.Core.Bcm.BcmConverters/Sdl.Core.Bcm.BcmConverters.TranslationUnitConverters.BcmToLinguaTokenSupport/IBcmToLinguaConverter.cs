using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public interface IBcmToLinguaConverter
	{
		Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaToken(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token bcmToken);

		Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmToken(Sdl.LanguagePlatform.Core.Tokenization.Token linguaToken);
	}
}
