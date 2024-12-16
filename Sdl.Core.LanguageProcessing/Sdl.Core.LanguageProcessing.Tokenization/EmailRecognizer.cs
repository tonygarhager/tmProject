using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Globalization;
using System.Net.Mail;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class EmailRecognizer : RegexRecognizer, IRecognizerTextFilter
	{
		public EmailRecognizer(RecognizerSettings settings, int priority, CultureInfo culture)
			: base(settings, TokenType.OtherTextPlaceable, priority, "EMAIL", "DEFAULT_URI_REGOCNIZER", autoSubstitutable: true, culture)
		{
			Add("(mailto:)?(?!\\.)[^\\s@\\\\\\(\\);:<>\\[\\],\\\"]+@((?![\\\"<>\\[\\]])[\\p{L}\\p{N}\\p{Pc}\\p{Pd}\\p{S}])+\\.(((?![\\\"<>\\[\\]])[\\p{L}\\p{N}\\p{Pc}\\p{Pd}\\p{S}])+\\.)*((?![\\\"<>\\[\\]])[\\p{L}\\p{N}\\p{Pc}\\p{Pd}\\p{S}]){2,}", null, caseInsensitive: true);
		}

		public new bool ExcludeText(string s)
		{
			if (base.ExcludeText(s))
			{
				return true;
			}
			int num = s.IndexOf('@');
			return num == -1;
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			Token token = base.Recognize(s, from, allowTokenBundles, ref consumedLength);
			if (token != null)
			{
				try
				{
					string text = token.Text.ToLower();
					if (text.StartsWith("mailto:"))
					{
						text = text.Substring(7);
					}
					MailAddress mailAddress = new MailAddress(text);
					token.Culture = _Culture;
					return token;
				}
				catch (FormatException)
				{
				}
			}
			return null;
		}
	}
}
