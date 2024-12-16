using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class DefaultChineseFallbackRecognizer : DefaultFallbackRecognizer
	{
		private const string DefaultPuncCs = "[\u3000-〿㈀-\u32ff！-／：-＠［-］｛-､]";

		private readonly CharacterSet _defaultPunctCharset;

		public DefaultChineseFallbackRecognizer(RecognizerSettings settings, TokenType t, int priority, CultureInfo culture, IResourceDataAccessor dataAccessor)
			: base(settings, t, priority, culture, dataAccessor)
		{
			int p = 0;
			_IsFallbackRecognizer = true;
			_defaultPunctCharset = CharacterSetParser.Parse("[\u3000-〿㈀-\u32ff！-／：-＠［-］｛-､]", ref p);
		}

		public override string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetSignature(culture));
			stringBuilder.Append("DefaultChinese");
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
			if (s[from] < '一' || s[from] > '\u9fff')
			{
				return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
			}
			while (from < s.Length && s[from] >= '一' && s[from] <= '\u9fff')
			{
				consumedLength++;
				from++;
			}
			SimpleToken simpleToken = new SimpleToken(s.Substring(startIndex, consumedLength), TokenType.CharSequence);
			simpleToken.Culture = _Culture;
			return simpleToken;
		}
	}
}
