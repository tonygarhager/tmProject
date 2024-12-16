using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	internal class AlignmentProviderFromFile : IAlignmentProvider
	{
		private WordAlignmentFileReader _alignmentFile;

		private bool _isOpen;

		public int Items
		{
			get
			{
				EnsureOpen();
				return _alignmentFile.Items;
			}
		}

		public AlignmentProviderFromFile(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			DataLocation location2 = location ?? throw new ArgumentNullException("location");
			_alignmentFile = new WordAlignmentFileReader(location2, srcCulture, trgCulture);
			if (!_alignmentFile.Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "Alignment file");
			}
			_isOpen = false;
		}

		private void EnsureOpen()
		{
			if (!_isOpen)
			{
				_alignmentFile.Open();
				_isOpen = true;
			}
		}

		public AlignmentTable GetAlignment(int segmentNumber, out List<BilingualPhrase> phrases)
		{
			phrases = null;
			EnsureOpen();
			return _alignmentFile[segmentNumber];
		}

		public void Close()
		{
			if (_isOpen)
			{
				_alignmentFile.Close();
				_alignmentFile.Dispose();
				_alignmentFile = null;
				_isOpen = false;
			}
		}
	}
}
