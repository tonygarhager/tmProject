using Sdl.Core.Bcm.BcmModel.Tokenization.Tokens;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	public static class TokenMapping
	{
		private static readonly Dictionary<string, Func<Token>> TypeMappingDictionary = new Dictionary<string, Func<Token>>
		{
			{
				"number",
				() => new NumberToken()
			},
			{
				"measure",
				() => new MeasureToken()
			},
			{
				"tag",
				() => new TagToken()
			},
			{
				"datetime",
				() => new DateTimeToken()
			},
			{
				"word",
				() => new WordToken()
			},
			{
				"uri",
				() => new URIToken()
			},
			{
				"ipadress",
				() => new IPAdressToken()
			},
			{
				"punctuation",
				() => new GeneralPunctuationToken()
			},
			{
				"whitespace",
				() => new WhiteSpaceToken()
			},
			{
				"alphanum",
				() => new AlphanumericToken()
			},
			{
				"variable",
				() => new VariableToken()
			},
			{
				"acronym",
				() => new AcronymToken()
			},
			{
				"charseq",
				() => new CharSequenceToken()
			},
			{
				"abbreviation",
				() => new AbbreviationToken()
			}
		};

		public static Token GetType(string type)
		{
			return TypeMappingDictionary[type]();
		}
	}
}
