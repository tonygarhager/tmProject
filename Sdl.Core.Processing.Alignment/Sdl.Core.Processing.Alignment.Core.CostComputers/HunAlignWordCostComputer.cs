using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Tokenization;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class HunAlignWordCostComputer : IAlignmentCostComputer
	{
		private readonly IDictionary<string, string> _translatedSentencesCache = new Dictionary<string, string>();

		private readonly IDictionary<string, IList<Token>> _sortedWordTokensCache = new Dictionary<string, IList<Token>>();

		private readonly IBilingualDictionary _bilingualDictionary;

		private readonly LanguageResources _sourceLanguageResources;

		private readonly LanguageResources _targetLanguageResources;

		public HunAlignWordCostComputer(IBilingualDictionary bilingualDictionary)
			: this(bilingualDictionary, null)
		{
		}

		public HunAlignWordCostComputer(IBilingualDictionary bilingualDictionary, IResourceDataAccessor resourceDataAccessor)
		{
			if (bilingualDictionary == null)
			{
				throw new ArgumentNullException("bilingualDictionary");
			}
			if (resourceDataAccessor == null)
			{
				resourceDataAccessor = new ResourceFileResourceAccessor();
			}
			_bilingualDictionary = bilingualDictionary;
			_sourceLanguageResources = new LanguageResources(bilingualDictionary.SourceCulture, resourceDataAccessor);
			_targetLanguageResources = new LanguageResources(bilingualDictionary.TargetCulture, resourceDataAccessor);
		}

		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			IEnumerable<string> sentences = sourceElements.Select((AlignmentElement sourceElement) => sourceElement.TextContent);
			IEnumerable<string> sentences2 = targetElements.Select((AlignmentElement targetElement) => targetElement.TextContent);
			string mergedSentence = GetMergedSentence(sentences);
			string mergedSentence2 = GetMergedSentence(sentences2);
			string translatedSentence = GetTranslatedSentence(mergedSentence);
			IList<Token> sortedWordTokens = GetSortedWordTokens(translatedSentence);
			IList<Token> sortedWordTokens2 = GetSortedWordTokens(mergedSentence2);
			int num = Math.Max(sortedWordTokens.Count, sortedWordTokens2.Count);
			if (num == 0)
			{
				return AlignmentCost.MinValue;
			}
			return (AlignmentCost)(1.0 - (double)GetIntersectionCount(sortedWordTokens, sortedWordTokens2) / (double)num);
		}

		private static string GetMergedSentence(IEnumerable<string> sentences)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string sentence in sentences)
			{
				stringBuilder.Append(sentence);
			}
			return stringBuilder.ToString();
		}

		private string GetTranslatedSentence(string sourceSentence)
		{
			if (!_translatedSentencesCache.TryGetValue(sourceSentence, out string value))
			{
				SimpleTranslator simpleTranslator = new SimpleTranslator(_bilingualDictionary);
				value = simpleTranslator.Translate(sourceSentence);
				_translatedSentencesCache[sourceSentence] = value;
			}
			return value;
		}

		private IList<Token> GetSortedWordTokens(string sentence)
		{
			if (!_sortedWordTokensCache.TryGetValue(sentence, out IList<Token> value))
			{
				Tokenizer tokenizer = TokenizerFactory.Create(_bilingualDictionary.TargetCulture);
				value = (from token in tokenizer.Tokenize(sentence)
					where (token.IsWord && !_sourceLanguageResources.IsStopword(token.Text) && !_targetLanguageResources.IsStopword(token.Text)) || token.IsSubstitutable
					select token).OrderBy((Token token) => token.Text, StringComparer.OrdinalIgnoreCase).ToList();
				_sortedWordTokensCache[sentence] = value;
			}
			return value;
		}

		internal int GetIntersectionCount(string translatedSentence, string targetSentence)
		{
			if (translatedSentence == null)
			{
				throw new ArgumentNullException("translatedSentence");
			}
			if (targetSentence == null)
			{
				throw new ArgumentNullException("targetSentence");
			}
			IList<Token> sortedWordTokens = GetSortedWordTokens(translatedSentence);
			IList<Token> sortedWordTokens2 = GetSortedWordTokens(targetSentence);
			return GetIntersectionCount(sortedWordTokens, sortedWordTokens2);
		}

		private static int GetIntersectionCount(IList<Token> sortedTranslatedWordTokens, IList<Token> sortedTargetWordTokens)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num2 < sortedTranslatedWordTokens.Count && num3 < sortedTargetWordTokens.Count)
			{
				string text = sortedTranslatedWordTokens[num2].Text;
				string text2 = sortedTargetWordTokens[num3].Text;
				int num4 = string.Compare(text, text2, StringComparison.OrdinalIgnoreCase);
				if (num4 == 0)
				{
					num++;
					num2++;
					num3++;
				}
				else if (num4 < 0)
				{
					num2++;
				}
				else
				{
					num3++;
				}
			}
			return num;
		}
	}
}
