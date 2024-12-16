using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal class AlignmentProviderDirect : IAlignmentProvider
	{
		private readonly TokenFileReader _srcSentences;

		private readonly TokenFileReader _trgSentences;

		private readonly WordAlignmentComputer _alignmentComputer;

		private bool _isOpen;

		public int Items
		{
			get
			{
				if (!_isOpen)
				{
					Open();
				}
				return _srcSentences.Segments;
			}
		}

		public AlignmentProviderDirect(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture, ScoreProviderType providerType)
		{
			DataLocation location2 = location ?? throw new ArgumentNullException("location");
			IScoreProvider scoreProvider;
			switch (providerType)
			{
			case ScoreProviderType.LTP:
				scoreProvider = new LTPScoreProvider(location, srcCulture, trgCulture);
				break;
			case ScoreProviderType.Chi2:
				scoreProvider = new ChiSquareScoreProvider(location, srcCulture, trgCulture);
				break;
			default:
				throw new Exception("Unknown score provider type");
			}
			if (!scoreProvider.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "bilingual association scores");
			}
			_srcSentences = new TokenFileReader(location2, srcCulture);
			if (!_srcSentences.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "Source sentences file");
			}
			_trgSentences = new TokenFileReader(location2, trgCulture);
			if (!_trgSentences.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "Target sentences file");
			}
			_alignmentComputer = new WordAlignmentComputer(location2, srcCulture, trgCulture, scoreProvider);
			_isOpen = false;
		}

		private void Open()
		{
			_srcSentences.Open();
			_trgSentences.Open();
			_isOpen = true;
		}

		public AlignmentTable GetAlignment(int segmentNumber, out List<BilingualPhrase> phrases)
		{
			if (!_isOpen)
			{
				Open();
			}
			IntSegment segmentAt = _srcSentences.GetSegmentAt(segmentNumber);
			IntSegment segmentAt2 = _trgSentences.GetSegmentAt(segmentNumber);
			return _alignmentComputer.ComputeAlignment(segmentAt, segmentAt2, out phrases);
		}

		public void Close()
		{
			if (_isOpen)
			{
				_srcSentences.Close();
				_trgSentences.Close();
				_isOpen = false;
			}
		}
	}
}
