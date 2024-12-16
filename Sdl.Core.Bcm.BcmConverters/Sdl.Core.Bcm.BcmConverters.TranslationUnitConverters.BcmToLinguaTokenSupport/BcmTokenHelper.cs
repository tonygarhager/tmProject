using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaTokenSupport
{
	public static class BcmTokenHelper
	{
		private static readonly Dictionary<Type, Func<IBcmToLinguaConverter>> BcmtoConverterMap = new Dictionary<Type, Func<IBcmToLinguaConverter>>
		{
			{
				typeof(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.NumberToken),
				() => new NumberBcmTokenConverter()
			},
			{
				typeof(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.MeasureToken),
				() => new MeasureBcmTokenConverter()
			},
			{
				typeof(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.DateTimeToken),
				() => new DateTimeBcmTokenConverter()
			},
			{
				typeof(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.TagToken),
				() => new TagBcmTokenConverter()
			},
			{
				typeof(WordToken),
				() => new WordBcmTokenConverter()
			},
			{
				typeof(URIToken),
				() => new SimpleBcmTokenConverter(TokenType.Uri)
			},
			{
				typeof(IPAdressToken),
				() => new SimpleBcmTokenConverter(TokenType.OtherTextPlaceable)
			},
			{
				typeof(GeneralPunctuationToken),
				() => new SimpleBcmTokenConverter(TokenType.GeneralPunctuation)
			},
			{
				typeof(WhiteSpaceToken),
				() => new SimpleBcmTokenConverter(TokenType.Whitespace)
			},
			{
				typeof(AlphanumericToken),
				() => new SimpleBcmTokenConverter(TokenType.AlphaNumeric)
			},
			{
				typeof(VariableToken),
				() => new SimpleBcmTokenConverter(TokenType.Variable)
			},
			{
				typeof(AcronymToken),
				() => new SimpleBcmTokenConverter(TokenType.Acronym)
			},
			{
				typeof(CharSequenceToken),
				() => new SimpleBcmTokenConverter(TokenType.CharSequence)
			},
			{
				typeof(AbbreviationToken),
				() => new SimpleBcmTokenConverter(TokenType.Abbreviation)
			}
		};

		public static List<Sdl.LanguagePlatform.Core.Tokenization.Token> BcmToLinguaTokens(List<Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token> bcmTokens)
		{
			return bcmTokens?.Select(GetLinguaToken).ToList();
		}

		private static Sdl.LanguagePlatform.Core.Tokenization.Token GetLinguaToken(Sdl.Core.Bcm.BcmModel.Tokenization.Tokens.Token bcmToken)
		{
			IBcmToLinguaConverter bcmToLinguaConverter = BcmtoConverterMap[bcmToken.GetType()]();
			return bcmToLinguaConverter.ToLinguaToken(bcmToken);
		}
	}
}
