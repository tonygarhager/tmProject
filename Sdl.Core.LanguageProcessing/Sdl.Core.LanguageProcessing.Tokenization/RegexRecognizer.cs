using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class RegexRecognizer : Recognizer, IRecognizerTextFilter
	{
		internal const int TokenMaxSize = 2000;

		protected List<RegexRecognizerPattern> _Patterns;

		internal RegexRecognizer(RecognizerSettings settings, TokenType t, int priority, string tokenClassName, string recognizerName, bool autoSubstitutable, CultureInfo culture)
			: base(settings, t, priority, tokenClassName, recognizerName, autoSubstitutable, culture)
		{
			_Patterns = new List<RegexRecognizerPattern>();
		}

		public void Add(string rxPattern, CharacterSet first)
		{
			Add(rxPattern, first, 0);
		}

		public void Add(string rxPattern, CharacterSet first, bool caseInsensitive)
		{
			Add(rxPattern, first, 0, caseInsensitive);
		}

		public void Add(string rxPattern, CharacterSet first, int priority)
		{
			Add(rxPattern, first, priority, caseInsensitive: false);
		}

		public void Add(string rxPattern, CharacterSet first, int priority, bool caseInsensitive)
		{
			RegexOptions regexOptions = RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;
			if (caseInsensitive)
			{
				regexOptions |= RegexOptions.IgnoreCase;
			}
			if (string.IsNullOrEmpty(rxPattern))
			{
				throw new ArgumentNullException("rxPattern");
			}
			Add(new Regex(rxPattern, regexOptions), first, priority);
		}

		public void Add(Regex rx, CharacterSet first, int priority)
		{
			if (rx == null)
			{
				throw new ArgumentNullException("rx");
			}
			string a = rx.ToString();
			foreach (RegexRecognizerPattern pattern in _Patterns)
			{
				if (string.Equals(a, pattern.Pattern, StringComparison.Ordinal))
				{
					return;
				}
			}
			RegexRecognizerPattern item = new RegexRecognizerPattern(rx, first, priority);
			_Patterns.Add(item);
		}

		protected virtual Token CreateToken(string s, GroupCollection groups)
		{
			Token token = (_Type == TokenType.OtherTextPlaceable) ? new GenericPlaceableToken(s, base.TokenClassName, _AutoSubstitutable) : new SimpleToken(s, _Type);
			token.Culture = _Culture;
			return token;
		}

		public override string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetSignature(culture));
			stringBuilder.Append("Regex");
			List<string> list = new List<string>();
			if (_Patterns != null)
			{
				foreach (RegexRecognizerPattern pattern in _Patterns)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					if (pattern != null)
					{
						if (pattern.First != null)
						{
							stringBuilder2.Append(pattern.First.Signature());
						}
						stringBuilder2.Append(pattern.Pattern);
						stringBuilder2.Append(pattern.Priority);
						list.Add(stringBuilder2.ToString());
					}
				}
			}
			list.Sort((string a, string b) => string.CompareOrdinal(a, b));
			foreach (string item in list)
			{
				stringBuilder.Append(item);
			}
			if (string.CompareOrdinal(base.TokenClassName, "ACR") == 0)
			{
				stringBuilder.Append("-maxacro-" + 6.ToString());
				stringBuilder.Append("-jazh");
			}
			return stringBuilder.ToString();
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			Token token = null;
			int num = 0;
			int num2 = 0;
			foreach (RegexRecognizerPattern pattern in _Patterns)
			{
				Regex regex = pattern.Regex;
				CharacterSet first = pattern.First;
				if (first == null || from >= s.Length || first.Contains(s[from]))
				{
					System.Text.RegularExpressions.Match match = regex.Match(s, from);
					if (match.Success && match.Index == from && VerifyContextConstraints(s, match.Index + match.Value.Length))
					{
						Token token2 = CreateToken(match.Value, match.Groups);
						if (token2 != null && match.Length > 0)
						{
							if (match.Length > num || token == null || (match.Length == num && pattern.Priority > num2 && !allowTokenBundles))
							{
								num = match.Length;
								token = token2;
								num2 = pattern.Priority;
							}
							else if (allowTokenBundles && match.Length == num)
							{
								if (!(token is TokenBundle))
								{
									token = new TokenBundle(token, num2);
								}
								((TokenBundle)token).Add(token2, pattern.Priority);
								num2 = Math.Max(num2, pattern.Priority);
							}
						}
					}
				}
			}
			if (token == null)
			{
				return null;
			}
			consumedLength = num;
			return token;
		}

		public bool ExcludeText(string s)
		{
			if (s != null)
			{
				return s.Length > 2000;
			}
			return false;
		}
	}
}
