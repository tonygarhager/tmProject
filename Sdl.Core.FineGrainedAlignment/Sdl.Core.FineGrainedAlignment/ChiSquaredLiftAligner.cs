using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Stat;
using Sdl.LanguagePlatform.Stat.WordAlignment;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Sdl.Core.FineGrainedAlignment
{
	public class ChiSquaredLiftAligner : ChiSquaredAligner
	{
		private readonly LiftAligner _liftAligner;

		public ChiSquaredLiftAligner(ChiSquaredTranslationModel model)
			: base(model)
		{
			_liftAligner = new LiftAligner(model.SourceCulture, model.TargetCulture);
		}

		protected override AlignResult[] AlignInternal(IEnumerable<IAlignableContentPair> pairs, CancellationToken cancellationToken, IProgress<int> progress)
		{
			if (pairs == null)
			{
				throw new ArgumentNullException("pairs");
			}
			DataEncoder dataEncoder = new DataEncoder(_model.SourceCulture, _model.TargetCulture, forTrainedModel: false, _model.UseWordStems);
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (IAlignableContentPair pair2 in pairs)
			{
				if (pair2.SourceTokens == null || pair2.TargetTokens == null)
				{
					throw new Exception("Untokenized input");
				}
				dataEncoder.GetUniqueTokenStrings(pair2.SourceTokens, hashSet, forTraining: false, target: false);
				dataEncoder.GetUniqueTokenStrings(pair2.TargetTokens, hashSet2, forTraining: false, target: true);
			}
			_model.PartialLoad(hashSet, hashSet2);
			ChiSquareScoreProvider2 chiSquareScoreProvider = new ChiSquareScoreProvider2(_model.Matrix);
			foreach (IAlignableContentPair pair3 in pairs)
			{
				pair3.TranslationModelDate = _model.TranslationModelDate;
				if (pair3.SourceTokens.Count == 0 || pair3.TargetTokens.Count == 0)
				{
					pair3.AlignmentData = LiftAlignedSpanPairSet.CreateEmptyLiftAlignedSpanPairSet();
				}
				else
				{
					if (pair3.AlignmentData == null)
					{
						pair3.AlignmentData = new LiftAlignedSpanPairSet((short)pair3.SourceTokens.Count, (short)pair3.TargetTokens.Count);
					}
					else if (pair3.AlignmentData.Root().SourceLength != pair3.SourceTokens.Count || pair3.AlignmentData.Root().TargetLength != pair3.TargetTokens.Count)
					{
						throw new Exception("pair.AlignmentData is wrong size");
					}
					List<short> list = new List<short>();
					List<short> list2 = new List<short>();
					if (dataEncoder.Encode(pair3, _model.SourceVocab, _model.TargetVocab, list, list2, out IntSegment srcIntSegment, out IntSegment trgIntSegment, null, null, null, null))
					{
						List<short> featureIndicesOnly = SubsegmentUtilities.GetFeatureIndicesOnly(pair3.SourceTokens, _model.SourceCulture);
						List<short> featureIndicesOnly2 = SubsegmentUtilities.GetFeatureIndicesOnly(pair3.TargetTokens, _model.TargetCulture);
						if (featureIndicesOnly.Count != 0 && featureIndicesOnly2.Count != 0)
						{
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(featureIndicesOnly);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter2 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(featureIndicesOnly2);
							List<short> list3 = new List<short>();
							List<short> list4 = new List<short>();
							List<Token> sourceNonWhitespaceTokens = SubsegmentUtilities.GetNonWhitespaceTokens(pair3.SourceTokens, list3);
							List<Token> targetNonWhitespaceTokens = SubsegmentUtilities.GetNonWhitespaceTokens(pair3.TargetTokens, list4);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter3 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list3);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter4 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list4);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter5 = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)pair3.SourceTokens.Count);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter6 = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)pair3.TargetTokens.Count);
							LiftAlignedSpanPairSet liftAlignedSpanPairSet = SubsegmentUtilities.ConvertAlignmentSetIndices(pair3.AlignmentData, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter7 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter8 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list2);
							List<AlignmentEvidence> list5 = new List<AlignmentEvidence>();
							for (short num = 0; num < srcIntSegment.Count; num = (short)(num + 1))
							{
								int sW = srcIntSegment[num];
								for (short num2 = 0; num2 < trgIntSegment.Count; num2 = (short)(num2 + 1))
								{
									int tW = trgIntSegment[num2];
									double score = chiSquareScoreProvider.GetScore(sW, tW);
									if (score > 0.0)
									{
										score = Math.Min(score, 100.0);
										score /= 100.0;
										short index = rawIndexVsSignificantIndexConverter7.SignificantToRawIndex(num);
										short index2 = rawIndexVsSignificantIndexConverter8.SignificantToRawIndex(num2);
										short sourceIndex = (short)rawIndexVsSignificantIndexConverter3.RawToSignificantIndex(index);
										short targetIndex = (short)rawIndexVsSignificantIndexConverter4.RawToSignificantIndex(index2);
										SimpleAlignmentEvidence item = new SimpleAlignmentEvidence(sourceIndex, targetIndex, (float)score);
										list5.Add(item);
									}
								}
							}
							SignificantAlignableContentPair pair = new SignificantAlignableContentPair(pair3, liftAlignedSpanPairSet, list3, list4);
							_liftAligner.Align(pair, null, null, list5);
							liftAlignedSpanPairSet.Reduce();
							LiftAlignedSpanPairSet liftAlignedSpanPairSet2 = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4, rawIndexVsSignificantIndexConverter, rawIndexVsSignificantIndexConverter2);
							liftAlignedSpanPairSet2.Reduce();
							liftAlignedSpanPairSet2.DeduceFurtherAlignments(includesPunc: false, null, null);
							liftAlignedSpanPairSet2 = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet2, rawIndexVsSignificantIndexConverter, rawIndexVsSignificantIndexConverter2, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4);
							int num3 = SubsegmentUtilities.MergeAlignmentSets(liftAlignedSpanPairSet, liftAlignedSpanPairSet2);
							liftAlignedSpanPairSet.Reduce();
							liftAlignedSpanPairSet.DeduceFurtherAlignments(includesPunc: true, (short t) => sourceNonWhitespaceTokens[t].IsPunctuation, (short t) => targetNonWhitespaceTokens[t].IsPunctuation);
							liftAlignedSpanPairSet.Reduce();
							pair3.AlignmentData = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6);
						}
					}
					else
					{
						pair3.AlignmentData = LiftAlignedSpanPairSet.CreateEmptyLiftAlignedSpanPairSet();
					}
				}
			}
			return null;
		}
	}
}
