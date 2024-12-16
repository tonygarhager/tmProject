using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua.EditDistance;
using System;

namespace Sdl.LanguagePlatform.Lingua
{
	public class SimilarityComputers
	{
		public static double GetCharSimilarityEx(char a, char b, bool useToBase)
		{
			if (a == b)
			{
				return 1.0;
			}
			char c = char.ToLowerInvariant(a);
			char c2 = char.ToLowerInvariant(b);
			if (c == c2)
			{
				return 0.95;
			}
			if (!useToBase)
			{
				return 0.0;
			}
			char c3 = CharacterProperties.ToBase(c);
			char c4 = CharacterProperties.ToBase(c2);
			if (c3 != c4)
			{
				return 0.0;
			}
			return 0.9;
		}

		public static double GetCharSimilarity(char a, char b)
		{
			return GetCharSimilarityEx(a, b, useToBase: true);
		}

		public static double GetCharSimilarityWithoutToBase(char a, char b)
		{
			return GetCharSimilarityEx(a, b, useToBase: false);
		}

		public static double GetStringSimilarity(string a, string b, SimilarityComputer<char> similarityComputer = null, bool applySmallChangeAdjustment = true)
		{
			if (similarityComputer == null)
			{
				similarityComputer = GetCharSimilarity;
			}
			if (a.Equals(b, StringComparison.Ordinal))
			{
				return 1.0;
			}
			if (a.Equals(b, StringComparison.OrdinalIgnoreCase))
			{
				return 0.95;
			}
			EditDistanceComputer<char> editDistanceComputer = new EditDistanceComputer<char>(similarityComputer, applySmallChangeAdjustment);
			Sdl.LanguagePlatform.Core.EditDistance.EditDistance editDistance = editDistanceComputer.ComputeEditDistance(a.ToCharArray(), b.ToCharArray());
			return editDistance.Score;
		}

		public static double GetPlaceableSimilarity(Token a, Token b, BuiltinRecognizers disabledAutoSubstitutions)
		{
			if (!a.IsPlaceable || !b.IsPlaceable)
			{
				return 0.0;
			}
			if (a.Type != b.Type || a.GetType() != b.GetType())
			{
				return 0.0;
			}
			TagToken tagToken = a as TagToken;
			TagToken tagToken2 = b as TagToken;
			if (tagToken != null || tagToken2 != null)
			{
				if (tagToken == null || tagToken2 == null)
				{
					return -1.0;
				}
				if (tagToken.Tag.Type != tagToken2.Tag.Type)
				{
					return -1.0;
				}
			}
			if (disabledAutoSubstitutions != 0)
			{
				bool flag;
				switch (a.Type)
				{
				case TokenType.Abbreviation:
					flag = ((disabledAutoSubstitutions & BuiltinRecognizers.RecognizeAcronyms) != 0);
					break;
				case TokenType.Date:
					flag = ((disabledAutoSubstitutions & BuiltinRecognizers.RecognizeDates) != 0);
					break;
				case TokenType.Time:
					flag = ((disabledAutoSubstitutions & BuiltinRecognizers.RecognizeTimes) != 0);
					break;
				case TokenType.Variable:
					flag = ((disabledAutoSubstitutions & BuiltinRecognizers.RecognizeVariables) != 0);
					break;
				case TokenType.Number:
					flag = ((disabledAutoSubstitutions & BuiltinRecognizers.RecognizeNumbers) != 0);
					break;
				case TokenType.Measurement:
					flag = ((disabledAutoSubstitutions & BuiltinRecognizers.RecognizeMeasurements) != 0);
					break;
				default:
					flag = false;
					break;
				}
				if (flag)
				{
					if (!a.Equals(b))
					{
						return 0.7;
					}
					return 1.0;
				}
			}
			switch (a.GetSimilarity(b))
			{
			case SegmentElement.Similarity.None:
				return 0.7;
			case SegmentElement.Similarity.IdenticalType:
				return 0.85;
			case SegmentElement.Similarity.IdenticalValueAndType:
				return 1.0;
			default:
				return 0.0;
			}
		}

		public static bool StringEqualsOrdinalIgnoreCaseAndDiacritics(string a, string b, bool charactersNormalizeSafely = true)
		{
			if (a == null || b == null)
			{
				return string.Equals(a, b);
			}
			int length = a.Length;
			int length2 = b.Length;
			if (length != length2)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				char c = a[i];
				char c2 = b[i];
				if (c == c2)
				{
					continue;
				}
				if (charactersNormalizeSafely)
				{
					c = CharacterProperties.ToBase(c);
					c2 = CharacterProperties.ToBase(c2);
				}
				if (c != c2)
				{
					c = char.ToLowerInvariant(c);
					c2 = char.ToLowerInvariant(c2);
					if (c != c2)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static double GetTokenSimilarity(Token a, Token b, bool useStringEditDistance, BuiltinRecognizers disabledAutoSubstitutions, bool charactersNormalizeSafely = true, bool applySmallChangeAdjustment = true)
		{
			TagToken tagToken = a as TagToken;
			TagToken tagToken2 = b as TagToken;
			bool flag = tagToken != null;
			bool flag2 = tagToken2 != null;
			if (flag != flag2 || a.IsWhitespace != b.IsWhitespace || a.IsPunctuation != b.IsPunctuation)
			{
				return -1.0;
			}
			if (flag && flag2)
			{
				if (tagToken.Tag.Type == tagToken2.Tag.Type)
				{
					return 0.95;
				}
				if ((tagToken.Tag.Type == TagType.Standalone && tagToken2.Tag.Type == TagType.TextPlaceholder) || (tagToken.Tag.Type == TagType.TextPlaceholder && tagToken2.Tag.Type == TagType.Standalone))
				{
					return 0.85;
				}
				return -1.0;
			}
			double num = 0.0;
			if (a.IsPlaceable && b.IsPlaceable)
			{
				return GetPlaceableSimilarity(a, b, disabledAutoSubstitutions);
			}
			if (a.Text == null || b.Text == null)
			{
				return 0.0;
			}
			if (a.IsWord != b.IsWord)
			{
				num = 0.1;
			}
			double num2;
			if (a.Text.Equals(b.Text, StringComparison.Ordinal))
			{
				num2 = 1.0;
			}
			else if (a.IsWhitespace || a.IsPunctuation)
			{
				num2 = 0.94;
			}
			else if (StringEqualsOrdinalIgnoreCaseAndDiacritics(a.Text, b.Text, charactersNormalizeSafely))
			{
				num2 = 0.95;
			}
			else
			{
				SimpleToken simpleToken = a as SimpleToken;
				if (simpleToken != null)
				{
					SimpleToken simpleToken2 = b as SimpleToken;
					if (simpleToken2 != null)
					{
						if (simpleToken.Stem != null && simpleToken2.Stem != null && StringEqualsOrdinalIgnoreCaseAndDiacritics(simpleToken.Stem, simpleToken2.Stem))
						{
							num2 = 0.95;
						}
						else
						{
							num2 = 0.0;
							SimilarityComputer<char> similarityComputer = null;
							if (!useStringEditDistance)
							{
								return Math.Max(0.0, num2 - num);
							}
							string text = simpleToken.Text;
							string text2 = simpleToken2.Text;
							if (!charactersNormalizeSafely)
							{
								similarityComputer = GetCharSimilarityWithoutToBase;
							}
							num2 = GetThreshold(GetStringSimilarity(text, text2, similarityComputer, applySmallChangeAdjustment));
						}
						goto IL_025c;
					}
				}
				num2 = (useStringEditDistance ? (0.95 * GetThreshold(GetStringSimilarity(a.Text, b.Text))) : 0.0);
			}
			goto IL_025c;
			IL_025c:
			return Math.Max(0.0, num2 - num);
		}

		private static double GetThreshold(double sim)
		{
			if (sim != 1.0)
			{
				if (!(sim >= 0.9))
				{
					if (!(sim >= 0.75))
					{
						if (!(sim >= 0.5))
						{
							return 0.0;
						}
						return 0.5;
					}
					return 0.75;
				}
				return 0.9;
			}
			return 1.0;
		}
	}
}
