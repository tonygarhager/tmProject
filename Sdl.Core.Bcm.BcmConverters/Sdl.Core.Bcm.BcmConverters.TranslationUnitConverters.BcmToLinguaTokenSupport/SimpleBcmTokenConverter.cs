using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public class SimpleBcmTokenConverter : BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token, SimpleToken>
	{
		private readonly TokenType _linguatype;

		private static readonly Dictionary<TokenType, Func<string, Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token>> LinguaTypeToBcmToken = new Dictionary<TokenType, Func<string, Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token>>
		{
			{
				TokenType.Uri,
				(string text) => new URIToken(text)
			},
			{
				TokenType.OtherTextPlaceable,
				(string text) => new IPAdressToken(text)
			},
			{
				TokenType.GeneralPunctuation,
				(string text) => new GeneralPunctuationToken(text)
			},
			{
				TokenType.Whitespace,
				(string text) => new WhiteSpaceToken(text)
			},
			{
				TokenType.AlphaNumeric,
				(string text) => new AlphanumericToken(text)
			},
			{
				TokenType.Variable,
				(string text) => new VariableToken(text)
			},
			{
				TokenType.Acronym,
				(string text) => new AcronymToken(text)
			},
			{
				TokenType.CharSequence,
				(string text) => new CharSequenceToken(text)
			},
			{
				TokenType.Abbreviation,
				(string text) => new AbbreviationToken(text)
			}
		};

		public SimpleBcmTokenConverter(TokenType type)
		{
			_linguatype = type;
		}

		public override Sdl.LanguagePlatform.Core.Tokenization.Token ToLinguaTokenSpecific(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token token)
		{
			return new SimpleToken(token.Text, _linguatype)
			{
				Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token, SimpleToken>.BcmToLinguaSegmentRange(token.Span)
			};
		}

		public override Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token ToBcmTokenSpecific(SimpleToken lingua)
		{
			Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token token = LinguaTypeToBcmToken[lingua.Type](lingua.Text);
			token.Span = BcmTokenConverter<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token, SimpleToken>.LinguatoBcmSegmentRange(lingua.Span);
			return token;
		}
	}
}
