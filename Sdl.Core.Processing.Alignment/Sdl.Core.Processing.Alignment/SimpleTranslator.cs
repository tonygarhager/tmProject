using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Tokenization;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Core.Processing.Alignment
{
	internal class SimpleTranslator
	{
		private IBilingualDictionary _bilingualDictionary;

		public SimpleTranslator(IBilingualDictionary bilingualDictionary)
		{
			if (bilingualDictionary == null)
			{
				throw new ArgumentNullException("bilingualDictionary");
			}
			_bilingualDictionary = bilingualDictionary;
		}

		public string Translate(string sourceSentence)
		{
			if (sourceSentence == null)
			{
				throw new ArgumentNullException("sourceSentence");
			}
			if (sourceSentence.Length == 0)
			{
				return sourceSentence;
			}
			StringBuilder stringBuilder = new StringBuilder();
			Tokenizer tokenizer = TokenizerFactory.Create(_bilingualDictionary.SourceCulture);
			IList<Token> list = tokenizer.Tokenize(sourceSentence);
			foreach (Token item in list)
			{
				string text = item.Text;
				string value = text;
				if (item.IsWord)
				{
					IEnumerable<string> targetWords = _bilingualDictionary.GetTargetWords(text);
					string text2 = targetWords.FirstOrDefault();
					if (text2 != null)
					{
						value = text2;
					}
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
	}
}
