using Sdl.Core.Globalization;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryTools;
using System;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class WordCountProcessor : AbstractBilingualContentHandler
	{
		private readonly Tokenizer _tokenizer;

		private readonly Language _sourceLanguage;

		public int WordCountResult
		{
			get;
			private set;
		}

		public int SegmentCountResult
		{
			get;
			private set;
		}

		public WordCountProcessor(Language sourceLanguage, Tokenizer tokenizer)
		{
			_sourceLanguage = sourceLanguage;
			_tokenizer = tokenizer;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			bool flag = paragraphUnit.SegmentPairs.Count() == 1;
			foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
			{
				Segment s = TUConverter.BuildLinguaSegment(_sourceLanguage.CultureInfo, segmentPair.Source, stripTags: false, excludeTagsInLockedContentText: false);
				WordCounts wordCounts = new WordCounts(_tokenizer.Tokenize(s));
				if (flag && paragraphUnit.Properties.SourceCount != null)
				{
					SourceCount sourceCount = paragraphUnit.Properties.SourceCount;
					WordCountResult += ((sourceCount.Unit == SourceCount.CountUnit.word) ? Convert.ToInt32(sourceCount.Value) : wordCounts.Words);
				}
				else
				{
					WordCountResult += wordCounts.Words;
				}
				SegmentCountResult += wordCounts.Segments;
			}
		}

		public override void FileComplete()
		{
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
		}
	}
}
