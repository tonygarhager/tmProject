using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class DefaultJapaneseFallbackRecognizer : DefaultFallbackRecognizer
	{
		private const string DefaultWordRx = "[゠-ヿ･-\uff9f]+|[\u3040-ゑん-ゟ]+|[を]|[一-\u9fff]+|[Ａ-Ｚａ-ｚ]+";

		private const string DefaultPuncCs = "[\u3000-〿㈀-\u32ff！-／：-＠［-］｛-､]";

		private readonly Regex _defaultWordRegex;

		private readonly CharacterSet _defaultPunctCharset;

		public DefaultJapaneseFallbackRecognizer(RecognizerSettings settings, TokenType t, int priority, CultureInfo culture, IResourceDataAccessor dataAccessor)
			: base(settings, t, priority, culture, dataAccessor)
		{
			_defaultWordRegex = new Regex("[゠-ヿ･-\uff9f]+|[\u3040-ゑん-ゟ]+|[を]|[一-\u9fff]+|[Ａ-Ｚａ-ｚ]+", RegexOptions.ExplicitCapture);
			int p = 0;
			_IsFallbackRecognizer = true;
			_defaultPunctCharset = CharacterSetParser.Parse("[\u3000-〿㈀-\u32ff！-／：-＠［-］｛-､]", ref p);
		}

		public override string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetSignature(culture));
			stringBuilder.Append("DefaultJapanese");
			if (_defaultWordRegex != null)
			{
				stringBuilder.Append(_defaultWordRegex);
			}
			if (_defaultPunctCharset != null)
			{
				stringBuilder.Append(_defaultPunctCharset.Signature());
			}
			return stringBuilder.ToString();
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			if (string.IsNullOrEmpty(s) || from >= s.Length)
			{
				return null;
			}
			consumedLength = 0;
			int startIndex = from;
			if (_defaultPunctCharset.Contains(s[from]))
			{
				while (from < s.Length && _defaultPunctCharset.Contains(s[from]))
				{
					consumedLength++;
					from++;
				}
				return new SimpleToken(s.Substring(startIndex, consumedLength), TokenType.GeneralPunctuation)
				{
					Culture = _Culture
				};
			}
			System.Text.RegularExpressions.Match match = _defaultWordRegex.Match(s, from);
			if (!match.Success || match.Index != from)
			{
				return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
			}
			consumedLength = match.Length;
			SimpleToken simpleToken = new SimpleToken(match.Value, TokenType.Word);
			simpleToken.Culture = _Culture;
			return simpleToken;
		}
	}
}
