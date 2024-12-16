using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.Core.Processing.Alignment.Tokenization;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class CorpusStatistics
	{
		private readonly Tokenizer _sourceTokenizer;

		private readonly Tokenizer _targetTokenizer;

		private Counter<string> _sourceWordCount = new Counter<string>();

		private Counter<string> _targetWordCount = new Counter<string>();

		private Counter<StringPair> _sourceTargetWordPairCount = new Counter<StringPair>();

		public IEnumerable<StringPair> SourceTargetWordPairs => _sourceTargetWordPairCount;

		public CorpusStatistics(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (sourceCulture == null)
			{
				throw new ArgumentNullException("sourceCulture");
			}
			if (targetCulture == null)
			{
				throw new ArgumentNullException("targetCulture");
			}
			_sourceTokenizer = TokenizerFactory.Create(sourceCulture);
			_targetTokenizer = TokenizerFactory.Create(targetCulture);
		}

		public void AddAlignedPair(AlignedPair<AlignmentElement> alignedPair)
		{
			if (alignedPair == null)
			{
				throw new ArgumentNullException("alignedPair");
			}
			string s = (alignedPair.LeftObject != null) ? alignedPair.LeftObject.ToString() : string.Empty;
			string s2 = (alignedPair.RightObject != null) ? alignedPair.RightObject.ToString() : string.Empty;
			IList<Token> tokens = _sourceTokenizer.Tokenize(s);
			UpdateWordCount(_sourceWordCount, tokens);
			IList<Token> tokens2 = _targetTokenizer.Tokenize(s2);
			UpdateWordCount(_targetWordCount, tokens2);
			Counter<string> counter = new Counter<string>();
			UpdateWordCount(counter, tokens);
			Counter<string> counter2 = new Counter<string>();
			UpdateWordCount(counter2, tokens2);
			foreach (string item2 in counter)
			{
				int count = counter.GetCount(item2);
				foreach (string item3 in counter2)
				{
					int count2 = counter2.GetCount(item3);
					StringPair item = new StringPair(item2, item3);
					int numberOfTimes = Math.Min(count, count2);
					_sourceTargetWordPairCount.Add(item, numberOfTimes);
				}
			}
		}

		private static void UpdateWordCount(Counter<string> wordCount, IEnumerable<Token> tokens)
		{
			foreach (Token token in tokens)
			{
				if (token.IsWord)
				{
					wordCount.Add(token.Text);
				}
			}
		}

		private void AddSourceWord(string sourceWord)
		{
			_sourceWordCount.Add(sourceWord);
		}

		private void AddTargetWord(string targetWord)
		{
			_targetWordCount.Add(targetWord);
		}

		private void AddSourceTargetWordPair(StringPair sourceTargetWordPair)
		{
			_sourceTargetWordPairCount.Add(sourceTargetWordPair);
		}

		public int GetSourceWordCount(string sourceWord)
		{
			if (sourceWord == null)
			{
				throw new ArgumentNullException("sourceWord");
			}
			return _sourceWordCount.GetCount(sourceWord);
		}

		public int GetTargetWordCount(string targetWord)
		{
			if (targetWord == null)
			{
				throw new ArgumentNullException("targetWord");
			}
			return _targetWordCount.GetCount(targetWord);
		}

		public int GetSourceTargetWordPairCount(StringPair sourceTargetWordPair)
		{
			if (sourceTargetWordPair == null)
			{
				throw new ArgumentNullException("sourceTargetWordPair");
			}
			return _sourceTargetWordPairCount.GetCount(sourceTargetWordPair);
		}
	}
}
