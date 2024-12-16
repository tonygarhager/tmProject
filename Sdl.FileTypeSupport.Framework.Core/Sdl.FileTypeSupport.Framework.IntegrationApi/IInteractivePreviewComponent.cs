using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IInteractivePreviewComponent
	{
		event PreviewControlHandler WindowSelectionChanged;

		Control GetControl();

		void Generate(IFileTypeManager filterManager, BilingualParserFactory bilingualParserFactory);

		void Close();

		void ScrollToSegment(SegmentReference segment);

		string CreateScratchDocument(SegmentReference segment);

		void UpdateSegment(SegmentReference segment, string filename);

		SegmentReference GetSelectedSegment();
	}
}
