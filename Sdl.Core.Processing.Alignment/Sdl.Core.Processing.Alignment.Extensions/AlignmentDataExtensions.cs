using Sdl.Core.Processing.Alignment.Core;

namespace Sdl.Core.Processing.Alignment.Extensions
{
	internal static class AlignmentDataExtensions
	{
		internal static DocumentAlignmentData ConvertToDocumentAlignmentData(this AlignmentData alignment)
		{
			DocumentAlignmentData documentAlignmentData = new DocumentAlignmentData();
			foreach (DocumentSegmentId leftId in alignment.LeftIds)
			{
				documentAlignmentData.LeftSegmentIds.Add(leftId);
			}
			foreach (DocumentSegmentId rightId in alignment.RightIds)
			{
				documentAlignmentData.RightSegmentIds.Add(rightId);
			}
			documentAlignmentData.Quality = alignment.Quality;
			documentAlignmentData.Confirmed = alignment.Confirmed;
			documentAlignmentData.AlignmentCost = alignment.Cost;
			documentAlignmentData.AlignmentType = alignment.AlignmentType;
			return documentAlignmentData;
		}

		internal static AlignmentData ConvertToAlignmentData(this DocumentAlignmentData documentAlignmentData)
		{
			AlignmentData alignmentData = new AlignmentData(documentAlignmentData.AlignmentType);
			foreach (DocumentSegmentId leftSegmentId in documentAlignmentData.LeftSegmentIds)
			{
				alignmentData.LeftIds.Add(leftSegmentId);
			}
			foreach (DocumentSegmentId rightSegmentId in documentAlignmentData.RightSegmentIds)
			{
				alignmentData.RightIds.Add(rightSegmentId);
			}
			alignmentData.Quality = documentAlignmentData.Quality;
			alignmentData.Confirmed = documentAlignmentData.Confirmed;
			alignmentData.Cost = new AlignmentCost(documentAlignmentData.AlignmentCost);
			return alignmentData;
		}
	}
}
