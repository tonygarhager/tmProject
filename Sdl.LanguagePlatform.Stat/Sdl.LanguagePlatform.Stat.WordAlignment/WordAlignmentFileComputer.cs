using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class WordAlignmentFileComputer
	{
		public EventHandler<ProgressEventArgs> Progress;

		private const int ReportProgressPeriod = 100;

		private int _cap;

		private bool _log;

		private bool _verbose;

		private readonly DataLocation _location;

		private readonly CultureInfo _srcCulture;

		private readonly CultureInfo _trgCulture;

		private readonly TokenFileReader _srcSentences;

		private readonly TokenFileReader _trgSentences;

		private readonly IScoreProvider _scoreProvider;

		public WordAlignmentFileComputer(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture, ScoreProviderType providerType)
		{
			_location = (location ?? throw new ArgumentNullException("location"));
			switch (providerType)
			{
			case ScoreProviderType.LTP:
				_scoreProvider = new LTPScoreProvider(location, srcCulture, trgCulture);
				break;
			case ScoreProviderType.Chi2:
				_scoreProvider = new ChiSquareScoreProvider(location, srcCulture, trgCulture);
				break;
			default:
				throw new Exception("Unknown score provider type");
			}
			_srcCulture = srcCulture;
			_trgCulture = trgCulture;
			_srcSentences = new TokenFileReader(_location, _srcCulture);
			_trgSentences = new TokenFileReader(_location, _trgCulture);
		}

		public void ComputeAlignments(TextWriter logStream, int cap, bool verbose)
		{
			_cap = Math.Max(0, cap);
			_log = (logStream != null);
			if (_log)
			{
				_verbose = verbose;
			}
			WordAlignmentFileWriter wordAlignmentFileWriter = new WordAlignmentFileWriter(_location, _srcCulture, _trgCulture);
			wordAlignmentFileWriter.Create();
			if (_log && logStream != null)
			{
				logStream.WriteLine();
				logStream.Flush();
			}
			_srcSentences.Open();
			_trgSentences.Open();
			if (_srcSentences.Segments != _trgSentences.Segments)
			{
				throw new LanguagePlatformException(ErrorCode.CorruptData);
			}
			WordAlignmentComputer wordAlignmentComputer = new WordAlignmentComputer(_location, _srcCulture, _trgCulture, _scoreProvider);
			for (int i = 0; i < _srcSentences.Segments; i++)
			{
				if (i % 100 == 0)
				{
					OnProgress(ProgressStage.WordAlignment, i);
				}
				IntSegment segmentAt = _srcSentences.GetSegmentAt(i);
				IntSegment segmentAt2 = _trgSentences.GetSegmentAt(i);
				List<BilingualPhrase> phrases;
				AlignmentTable table = wordAlignmentComputer.ComputeAlignment(segmentAt, segmentAt2, out phrases);
				wordAlignmentFileWriter.Write(table);
				if (_log && !_verbose && i % 1000 == 0 && logStream != null)
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
			wordAlignmentFileWriter.Close();
			_srcSentences.Close();
			_trgSentences.Close();
			if (_log)
			{
				logStream?.WriteLine();
				logStream?.WriteLine("Finished alignment.");
				logStream?.WriteLine();
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
