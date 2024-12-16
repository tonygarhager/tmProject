using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class BilingualPhraseFileComputer
	{
		public EventHandler<ProgressEventArgs> Progress;

		private const int ReportProgressPeriod = 100;

		private int _cap;

		private bool _log;

		private bool _verbose;

		private const double MinLikelihood = 0.005;

		private readonly DataLocation _location;

		private readonly CultureInfo _srcCulture;

		private readonly CultureInfo _trgCulture;

		private readonly TokenFileReader _srcSentences;

		private readonly TokenFileReader _trgSentences;

		public BilingualPhraseFileComputer(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_location = (location ?? throw new ArgumentNullException("location"));
			_srcCulture = srcCulture;
			_trgCulture = trgCulture;
			_srcSentences = new TokenFileReader(_location, _srcCulture);
			if (!_srcSentences.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "Source sentences file");
			}
			_trgSentences = new TokenFileReader(_location, _trgCulture);
			if (!_trgSentences.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "Target sentences file");
			}
		}

		public void ComputePhrases(TextWriter logStream, int cap, bool verbose, IScoreProvider scoreProvider)
		{
			if (scoreProvider == null)
			{
				throw new ArgumentNullException("scoreProvider");
			}
			_cap = Math.Max(0, cap);
			_log = (logStream != null);
			if (_log)
			{
				_verbose = verbose;
			}
			_srcSentences.Open();
			_trgSentences.Open();
			IBilingualPhraseCounter bilingualPhraseCounter = new DiskBasedBilingualPhraseCounter(_location, _srcCulture, _trgCulture);
			BilingualPhraseComputer bilingualPhraseComputer = new BilingualPhraseComputer(_location, _srcCulture, _trgCulture, bilingualPhraseCounter);
			WordAlignmentComputer wordAlignmentComputer = new WordAlignmentComputer(_location, _srcCulture, _trgCulture, scoreProvider);
			DateTime dateTime = default(DateTime);
			if (_log && logStream != null)
			{
				logStream.WriteLine("Phase I: Computing phrase proposals");
				logStream.WriteLine();
				logStream.Flush();
			}
			int maxSegmentLength = WordAlignmentComputer.MaxSegmentLength;
			int i;
			for (i = 0; i < _srcSentences.Segments; i++)
			{
				if (i % 100 == 0)
				{
					OnProgress(ProgressStage.ComputeBilingual, i);
				}
				if (dateTime == default(DateTime))
				{
					dateTime = DateTime.Now;
				}
				IntSegment segmentAt = _srcSentences.GetSegmentAt(i);
				IntSegment segmentAt2 = _trgSentences.GetSegmentAt(i);
				if (segmentAt.Count <= maxSegmentLength && segmentAt2.Count <= maxSegmentLength)
				{
					List<BilingualPhrase> phrases;
					AlignmentTable at = wordAlignmentComputer.ComputeAlignment(segmentAt, segmentAt2, out phrases);
					bilingualPhraseComputer.Compute(segmentAt, segmentAt2, at, phrases);
				}
				if (_log && !_verbose && i % 1000 == 0)
				{
					double num = 1000.0 * (double)i / (DateTime.Now - dateTime).TotalMilliseconds;
					logStream.WriteLine("{0} TUs, {1:f2} TUs/s", i, num);
					logStream.Flush();
				}
				if (_cap > 0 && i > _cap)
				{
					break;
				}
			}
			OnProgress(ProgressStage.PhraseCountAndMerge, 0);
			BilingualDictionaryFile bilingualDictionaryFile = bilingualPhraseCounter.FinishCounting();
			if (_log && logStream != null)
			{
				logStream.WriteLine();
				logStream.WriteLine("Phase II: Computing absolute monolingual phrase counts");
				logStream.WriteLine();
				logStream.Flush();
			}
			if (!(bilingualPhraseCounter is DiskBasedBilingualPhraseCounter))
			{
				bilingualDictionaryFile.SourcePhrases.ResetCounts();
				bilingualDictionaryFile.TargetPhrases.ResetCounts();
				for (i = 0; i < _srcSentences.Segments; i++)
				{
					IntSegment segmentAt = _srcSentences.GetSegmentAt(i);
					IntSegment segmentAt2 = _trgSentences.GetSegmentAt(i);
					ComputePhraseCounts(bilingualDictionaryFile.SourcePhrases, segmentAt);
					ComputePhraseCounts(bilingualDictionaryFile.TargetPhrases, segmentAt2);
					if (_log && i % 1000 == 0)
					{
						logStream.Write('.');
						if (i % 50000 == 0)
						{
							logStream.WriteLine(i);
							logStream.Flush();
						}
					}
					if (_cap > 0 && i > _cap)
					{
						break;
					}
				}
			}
			_srcSentences.Close();
			_trgSentences.Close();
			if (_log)
			{
				logStream.WriteLine();
				logStream.WriteLine("Phase III: Applying phrase filters to {0} phrase pairs", bilingualDictionaryFile.Count);
				logStream.WriteLine();
				logStream.Flush();
			}
			int num2 = 0;
			i = 0;
			for (int j = 0; j < bilingualDictionaryFile.Count; j++)
			{
				i++;
				PhrasePair phrasePair = bilingualDictionaryFile[j];
				if (phrasePair.Count == 0)
				{
					continue;
				}
				Phrase phrase = bilingualDictionaryFile.SourcePhrases[phrasePair.SourcePhraseKey];
				Phrase phrase2 = bilingualDictionaryFile.TargetPhrases[phrasePair.TargetPhraseKey];
				bool flag = phrase.Count < 2 || phrase2.Count < 2 || phrasePair.Count < 2;
				if (!flag)
				{
					double num3 = (double)phrasePair.Count / (double)phrase.Count;
					double num4 = (double)phrasePair.Count / (double)phrase2.Count;
					if (num3 < 0.005 || num4 < 0.005)
					{
						flag = true;
					}
				}
				if (flag)
				{
					phrasePair.Count = 0;
					num2++;
				}
				if (_log && i % 10000 == 0)
				{
					logStream.Write('.');
					if (i % 500000 == 0)
					{
						logStream.WriteLine(i);
						logStream.Flush();
					}
				}
			}
			if (num2 > 0)
			{
				if (_log)
				{
					logStream.WriteLine();
					logStream.WriteLine("{0} phrases discarded by heuristics. Updating indices...", num2);
					logStream.WriteLine();
					logStream.Flush();
				}
				bilingualDictionaryFile.PurgeAndUpdateIndex();
			}
			if (_log)
			{
				logStream.WriteLine();
				logStream.WriteLine("Phase IV: Saving {0} phrase pairs ({1} src, {2} trg phrases)", bilingualDictionaryFile.Count, bilingualDictionaryFile.SourcePhrases.Count, bilingualDictionaryFile.TargetPhrases.Count);
				logStream.WriteLine();
				logStream.Flush();
			}
			bilingualDictionaryFile.Save();
		}

		private static void ComputePhraseCounts(PhraseDictionary pd, IntSegment s)
		{
			foreach (KeyValuePair<Phrase, int> item in pd.FindPhrasesInSegment(s))
			{
				item.Key.Count++;
			}
		}

		private void OnProgress(ProgressStage progressStage, int progressNumber)
		{
			if (Progress != null)
			{
				ProgressEventArgs e = new ProgressEventArgs(progressStage, progressNumber);
				Progress(this, e);
			}
		}
	}
}
