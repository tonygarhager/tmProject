using Sdl.Core.LanguageProcessing.ICU2;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class Tokenizer
	{
		internal const int MaxAcroLength = 6;

		public string Signature
		{
			get
			{
				string signature = Parameters.Signature;
				string text = "Tokenizer2";
				if (TokenizerHelper.UsesAdvancedTokenization(Parameters.Culture))
				{
					text += WordBoundaryFinder.IcuVersion;
				}
				if (!CultureInfoExtensions.UseBlankAsWordSeparator(Parameters.Culture))
				{
					text += "-crq18795";
				}
				return signature + "-" + text;
			}
		}

		public TokenizerParameters Parameters
		{
			get;
		}

		public TokenizationContext GetTokenizationContext()
		{
			List<SeparatorCombination> list = null;
			Dictionary<DateTimePatternType, List<string>> dictionary = null;
			Dictionary<string, CustomUnitDefinition> dictionary2 = null;
			List<CurrencyFormat> list2 = null;
			for (int i = 0; i < Parameters.Count; i++)
			{
				Recognizer recognizer = Parameters[i];
				NumberFSTRecognizer numberFSTRecognizer = recognizer as NumberFSTRecognizer;
				if (numberFSTRecognizer == null)
				{
					MeasureFSTRecognizer measureFSTRecognizer = recognizer as MeasureFSTRecognizer;
					if (measureFSTRecognizer == null)
					{
						DateTimeRecognizer dateTimeRecognizer = recognizer as DateTimeRecognizer;
						if (dateTimeRecognizer != null && dateTimeRecognizer.PatternsComputedPerType != null && dateTimeRecognizer.PatternsComputedPerType.Count > 0)
						{
							if (dictionary == null)
							{
								dictionary = new Dictionary<DateTimePatternType, List<string>>();
							}
							foreach (KeyValuePair<DateTimePatternType, DateTimeFSTEx> item in dateTimeRecognizer.PatternsComputedPerType)
							{
								if (!dictionary.ContainsKey(item.Key))
								{
									dictionary.Add(item.Key, new List<string>());
								}
								foreach (string pattern in item.Value.Patterns)
								{
									dictionary[item.Key].Add(pattern);
								}
							}
						}
					}
					else
					{
						dictionary2 = measureFSTRecognizer.UnitDefinitions;
					}
				}
				else
				{
					list = numberFSTRecognizer.SeparatorCombinationsComputed;
				}
				if (recognizer is CurrencyFSTRecognizer)
				{
					CurrencyFSTRecognizer currencyFSTRecognizer = recognizer as CurrencyFSTRecognizer;
					list2 = currencyFSTRecognizer.CurrencyFormats;
				}
			}
			if (list == null && dictionary2 == null && dictionary == null && list2 == null)
			{
				return null;
			}
			return new TokenizationContext(Parameters.Culture, dictionary, list, dictionary2, list2);
		}

		public Tokenizer(TokenizerParameters parameters)
		{
			Parameters = (parameters ?? throw new ArgumentNullException("parameters"));
		}

		public Tokenizer(TokenizerSetup setup)
			: this(new TokenizerParameters(setup, null))
		{
		}

		public Tokenizer(TokenizerSetup setup, IResourceDataAccessor resourceDataAccessor)
			: this(new TokenizerParameters(setup, resourceDataAccessor))
		{
		}

		public List<Token> Tokenize(string s)
		{
			return GetTokens(s, enhancedAsian: true);
		}

		public List<Token> Tokenize(Segment s)
		{
			return GetTokens(s, allowTokenBundles: false, enhancedAsian: true);
		}

		public List<Token> Tokenize(Segment s, bool allowTokenBundles)
		{
			return GetTokens(s, allowTokenBundles, enhancedAsian: true);
		}

		private List<Recognizer> GetFilteredRecognizers(string s)
		{
			List<Recognizer> list = new List<Recognizer>();
			for (int i = 0; i < Parameters.Count; i++)
			{
				Recognizer recognizer = Parameters[i];
				IRecognizerTextFilter recognizerTextFilter = recognizer as IRecognizerTextFilter;
				if (recognizerTextFilter == null || !recognizerTextFilter.ExcludeText(s))
				{
					list.Add(recognizer);
				}
			}
			return list;
		}

		public List<Token> GetTokens(string s, bool enhancedAsian)
		{
			List<Token> list = TokenizeInternal(s, 0, Parameters.CreateWhitespaceTokens, allowTokenBundles: false, GetFilteredRecognizers(s));
			ReclassifyAcronyms(list, enhancedAsian);
			AdjustNumberRangeTokenization(list);
			if (!enhancedAsian)
			{
				return list;
			}
			return GetAdvanceTokens(list);
		}

		public List<Token> GetTokens(Segment s, bool enhancedAsian)
		{
			return GetTokens(s, allowTokenBundles: false, enhancedAsian);
		}

		public List<Token> GetTokens(Segment s, bool allowTokenBundles, bool enhancedAsian)
		{
			List<Token> list = new List<Token>();
			int num = -1;
			foreach (SegmentElement element in s.Elements)
			{
				num++;
				if (element != null)
				{
					Tag tag = element as Tag;
					if (tag == null)
					{
						Token token = element as Token;
						if (token == null)
						{
							Text text = element as Text;
							if (text != null)
							{
								List<Token> list2 = TokenizeInternal(text.Value, num, Parameters.CreateWhitespaceTokens, allowTokenBundles, GetFilteredRecognizers(text.Value));
								if (enhancedAsian)
								{
									list2 = GetAdvanceTokens(list2);
								}
								if (list2 != null && list2.Count > 0)
								{
									list.AddRange(list2);
								}
							}
						}
						else
						{
							token.Span = new SegmentRange(num, 0, 0);
							list.Add(token);
							token.Culture = Parameters.Culture;
						}
					}
					else
					{
						Token token2 = new TagToken(tag);
						token2.Span = new SegmentRange(num, 0, 0);
						token2.Culture = Parameters.Culture;
						list.Add(token2);
					}
				}
			}
			ReclassifyAcronyms(list, enhancedAsian);
			AdjustNumberRangeTokenization(list);
			return list;
		}

		private List<Token> GetAdvanceTokens(List<Token> tokens)
		{
			TokenizerHelper tokenizerHelper = new TokenizerHelper();
			return tokenizerHelper.TokenizeIcu(tokens, Parameters.Culture, Parameters.AdvancedTokenizationStopwordList);
		}

		private static bool TokenTest(Token t, Func<Token, bool> f)
		{
			TokenBundle tokenBundle = t as TokenBundle;
			if (tokenBundle != null)
			{
				if (tokenBundle.Any((PrioritizedToken x) => f(x.Token)))
				{
					return true;
				}
				return false;
			}
			return f(t);
		}

		private static bool IsNegative(Token t)
		{
			if (TokenTest(t, delegate(Token x)
			{
				MeasureToken measureToken = x as MeasureToken;
				return measureToken != null && measureToken.Unit == Unit.Currency;
			}))
			{
				TokenBundle tokenBundle = t as TokenBundle;
				if (tokenBundle != null)
				{
					return tokenBundle.Any(delegate(PrioritizedToken x)
					{
						NumberToken numberToken2 = x.Token as NumberToken;
						return numberToken2 != null && numberToken2.Value < 0.0;
					});
				}
			}
			return TokenTest(t, delegate(Token x)
			{
				NumberToken numberToken = x as NumberToken;
				return numberToken != null && numberToken.Value < 0.0;
			});
		}

		private static bool IsCurrency(Token t)
		{
			return TokenTest(t, delegate(Token x)
			{
				MeasureToken measureToken = x as MeasureToken;
				return measureToken != null && measureToken.Unit == Unit.Currency;
			});
		}

		private static bool UnitStringInFront(Token t)
		{
			return TokenTest(t, delegate(Token x)
			{
				MeasureToken measureToken = x as MeasureToken;
				return measureToken != null && measureToken.Unit == Unit.Currency && !string.IsNullOrEmpty(measureToken.UnitString) && !string.IsNullOrEmpty(measureToken.Text) && measureToken.UnitString[0] == measureToken.Text[0];
			});
		}

		private static bool FormsNumberRange(Token nt1, Token nt2)
		{
			if (nt1.Type == TokenType.Number)
			{
				if (nt2.Type == TokenType.Number)
				{
					return true;
				}
				if (nt2.Type == TokenType.Measurement)
				{
					if (!IsCurrency(nt2))
					{
						return true;
					}
					return !UnitStringInFront(nt2);
				}
			}
			if (nt2.Type == TokenType.Measurement)
			{
				return string.CompareOrdinal(UnitString(nt1), UnitString(nt2)) == 0;
			}
			return false;
		}

		private static string UnitString(Token t)
		{
			TokenBundle tokenBundle = t as TokenBundle;
			if (tokenBundle != null)
			{
				if (tokenBundle.Any(delegate(PrioritizedToken x)
				{
					MeasureToken measureToken = x.Token as MeasureToken;
					return measureToken != null;
				}))
				{
					return (tokenBundle[0].Token as MeasureToken).UnitString;
				}
				return null;
			}
			return (t as MeasureToken)?.UnitString;
		}

		private static string GetTokenText(Token t)
		{
			TokenBundle tokenBundle = t as TokenBundle;
			if (tokenBundle != null)
			{
				return tokenBundle[0].Token.Text;
			}
			return t.Text;
		}

		private static void SetSpan(Token t, SegmentRange r)
		{
			TokenBundle tokenBundle = t as TokenBundle;
			if (tokenBundle != null)
			{
				tokenBundle.Span = r;
				foreach (PrioritizedToken item in tokenBundle)
				{
					item.Token.Span = r;
				}
			}
			else
			{
				t.Span = r;
			}
		}

		private static Token MakePositive(Token t)
		{
			TokenBundle tokenBundle = t as TokenBundle;
			if (tokenBundle != null)
			{
				TokenBundle tokenBundle2 = null;
				{
					foreach (PrioritizedToken item in tokenBundle)
					{
						if (tokenBundle2 == null)
						{
							tokenBundle2 = new TokenBundle(MakePositive(item.Token), item.Priority);
						}
						else
						{
							tokenBundle2.Add(MakePositive(item.Token), item.Priority);
						}
					}
					return tokenBundle2;
				}
			}
			NumberToken numberToken = t as NumberToken;
			NumberToken numberToken2 = new NumberToken(numberToken.Text.Substring(1), numberToken.GroupSeparator, numberToken.DecimalSeparator, numberToken.AlternateGroupSeparator, numberToken.AlternateDecimalSeparator, Sign.None, string.Empty, numberToken.RawDecimalDigits, numberToken.RawFractionalDigits);
			switch (t.Type)
			{
			case TokenType.Number:
				return numberToken2;
			case TokenType.Measurement:
			{
				MeasureToken measureToken = t as MeasureToken;
				return new MeasureToken(measureToken.Text.Substring(1), numberToken2, measureToken.Unit, measureToken.UnitString, measureToken.UnitSeparator, measureToken.CustomCategory);
			}
			default:
				throw new ArgumentException("Invalid token type", "t");
			}
		}

		private void AdjustNumberRangeTokenization(List<Token> tokens)
		{
			if (tokens == null)
			{
				return;
			}
			int startIndex = 0;
			HashSet<TokenType> typesToFind = new HashSet<TokenType>
			{
				TokenType.Number,
				TokenType.Measurement
			};
			int num = tokens.FindIndex(startIndex, (Token x) => typesToFind.Contains(x.Type));
			while (num >= 0 && num < tokens.Count - 1)
			{
				Token token = tokens[num];
				if (!IsNegative(token))
				{
					Token token2 = tokens[num + 1];
					if (IsNegative(token2) && FormsNumberRange(token, token2))
					{
						int index = token.Span.From.Index;
						if (token.Span.Into.Index == index && token2.Span.From.Index == index && token2.Span.Into.Index == index)
						{
							string tokenText = GetTokenText(token2);
							if (tokenText.Length >= 2 && !char.IsDigit(tokenText[0]) && char.IsDigit(tokenText[1]))
							{
								SimpleToken simpleToken = new SimpleToken(tokenText[0].ToString() + string.Empty, TokenType.GeneralPunctuation)
								{
									Culture = Parameters.Culture
								};
								simpleToken.Span = new SegmentRange(new SegmentPosition(index, token2.Span.From.Position), new SegmentPosition(index, token2.Span.From.Position));
								Token token3 = MakePositive(token2);
								token3.Culture = Parameters.Culture;
								token2.Span.From.Position++;
								SetSpan(token3, token2.Span);
								tokens[num + 1] = token3;
								tokens.Insert(num + 1, simpleToken);
							}
						}
					}
				}
				startIndex = num + 1;
				num = tokens.FindIndex(startIndex, (Token x) => typesToFind.Contains(x.Type));
			}
		}

		private void ReclassifyAcronyms(List<Token> tokens, bool enhancedAsian)
		{
			if (!Parameters.ReclassifyAcronyms)
			{
				return;
			}
			List<Token> list = tokens.FindAll((Token x) => x.Type == TokenType.Acronym);
			if (list.Count == 0)
			{
				return;
			}
			if (enhancedAsian)
			{
				int num = 0;
				foreach (Token item in list)
				{
					if (item.Text.Length > 6 && item is SimpleToken)
					{
						item.Type = TokenType.Word;
					}
					else
					{
						num++;
					}
				}
				if (num == 0)
				{
					return;
				}
			}
			int upperCase = 0;
			int lowerCase = 0;
			int noCase = 0;
			int noChar = 0;
			int num2 = 0;
			foreach (Token token in tokens)
			{
				switch (token.Type)
				{
				case TokenType.Word:
					num2++;
					CountLetters(token.Text, ref upperCase, ref lowerCase, ref noCase, ref noChar);
					break;
				case TokenType.Abbreviation:
					num2++;
					CountLetters(token.Text, ref upperCase, ref lowerCase, ref noCase, ref noChar);
					break;
				case TokenType.CharSequence:
					num2++;
					CountLetters(token.Text, ref upperCase, ref lowerCase, ref noCase, ref noChar);
					break;
				case TokenType.Acronym:
					num2++;
					CountLetters(token.Text, ref upperCase, ref lowerCase, ref noCase, ref noChar);
					break;
				}
			}
			int num3 = lowerCase * 2;
			if (enhancedAsian)
			{
				num3 = (lowerCase + noCase) * 2;
			}
			if (upperCase > num3 && (num2 != 1 || !tokens.Any((Token x) => x.Type == TokenType.Abbreviation || x.Type == TokenType.AlphaNumeric || x.Type == TokenType.CharSequence || x.Type == TokenType.Date || x.Type == TokenType.Measurement || x.Type == TokenType.Number || x.Type == TokenType.Time || x.Type == TokenType.Variable || x.Type == TokenType.Word || x.Type == TokenType.Uri)))
			{
				foreach (Token token2 in tokens)
				{
					if (token2.Type == TokenType.Acronym && token2 is SimpleToken && !token2.Text.Contains('&'))
					{
						token2.Type = TokenType.Word;
					}
				}
			}
		}

		private static void CountLetters(string s, ref int upperCase, ref int lowerCase, ref int noCase, ref int noChar)
		{
			foreach (char c in s)
			{
				if (char.IsUpper(c))
				{
					upperCase++;
				}
				else if (char.IsLower(c))
				{
					lowerCase++;
				}
				else if (char.IsLetter(c))
				{
					noCase++;
				}
				else
				{
					noChar++;
				}
			}
		}

		private List<Token> TokenizeInternal(string s, int currentRun, bool createWhitespaceTokens, bool allowTokenBundles, List<Recognizer> recognizers)
		{
			List<Token> result = new List<Token>();
			int startChainTokenPosition = -1;
			bool flag = !CultureInfoExtensions.UseBlankAsWordSeparator(Parameters.Culture);
			int i = 0;
			int length = s.Length;
			while (i < length)
			{
				int num = i;
				for (; i < length && char.IsWhiteSpace(s, i); i++)
				{
				}
				if (i > num)
				{
					if (createWhitespaceTokens)
					{
						Token token = new SimpleToken(s.Substring(num, i - num), TokenType.Whitespace);
						token.Culture = Parameters.Culture;
						token.Span = new SegmentRange(currentRun, num, i - 1);
						result.Add(token);
					}
					num = i;
				}
				if (i >= length)
				{
					break;
				}
				Recognizer recognizer = null;
				int num2 = 0;
				Token token2 = null;
				for (int j = 0; j < recognizers.Count; j++)
				{
					Recognizer recognizer2 = recognizers[j];
					int consumedLength = 0;
					Token token3 = recognizer2.Recognize(s, num, allowTokenBundles, ref consumedLength);
					if (token3 != null && (!flag || !(token3 is NumberToken) || num + consumedLength >= length || !CharacterProperties.IsLatinLetter(s[num + consumedLength])))
					{
						if (recognizer == null || (num2 < consumedLength && (!recognizer.OverrideFallbackRecognizer || !recognizer2.IsFallbackRecognizer)))
						{
							token2 = token3;
							recognizer = recognizer2;
							num2 = consumedLength;
							i = num + consumedLength;
						}
						else if (recognizer.Priority < recognizer2.Priority)
						{
							token2 = token3;
							recognizer = recognizer2;
							num2 = consumedLength;
							i = num + consumedLength;
						}
					}
				}
				if (token2 != null)
				{
					TokenBundle tokenBundle = token2 as TokenBundle;
					if (tokenBundle != null)
					{
						TokenBundle tokenBundle2 = tokenBundle;
						if (tokenBundle2.Count == 1)
						{
							token2 = tokenBundle2[0].Token;
						}
					}
				}
				else
				{
					for (UnicodeCategory unicodeCategory = char.GetUnicodeCategory(s, num); i < length && char.GetUnicodeCategory(s, i) == unicodeCategory; i++)
					{
					}
					num2 = i - num;
					token2 = new SimpleToken(s.Substring(num, i - num), TokenType.Word)
					{
						Culture = Parameters.Culture
					};
					recognizer = null;
				}
				if (token2 == null)
				{
					throw new NullReferenceException("winningToken can't be null");
				}
				token2.Span = new SegmentRange(currentRun, num, i - 1);
				result.Add(token2);
				if (Parameters.HasVariableRecognizer)
				{
					ApplyVariableRecognizer(s, token2, ref result, ref startChainTokenPosition);
				}
			}
			return result;
		}

		private void ApplyVariableRecognizer(string s, Token token, ref List<Token> result, ref int startChainTokenPosition)
		{
			if (IsVariablePart(token))
			{
				CheckForVariableTokens(token, s, ref result, ref startChainTokenPosition);
			}
			else
			{
				startChainTokenPosition = -1;
			}
		}

		private static bool IsVariablePart(Token token)
		{
			if (!(token is NumberToken) && !(token is DateTimeToken) && (!(token is SimpleToken) || (token.Type != TokenType.Word && token.Type != TokenType.Variable && token.Type != TokenType.GeneralPunctuation && token.Type != TokenType.Acronym && token.Type != TokenType.CharSequence)))
			{
				return token.Type == TokenType.AlphaNumeric;
			}
			return true;
		}

		private void CheckForVariableTokens(Token winningToken, string s, ref List<Token> result, ref int startChainTokenPosition)
		{
			if (startChainTokenPosition == -1)
			{
				startChainTokenPosition = result.Count - 1;
			}
			int i = startChainTokenPosition;
			int num = result.Count - i;
			string text;
			while (true)
			{
				if (num > 0)
				{
					int position = result[i].Span.From.Position;
					int position2 = winningToken.Span.Into.Position;
					text = s.Substring(position, position2 - position + 1);
					string item = CultureInfoExtensions.UseFullWidth(Parameters.Culture) ? StringUtilities.HalfWidthToFullWidth(text) : text;
					if (Parameters.Variables.Contains(item))
					{
						break;
					}
					for (i++; i <= result.Count - 1 && !IsVariablePart(result[i]); i++)
					{
					}
					num = result.Count - i;
					continue;
				}
				return;
			}
			MergelastTokens(text, num, ref result, i, Parameters.Culture);
		}

		private static void MergelastTokens(string variableCandidate, int nrTokens, ref List<Token> result, int indexToUpdate, CultureInfo culture)
		{
			SegmentRange span = result[indexToUpdate].Span;
			result[indexToUpdate] = new SimpleToken(variableCandidate, TokenType.Variable)
			{
				Span = span,
				Culture = culture
			};
			if (nrTokens > 1)
			{
				result[indexToUpdate].Span = new SegmentRange(result[indexToUpdate].Span.From, result[result.Count - 1].Span.Into);
				result.RemoveRange(indexToUpdate + 1, nrTokens - 1);
			}
		}
	}
}
