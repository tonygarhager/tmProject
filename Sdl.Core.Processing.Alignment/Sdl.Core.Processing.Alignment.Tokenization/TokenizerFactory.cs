using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Tokenization
{
	internal class TokenizerFactory
	{
		private static readonly Dictionary<CultureInfo, Tokenizer> LpTokenizerMap = new Dictionary<CultureInfo, Tokenizer>();

		private static object _lock = new object();

		public static Tokenizer Create(CultureInfo culture, bool customRecognizers = false, BuiltinRecognizers recognizers = BuiltinRecognizers.RecognizeAll)
		{
			lock (_lock)
			{
				if (!LpTokenizerMap.TryGetValue(culture, out Tokenizer value))
				{
					TokenizerSetup tokenizerSetup = customRecognizers ? TokenizerSetupFactory.Create(culture, recognizers) : TokenizerSetupFactory.Create(culture);
					tokenizerSetup.CreateWhitespaceTokens = true;
					tokenizerSetup.BreakOnWhitespace = CultureInfoExtensions.UseBlankAsWordSeparator(culture);
					value = new Tokenizer(tokenizerSetup);
					LpTokenizerMap.Add(culture, value);
				}
				return value;
			}
		}
	}
}
