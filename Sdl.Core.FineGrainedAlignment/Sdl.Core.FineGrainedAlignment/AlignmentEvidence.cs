using Sdl.Core.FineGrainedAlignment.Core;

namespace Sdl.Core.FineGrainedAlignment
{
	public abstract class AlignmentEvidence
	{
		public abstract short FirstSourceTokenIndex
		{
			get;
		}

		public abstract short FirstTargetTokenIndex
		{
			get;
		}

		public abstract short LastSourceTokenIndex
		{
			get;
		}

		public abstract short LastTargetTokenIndex
		{
			get;
		}

		public abstract bool Covers(short sourceTokenIndex, short targetTokenIndex);

		public abstract bool Concerns(LiftAlignedSpanPair pair, bool outside);

		public abstract float ConfidenceFor(LiftAlignedSpanPair pair, bool outside);

		public abstract float SourceConfidenceFor(LiftAlignedSpanPair pair, bool outside);

		public abstract float TargetConfidenceFor(LiftAlignedSpanPair pair, bool outside);

		public abstract bool AttemptToUse(LiftAlignedSpanPair pair, bool[] sourceTokenUseFlags, bool[] targetTokenUseFlags, bool outside);

		public abstract bool GetIsNoLongerValid(LiftAlignedSpanPair newAlignment);
	}
}
