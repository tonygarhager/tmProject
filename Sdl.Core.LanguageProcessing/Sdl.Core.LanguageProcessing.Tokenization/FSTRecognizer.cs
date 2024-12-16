using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class FSTRecognizer
	{
		public FST Fst
		{
			get;
		}

		public List<int> First
		{
			get;
		}

		public CultureInfo Culture
		{
			get;
		}

		public FSTRecognizer(FST fst, CultureInfo culture)
		{
			if (Culture == null)
			{
				Culture = CultureInfo.InvariantCulture;
			}
			Fst = (fst ?? throw new ArgumentNullException("fst"));
			Culture = culture;
			First = Fst.GetFirstSet(forOutput: false);
		}

		public List<FSTMatch> ComputeMatches(string s, int startOffset, bool ignoreCase, int cap, bool keepLongestMatchesOnly)
		{
			if (Fst == null)
			{
				return null;
			}
			char c = s[startOffset];
			if (startOffset < s.Length && First != null && !Label.Matches(c, First, ignoreCase))
			{
				return null;
			}
			Matcher matcher = new Matcher(Fst);
			List<Sdl.Core.LanguageProcessing.Tokenization.Transducer.MatchState> matches = new List<Sdl.Core.LanguageProcessing.Tokenization.Transducer.MatchState>();
			matcher.Match(s, matchWholeInput: false, Matcher.MatchMode.Analyse, startOffset, ignoreCase, delegate(Sdl.Core.LanguageProcessing.Tokenization.Transducer.MatchState ms)
			{
				matches.Add(ms);
				return true;
			}, null);
			if (matches.Count == 0)
			{
				return null;
			}
			matches.Sort((Sdl.Core.LanguageProcessing.Tokenization.Transducer.MatchState a, Sdl.Core.LanguageProcessing.Tokenization.Transducer.MatchState b) => b.ConsumedSymbols - a.ConsumedSymbols);
			int consumedSymbols = matches[0].ConsumedSymbols;
			List<FSTMatch> list = new List<FSTMatch>();
			foreach (Sdl.Core.LanguageProcessing.Tokenization.Transducer.MatchState item in matches)
			{
				if (keepLongestMatchesOnly && item.ConsumedSymbols < consumedSymbols)
				{
					return list;
				}
				list.Add(new FSTMatch(startOffset, item.ConsumedSymbols, item.GetOutputAsString()));
				if (cap > 0 && list.Count >= cap)
				{
					return list;
				}
			}
			return list;
		}
	}
}
