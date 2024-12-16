using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Tokenization
{
	internal class TokensContainer
	{
		private readonly Dictionary<Segment, List<Token>> _tokens = new Dictionary<Segment, List<Token>>();

		internal List<Token> this[Segment segment]
		{
			get
			{
				if (!_tokens.ContainsKey(segment))
				{
					return new List<Token>();
				}
				return _tokens[segment];
			}
		}

		internal TokensContainer()
		{
		}

		internal TokensContainer(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, CultureInfo leftCulture, CultureInfo rightCulture)
		{
			Add((from x in leftElements.ToList()
				select x.Content).ToList(), leftCulture);
			Add((from x in rightElements.ToList()
				select x.Content).ToList(), rightCulture);
		}

		internal void Add(List<Segment> segments, CultureInfo culture)
		{
			Tokenizer tokenizer = TokenizerFactory.Create(culture, customRecognizers: true, BuiltinRecognizers.RecognizeDates | BuiltinRecognizers.RecognizeTimes | BuiltinRecognizers.RecognizeNumbers | BuiltinRecognizers.RecognizeAcronyms | BuiltinRecognizers.RecognizeMeasurements);
			foreach (Segment segment in segments)
			{
				_tokens[segment] = tokenizer.Tokenize(segment);
			}
		}
	}
}
