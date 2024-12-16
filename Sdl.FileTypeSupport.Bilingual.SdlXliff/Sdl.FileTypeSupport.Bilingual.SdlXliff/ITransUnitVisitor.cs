using Oasis.Xliff12;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public interface ITransUnitVisitor
	{
		void Text(string text);

		void PlaceholderTag(x x);

		void StartPairedTag(g g);

		void EndPairedTag(g g);

		void SegmentStart(mrk mrk);

		void SegmentEnd(mrk mrk);

		void LocationMarker(mrk mrk);

		void CommentsMarkerStart(mrk mrk);

		void CommentsMarkerEnd(mrk mrk);

		void RevisionMarkerStart(mrk mrk);

		void RevisionMarkerEnd(mrk mrk);

		void MarkStart(mrk mrk);

		void MarkEnd(mrk mrk);

		void UnknownContent(object element);
	}
}
