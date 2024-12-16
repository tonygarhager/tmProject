namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IPreviewUpdatedViaSegmentFile : IAbstractUpdatablePreview
	{
		TempFileManager CreateSegmentFile(SegmentReference segment);

		void UpdatePreviewFromSegmentFile(SegmentReference segment, TempFileManager translatedSegmentFile);
	}
}
