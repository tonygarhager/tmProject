using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	public class SegmentationInfo
	{
		public SegmentationHint SegmentationHintFinal;

		public bool FollowedTagSpace
		{
			get;
			set;
		}

		public bool PrecededTagSpace
		{
			get;
			set;
		}

		public bool IsInlineElement
		{
			get;
			set;
		}

		public bool TriggerSegmentCreation
		{
			get;
			set;
		}

		public bool IsSegmentEnder
		{
			get;
			set;
		}

		public bool IsAfterSegmentEnder
		{
			get;
			set;
		}

		public bool MayExclude
		{
			get;
			set;
		}

		public SegmentationInfo Clone()
		{
			return new SegmentationInfo
			{
				FollowedTagSpace = FollowedTagSpace,
				PrecededTagSpace = PrecededTagSpace,
				IsInlineElement = IsInlineElement,
				TriggerSegmentCreation = TriggerSegmentCreation,
				IsSegmentEnder = IsSegmentEnder,
				IsAfterSegmentEnder = IsAfterSegmentEnder,
				SegmentationHintFinal = SegmentationHintFinal,
				MayExclude = MayExclude
			};
		}
	}
}
