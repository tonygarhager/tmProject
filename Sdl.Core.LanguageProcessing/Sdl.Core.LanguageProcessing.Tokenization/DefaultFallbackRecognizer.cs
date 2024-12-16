using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class DefaultFallbackRecognizer : Recognizer
	{
		private readonly Trie<char, int> _leadingClitics;

		protected LanguageResources Resources;

		public DefaultFallbackRecognizer(RecognizerSettings settings, TokenType t, int priority, CultureInfo culture, IResourceDataAccessor dataAccessor)
			: base(settings, t, priority, null, "DefaultFallbackRecognizer", autoSubstitutable: false, culture)
		{
			if (dataAccessor != null)
			{
				Resources = new LanguageResources(culture, dataAccessor);
			}
			_IsFallbackRecognizer = true;
			HashSet<string> leadingClitics = CultureInfoExtensions.GetLeadingClitics(culture);
			if (leadingClitics != null)
			{
				int num = 0;
				_leadingClitics = new Trie<char, int>();
				foreach (string item in leadingClitics)
				{
					_leadingClitics.Add(item.ToCharArray(), num++);
				}
			}
		}

		public override string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetSignature(culture));
			if (_leadingClitics != null)
			{
				List<string> list = new List<string>();
				using (IEnumerator<KeyValuePair<IList<char>, int>> enumerator = _leadingClitics.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Key != null)
						{
							list.Add(string.Concat(enumerator.Current.Key));
						}
					}
				}
				list.Sort((string a, string b) => string.CompareOrdinal(a, b));
				foreach (string item in list)
				{
					stringBuilder.Append(item + "|");
				}
			}
			if (Resources != null)
			{
				stringBuilder.Append(Resources.AbbreviationsSignature);
			}
			return stringBuilder.ToString();
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			consumedLength = 0;
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			int length = s.Length;
			int i;
			for (i = from; i < length && (char.IsWhiteSpace(s, i) || char.IsSeparator(s, i)); i++)
			{
			}
			if (i > from)
			{
				consumedLength = i - from;
				Token token = new SimpleToken(s.Substring(from, consumedLength), TokenType.Whitespace);
				token.Culture = _Culture;
				return token;
			}
			if (IsHardTokenTerminator(s, i, _Settings))
			{
				consumedLength = 1;
				Token token2 = new SimpleToken(s.Substring(from, consumedLength), TokenType.GeneralPunctuation);
				token2.Culture = _Culture;
				return token2;
			}
			if (_leadingClitics != null)
			{
				TrieIterator<char, int> iterator = _leadingClitics.GetIterator();
				int num = 0;
				while (iterator != null && i + num < length && !iterator.IsFinal && iterator.Traverse(s[i + num]))
				{
					num++;
				}
				if (iterator != null && iterator.IsValid && iterator.IsFinal)
				{
					consumedLength = num;
					SimpleToken simpleToken = new SimpleToken(s.Substring(from, num), TokenType.Word);
					simpleToken.IsStopword = Resources.IsStopword(simpleToken.Text.ToLowerInvariant());
					simpleToken.Culture = _Culture;
					return simpleToken;
				}
			}
			char c = s[i];
			bool flag = CharacterProperties.IsCJKChar(c);
			while (i < length && !char.IsWhiteSpace(c) && !char.IsSeparator(c) && !IsHardTokenTerminator(s, i, _Settings))
			{
				bool flag2 = CharacterProperties.IsCJKChar(c);
				if (flag2 != flag)
				{
					break;
				}
				i++;
				if (i < length)
				{
					c = s[i];
					flag = flag2;
				}
			}
			int num2 = i;
			for (i = from; i < num2 && (IsSeparablePunct(s, i) || s[i] == '.'); i++)
			{
			}
			if (i > from)
			{
				consumedLength = i - from;
				Token token3 = new SimpleToken(s.Substring(from, consumedLength), TokenType.GeneralPunctuation);
				token3.Culture = _Culture;
				return token3;
			}
			bool flag3 = false;
			bool flag4;
			do
			{
				flag4 = false;
				while (num2 - 1 > i && IsSeparablePunct(s, num2 - 1))
				{
					num2--;
					flag4 = true;
				}
				int j;
				for (j = 0; num2 - 1 - j > i && s[num2 - 1 - j] == '.'; j++)
				{
				}
				if (j > 1)
				{
					num2 -= j;
					flag4 = true;
				}
				else if (j == 1)
				{
					if (Resources == null || !Resources.IsAbbreviation(s.Substring(from, num2 - from)))
					{
						num2--;
						flag4 = true;
					}
					else
					{
						flag3 = true;
					}
				}
			}
			while (flag4);
			consumedLength = num2 - from;
			SimpleToken simpleToken2 = new SimpleToken(s.Substring(from, consumedLength), (!flag3) ? TokenType.Word : TokenType.Abbreviation);
			simpleToken2.IsStopword = Resources.IsStopword(simpleToken2.Text.ToLowerInvariant());
			simpleToken2.Culture = _Culture;
			return simpleToken2;
		}

		private static bool IsSeparablePunct(string s, int pos)
		{
			int result;
			switch (char.GetUnicodeCategory(s, pos))
			{
			case UnicodeCategory.OtherPunctuation:
				result = ((s[pos] != '.') ? 1 : 0);
				break;
			default:
				result = 0;
				break;
			case UnicodeCategory.OpenPunctuation:
			case UnicodeCategory.ClosePunctuation:
			case UnicodeCategory.InitialQuotePunctuation:
			case UnicodeCategory.FinalQuotePunctuation:
			case UnicodeCategory.MathSymbol:
				result = 1;
				break;
			}
			return (byte)result != 0;
		}

		private static bool IsHardTokenTerminator(string s, int p, RecognizerSettings settings)
		{
			UnicodeCategory unicodeCategory = char.GetUnicodeCategory(s, p);
			if (unicodeCategory == UnicodeCategory.SpaceSeparator || unicodeCategory == UnicodeCategory.ParagraphSeparator || unicodeCategory == UnicodeCategory.LineSeparator || unicodeCategory == UnicodeCategory.OpenPunctuation || unicodeCategory == UnicodeCategory.ClosePunctuation || (unicodeCategory == UnicodeCategory.FinalQuotePunctuation && (settings.BreakOnApostrophe || !CharacterProperties.IsApostrophe(s[p]))) || unicodeCategory == UnicodeCategory.InitialQuotePunctuation || (unicodeCategory == UnicodeCategory.MathSymbol && (settings.BreakOnHyphen || !CharacterProperties.IsHyphen(s[p]))))
			{
				return true;
			}
			char c = s[p];
			if (c != '/' && c != '\\' && !CharacterProperties.IsColon(c) && !CharacterProperties.IsSemicolon(c) && (!CharacterProperties.IsHyphen(c) || !settings.BreakOnHyphen) && (!CharacterProperties.IsDash(c) || !settings.BreakOnDash))
			{
				if (CharacterProperties.IsApostrophe(c))
				{
					return settings.BreakOnApostrophe;
				}
				return false;
			}
			return true;
		}
	}
}
