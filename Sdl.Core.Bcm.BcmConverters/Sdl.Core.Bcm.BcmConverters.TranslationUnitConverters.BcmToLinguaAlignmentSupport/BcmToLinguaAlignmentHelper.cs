using Sdl.Core.Bcm.BcmModel.Alignment;
using Sdl.Core.FineGrainedAlignment.Core;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.BcmToLinguaAlignmentSupport
{
	public static class BcmToLinguaAlignmentHelper
	{
		internal static LiftAlignedSpanPairSet BcmToLinguaAlignment(AlignmentData alignmentData)
		{
			List<SpanPairNode> spanPairSet = alignmentData.SpanPairSet;
			LiftAlignedSpanPairSet liftAlignedSpanPairSet;
			if ((spanPairSet != null && spanPairSet.Count == 0) || alignmentData.SpanPairSet == null)
			{
				liftAlignedSpanPairSet = LiftAlignedSpanPairSet.CreateEmptyLiftAlignedSpanPairSet();
			}
			else
			{
				short sourceLength = (short)alignmentData.SpanPairSet[0].SourceSpan.Length;
				short targetLength = (short)alignmentData.SpanPairSet[0].TargetSpan.Length;
				liftAlignedSpanPairSet = new LiftAlignedSpanPairSet(sourceLength, targetLength);
				for (int i = 1; i < alignmentData.SpanPairSet.Count; i++)
				{
					Sdl.Core.Bcm.BcmModel.Alignment.LiftSpan sourceSpan = alignmentData.SpanPairSet[i].SourceSpan;
					Sdl.Core.Bcm.BcmModel.Alignment.LiftSpan targetSpan = alignmentData.SpanPairSet[i].TargetSpan;
					LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair((short)sourceSpan.StartIndex, (short)sourceSpan.Length, (short)targetSpan.StartIndex, (short)targetSpan.Length);
					liftAlignedSpanPair.Confidence = alignmentData.SpanPairSet[i].Confidence;
					liftAlignedSpanPair.Provenance = (byte)alignmentData.SpanPairSet[i].Provenance;
					liftAlignedSpanPairSet.Add(liftAlignedSpanPair);
				}
			}
			return liftAlignedSpanPairSet;
		}
	}
}
