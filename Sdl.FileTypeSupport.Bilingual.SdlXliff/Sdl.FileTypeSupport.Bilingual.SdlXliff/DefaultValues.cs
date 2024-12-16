using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Drawing;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public static class DefaultValues
	{
		public const bool CanHide = false;

		public const bool IsSoftBreak = true;

		public const bool IsWordStop = true;

		public const bool IsSupported = true;

		public const DetectionLevel DefaultDetectionLevel = DetectionLevel.Unknown;

		public const EncodingCategory TargetEncodingCategory = EncodingCategory.NotSpecified;

		public const SegmentationHint DefaultSegmentationHint = SegmentationHint.MayExclude;

		public const bool IsBreakableWhitespace = false;

		public const bool SegmentLocked = false;

		public const ContextPurpose SdlContextPurpose = ContextPurpose.Information;

		public const bool UseDisplayName = true;

		public const ConfirmationLevel SegmentConfirmationLevel = ConfirmationLevel.Unspecified;

		public const RevisionType DefaultRevisionType = RevisionType.Insert;

		public static Color ContextColor => Color.Silver;
	}
}
