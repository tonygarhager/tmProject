using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Sdl.Core.FineGrainedAlignment
{
	public class TrainedModelAligner : IFineGrainedAligner
	{
		public class CodedTokenIndexConverter
		{
			private readonly List<short> _tokenIndexTranslationList;

			public int Count => _tokenIndexTranslationList.Count;

			public CodedTokenIndexConverter(List<short> tokenInfo)
			{
				_tokenIndexTranslationList = tokenInfo;
			}

			public int CodedTokenIndexToRawIndex(int ix)
			{
				return _tokenIndexTranslationList[ix];
			}

			public LiftSpan CodedTokenIndexToTUSpan(int ix)
			{
				LiftSpan liftSpan = new LiftSpan();
				liftSpan.StartIndex = _tokenIndexTranslationList[ix];
				liftSpan.Length = 1;
				return liftSpan;
			}

			public LiftSpan TUSpanToCodedTokenSpan(LiftSpan tuSpan)
			{
				int num = _tokenIndexTranslationList.IndexOf(tuSpan.StartIndex);
				if (num == -1)
				{
					return null;
				}
				int num2 = _tokenIndexTranslationList.IndexOf(tuSpan.EndIndex);
				if (num2 == -1)
				{
					return null;
				}
				return new LiftSpan((short)num, (short)(num2 - num + 1));
			}
		}

		private readonly TrainedTranslationModel _model;

		public bool BulkMode
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public TrainedModelAligner(TrainedTranslationModel model)
		{
			_model = model;
		}

		public AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress)
		{
			throw new NotImplementedException();
		}

		public bool[] CanAlign(IEnumerable<IAlignableContentPair> pairs)
		{
			if (pairs == null)
			{
				return null;
			}
			List<bool> list = new List<bool>();
			foreach (IAlignableContentPair pair in pairs)
			{
				if (pair.SourceTokens == null || pair.TargetTokens == null)
				{
					throw new Exception("Untokenized input");
				}
				list.Add(CanAlign(pair));
			}
			return list.ToArray();
		}

		public void SetErrorLogger(Action<Exception, string> logger)
		{
			throw new NotImplementedException();
		}

		public bool Align(IEnumerable<IAlignableContentPair> pairs)
		{
			if (!_model.TranslationModelDate.HasValue)
			{
				foreach (IAlignableContentPair pair in pairs)
				{
					pair.AlignmentData = null;
					pair.TranslationModelDate = null;
				}
				return false;
			}
			AlignInternal(pairs);
			return true;
		}

		private SymmetrizedAlignmentResults SymmetrizeAlignments(List<WordAlignmentWithAlternatives> sourceToTargetAlignments, List<WordAlignmentWithAlternatives> targetToSourceAlignments)
		{
			SymmetrizedAlignmentResults symmetrizedAlignmentResults = new SymmetrizedAlignmentResults();
			for (int i = 0; i < sourceToTargetAlignments.Count; i++)
			{
				WordAlignmentWithAlternatives wordAlignmentWithAlternatives = sourceToTargetAlignments[i];
				if (wordAlignmentWithAlternatives.otherWordIndex == -1)
				{
					symmetrizedAlignmentResults.SymmetrizedAlignments.Add(null);
					symmetrizedAlignmentResults.AsymmetricalSourceToTargetAlignments.Add(wordAlignmentWithAlternatives);
				}
				else if (targetToSourceAlignments[wordAlignmentWithAlternatives.otherWordIndex].otherWordIndex == i)
				{
					WordAlignment wordAlignment = new WordAlignment(AlignmentDirection.sourceToTarget);
					wordAlignment.otherWordIndex = wordAlignmentWithAlternatives.otherWordIndex;
					wordAlignment.confidence = (wordAlignmentWithAlternatives.confidence + targetToSourceAlignments[wordAlignmentWithAlternatives.otherWordIndex].confidence) / 2.0;
					symmetrizedAlignmentResults.SymmetrizedAlignments.Add(wordAlignment);
					symmetrizedAlignmentResults.AsymmetricalSourceToTargetAlignments.Add(null);
				}
				else
				{
					symmetrizedAlignmentResults.SymmetrizedAlignments.Add(null);
					symmetrizedAlignmentResults.AsymmetricalSourceToTargetAlignments.Add(wordAlignmentWithAlternatives);
				}
			}
			for (int j = 0; j < targetToSourceAlignments.Count; j++)
			{
				WordAlignmentWithAlternatives wordAlignmentWithAlternatives2 = targetToSourceAlignments[j];
				if (wordAlignmentWithAlternatives2.otherWordIndex == -1)
				{
					symmetrizedAlignmentResults.AsymmetricalTargetToSourceAlignments.Add(wordAlignmentWithAlternatives2);
				}
				else if (sourceToTargetAlignments[wordAlignmentWithAlternatives2.otherWordIndex].otherWordIndex != j)
				{
					symmetrizedAlignmentResults.AsymmetricalTargetToSourceAlignments.Add(wordAlignmentWithAlternatives2);
				}
			}
			return symmetrizedAlignmentResults;
		}

		private int AlignTargetToSource(List<WordAlignmentWithAlternatives> t2sAlignments, IntSegment srcIntSegment, IntSegment trgIntSegment, SparseMatrix<double> translationTable, int srcNullTokenKey, double initialProb, bool reversed)
		{
			int num = 0;
			for (int i = 0; i < trgIntSegment.Count; i++)
			{
				WordAlignmentWithAlternatives wordAlignmentWithAlternatives = t2sAlignments[i];
				int row = trgIntSegment[i];
				double confidence = Math.Max(translationTable[row, srcNullTokenKey], 1E-12);
				int num2 = -1;
				wordAlignmentWithAlternatives.allAlternatives.Add(new WordAlignment((!reversed) ? AlignmentDirection.targetToSource : AlignmentDirection.sourceToTarget)
				{
					confidence = confidence,
					otherWordIndex = -1
				});
				for (int j = 0; j < srcIntSegment.Count; j++)
				{
					int column = srcIntSegment[j];
					if (translationTable.HasValue(row, column))
					{
						double confidence2 = translationTable[row, column];
						wordAlignmentWithAlternatives.allAlternatives.Add(new WordAlignment((!reversed) ? AlignmentDirection.targetToSource : AlignmentDirection.sourceToTarget)
						{
							confidence = confidence2,
							otherWordIndex = j
						});
					}
				}
				wordAlignmentWithAlternatives.allAlternatives.Sort((WordAlignment a, WordAlignment b) => b.confidence.CompareTo(a.confidence));
				if (!wordAlignmentWithAlternatives.IsAmbiguous())
				{
					wordAlignmentWithAlternatives.UseBest();
					num2 = wordAlignmentWithAlternatives.otherWordIndex;
					num++;
				}
			}
			return num;
		}

		private bool CanAlign(IAlignableContentPair pair)
		{
			if (pair.SourceTokens.Count > 200 || pair.TargetTokens.Count > 200)
			{
				return false;
			}
			return true;
		}

		private void AlignInternal(IEnumerable<IAlignableContentPair> pairs)
		{
			if (pairs == null)
			{
				throw new ArgumentNullException("pairs");
			}
			DataEncoder dataEncoder = new DataEncoder(_model.SourceCulture, _model.TargetCulture, forTrainedModel: true, stemming: false);
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (IAlignableContentPair pair in pairs)
			{
				if (pair.SourceTokens == null || pair.TargetTokens == null)
				{
					throw new Exception("Untokenized input");
				}
				dataEncoder.GetUniqueTokenStrings(pair.SourceTokens, hashSet, forTraining: true, target: false);
				dataEncoder.GetUniqueTokenStrings(pair.TargetTokens, hashSet2, forTraining: true, target: true);
			}
			_model.PartialLoad(hashSet, hashSet2);
			double initialProb = 1.0 / (double)_model.TotalVocabSize(target: false);
			double reversedInitialProb = 1.0 / (double)_model.TotalVocabSize(target: true);
			int srcNullTokenKey = _model.SourceVocab.Lookup(" ");
			int trgNullTokenKey = _model.SourceVocab.Lookup(" ");
			foreach (IAlignableContentPair pair2 in pairs)
			{
				pair2.TranslationModelDate = _model.TranslationModelDate;
				if (pair2.SourceTokens.Count == 0 || pair2.TargetTokens.Count == 0 || !CanAlign(pair2))
				{
					pair2.AlignmentData = LiftAlignedSpanPairSet.CreateEmptyLiftAlignedSpanPairSet();
				}
				else
				{
					if (pair2.AlignmentData == null)
					{
						pair2.AlignmentData = new LiftAlignedSpanPairSet((short)pair2.SourceTokens.Count, (short)pair2.TargetTokens.Count);
					}
					else if (pair2.AlignmentData.Root().SourceLength != pair2.SourceTokens.Count || pair2.AlignmentData.Root().TargetLength != pair2.TargetTokens.Count)
					{
						throw new Exception("pair.AlignmentData is wrong size");
					}
					List<short> list = new List<short>();
					List<short> list2 = new List<short>();
					List<bool> srcStopWords = new List<bool>();
					List<bool> trgStopWords = new List<bool>();
					if (dataEncoder.Encode(pair2, _model.SourceVocab, _model.TargetVocab, list, list2, out IntSegment srcIntSegment, out IntSegment trgIntSegment, srcStopWords, trgStopWords, null, null))
					{
						List<short> featureIndicesOnly = SubsegmentUtilities.GetFeatureIndicesOnly(pair2.SourceTokens, _model.SourceCulture);
						List<short> featureIndicesOnly2 = SubsegmentUtilities.GetFeatureIndicesOnly(pair2.TargetTokens, _model.TargetCulture);
						if (featureIndicesOnly.Count != 0 && featureIndicesOnly2.Count != 0)
						{
							List<short> list3 = new List<short>();
							List<short> list4 = new List<short>();
							List<Token> sourceNonWhitespaceTokens = SubsegmentUtilities.GetNonWhitespaceTokens(pair2.SourceTokens, list3);
							List<Token> targetNonWhitespaceTokens = SubsegmentUtilities.GetNonWhitespaceTokens(pair2.TargetTokens, list4);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(featureIndicesOnly);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter2 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(featureIndicesOnly2);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter3 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list3);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter4 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list4);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter5 = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)pair2.SourceTokens.Count);
							SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter6 = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)pair2.TargetTokens.Count);
							LiftAlignedSpanPairSet liftAlignedSpanPairSet = SubsegmentUtilities.ConvertAlignmentSetIndices(pair2.AlignmentData, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4);
							CodedTokenIndexConverter srcCodedIxConverter = new CodedTokenIndexConverter(list);
							CodedTokenIndexConverter trgCodedIxConverter = new CodedTokenIndexConverter(list2);
							AlignmentStrategy4(pair2, srcCodedIxConverter, trgCodedIxConverter, srcIntSegment, trgIntSegment, srcNullTokenKey, initialProb, trgNullTokenKey, reversedInitialProb, liftAlignedSpanPairSet, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4, srcStopWords, trgStopWords);
							liftAlignedSpanPairSet.Reduce();
							LiftAlignedSpanPairSet liftAlignedSpanPairSet2 = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4, rawIndexVsSignificantIndexConverter, rawIndexVsSignificantIndexConverter2);
							liftAlignedSpanPairSet2.Reduce();
							liftAlignedSpanPairSet2.DeduceFurtherAlignments(includesPunc: false, null, null);
							liftAlignedSpanPairSet2 = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet2, rawIndexVsSignificantIndexConverter, rawIndexVsSignificantIndexConverter2, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4);
							int num = SubsegmentUtilities.MergeAlignmentSets(liftAlignedSpanPairSet, liftAlignedSpanPairSet2);
							liftAlignedSpanPairSet.Reduce();
							liftAlignedSpanPairSet.DeduceFurtherAlignments(includesPunc: true, (short t) => sourceNonWhitespaceTokens[t].IsPunctuation, (short t) => targetNonWhitespaceTokens[t].IsPunctuation);
							liftAlignedSpanPairSet.Reduce();
							pair2.AlignmentData = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6);
						}
					}
					else
					{
						pair2.AlignmentData = LiftAlignedSpanPairSet.CreateEmptyLiftAlignedSpanPairSet();
					}
				}
			}
		}

		private bool IsSourceWordAligned(double?[,] alignmentMatrix, int sourceIx)
		{
			int upperBound = alignmentMatrix.GetUpperBound(1);
			for (int i = 0; i <= upperBound; i++)
			{
				if (alignmentMatrix[sourceIx, i].HasValue)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsTargetWordAligned(double?[,] alignmentMatrix, int targetIx)
		{
			int upperBound = alignmentMatrix.GetUpperBound(0);
			for (int i = 0; i <= upperBound; i++)
			{
				if (alignmentMatrix[i, targetIx].HasValue)
				{
					return true;
				}
			}
			return false;
		}

		private List<Pair<int>> GetNeighbouringPoints(int x, int y, int maxx, int maxy)
		{
			List<Pair<int>> list = new List<Pair<int>>();
			if (y > 0)
			{
				if (x > 0)
				{
					list.Add(new Pair<int>(x - 1, y - 1));
				}
				list.Add(new Pair<int>(x, y - 1));
				if (x < maxx)
				{
					list.Add(new Pair<int>(x + 1, y - 1));
				}
			}
			if (x > 0)
			{
				list.Add(new Pair<int>(x - 1, y));
			}
			if (x < maxx)
			{
				list.Add(new Pair<int>(x + 1, y));
			}
			if (y < maxy)
			{
				if (x > 0)
				{
					list.Add(new Pair<int>(x - 1, y + 1));
				}
				list.Add(new Pair<int>(x, y + 1));
				if (x < maxx)
				{
					list.Add(new Pair<int>(x + 1, y + 1));
				}
			}
			return list;
		}

		private void AlignmentStrategy4(IAlignableContentPair pair, CodedTokenIndexConverter srcCodedIxConverter, CodedTokenIndexConverter trgCodedIxConverter, IntSegment srcIntSegment, IntSegment trgIntSegment, int srcNullTokenKey, double initialProb, int trgNullTokenKey, double reversedInitialProb, LiftAlignedSpanPairSet alignmentDataAllButWhitespace, SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcNonWhitespaceSpanConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgNonWhitespaceSpanConverter, List<bool> srcStopWords, List<bool> trgStopWords)
		{
			List<WordAlignmentWithAlternatives> list = new List<WordAlignmentWithAlternatives>();
			List<WordAlignmentWithAlternatives> list2 = new List<WordAlignmentWithAlternatives>();
			for (int i = 0; i < trgIntSegment.Count; i++)
			{
				list2.Add(new WordAlignmentWithAlternatives(AlignmentDirection.targetToSource, trgStopWords[i], _model.TargetVocab.LookupFull(trgIntSegment[i]).Count)
				{
					otherWordIndex = -1
				});
			}
			for (int j = 0; j < srcIntSegment.Count; j++)
			{
				list.Add(new WordAlignmentWithAlternatives(AlignmentDirection.sourceToTarget, srcStopWords[j], _model.SourceVocab.LookupFull(srcIntSegment[j]).Count)
				{
					otherWordIndex = -1
				});
			}
			int num = AlignTargetToSource(list2, srcIntSegment, trgIntSegment, _model.Matrix, srcNullTokenKey, initialProb, reversed: false);
			num += AlignTargetToSource(list, trgIntSegment, srcIntSegment, _model.ReverseMatrix, trgNullTokenKey, reversedInitialProb, reversed: true);
			SymmetrizedAlignmentResults results = SymmetrizeAlignments(list, list2);
			double?[,] array = new double?[srcIntSegment.Count, trgIntSegment.Count];
			int num2 = ConvertSymmetrizedAlignmentToMatrix(srcIntSegment, srcStopWords, trgStopWords, skipStopWords: true, results, array);
			ImportAlignments(array, alignmentDataAllButWhitespace, pair.SourceTokens, pair.TargetTokens, srcCodedIxConverter, trgCodedIxConverter, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter);
			if (num2 == 0)
			{
				return;
			}
			bool[,] array2 = new bool[srcIntSegment.Count, trgIntSegment.Count];
			int generation = 0;
			GrowDiag(array, srcIntSegment, trgIntSegment, pair, srcCodedIxConverter, trgCodedIxConverter, array2, list, list2, alignmentDataAllButWhitespace, generation, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter, srcStopWords, trgStopWords, skipStopWords: true);
			bool unalignedFound = false;
			bool ambiguousFound = false;
			RedeemAmbiguous(srcIntSegment, trgIntSegment, srcStopWords, trgStopWords, skipStopWords: true, array, srcCodedIxConverter, trgCodedIxConverter, list, list2, out unalignedFound, out ambiguousFound, pair);
			generation = 2;
			if (!unalignedFound)
			{
				return;
			}
			if (ambiguousFound)
			{
				GrowDiag(array, srcIntSegment, trgIntSegment, pair, srcCodedIxConverter, trgCodedIxConverter, array2, list, list2, alignmentDataAllButWhitespace, generation, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter, srcStopWords, trgStopWords, skipStopWords: true);
			}
			int num3 = 3;
			if (num3 != -1)
			{
				foreach (WordAlignmentWithAlternatives item in list)
				{
					if (item.allAlternatives.Count > num3)
					{
						item.allAlternatives = new List<WordAlignment>(item.allAlternatives.GetRange(0, num3));
					}
					item.StoreAlternatives();
				}
				foreach (WordAlignmentWithAlternatives item2 in list2)
				{
					if (item2.allAlternatives.Count > num3)
					{
						item2.allAlternatives = new List<WordAlignment>(item2.allAlternatives.GetRange(0, num3));
					}
					item2.StoreAlternatives();
				}
			}
			TryAlternatives(ref generation, srcIntSegment, trgIntSegment, srcStopWords, trgStopWords, skipStopWords: true, array, list, list2, srcCodedIxConverter, trgCodedIxConverter, pair, array2, alignmentDataAllButWhitespace, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter);
			ConvertSymmetrizedAlignmentToMatrix(srcIntSegment, srcStopWords, trgStopWords, skipStopWords: false, results, array);
			ImportAlignments(array, alignmentDataAllButWhitespace, pair.SourceTokens, pair.TargetTokens, srcCodedIxConverter, trgCodedIxConverter, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter);
			generation++;
			GrowDiag(array, srcIntSegment, trgIntSegment, pair, srcCodedIxConverter, trgCodedIxConverter, array2, list, list2, alignmentDataAllButWhitespace, generation, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter, srcStopWords, trgStopWords, skipStopWords: false);
			unalignedFound = false;
			ambiguousFound = false;
			RedeemAmbiguous(srcIntSegment, trgIntSegment, srcStopWords, trgStopWords, skipStopWords: false, array, srcCodedIxConverter, trgCodedIxConverter, list, list2, out unalignedFound, out ambiguousFound, pair);
			generation++;
			if (!unalignedFound)
			{
				return;
			}
			if (ambiguousFound)
			{
				GrowDiag(array, srcIntSegment, trgIntSegment, pair, srcCodedIxConverter, trgCodedIxConverter, array2, list, list2, alignmentDataAllButWhitespace, generation, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter, srcStopWords, trgStopWords, skipStopWords: false);
			}
			foreach (WordAlignmentWithAlternatives item3 in list)
			{
				item3.RestoreAlternatives();
				item3.UseBest();
			}
			foreach (WordAlignmentWithAlternatives item4 in list2)
			{
				item4.RestoreAlternatives();
				item4.UseBest();
			}
			TryAlternatives(ref generation, srcIntSegment, trgIntSegment, srcStopWords, trgStopWords, skipStopWords: false, array, list, list2, srcCodedIxConverter, trgCodedIxConverter, pair, array2, alignmentDataAllButWhitespace, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter);
			foreach (WordAlignmentWithAlternatives item5 in list)
			{
				if (item5.allAlternatives.Count > num3)
				{
					item5.allAlternatives = new List<WordAlignment>(item5.allAlternatives.GetRange(0, num3));
				}
				item5.RestoreAlternatives();
				item5.UseBest();
			}
			foreach (WordAlignmentWithAlternatives item6 in list2)
			{
				if (item6.allAlternatives.Count > num3)
				{
					item6.allAlternatives = new List<WordAlignment>(item6.allAlternatives.GetRange(0, num3));
				}
				item6.RestoreAlternatives();
				item6.UseBest();
			}
			for (int k = 0; k < srcIntSegment.Count; k++)
			{
				if (srcStopWords[k])
				{
					continue;
				}
				for (int l = 0; l < trgIntSegment.Count; l++)
				{
					if (trgStopWords[l])
					{
						continue;
					}
					bool flag = false;
					if (IsSourceWordAligned(array, k) && IsTargetWordAligned(array, l))
					{
						continue;
					}
					bool flag2 = false;
					bool flag3 = false;
					if (list2[l].otherWordIndex == k)
					{
						flag2 = true;
					}
					if (list[k].otherWordIndex == l)
					{
						flag3 = true;
					}
					if (!(flag2 | flag3))
					{
						continue;
					}
					int num4 = -1;
					if (k != -1)
					{
						num4 = srcCodedIxConverter.CodedTokenIndexToRawIndex(k);
					}
					int num5 = -1;
					if (l != -1)
					{
						num5 = trgCodedIxConverter.CodedTokenIndexToRawIndex(l);
					}
					double num6 = 0.0;
					if (flag2 && flag3)
					{
						num6 = (list2[l].confidence + list[k].confidence) / 2.0;
						flag = (list2[l].IsAmbiguous() && list[k].IsAmbiguous());
					}
					else
					{
						if (flag2)
						{
							flag = list2[l].IsAmbiguous();
							num6 = list2[l].confidence;
						}
						if (flag3)
						{
							num6 = list[k].confidence;
							flag = list[k].IsAmbiguous();
						}
					}
					int sourceTokenIx = srcNonWhitespaceSpanConverter.RawToSignificantIndex((short)num4);
					int targetTokenIx = trgNonWhitespaceSpanConverter.RawToSignificantIndex((short)num5);
					if (!flag)
					{
						if (!AddorGrowAlignment(alignmentDataAllButWhitespace, sourceTokenIx, targetTokenIx, num6))
						{
							array2[k, l] = true;
						}
						else
						{
							array[k, l] = num6;
						}
					}
				}
			}
		}

		private void TryAlternatives(ref int generation, IntSegment srcIntSegment, IntSegment trgIntSegment, List<bool> srcStopWords, List<bool> trgStopWords, bool skipStopWords, double?[,] alignmentMatrix, List<WordAlignmentWithAlternatives> s2tAlignments, List<WordAlignmentWithAlternatives> t2sAlignments, CodedTokenIndexConverter srcCodedIxConverter, CodedTokenIndexConverter trgCodedIxConverter, IAlignableContentPair pair, bool[,] contradictoryMatrix, LiftAlignedSpanPairSet alignmentDataAllButWhitespace, SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcNonWhitespaceSpanConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgNonWhitespaceSpanConverter)
		{
			while (true)
			{
				generation++;
				bool flag = false;
				WordAlignmentWithAlternatives wordAlignmentWithAlternatives = null;
				int num = -1;
				for (int i = 0; i < srcIntSegment.Count; i++)
				{
					if ((!skipStopWords || !srcStopWords[i]) && !IsSourceWordAligned(alignmentMatrix, i))
					{
						flag = true;
						WordAlignmentWithAlternatives wordAlignmentWithAlternatives2 = s2tAlignments[i];
						if (wordAlignmentWithAlternatives2.allAlternatives.Count > 1 && (wordAlignmentWithAlternatives == null || wordAlignmentWithAlternatives.allAlternatives[1].confidence < wordAlignmentWithAlternatives2.allAlternatives[1].confidence))
						{
							wordAlignmentWithAlternatives = wordAlignmentWithAlternatives2;
							num = i;
						}
					}
				}
				for (int j = 0; j < trgIntSegment.Count; j++)
				{
					if ((!skipStopWords || !trgStopWords[j]) && !IsTargetWordAligned(alignmentMatrix, j))
					{
						flag = true;
						WordAlignmentWithAlternatives wordAlignmentWithAlternatives3 = t2sAlignments[j];
						if (wordAlignmentWithAlternatives3.allAlternatives.Count > 1 && (wordAlignmentWithAlternatives == null || wordAlignmentWithAlternatives.allAlternatives[1].confidence < wordAlignmentWithAlternatives3.allAlternatives[1].confidence))
						{
							wordAlignmentWithAlternatives = wordAlignmentWithAlternatives3;
							num = j;
						}
					}
				}
				if (flag && wordAlignmentWithAlternatives != null)
				{
					wordAlignmentWithAlternatives.allAlternatives.RemoveAt(0);
					wordAlignmentWithAlternatives.UseBest();
					int num2 = num;
					int num3 = wordAlignmentWithAlternatives.otherWordIndex;
					if (wordAlignmentWithAlternatives.direction == AlignmentDirection.targetToSource)
					{
						num2 = wordAlignmentWithAlternatives.otherWordIndex;
						num3 = num;
					}
					int num4 = -1;
					if (num2 != -1)
					{
						num4 = srcCodedIxConverter.CodedTokenIndexToRawIndex(num2);
					}
					int num5 = -1;
					if (num3 != -1)
					{
						num5 = trgCodedIxConverter.CodedTokenIndexToRawIndex(num3);
					}
					GrowDiagSingle(wordAlignmentWithAlternatives, num2, num3, alignmentMatrix, srcIntSegment, trgIntSegment, pair, srcCodedIxConverter, trgCodedIxConverter, contradictoryMatrix, s2tAlignments, t2sAlignments, alignmentDataAllButWhitespace, generation, srcNonWhitespaceSpanConverter, trgNonWhitespaceSpanConverter);
					continue;
				}
				break;
			}
		}

		private void RedeemAmbiguous(IntSegment srcIntSegment, IntSegment trgIntSegment, List<bool> srcStopWords, List<bool> trgStopWords, bool skipStopWords, double?[,] alignmentMatrix, CodedTokenIndexConverter srcCodedIxConverter, CodedTokenIndexConverter trgCodedIxConverter, List<WordAlignmentWithAlternatives> s2tAlignments, List<WordAlignmentWithAlternatives> t2sAlignments, out bool unalignedFound, out bool ambiguousFound, IAlignableContentPair pair)
		{
			unalignedFound = false;
			ambiguousFound = false;
			for (int i = 0; i < srcIntSegment.Count; i++)
			{
				if ((skipStopWords && srcStopWords[i]) || IsSourceWordAligned(alignmentMatrix, i))
				{
					continue;
				}
				unalignedFound = true;
				WordAlignmentWithAlternatives wordAlignmentWithAlternatives = s2tAlignments[i];
				if (wordAlignmentWithAlternatives.IsAmbiguous() && wordAlignmentWithAlternatives.Freq > 1)
				{
					ambiguousFound = true;
					wordAlignmentWithAlternatives.UseBest();
					int otherWordIndex = wordAlignmentWithAlternatives.otherWordIndex;
					int num = -1;
					if (i != -1)
					{
						num = srcCodedIxConverter.CodedTokenIndexToRawIndex(i);
					}
					int num2 = -1;
					if (otherWordIndex != -1)
					{
						num2 = trgCodedIxConverter.CodedTokenIndexToRawIndex(otherWordIndex);
					}
				}
			}
			for (int j = 0; j < trgIntSegment.Count; j++)
			{
				if ((skipStopWords && trgStopWords[j]) || IsTargetWordAligned(alignmentMatrix, j))
				{
					continue;
				}
				unalignedFound = true;
				WordAlignmentWithAlternatives wordAlignmentWithAlternatives2 = t2sAlignments[j];
				if (wordAlignmentWithAlternatives2.IsAmbiguous() && wordAlignmentWithAlternatives2.Freq > 1)
				{
					ambiguousFound = true;
					wordAlignmentWithAlternatives2.UseBest();
					int otherWordIndex2 = wordAlignmentWithAlternatives2.otherWordIndex;
					int num3 = -1;
					if (otherWordIndex2 != -1)
					{
						num3 = srcCodedIxConverter.CodedTokenIndexToRawIndex(otherWordIndex2);
					}
					int num4 = -1;
					if (j != -1)
					{
						num4 = trgCodedIxConverter.CodedTokenIndexToRawIndex(j);
					}
				}
			}
		}

		private int ConvertSymmetrizedAlignmentToMatrix(IntSegment srcIntSegment, List<bool> srcStopWords, List<bool> trgStopWords, bool skipStopWords, SymmetrizedAlignmentResults results, double?[,] alignmentMatrix)
		{
			int num = 0;
			for (int i = 0; i < srcIntSegment.Count; i++)
			{
				if (!skipStopWords || !srcStopWords[i])
				{
					WordAlignment wordAlignment = results.SymmetrizedAlignments[i];
					if (wordAlignment != null && (wordAlignment.otherWordIndex == -1 || !skipStopWords || !trgStopWords[wordAlignment.otherWordIndex]))
					{
						alignmentMatrix[i, wordAlignment.otherWordIndex] = wordAlignment.confidence;
						num++;
					}
				}
			}
			return num;
		}

		private string GetTokenStringOrNull(List<Token> tokens, int ix)
		{
			if (ix == -1)
			{
				return "NULL";
			}
			return tokens[ix].ToString();
		}

		private bool GrowDiagSingle(WordAlignmentWithAlternatives a, int sourceIx, int targetIx, double?[,] alignmentMatrix, IntSegment srcIntSegment, IntSegment trgIntSegment, IAlignableContentPair pair, CodedTokenIndexConverter srcCodedIxConverter, CodedTokenIndexConverter trgCodedIxConverter, bool[,] contradictoryMatrix, List<WordAlignmentWithAlternatives> s2tAlignments, List<WordAlignmentWithAlternatives> t2sAlignments, LiftAlignedSpanPairSet alignmentDataAllButWhitespace, int generation, SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcNonWhitespaceSpanConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgNonWhitespaceSpanConverter)
		{
			if (sourceIx == -1 || targetIx == -1)
			{
				return false;
			}
			if (contradictoryMatrix[sourceIx, targetIx])
			{
				return false;
			}
			List<Pair<int>> neighbouringPoints = GetNeighbouringPoints(sourceIx, targetIx, srcIntSegment.Count - 1, trgIntSegment.Count - 1);
			int num = srcCodedIxConverter.CodedTokenIndexToRawIndex(sourceIx);
			int num2 = trgCodedIxConverter.CodedTokenIndexToRawIndex(targetIx);
			foreach (Pair<int> item in neighbouringPoints)
			{
				if (alignmentMatrix[item.Left, item.Right].HasValue)
				{
					int sourceTokenIx = srcNonWhitespaceSpanConverter.RawToSignificantIndex((short)num);
					int targetTokenIx = trgNonWhitespaceSpanConverter.RawToSignificantIndex((short)num2);
					if (!AddorGrowAlignment(alignmentDataAllButWhitespace, sourceTokenIx, targetTokenIx, a.confidence))
					{
						contradictoryMatrix[item.Left, item.Right] = true;
						return false;
					}
					alignmentMatrix[item.Left, item.Right] = a.confidence;
					return true;
				}
			}
			return false;
		}

		private int GrowDiag(double?[,] alignmentMatrix, IntSegment srcIntSegment, IntSegment trgIntSegment, IAlignableContentPair pair, CodedTokenIndexConverter srcCodedIxConverter, CodedTokenIndexConverter trgCodedIxConverter, bool[,] contradictoryMatrix, List<WordAlignmentWithAlternatives> s2tAlignments, List<WordAlignmentWithAlternatives> t2sAlignments, LiftAlignedSpanPairSet alignmentDataAllButWhitespace, int generation, SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcNonWhitespaceSpanConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgNonWhitespaceSpanConverter, List<bool> srcStopWords, List<bool> trgStopWords, bool skipStopWords)
		{
			int num = 0;
			int num2;
			do
			{
				num2 = 0;
				for (int i = 0; i < srcIntSegment.Count; i++)
				{
					for (int j = 0; j < trgIntSegment.Count; j++)
					{
						if (alignmentMatrix[i, j].HasValue)
						{
							int num3 = srcCodedIxConverter.CodedTokenIndexToRawIndex(i);
							int num4 = trgCodedIxConverter.CodedTokenIndexToRawIndex(j);
							List<Pair<int>> neighbouringPoints = GetNeighbouringPoints(i, j, srcIntSegment.Count - 1, trgIntSegment.Count - 1);
							foreach (Pair<int> item in neighbouringPoints)
							{
								if ((!skipStopWords || !srcStopWords[item.Left]) && !trgStopWords[item.Right] && !alignmentMatrix[item.Left, item.Right].HasValue && !contradictoryMatrix[item.Left, item.Right])
								{
									bool flag = s2tAlignments[item.Left].otherWordIndex == item.Right;
									bool flag2 = t2sAlignments[item.Right].otherWordIndex == item.Left;
									if (flag | flag2)
									{
										num3 = srcCodedIxConverter.CodedTokenIndexToRawIndex(item.Left);
										num4 = trgCodedIxConverter.CodedTokenIndexToRawIndex(item.Right);
										double num5 = 0.0;
										num5 = ((flag && flag2) ? ((s2tAlignments[item.Left].confidence + t2sAlignments[item.Right].confidence) / 2.0) : ((!flag) ? t2sAlignments[item.Right].confidence : s2tAlignments[item.Left].confidence));
										num2++;
										int sourceTokenIx = srcNonWhitespaceSpanConverter.RawToSignificantIndex((short)num3);
										int targetTokenIx = trgNonWhitespaceSpanConverter.RawToSignificantIndex((short)num4);
										if (!AddorGrowAlignment(alignmentDataAllButWhitespace, sourceTokenIx, targetTokenIx, alignmentMatrix[i, j].Value))
										{
											contradictoryMatrix[item.Left, item.Right] = true;
										}
										else
										{
											alignmentMatrix[item.Left, item.Right] = num5;
										}
									}
								}
							}
						}
					}
				}
				num += num2;
			}
			while (num2 != 0);
			return num;
		}

		private bool AddorGrowAlignment(LiftAlignedSpanPairSet alignmentData, int sourceTokenIx, int targetTokenIx, double confidence)
		{
			LiftSpan liftSpan = new LiftSpan((short)sourceTokenIx, 1);
			LiftSpan liftSpan2 = new LiftSpan((short)targetTokenIx, 1);
			LiftAlignedSpanPair liftAlignedSpanPair = alignmentData.GetSmallestContainingPair(liftSpan, searchTargetText: false, exceptionIfContradicts: false);
			if (liftAlignedSpanPair.SourceLength == alignmentData.Root().SourceLength)
			{
				liftAlignedSpanPair = null;
			}
			LiftAlignedSpanPair liftAlignedSpanPair2 = alignmentData.GetSmallestContainingPair(liftSpan2, searchTargetText: true, exceptionIfContradicts: false);
			if (liftAlignedSpanPair2.TargetLength == alignmentData.Root().TargetLength)
			{
				liftAlignedSpanPair2 = null;
			}
			LiftAlignedSpanPair liftAlignedSpanPair3 = new LiftAlignedSpanPair(liftSpan, liftSpan2);
			liftAlignedSpanPair3.Confidence = (float)confidence;
			liftAlignedSpanPair3.Provenance = 10;
			if (liftAlignedSpanPair == null && liftAlignedSpanPair2 == null)
			{
				if (alignmentData.Contradicts(liftAlignedSpanPair3, repetitionIsContradiction: true))
				{
					return false;
				}
				alignmentData.Add(liftAlignedSpanPair3);
				return true;
			}
			if (liftAlignedSpanPair != null && liftAlignedSpanPair2 != null)
			{
				if (liftAlignedSpanPair != liftAlignedSpanPair2)
				{
					return false;
				}
				if (liftAlignedSpanPair.SourceLength == 1 || liftAlignedSpanPair2.TargetLength == 1)
				{
					return false;
				}
				if (alignmentData.Contradicts(liftAlignedSpanPair3, repetitionIsContradiction: true))
				{
					return false;
				}
				alignmentData.Add(liftAlignedSpanPair3);
				return true;
			}
			int num2;
			double num3;
			if (liftAlignedSpanPair != null)
			{
				if (targetTokenIx != liftAlignedSpanPair.TargetEndIndex + 1 && targetTokenIx != liftAlignedSpanPair.TargetStartIndex - 1)
				{
					return false;
				}
				short targetLength = liftAlignedSpanPair.TargetLength;
				short num = liftAlignedSpanPair.TargetStartIndex;
				targetLength = (short)(targetLength + 1);
				if (targetTokenIx == liftAlignedSpanPair.TargetStartIndex - 1)
				{
					num = (short)(num - 1);
				}
				num2 = liftAlignedSpanPair.SourceLength + liftAlignedSpanPair.TargetLength;
				num3 = ((double)(liftAlignedSpanPair.Confidence * (float)num2) + confidence) / (double)(num2 + 1);
				liftAlignedSpanPair3 = new LiftAlignedSpanPair(liftAlignedSpanPair.SourceStartIndex, liftAlignedSpanPair.SourceLength, num, targetLength);
				liftAlignedSpanPair3.Confidence = (float)num3;
				liftAlignedSpanPair3.Provenance = 10;
				alignmentData.Remove(liftAlignedSpanPair);
				if (alignmentData.Contradicts(liftAlignedSpanPair3, repetitionIsContradiction: true))
				{
					alignmentData.Add(liftAlignedSpanPair);
					return false;
				}
				alignmentData.Add(liftAlignedSpanPair3);
				return true;
			}
			if (sourceTokenIx != liftAlignedSpanPair2.SourceEndIndex + 1 && sourceTokenIx != liftAlignedSpanPair2.SourceStartIndex - 1)
			{
				return false;
			}
			short sourceLength = liftAlignedSpanPair2.SourceLength;
			short num4 = liftAlignedSpanPair2.SourceStartIndex;
			sourceLength = (short)(sourceLength + 1);
			if (sourceTokenIx == liftAlignedSpanPair2.SourceStartIndex - 1)
			{
				num4 = (short)(num4 - 1);
			}
			num2 = liftAlignedSpanPair2.SourceLength + liftAlignedSpanPair2.TargetLength;
			num3 = ((double)(liftAlignedSpanPair2.Confidence * (float)num2) + confidence) / (double)(num2 + 1);
			liftAlignedSpanPair3 = new LiftAlignedSpanPair(num4, sourceLength, liftAlignedSpanPair2.TargetStartIndex, liftAlignedSpanPair2.TargetLength);
			liftAlignedSpanPair3.Confidence = (float)num3;
			liftAlignedSpanPair3.Provenance = 10;
			alignmentData.Remove(liftAlignedSpanPair2);
			if (alignmentData.Contradicts(liftAlignedSpanPair3, repetitionIsContradiction: true))
			{
				alignmentData.Add(liftAlignedSpanPair2);
				return false;
			}
			alignmentData.Add(liftAlignedSpanPair3);
			return true;
		}

		private void ImportAlignments(double?[,] alignmentMatrix, LiftAlignedSpanPairSet alignmentData, List<Token> sourceTokens, List<Token> targetTokens, CodedTokenIndexConverter srcCodedIxConverter, CodedTokenIndexConverter trgCodedIxConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcRawToSigIndexConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgRawToSigIndexConverter)
		{
			for (int i = 0; i < srcCodedIxConverter.Count; i++)
			{
				for (int j = 0; j < trgCodedIxConverter.Count; j++)
				{
					if (alignmentMatrix[i, j].HasValue)
					{
						short index = (short)srcCodedIxConverter.CodedTokenIndexToRawIndex(i);
						short index2 = (short)trgCodedIxConverter.CodedTokenIndexToRawIndex(j);
						int sourceTokenIx = srcRawToSigIndexConverter.RawToSignificantIndex(index);
						int targetTokenIx = trgRawToSigIndexConverter.RawToSignificantIndex(index2);
						AddorGrowAlignment(alignmentData, sourceTokenIx, targetTokenIx, alignmentMatrix[i, j].Value);
					}
				}
			}
		}

		public bool Align(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress)
		{
			throw new NotImplementedException();
		}

		public AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs)
		{
			throw new NotImplementedException();
		}
	}
}
