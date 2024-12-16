using Sdl.LanguagePlatform.Core;
using System.Text.RegularExpressions;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class RegexRecognizerPattern
	{
		public Regex Regex
		{
			get;
			set;
		}

		public CharacterSet First
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public string Pattern => Regex?.ToString();

		public RegexRecognizerPattern(Regex rx)
			: this(rx, null, 0)
		{
		}

		public RegexRecognizerPattern(Regex rx, CharacterSet first)
			: this(rx, first, 0)
		{
		}

		public RegexRecognizerPattern(Regex rx, CharacterSet first, int priority)
		{
			Regex = rx;
			First = first;
			Priority = priority;
		}
	}
}
