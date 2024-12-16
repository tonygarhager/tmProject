using Sdl.Core.LanguageProcessing;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class WordCounts
	{
		[DataMember]
		public int Segments
		{
			get;
			set;
		}

		[DataMember]
		public int Words
		{
			get;
			set;
		}

		[DataMember]
		public int Characters
		{
			get;
			set;
		}

		[DataMember]
		public int Placeables
		{
			get;
			set;
		}

		[DataMember]
		public int Tags
		{
			get;
			set;
		}

		public bool IsZero
		{
			get
			{
				if (Segments == 0 && Words == 0 && Characters == 0 && Placeables == 0)
				{
					return Tags == 0;
				}
				return false;
			}
		}

		public WordCounts()
		{
			Segments = (Words = (Characters = (Placeables = (Tags = 0))));
		}

		public WordCounts(WordCounts other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Assign(other);
		}

		public WordCounts(IList<Token> tokens)
			: this(tokens, breakOnHyphen: false, breakOnDash: false, breakOnTag: true, breakOnApostrophe: false)
		{
		}

		public WordCounts(IList<Token> tokens, bool breakOnHyphen, bool breakOnDash, bool breakOnTag, bool breakOnApostrophe)
		{
			DoWordCounts(tokens, breakOnHyphen, breakOnDash, breakOnTag, breakOnApostrophe);
		}

		public WordCounts(IList<Token> tokens, WordCountsOptions options, CultureInfo culture)
		{
			if (options.BreakAdvancedTokensByCharacter && !CultureInfoExtensions.UseBlankAsWordSeparator(culture) && AdvancedTokenization.TokenizesToWords(culture))
			{
				Tokenizer tokenizer = new Tokenizer(TokenizerSetupFactory.Create(culture, BuiltinRecognizers.RecognizeNone));
				List<Token> list = new List<Token>();
				foreach (Token token in tokens)
				{
					if (token.Type != TokenType.Word)
					{
						list.Add(token);
					}
					else
					{
						List<Token> tokens2 = tokenizer.GetTokens(token.Text, enhancedAsian: false);
						list.AddRange(tokens2);
					}
				}
				tokens = list;
			}
			DoWordCounts(tokens, options.BreakOnHyphen, options.BreakOnDash, options.BreakOnTag, options.BreakOnApostrophe);
		}

		private void DoWordCounts(IList<Token> tokens, bool breakOnHyphen, bool breakOnDash, bool breakOnTag, bool breakOnApostrophe)
		{
			Segments = 1;
			bool wasWord = false;
			byte apostropheRepetitions = 0;
			byte hyphenRepetitions = 0;
			byte dashRepetitions = 0;
			byte tagRepetitions = 0;
			if (tokens != null)
			{
				foreach (Token token in tokens)
				{
					if (token != null)
					{
						switch (token.Type)
						{
						case TokenType.Word:
						case TokenType.Abbreviation:
						case TokenType.UserDefined:
						{
							int num = ++Words;
							HandleWordCharactersCount(token, ref wasWord, ref apostropheRepetitions, ref hyphenRepetitions, ref dashRepetitions, ref tagRepetitions);
							break;
						}
						case TokenType.CharSequence:
							if (token.Text != null)
							{
								Words += token.Text.Length;
								HandleWordCharactersCount(token, ref wasWord, ref apostropheRepetitions, ref hyphenRepetitions, ref dashRepetitions, ref tagRepetitions);
							}
							break;
						case TokenType.GeneralPunctuation:
							if (token.Text != null)
							{
								if (token.Text.Length == 1)
								{
									if (wasWord && CharacterProperties.IsHyphen(token.Text[0]))
									{
										if (!breakOnHyphen)
										{
											hyphenRepetitions = (byte)(hyphenRepetitions + 1);
										}
										else
										{
											SetDefaultRepetitions(out apostropheRepetitions, out hyphenRepetitions, out dashRepetitions, out tagRepetitions, out wasWord);
										}
									}
									else if (wasWord && CharacterProperties.IsDash(token.Text[0]))
									{
										if (!breakOnDash)
										{
											dashRepetitions = (byte)(dashRepetitions + 1);
										}
										else
										{
											SetDefaultRepetitions(out apostropheRepetitions, out hyphenRepetitions, out dashRepetitions, out tagRepetitions, out wasWord);
										}
									}
									else if ((wasWord || apostropheRepetitions == 1) && CharacterProperties.IsApostrophe(token.Text[0]))
									{
										if (!breakOnApostrophe)
										{
											apostropheRepetitions = (byte)(apostropheRepetitions + 1);
										}
										else
										{
											SetDefaultRepetitions(out apostropheRepetitions, out hyphenRepetitions, out dashRepetitions, out tagRepetitions, out wasWord);
										}
									}
								}
								Characters += token.Text.Length;
							}
							break;
						case TokenType.OpeningPunctuation:
						case TokenType.ClosingPunctuation:
							SetDefaultRepetitions(out apostropheRepetitions, out hyphenRepetitions, out dashRepetitions, out tagRepetitions, out wasWord);
							if (token.Text != null)
							{
								Characters += token.Text.Length;
							}
							break;
						case TokenType.Date:
						case TokenType.Time:
						case TokenType.Variable:
						case TokenType.Number:
						case TokenType.Measurement:
						case TokenType.Acronym:
						case TokenType.Uri:
						case TokenType.OtherTextPlaceable:
						case TokenType.AlphaNumeric:
						{
							int num = ++Words;
							num = ++Placeables;
							HandleWordCharactersCount(token, ref wasWord, ref apostropheRepetitions, ref hyphenRepetitions, ref dashRepetitions, ref tagRepetitions);
							break;
						}
						case TokenType.Tag:
						{
							int num = ++Placeables;
							num = ++Tags;
							if (wasWord)
							{
								TagToken tagToken = token as TagToken;
								if (tagToken != null && tagToken.Tag != null && tagToken.Tag.CanHide && !breakOnTag)
								{
									tagRepetitions = (byte)(tagRepetitions + 1);
									break;
								}
							}
							SetDefaultRepetitions(out apostropheRepetitions, out hyphenRepetitions, out dashRepetitions, out tagRepetitions, out wasWord);
							break;
						}
						case TokenType.Whitespace:
							SetDefaultRepetitions(out apostropheRepetitions, out hyphenRepetitions, out dashRepetitions, out tagRepetitions, out wasWord);
							break;
						}
					}
				}
			}
		}

		private static void SetDefaultRepetitions(out byte apostropheRepetitions, out byte hyphenRepetitions, out byte dashRepetitions, out byte tagRepetitions, out bool wasWord)
		{
			apostropheRepetitions = 0;
			hyphenRepetitions = 0;
			dashRepetitions = 0;
			tagRepetitions = 0;
			wasWord = false;
		}

		private void HandleWordCharactersCount(Token t, ref bool wasWord, ref byte apostropheRepetitions, ref byte hyphenRepetitions, ref byte dashRepetitions, ref byte tagRepetitions)
		{
			if (!string.IsNullOrEmpty(t.Text))
			{
				Characters += t.Text.Length;
				wasWord = true;
			}
			if (apostropheRepetitions == 1 || apostropheRepetitions == 2 || hyphenRepetitions == 1 || dashRepetitions == 1 || tagRepetitions > 0)
			{
				Words--;
				apostropheRepetitions = 0;
				hyphenRepetitions = 0;
				dashRepetitions = 0;
				tagRepetitions = 0;
			}
		}

		public WordCounts Duplicate()
		{
			return new WordCounts(this);
		}

		public void Inc(WordCounts other)
		{
			Segments += other.Segments;
			Words += other.Words;
			Characters += other.Characters;
			Placeables += other.Placeables;
			Tags += other.Tags;
		}

		public void Assign(WordCounts other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Segments = other.Segments;
			Words = other.Words;
			Characters = other.Characters;
			Placeables = other.Placeables;
			Tags = other.Tags;
		}
	}
}
