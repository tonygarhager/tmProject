using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class LocationInfo : ICloneable
	{
		public ParagraphUnitId? ParagraphUnitId;

		public SegmentId? SegmentId;

		public bool IsInSegment;

		public int CharactersAfterSegmentStart;

		public int CharactersAfterSegment;

		public int CharactersAfterParagraphStart;

		public int LinesFromStartOfFile;

		public int OffsetFromStartOfLine;

		public ContentRestriction SourceOrTarget;

		public object Clone()
		{
			LocationInfo locationInfo = new LocationInfo();
			locationInfo.CharactersAfterParagraphStart = CharactersAfterParagraphStart;
			locationInfo.CharactersAfterSegment = CharactersAfterSegment;
			locationInfo.CharactersAfterSegmentStart = CharactersAfterSegmentStart;
			locationInfo.IsInSegment = IsInSegment;
			locationInfo.LinesFromStartOfFile = LinesFromStartOfFile;
			locationInfo.OffsetFromStartOfLine = OffsetFromStartOfLine;
			locationInfo.ParagraphUnitId = ParagraphUnitId;
			locationInfo.SegmentId = SegmentId;
			locationInfo.SourceOrTarget = SourceOrTarget;
			return locationInfo;
		}
	}
}
