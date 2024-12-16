using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public abstract class SegmentationEngine
	{
		protected CultureInfo _Culture;

		public CultureInfo Culture => _Culture;

		protected SegmentationEngine(CultureInfo culture)
		{
			_Culture = (culture ?? throw new ArgumentNullException("culture"));
		}

		public virtual int GetNeutralPrefixLength(string text, int startIndex)
		{
			int i;
			for (i = startIndex; i < text.Length && CharacterProperties.IsWhitespace(text[i]); i++)
			{
			}
			return i - startIndex;
		}

		public abstract Chunk GetNextChunk(string text, int startIndex, bool assumeEof, bool followedByWordBreakTag);
	}
}
