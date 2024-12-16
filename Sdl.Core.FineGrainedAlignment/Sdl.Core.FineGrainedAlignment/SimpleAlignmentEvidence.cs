using Sdl.Core.FineGrainedAlignment.Core;
using System;

namespace Sdl.Core.FineGrainedAlignment
{
	public class SimpleAlignmentEvidence : AlignmentEvidence
	{
		private readonly LiftAlignedSpanPair _evidencePair;

		private readonly short _firstSourceTokenIndex;

		private readonly short _firstTargetTokenIndex;

		public override short FirstSourceTokenIndex => _firstSourceTokenIndex;

		public override short FirstTargetTokenIndex => _firstTargetTokenIndex;

		public override short LastSourceTokenIndex => _firstSourceTokenIndex;

		public override short LastTargetTokenIndex => _firstTargetTokenIndex;

		public SimpleAlignmentEvidence(short sourceIndex, short targetIndex, float confidence)
		{
			_evidencePair = new LiftAlignedSpanPair(new LiftSpan(sourceIndex, 1), new LiftSpan(targetIndex, 1));
			_evidencePair.Confidence = confidence;
			_firstSourceTokenIndex = _evidencePair.SourceStartIndex;
			_firstTargetTokenIndex = _evidencePair.TargetStartIndex;
		}

		public override bool GetIsNoLongerValid(LiftAlignedSpanPair newAlignment)
		{
			return newAlignment.Contradicts(_evidencePair);
		}

		public override bool Covers(short sourceTokenIndex, short targetTokenIndex)
		{
			if (_evidencePair.SourceSpan.Covers(sourceTokenIndex))
			{
				return _evidencePair.TargetSpan.Covers(targetTokenIndex);
			}
			return false;
		}

		public override bool Concerns(LiftAlignedSpanPair pair, bool outside)
		{
			if (!outside)
			{
				if (pair.SourceSpan.Overlaps(_evidencePair.SourceSpan))
				{
					return pair.TargetSpan.Overlaps(_evidencePair.TargetSpan);
				}
				return false;
			}
			if (!pair.SourceSpan.Overlaps(_evidencePair.SourceSpan))
			{
				return !pair.TargetSpan.Overlaps(_evidencePair.TargetSpan);
			}
			return false;
		}

		public override float ConfidenceFor(LiftAlignedSpanPair pair, bool outside)
		{
			return _evidencePair.Confidence;
		}

		public override float SourceConfidenceFor(LiftAlignedSpanPair pair, bool outside)
		{
			return _evidencePair.Confidence;
		}

		public override float TargetConfidenceFor(LiftAlignedSpanPair pair, bool outside)
		{
			return _evidencePair.Confidence;
		}

		public override bool AttemptToUse(LiftAlignedSpanPair pair, bool[] sourceTokenUseFlags, bool[] targetTokenUseFlags, bool outside)
		{
			if (outside)
			{
				for (int i = _evidencePair.SourceStartIndex; i <= _evidencePair.SourceEndIndex; i++)
				{
					if (!pair.SourceSpan.Covers(i) && sourceTokenUseFlags[i])
					{
						return false;
					}
				}
				for (int j = _evidencePair.TargetStartIndex; j <= _evidencePair.TargetEndIndex; j++)
				{
					if (!pair.TargetSpan.Covers(j) && targetTokenUseFlags[j])
					{
						return false;
					}
				}
				for (int k = _evidencePair.SourceStartIndex; k <= _evidencePair.SourceEndIndex; k++)
				{
					sourceTokenUseFlags[k] = true;
				}
				for (int l = _evidencePair.TargetStartIndex; l <= _evidencePair.TargetEndIndex; l++)
				{
					targetTokenUseFlags[l] = true;
				}
				return true;
			}
			short num = Math.Max(_evidencePair.SourceStartIndex, pair.SourceStartIndex);
			short num2 = Math.Min(_evidencePair.SourceEndIndex, pair.SourceEndIndex);
			for (short num3 = num; num3 <= num2; num3 = (short)(num3 + 1))
			{
				if (sourceTokenUseFlags[num3])
				{
					return false;
				}
			}
			short num4 = Math.Max(_evidencePair.TargetStartIndex, pair.TargetStartIndex);
			short num5 = Math.Min(_evidencePair.TargetEndIndex, pair.TargetEndIndex);
			for (short num6 = num4; num6 <= num5; num6 = (short)(num6 + 1))
			{
				if (targetTokenUseFlags[num6])
				{
					return false;
				}
			}
			for (short num7 = num; num7 <= num2; num7 = (short)(num7 + 1))
			{
				sourceTokenUseFlags[num7] = true;
			}
			for (short num8 = num4; num8 <= num5; num8 = (short)(num8 + 1))
			{
				targetTokenUseFlags[num8] = true;
			}
			return true;
		}
	}
}
