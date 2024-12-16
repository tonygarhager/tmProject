using Sdl.Core.Bcm.BcmModel.Tokenization;
using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public class TagBcmTokenConverter : BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken, Sdl.LanguagePlatform.Core.Tokenization.TagToken>
	{
		public override Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaTokenSpecific(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken token)
		{
			return new Sdl.LanguagePlatform.Core.Tokenization.TagToken
			{
				Tag = CreateLinguaTag(token),
				Text = token.Text,
				Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken, Sdl.LanguagePlatform.Core.Tokenization.TagToken>.BcmToLinguaSegmentRange(token.Span)
			};
		}

		public override Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmTokenSpecific(Sdl.LanguagePlatform.Core.Tokenization.TagToken linguaToken)
		{
			Tag tag = linguaToken?.Tag;
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken tagToken = new Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken();
			if (tag == null)
			{
				tagToken.Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken, Sdl.LanguagePlatform.Core.Tokenization.TagToken>.LinguatoBcmSegmentRange(linguaToken?.Span);
			}
			else
			{
				tagToken = new Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken((Sdl.Core.Bcm.BcmModel.Tokenization.TagType)tag.Type, tag.TagID, tag.Anchor, tag.AlignmentAnchor, tag.TextEquivalent, tag.CanHide)
				{
					Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken, Sdl.LanguagePlatform.Core.Tokenization.TagToken>.LinguatoBcmSegmentRange(linguaToken.Span),
					Text = linguaToken.Text
				};
			}
			return tagToken;
		}

		private Tag CreateLinguaTag(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken token)
		{
			int num;
			if (token == null)
			{
				num = 1;
			}
			else
			{
				_ = token.TagType;
				num = 0;
			}
			if (num != 0)
			{
				return null;
			}
			return new Tag((Sdl.LanguagePlatform.Core.TagType)token.TagType, token.TagId, token.Anchor.GetValueOrDefault(), token.AlignmentAnchor.GetValueOrDefault(), token.TextEquivalent, token.CanHide.GetValueOrDefault());
		}
	}
}
