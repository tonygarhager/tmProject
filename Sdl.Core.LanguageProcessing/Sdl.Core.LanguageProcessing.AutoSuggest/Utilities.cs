using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.AutoSuggest
{
	public static class Utilities
	{
		public static List<int> IndexOf(IList<Token> phraseTokens, IList<Token> segment)
		{
			if (phraseTokens == null || phraseTokens.Count == 0 || segment == null || segment.Count == 0)
			{
				return null;
			}
			List<int> list = null;
			for (int i = 0; i < segment.Count; i++)
			{
				Token token = segment[i];
				if (token != null && !token.IsWhitespace && token.Type != TokenType.Tag && StartsAt(phraseTokens, segment, i))
				{
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(i);
				}
			}
			return list;
		}

		public static bool StartsAt(IList<Token> phrase, IList<Token> segment, int start)
		{
			if (phrase == null)
			{
				throw new ArgumentNullException("phrase");
			}
			if (segment == null)
			{
				throw new ArgumentNullException("segment");
			}
			int count = segment.Count;
			int count2 = phrase.Count;
			if (start < 0 || start >= count)
			{
				return false;
			}
			int i = start;
			int j = 0;
			while (true)
			{
				if (i > start)
				{
					for (; j < count2 && (phrase[j].IsWhitespace || phrase[j].Type == TokenType.Tag); j++)
					{
					}
					for (; i < count && (segment[i].IsWhitespace || segment[i].Type == TokenType.Tag); i++)
					{
					}
				}
				if (j >= count2)
				{
					return true;
				}
				if (i >= count)
				{
					return false;
				}
				Token segmentToken = segment[i];
				Token phraseToken = phrase[j];
				if (!AreTokensCompatible(phraseToken, segmentToken))
				{
					break;
				}
				j++;
				i++;
			}
			return false;
		}

		public static bool AreTokensCompatible(Token phraseToken, Token segmentToken)
		{
			switch (phraseToken.Type)
			{
			case TokenType.GeneralPunctuation:
			case TokenType.OpeningPunctuation:
			case TokenType.ClosingPunctuation:
				return segmentToken.IsPunctuation;
			case TokenType.Date:
			case TokenType.Time:
			case TokenType.Number:
			case TokenType.Measurement:
				return segmentToken.Type == phraseToken.Type;
			case TokenType.Whitespace:
				return segmentToken.IsWhitespace;
			case TokenType.Tag:
				return segmentToken.Type == TokenType.Tag;
			default:
				return string.Compare(segmentToken.Text, phraseToken.Text, StringComparison.InvariantCultureIgnoreCase) == 0;
			}
		}
	}
}
