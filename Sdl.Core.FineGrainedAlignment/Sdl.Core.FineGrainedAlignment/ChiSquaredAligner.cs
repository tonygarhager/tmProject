using Sdl.Core.FineGrainedAlignment.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Stat;
using Sdl.LanguagePlatform.Stat.WordAlignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sdl.Core.FineGrainedAlignment
{
	public class ChiSquaredAligner : IFineGrainedAligner
	{
		public class NullModelException : Exception
		{
		}

		protected class CodedTokenIndexConverter
		{
			private readonly List<short> _tokenIndexTranslationList;

			public CodedTokenIndexConverter(List<short> tokenInfo)
			{
				_tokenIndexTranslationList = tokenInfo;
			}

			public LiftSpan CodedTokenIndexToTUSpan(int ix)
			{
				LiftSpan liftSpan = new LiftSpan();
				liftSpan.StartIndex = _tokenIndexTranslationList[ix];
				liftSpan.Length = 1;
				return liftSpan;
			}
		}

		protected ChiSquaredTranslationModel _model;

		private bool _bulkMode;

		private Action<Exception, string> _errorLogger;

		private readonly object _locker = new object();

		public bool SupportsSynchronousAlign => true;

		public bool BulkMode
		{
			get
			{
				return _bulkMode;
			}
			set
			{
				if (_bulkMode != value)
				{
					CheckModelSupplied();
					_bulkMode = value;
					if (!_bulkMode)
					{
						_model.Unload();
					}
					else
					{
						_model.Load();
					}
				}
			}
		}

		public event EventHandler<ProgressEventArgs> Progress;

		public ChiSquaredAligner(ChiSquaredTranslationModel model)
		{
			_model = model;
		}

		private void LogError(Exception ex, string s)
		{
			lock (_locker)
			{
				if (_errorLogger != null)
				{
					_errorLogger(ex, s);
				}
			}
		}

		public bool Align(IEnumerable<IAlignableContentPair> pairs)
		{
			return Align(pairs, default(CancellationToken), null);
		}

		public AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs)
		{
			return AlignEx(pairs, default(CancellationToken), null);
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
			lock (_locker)
			{
				_errorLogger = logger;
			}
		}

		private void CheckModelSupplied()
		{
			if (_model == null)
			{
				throw new NullModelException();
			}
		}

		public bool Align(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress)
		{
			CheckModelSupplied();
			if (!_model.TranslationModelDate.HasValue)
			{
				foreach (IAlignableContentPair pair in pairs)
				{
					pair.AlignmentData = null;
					pair.TranslationModelDate = null;
				}
				return false;
			}
			AlignResult[] source = AlignEx(pairs, token, progress);
			if (source.Any((AlignResult x) => x == AlignResult.PairUntokenized))
			{
				throw new Exception("Untokenized input");
			}
			if (source.Any((AlignResult x) => x == AlignResult.InvalidAlignmentData))
			{
				throw new Exception("pair.AlignmentData is wrong size");
			}
			return true;
		}

		public AlignResult[] AlignEx(IEnumerable<IAlignableContentPair> pairs, CancellationToken token, IProgress<int> progress)
		{
			CheckModelSupplied();
			AlignResult? alignResult = null;
			if (!_model.TranslationModelDate.HasValue)
			{
				alignResult = AlignResult.NoModel;
			}
			if (!alignResult.HasValue && _model.SampleCount == 0)
			{
				LogError(null, "A model exists but the SampleCount is 0");
				alignResult = AlignResult.CorruptModel;
			}
			if (alignResult.HasValue)
			{
				List<AlignResult> list = new List<AlignResult>();
				foreach (IAlignableContentPair pair in pairs)
				{
					pair.AlignmentData = null;
					pair.TranslationModelDate = null;
					list.Add(alignResult.Value);
				}
				return list.ToArray();
			}
			return AlignInternal(pairs, token, progress);
		}

		private float AssociationToConfidence(double association, out bool modelBad)
		{
			CheckModelSupplied();
			modelBad = false;
			if (association > (double)_model.SampleCount)
			{
				LogError(null, "An association value larger than the sample count was encountered: " + association.ToString() + " and " + _model.SampleCount.ToString());
				modelBad = true;
				return 0f;
			}
			return (float)(association / (double)(_model.SampleCount * 2) + 0.5);
		}

		private bool CanAlign(IAlignableContentPair pair)
		{
			if (pair.SourceTokens.Count > 200 || pair.TargetTokens.Count > 200)
			{
				return false;
			}
			return true;
		}

		protected virtual AlignResult[] AlignInternal(IEnumerable<IAlignableContentPair> pairs, CancellationToken cancellationToken, IProgress<int> progress)
		{
			if (pairs == null)
			{
				throw new ArgumentNullException("pairs");
			}
			List<AlignResult> list = new List<AlignResult>();
			CheckModelSupplied();
			DataEncoder dataEncoder = new DataEncoder(_model.SourceCulture, _model.TargetCulture, forTrainedModel: false, _model.UseWordStems);
			try
			{
				pairs = AlignableContentPairWrapper.WrapList(pairs);
				if (!BulkMode)
				{
					HashSet<string> hashSet = new HashSet<string>();
					HashSet<string> hashSet2 = new HashSet<string>();
					foreach (IAlignableContentPair pair in pairs)
					{
						if (pair.SourceTokens != null && pair.TargetTokens != null)
						{
							dataEncoder.GetUniqueTokenStrings(pair.SourceTokens, hashSet, forTraining: false, target: false);
							dataEncoder.GetUniqueTokenStrings(pair.TargetTokens, hashSet2, forTraining: false, target: true);
						}
					}
					_model.PartialLoad(hashSet, hashSet2);
				}
			}
			catch (Exception ex)
			{
				foreach (IAlignableContentPair pair2 in pairs)
				{
					list.Add(AlignResult.AlignError);
				}
				LogError(ex, null);
				return list.ToArray();
			}
			ChiSquareScoreProvider2 scoreProvider = new ChiSquareScoreProvider2(_model.Matrix);
			BilingualPhraseComputer3.Settings settings = new BilingualPhraseComputer3.Settings();
			settings.Incremental = true;
			BilingualPhraseComputer3 bilingualPhraseComputer = new BilingualPhraseComputer3(_model.SourceCulture, _model.TargetCulture, settings, null, _model.SourceVocab, _model.TargetVocab);
			WordAlignmentComputer3 wordAlignmentComputer = new WordAlignmentComputer3(scoreProvider, _model.SourceVocab, _model.TargetVocab);
			int num = 0;
			IEnumerator<IAlignableContentPair> enumerator3 = pairs.GetEnumerator();
			List<Token> sourceNonWhitespaceTokens;
			List<Token> targetNonWhitespaceTokens;
			while (enumerator3.MoveNext())
			{
				num++;
				try
				{
					progress?.Report(num);
					if (!cancellationToken.IsCancellationRequested)
					{
						IAlignableContentPair current3 = enumerator3.Current;
						current3.TranslationModelDate = _model.TranslationModelDate;
						if (current3.SourceTokens == null || current3.TargetTokens == null)
						{
							list.Add(AlignResult.PairUntokenized);
						}
						else if (current3.SourceTokens.Count == 0 || current3.TargetTokens.Count == 0 || !CanAlign(current3))
						{
							current3.AlignmentData = LiftAlignedSpanPairSet.CreateEmptyLiftAlignedSpanPairSet();
							list.Add(AlignResult.PairEmptyOrTooLarge);
						}
						else
						{
							if (current3.AlignmentData == null)
							{
								current3.AlignmentData = new LiftAlignedSpanPairSet((short)current3.SourceTokens.Count, (short)current3.TargetTokens.Count);
							}
							else if (current3.AlignmentData.Root().SourceLength != current3.SourceTokens.Count || current3.AlignmentData.Root().TargetLength != current3.TargetTokens.Count)
							{
								list.Add(AlignResult.InvalidAlignmentData);
								continue;
							}
							List<short> list2 = new List<short>();
							List<short> list3 = new List<short>();
							if (!dataEncoder.Encode(current3, _model.SourceVocab, _model.TargetVocab, list2, list3, out IntSegment srcIntSegment, out IntSegment trgIntSegment, null, null, null, null))
							{
								list.Add(AlignResult.PairEmptyOrTooLarge);
								current3.AlignmentData = LiftAlignedSpanPairSet.CreateEmptyLiftAlignedSpanPairSet();
							}
							else
							{
								List<short> featureIndicesOnly = SubsegmentUtilities.GetFeatureIndicesOnly(current3.SourceTokens, _model.SourceCulture);
								List<short> featureIndicesOnly2 = SubsegmentUtilities.GetFeatureIndicesOnly(current3.TargetTokens, _model.TargetCulture);
								if (featureIndicesOnly.Count == 0 || featureIndicesOnly2.Count == 0)
								{
									list.Add(AlignResult.PairEmptyOrTooLarge);
								}
								else
								{
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)current3.SourceTokens.Count);
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter2 = SubsegmentUtilities.RawIndexVsSignificantIndexConverter.CreateNoopConverter((short)current3.TargetTokens.Count);
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcToConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list2);
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgToConverter = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list3);
									LiftAlignedSpanPairSet liftAlignedSpanPairSet = SubsegmentUtilities.ConvertAlignmentSetIndices(current3.AlignmentData, rawIndexVsSignificantIndexConverter, rawIndexVsSignificantIndexConverter2, srcToConverter, trgToConverter);
									Dictionary<int, int> dictionary = new Dictionary<int, int>();
									if (liftAlignedSpanPairSet.Count > 1)
									{
										List<LiftAlignedSpanPair> allAlignedSpanPairs = liftAlignedSpanPairSet.GetAllAlignedSpanPairs(includeIncompatible: false);
										foreach (LiftAlignedSpanPair item in allAlignedSpanPairs)
										{
											if (item.SourceLength != liftAlignedSpanPairSet.Root().SourceLength && item.SourceLength == 1 && item.TargetLength == 1)
											{
												dictionary.Add(item.SourceStartIndex, item.TargetStartIndex);
											}
										}
									}
									List<ExtendableBilingualPhrase> phrases;
									AlignmentTable at = wordAlignmentComputer.ComputeAlignment(srcIntSegment, trgIntSegment, dictionary, _model.SampleCount, out phrases);
									List<AlignedPhrase> list4 = new List<AlignedPhrase>();
									if (phrases != null)
									{
										foreach (ExtendableBilingualPhrase item2 in phrases)
										{
											list4.Add(new AlignedPhrase(item2.FromSrcIndex, item2.IntoSrcIndex - item2.FromSrcIndex + 1, item2.FromTrgIndex, item2.IntoTrgIndex - item2.FromTrgIndex + 1, (float)item2.Association));
										}
									}
									List<AlignedPhrase> alignedPhrases = bilingualPhraseComputer.Compute(srcIntSegment, trgIntSegment, at);
									List<short> list5 = new List<short>();
									List<short> list6 = new List<short>();
									sourceNonWhitespaceTokens = SubsegmentUtilities.GetNonWhitespaceTokens(current3.SourceTokens, list5);
									targetNonWhitespaceTokens = SubsegmentUtilities.GetNonWhitespaceTokens(current3.TargetTokens, list6);
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter3 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(featureIndicesOnly);
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter4 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(featureIndicesOnly2);
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter5 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list5);
									SubsegmentUtilities.RawIndexVsSignificantIndexConverter rawIndexVsSignificantIndexConverter6 = new SubsegmentUtilities.RawIndexVsSignificantIndexConverter(list6);
									LiftAlignedSpanPairSet liftAlignedSpanPairSet2 = SubsegmentUtilities.ConvertAlignmentSetIndices(current3.AlignmentData, rawIndexVsSignificantIndexConverter, rawIndexVsSignificantIndexConverter2, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6);
									CodedTokenIndexConverter srcCodedIxConverter = new CodedTokenIndexConverter(list2);
									CodedTokenIndexConverter trgCodedIxConverter = new CodedTokenIndexConverter(list3);
									if (ImportAlignedPhrases(at, list4, liftAlignedSpanPairSet2, current3.SourceTokens, current3.TargetTokens, srcCodedIxConverter, trgCodedIxConverter, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6))
									{
										list.Add(AlignResult.CorruptModel);
									}
									else
									{
										bool flag = ImportAlignedPhrases(at, alignedPhrases, liftAlignedSpanPairSet2, current3.SourceTokens, current3.TargetTokens, srcCodedIxConverter, trgCodedIxConverter, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6);
										liftAlignedSpanPairSet2.Reduce();
										LiftAlignedSpanPairSet liftAlignedSpanPairSet3 = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet2, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4);
										liftAlignedSpanPairSet3.Reduce();
										liftAlignedSpanPairSet3.DeduceFurtherAlignments(includesPunc: false, null, null);
										liftAlignedSpanPairSet3 = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet3, rawIndexVsSignificantIndexConverter3, rawIndexVsSignificantIndexConverter4, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6);
										int num2 = SubsegmentUtilities.MergeAlignmentSets(liftAlignedSpanPairSet2, liftAlignedSpanPairSet3);
										liftAlignedSpanPairSet2.Reduce();
										liftAlignedSpanPairSet2.DeduceFurtherAlignments(includesPunc: true, (short t) => sourceNonWhitespaceTokens[t].IsPunctuation, (short t) => targetNonWhitespaceTokens[t].IsPunctuation);
										liftAlignedSpanPairSet2.Reduce();
										current3.AlignmentData = SubsegmentUtilities.ConvertAlignmentSetIndices(liftAlignedSpanPairSet2, rawIndexVsSignificantIndexConverter5, rawIndexVsSignificantIndexConverter6, rawIndexVsSignificantIndexConverter, rawIndexVsSignificantIndexConverter2);
										list.Add(AlignResult.Aligned);
									}
								}
							}
						}
						continue;
					}
				}
				catch (Exception arg)
				{
					if (_errorLogger != null)
					{
						_errorLogger(arg, null);
					}
					list.Add(AlignResult.AlignError);
					continue;
				}
				break;
			}
			progress?.Report(num);
			return list.ToArray();
		}

		private bool ImportAlignedPhrases(AlignmentTable at, List<AlignedPhrase> alignedPhrases, LiftAlignedSpanPairSet alignmentData, List<Token> sourceTokens, List<Token> targetTokens, CodedTokenIndexConverter srcCodedIxConverter, CodedTokenIndexConverter trgCodedIxConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter srcFeatureSpanConverter, SubsegmentUtilities.RawIndexVsSignificantIndexConverter trgFeatureSpanConverter)
		{
			bool modelBad = false;
			foreach (AlignedPhrase alignedPhrase in alignedPhrases)
			{
				LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair();
				LiftSpan liftSpan = srcCodedIxConverter.CodedTokenIndexToTUSpan(alignedPhrase.SrcStartPosition);
				LiftSpan liftSpan2 = srcCodedIxConverter.CodedTokenIndexToTUSpan(alignedPhrase.SrcStartPosition + alignedPhrase.SrcLength - 1);
				LiftSpan liftSpan3 = new LiftSpan(liftSpan.StartIndex, (short)(liftSpan2.StartIndex + liftSpan2.Length - liftSpan.StartIndex));
				liftAlignedSpanPair.SourceSpan = ((srcFeatureSpanConverter == null) ? liftSpan3 : srcFeatureSpanConverter.RawSpanToSignificantSpan(liftSpan3));
				LiftSpan liftSpan4 = trgCodedIxConverter.CodedTokenIndexToTUSpan(alignedPhrase.TrgStartPosition);
				LiftSpan liftSpan5 = trgCodedIxConverter.CodedTokenIndexToTUSpan(alignedPhrase.TrgStartPosition + alignedPhrase.TrgLength - 1);
				LiftSpan liftSpan6 = new LiftSpan(liftSpan4.StartIndex, (short)(liftSpan5.StartIndex + liftSpan5.Length - liftSpan4.StartIndex));
				liftAlignedSpanPair.TargetSpan = ((trgFeatureSpanConverter == null) ? liftSpan6 : trgFeatureSpanConverter.RawSpanToSignificantSpan(liftSpan6));
				if (liftAlignedSpanPair.SourceSpan != null && liftAlignedSpanPair.TargetSpan != null)
				{
					liftAlignedSpanPair.Provenance = 4;
					if (!alignmentData.Contradicts(liftAlignedSpanPair, repetitionIsContradiction: true))
					{
						alignmentData.Add(liftAlignedSpanPair);
						float num = 0f;
						if (alignedPhrase.Association < 0f)
						{
							double num2 = 0.0;
							double num3 = 0.0;
							for (int i = 0; i < alignedPhrase.SrcLength; i++)
							{
								for (int j = 0; j < alignedPhrase.TrgLength; j++)
								{
									if (at[i + alignedPhrase.SrcStartPosition, j + alignedPhrase.TrgStartPosition])
									{
										num2 += at.Associations[i + alignedPhrase.SrcStartPosition, j + alignedPhrase.TrgStartPosition];
										num3 += 1.0;
									}
								}
							}
							num = AssociationToConfidence(num2 / num3, out modelBad);
						}
						else
						{
							num = AssociationToConfidence(alignedPhrase.Association, out modelBad);
						}
						liftAlignedSpanPair.Confidence = num;
					}
					if (modelBad)
					{
						return modelBad;
					}
				}
			}
			return modelBad;
		}
	}
}
