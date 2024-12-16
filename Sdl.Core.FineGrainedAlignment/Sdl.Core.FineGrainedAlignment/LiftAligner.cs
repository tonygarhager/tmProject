using Sdl.Core.FineGrainedAlignment.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sdl.Core.FineGrainedAlignment
{
	public class LiftAligner
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct SpanHypothesisData
		{
			public short _sourceStartIndex;

			public short _sourceLength;

			public short _targetStartIndex;

			public short _targetLength;

			public float _confidence;

			public bool _blocked;

			public bool _confirmed;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public class SpanHypothesis
		{
			private SpanHypothesisData _data;

			public float Confidence
			{
				get
				{
					return _data._confidence;
				}
				set
				{
					_data._confidence = value;
				}
			}

			public float ComputedConfidence
			{
				get
				{
					if (_data._blocked)
					{
						return 0f;
					}
					return _data._confidence;
				}
			}

			public bool Blocked => _data._blocked;

			public bool Confirmed => _data._confirmed;

			internal LiftAlignedSpanPair TempThis => new LiftAlignedSpanPair(_data._sourceStartIndex, _data._sourceLength, _data._targetStartIndex, _data._targetLength);

			public SpanHypothesis(int sourceStartIndex, int sourceLength, int targetStartIndex, int targetLength)
			{
				_data._sourceStartIndex = (short)sourceStartIndex;
				_data._sourceLength = (short)sourceLength;
				_data._targetStartIndex = (short)targetStartIndex;
				_data._targetLength = (short)targetLength;
			}

			public void Block()
			{
				_data._blocked = true;
			}

			public void Confirm()
			{
				_data._confirmed = true;
				_data._blocked = true;
			}
		}

		public class SpanHypothesisArray
		{
			private int _sourceLength;

			private int _targetLength;

			private Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, SpanHypothesis>>>> _map3;

			private long _maxTotalHyps;

			private bool _initDone;

			public long MaxTotalHyps
			{
				get
				{
					if (!_initDone)
					{
						throw new Exception("SpanHypothesisArray not initialised");
					}
					return _maxTotalHyps;
				}
			}

			public SpanHypothesis this[int sourceStartOffset, int sourceLength, int targetStartOffset, int targetLength]
			{
				get
				{
					Dictionary<int, SpanHypothesis> dict = GetDict(sourceStartOffset, sourceLength, targetStartOffset, createIfMissing: false);
					if (dict == null)
					{
						return null;
					}
					if (dict.ContainsKey(targetLength))
					{
						return dict[targetLength];
					}
					return null;
				}
				set
				{
					Dictionary<int, SpanHypothesis> dict = GetDict(sourceStartOffset, sourceLength, targetStartOffset, createIfMissing: true);
					dict[targetLength] = value;
				}
			}

			public SpanHypothesisArray(int sourceLength, int targetLength, int minHypothesisSourceLength, int minHypothesisTargetLength)
			{
				Init(sourceLength, targetLength, minHypothesisSourceLength, minHypothesisTargetLength);
			}

			private int Sum1ToN(int N)
			{
				return N * (N + 1) / 2;
			}

			private void Init(int sourceLength, int targetLength, int minHypothesisSourceLength, int minHypothesisTargetLength)
			{
				_sourceLength = sourceLength;
				_targetLength = targetLength;
				int val = Sum1ToN(sourceLength - minHypothesisSourceLength + 1) - 1;
				int val2 = Sum1ToN(targetLength - minHypothesisTargetLength + 1) - 1;
				val = Math.Min(val, 0);
				val2 = Math.Min(val2, 0);
				_maxTotalHyps = val * val2;
				_map3 = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, SpanHypothesis>>>>(sourceLength);
				_initDone = true;
			}

			private Dictionary<int, SpanHypothesis> GetDict(int sourceStartOffset, int sourceLength, int targetStartOffset, bool createIfMissing)
			{
				Dictionary<int, Dictionary<int, Dictionary<int, SpanHypothesis>>> dictionary;
				if (_map3.ContainsKey(sourceStartOffset))
				{
					dictionary = _map3[sourceStartOffset];
				}
				else
				{
					if (!createIfMissing)
					{
						return null;
					}
					dictionary = new Dictionary<int, Dictionary<int, Dictionary<int, SpanHypothesis>>>();
					_map3.Add(sourceStartOffset, dictionary);
				}
				Dictionary<int, Dictionary<int, SpanHypothesis>> dictionary2;
				if (dictionary.ContainsKey(sourceLength))
				{
					dictionary2 = dictionary[sourceLength];
				}
				else
				{
					if (!createIfMissing)
					{
						return null;
					}
					dictionary2 = new Dictionary<int, Dictionary<int, SpanHypothesis>>();
					dictionary.Add(sourceLength, dictionary2);
				}
				Dictionary<int, SpanHypothesis> dictionary3;
				if (dictionary2.ContainsKey(targetStartOffset))
				{
					dictionary3 = dictionary2[targetStartOffset];
				}
				else
				{
					if (!createIfMissing)
					{
						return null;
					}
					dictionary3 = new Dictionary<int, SpanHypothesis>();
					dictionary2.Add(targetStartOffset, dictionary3);
				}
				return dictionary3;
			}
		}

		private short _minHypothesisSourceLength = 2;

		private short _minHypothesisTargetLength = 2;

		private readonly int _minSourceLengthForAbsurditySkip = 12;

		private readonly int _minTargetLengthForAbsurditySkip = 12;

		private const int _maxTokens = 100;

		private readonly bool ConfirmEvidence = true;

		private List<SpanHypothesis> _hypotheses;

		private SpanHypothesisArray _hypArray;

		private IAlignableContentPair _pair;

		private List<AlignmentEvidence>[,] _evidenceMatrix;

		private bool[] _sourceTokenHasEvidence;

		private bool[] _targetTokenHasEvidence;

		private double _sourceToTargetRatio;

		private List<AlignmentEvidence> _evidence;

		private List<AlignmentEvidence>[,] _coverageMap;

		private bool ConfirmEvidenceImmediately()
		{
			if (!ConfirmEvidence)
			{
				return false;
			}
			return true;
		}

		public LiftAligner(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
		}

		private void SetMinima(List<LiftSpan> sourceConstraintsList, List<LiftSpan> targetConstraintsList)
		{
			_minHypothesisSourceLength = 2;
			_minHypothesisTargetLength = 2;
			if (sourceConstraintsList != null)
			{
				_minHypothesisSourceLength = Math.Min(_minHypothesisSourceLength, sourceConstraintsList.Min((LiftSpan x) => x.Length));
				_minHypothesisTargetLength = Math.Min(_minHypothesisTargetLength, sourceConstraintsList.Min((LiftSpan x) => x.Length));
			}
			if (targetConstraintsList != null)
			{
				_minHypothesisSourceLength = Math.Min(_minHypothesisSourceLength, targetConstraintsList.Min((LiftSpan x) => x.Length));
				_minHypothesisTargetLength = Math.Min(_minHypothesisTargetLength, targetConstraintsList.Min((LiftSpan x) => x.Length));
			}
		}

		public void Align(IAlignableContentPair pair, SimpleTree<LiftSpan> sourceConstraints, SimpleTree<LiftSpan> targetConstraints, List<AlignmentEvidence> evidence)
		{
			CheckConstraints(sourceConstraints, "sourceConstraints");
			CheckConstraints(targetConstraints, "targetConstraints");
			AlignEx(pair, (sourceConstraints == null) ? null : new List<LiftSpan>
			{
				sourceConstraints.Key
			}, (targetConstraints == null) ? null : new List<LiftSpan>
			{
				targetConstraints.Key
			}, evidence);
		}

		private void CheckConstraints(SimpleTree<LiftSpan> constraints, string argName)
		{
			if (constraints == null || constraints.Value == null || constraints.Value.Count <= 0)
			{
				return;
			}
			throw new ArgumentException("Single-node tree expected", argName);
		}

		private void CreateCoverageMap(IAlignableContentPair pair, List<AlignmentEvidence> evidence)
		{
			_coverageMap = new List<AlignmentEvidence>[pair.SourceTokens.Count, pair.TargetTokens.Count];
			foreach (AlignmentEvidence item in evidence)
			{
				for (short num = item.FirstSourceTokenIndex; num <= item.LastSourceTokenIndex; num = (short)(num + 1))
				{
					for (short num2 = item.FirstTargetTokenIndex; num2 <= item.LastTargetTokenIndex; num2 = (short)(num2 + 1))
					{
						if (_coverageMap[num, num2] == null)
						{
							_coverageMap[num, num2] = new List<AlignmentEvidence>();
						}
						_coverageMap[num, num2].Add(item);
					}
				}
			}
		}

		public void AlignEx(IAlignableContentPair pair, List<LiftSpan> sourceConstraintsList, List<LiftSpan> targetConstraintsList, List<AlignmentEvidence> evidence)
		{
			if (pair.SourceTokens.Count > 100 || pair.TargetTokens.Count > 100)
			{
				return;
			}
			if (pair.AlignmentData == null)
			{
				pair.AlignmentData = new LiftAlignedSpanPairSet((short)pair.SourceTokens.Count, (short)pair.TargetTokens.Count);
			}
			_pair = pair;
			SetMinima(sourceConstraintsList, targetConstraintsList);
			_hypotheses = new List<SpanHypothesis>();
			_hypArray = new SpanHypothesisArray(pair.SourceTokens.Count, pair.TargetTokens.Count, _minHypothesisSourceLength, _minHypothesisTargetLength);
			_evidenceMatrix = new List<AlignmentEvidence>[pair.SourceTokens.Count, pair.TargetTokens.Count];
			_evidence = new List<AlignmentEvidence>(evidence);
			CreateCoverageMap(pair, _evidence);
			List<LiftAlignedSpanPair> allAlignedSpanPairs = _pair.AlignmentData.GetAllAlignedSpanPairs(includeIncompatible: false);
			foreach (LiftAlignedSpanPair item2 in allAlignedSpanPairs)
			{
				if (item2.SourceLength != _pair.SourceTokens.Count && item2.SourceLength == 1 && item2.TargetLength == 1)
				{
					float num = item2.Confidence;
					if (num == 0f)
					{
						num = 1f;
					}
					SimpleAlignmentEvidence item = new SimpleAlignmentEvidence(item2.SourceStartIndex, item2.TargetStartIndex, num);
					_evidence.Add(item);
				}
			}
			_sourceTokenHasEvidence = new bool[_pair.SourceTokens.Count];
			_targetTokenHasEvidence = new bool[_pair.TargetTokens.Count];
			for (short num2 = 0; num2 < pair.SourceTokens.Count; num2 = (short)(num2 + 1))
			{
				for (short num3 = 0; num3 < pair.TargetTokens.Count; num3 = (short)(num3 + 1))
				{
					foreach (AlignmentEvidence item3 in _evidence)
					{
						if (item3.Covers(num2, num3))
						{
							_sourceTokenHasEvidence[num2] = true;
							_targetTokenHasEvidence[num3] = true;
							if (_evidenceMatrix[num2, num3] == null)
							{
								_evidenceMatrix[num2, num3] = new List<AlignmentEvidence>
								{
									item3
								};
							}
							else
							{
								_evidenceMatrix[num2, num3].Add(item3);
							}
						}
					}
				}
			}
			CreateHypotheses(sourceConstraintsList, targetConstraintsList);
			ScoreHypotheses();
			EvalHypotheses();
			if (!ConfirmEvidenceImmediately() && ConfirmEvidence)
			{
				List<LiftAlignedSpanPair> allAlignedSpanPairs2 = _pair.AlignmentData.GetAllAlignedSpanPairs(includeIncompatible: false);
				allAlignedSpanPairs2.Sort((LiftAlignedSpanPair a, LiftAlignedSpanPair b) => (a.SourceLength + a.TargetLength).CompareTo(b.SourceLength + b.TargetLength));
				foreach (LiftAlignedSpanPair item4 in allAlignedSpanPairs2)
				{
					if (item4 != _pair.AlignmentData.Root())
					{
						SpanHypothesis h = _hypArray[item4.SourceStartIndex, item4.SourceLength, item4.TargetStartIndex, item4.TargetLength];
						ConfirmHypothesisEvidence(h);
					}
				}
			}
			if (ConfirmEvidence)
			{
				SpanHypothesis spanHypothesis = new SpanHypothesis(0, _pair.SourceTokens.Count, 0, _pair.TargetTokens.Count);
				spanHypothesis.Confidence = 1f;
				ConfirmHypothesisEvidence(spanHypothesis);
			}
			_pair.AlignmentData.DeduceFurtherAlignments(includesPunc: true, (short t) => _pair.SourceTokens[t].IsPunctuation, (short t) => _pair.TargetTokens[t].IsPunctuation);
			_pair.AlignmentData.Reduce();
		}

		private void InitialSort()
		{
			_hypotheses.Sort((SpanHypothesis a, SpanHypothesis b) => b.ComputedConfidence.CompareTo(a.ComputedConfidence));
		}

		private List<LiftAlignedSpanPair> ConfirmHypothesisEvidence(SpanHypothesis h)
		{
			LiftAlignedSpanPair tempThis = h.TempThis;
			List<LiftAlignedSpanPair> list = new List<LiftAlignedSpanPair>();
			foreach (AlignmentEvidence item in _evidence)
			{
				if (item.Concerns(tempThis, outside: false))
				{
					SimpleAlignmentEvidence simpleAlignmentEvidence = item as SimpleAlignmentEvidence;
					if (simpleAlignmentEvidence != null)
					{
						List<AlignmentEvidence> list2 = new List<AlignmentEvidence>();
						for (short num = tempThis.TargetStartIndex; num <= tempThis.TargetEndIndex; num = (short)(num + 1))
						{
							List<AlignmentEvidence> list3 = _coverageMap[item.FirstSourceTokenIndex, num];
							if (list3 != null)
							{
								list2.AddRange(list3);
							}
						}
						bool flag = false;
						foreach (AlignmentEvidence item2 in list2)
						{
							if (item2 != item)
							{
								SimpleAlignmentEvidence simpleAlignmentEvidence2 = item2 as SimpleAlignmentEvidence;
								if (simpleAlignmentEvidence2 == null)
								{
									flag = true;
									break;
								}
								if (simpleAlignmentEvidence2.FirstTargetTokenIndex != simpleAlignmentEvidence.FirstTargetTokenIndex)
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							List<AlignmentEvidence> list4 = new List<AlignmentEvidence>();
							for (short num2 = tempThis.SourceStartIndex; num2 <= tempThis.SourceEndIndex; num2 = (short)(num2 + 1))
							{
								List<AlignmentEvidence> list5 = _coverageMap[num2, item.FirstTargetTokenIndex];
								if (list5 != null)
								{
									list4.AddRange(list5);
								}
							}
							foreach (AlignmentEvidence item3 in list4)
							{
								if (item3 != item)
								{
									SimpleAlignmentEvidence simpleAlignmentEvidence3 = item3 as SimpleAlignmentEvidence;
									if (simpleAlignmentEvidence3 == null)
									{
										flag = true;
										break;
									}
									if (simpleAlignmentEvidence3.FirstSourceTokenIndex != simpleAlignmentEvidence.FirstSourceTokenIndex)
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair(new LiftSpan(simpleAlignmentEvidence.FirstSourceTokenIndex, 1), new LiftSpan(simpleAlignmentEvidence.FirstTargetTokenIndex, 1));
								liftAlignedSpanPair.Confidence = Math.Min(simpleAlignmentEvidence.ConfidenceFor(tempThis, outside: false), h.Confidence);
								liftAlignedSpanPair.Provenance = 1;
								if (!_pair.AlignmentData.Contradicts(liftAlignedSpanPair, repetitionIsContradiction: true))
								{
									_pair.AlignmentData.Add(liftAlignedSpanPair);
									list.Add(liftAlignedSpanPair);
								}
							}
						}
					}
				}
			}
			return list;
		}

		private void EvalHypotheses()
		{
			InitialSort();
			while (true)
			{
				List<SpanHypothesis> list = new List<SpanHypothesis>();
				List<SpanHypothesis> list2 = new List<SpanHypothesis>();
				int num = 0;
				while (num < _hypotheses.Count && _hypotheses[num].ComputedConfidence > 0f)
				{
					if (num == _hypotheses.Count - 1)
					{
						if (!InvolvedInSkipped(list2, _hypotheses[num]))
						{
							list.Add(_hypotheses[num]);
						}
						break;
					}
					if (_hypotheses[num].ComputedConfidence == _hypotheses[num + 1].ComputedConfidence)
					{
						int i = num + 1;
						double num2 = _hypotheses[num].ComputedConfidence;
						list.Add(_hypotheses[num]);
						for (; i < _hypotheses.Count && (double)_hypotheses[i].ComputedConfidence == num2; i++)
						{
							list.Add(_hypotheses[i]);
						}
						List<SpanHypothesis> list3 = new List<SpanHypothesis>(list);
						SpanHypothesis spanHypothesis = null;
						for (int j = 0; j < list3.Count - 1; j++)
						{
							for (int k = j + 1; k < list3.Count; k++)
							{
								if (!list3[j].TempThis.Contradicts(list3[k].TempThis))
								{
									continue;
								}
								SpanHypothesis spanHypothesis2 = list3[j];
								SpanHypothesis spanHypothesis3 = list3[k];
								list[j] = null;
								list[k] = null;
								if (spanHypothesis != null)
								{
									break;
								}
								LiftAlignedSpanPair unionHyp = null;
								LiftAlignedSpanPair intersectionHyp = null;
								if (!HypothesesIntersectWithRemainder(spanHypothesis2, spanHypothesis3, out unionHyp, out intersectionHyp))
								{
									break;
								}
								SpanHypothesis spanHypothesis4 = _hypArray[unionHyp.SourceStartIndex, unionHyp.SourceLength - 1, unionHyp.TargetStartIndex, unionHyp.TargetLength - 1];
								if (spanHypothesis4 != null && spanHypothesis4.ComputedConfidence > 0f && !list2.Contains(spanHypothesis4) && !list3.Contains(spanHypothesis4))
								{
									bool flag = true;
									foreach (SpanHypothesis item in list3)
									{
										if (item != spanHypothesis2 && item != spanHypothesis3 && item.TempThis.Contradicts(spanHypothesis4.TempThis))
										{
											flag = false;
											break;
										}
									}
									if (flag)
									{
										spanHypothesis = spanHypothesis4;
									}
								}
								if (spanHypothesis != null)
								{
									break;
								}
								SpanHypothesis spanHypothesis5 = _hypArray[intersectionHyp.SourceStartIndex, intersectionHyp.SourceLength - 1, intersectionHyp.TargetStartIndex, intersectionHyp.TargetLength - 1];
								if (spanHypothesis5 != null && spanHypothesis5.ComputedConfidence > 0f && !list2.Contains(spanHypothesis5) && !list3.Contains(spanHypothesis5))
								{
									bool flag2 = true;
									foreach (SpanHypothesis item2 in list3)
									{
										if (item2 != spanHypothesis2 && item2 != spanHypothesis3 && item2.TempThis.Contradicts(spanHypothesis5.TempThis))
										{
											flag2 = false;
											break;
										}
									}
									if (flag2)
									{
										spanHypothesis = spanHypothesis5;
									}
								}
								break;
							}
						}
						list = list.FindAll((SpanHypothesis h) => h != null);
						if (spanHypothesis != null)
						{
							list.Add(spanHypothesis);
						}
						if (list.Count > 0)
						{
							if (list2.Count > 0)
							{
								List<SpanHypothesis> list4 = new List<SpanHypothesis>();
								foreach (SpanHypothesis item3 in list)
								{
									if (InvolvedInSkipped(list2, item3))
									{
										list4.Add(item3);
									}
								}
								foreach (SpanHypothesis item4 in list4)
								{
									list.Remove(item4);
								}
							}
							if (list.Count > 0)
							{
								break;
							}
						}
						num += list3.Count;
						list2.AddRange(list3);
						list.Clear();
					}
					else
					{
						if (!InvolvedInSkipped(list2, _hypotheses[num]))
						{
							list.Add(_hypotheses[num]);
							break;
						}
						list2.Add(_hypotheses[num]);
						num++;
					}
				}
				if (list.Count == 0)
				{
					break;
				}
				if (ConfirmEvidenceImmediately() && list.Count > 1)
				{
					list.Sort((SpanHypothesis b, SpanHypothesis a) => (a.TempThis.SourceLength + a.TempThis.TargetLength).CompareTo(b.TempThis.SourceLength + b.TempThis.TargetLength));
					list.RemoveRange(1, list.Count - 1);
				}
				int num3 = 0;
				List<LiftAlignedSpanPair> list5 = new List<LiftAlignedSpanPair>();
				foreach (SpanHypothesis item5 in list)
				{
					list5.Add(item5.TempThis);
				}
				foreach (SpanHypothesis item6 in list)
				{
					LiftAlignedSpanPair tempThis = item6.TempThis;
					tempThis.Confidence = item6.Confidence;
					_pair.AlignmentData.Add(tempThis);
					item6.Confirm();
					num3++;
					if (ConfirmEvidenceImmediately())
					{
						List<LiftAlignedSpanPair> list6 = ConfirmHypothesisEvidence(item6);
						foreach (SpanHypothesis hypothesis in _hypotheses)
						{
							foreach (LiftAlignedSpanPair item7 in list6)
							{
								if (hypothesis.ComputedConfidence > 0f && hypothesis.TempThis.Contradicts(item7))
								{
									hypothesis.Block();
									num3++;
								}
							}
						}
						list5.AddRange(list6);
					}
					foreach (SpanHypothesis hypothesis2 in _hypotheses)
					{
						if (hypothesis2.ComputedConfidence > 0f && hypothesis2.TempThis.Contradicts(item6.TempThis))
						{
							hypothesis2.Block();
							num3++;
						}
					}
				}
				LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair();
				liftAlignedSpanPair.SourceSpan.Length = 1;
				liftAlignedSpanPair.TargetSpan.Length = 1;
				List<AlignmentEvidence> list7 = new List<AlignmentEvidence>();
				foreach (LiftAlignedSpanPair item8 in list5)
				{
					for (int num4 = _evidence.Count - 1; num4 >= 0; num4--)
					{
						AlignmentEvidence alignmentEvidence = _evidence[num4];
						if (alignmentEvidence.GetIsNoLongerValid(item8))
						{
							list7.Add(alignmentEvidence);
							_evidence.RemoveAt(num4);
						}
					}
				}
				for (short num5 = 0; num5 < _pair.SourceTokens.Count; num5 = (short)(num5 + 1))
				{
					for (short num6 = 0; num6 < _pair.TargetTokens.Count; num6 = (short)(num6 + 1))
					{
						if (_evidenceMatrix[num5, num6] != null)
						{
							foreach (AlignmentEvidence item9 in list7)
							{
								if (_evidenceMatrix[num5, num6].Contains(item9))
								{
									_evidenceMatrix[num5, num6].Remove(item9);
								}
							}
						}
					}
				}
				if (list7.Count > 0)
				{
					ScoreHypotheses();
					InitialSort();
				}
			}
		}

		private void RepeatedSort(List<SpanHypothesis> rescoredHyps)
		{
			List<SpanHypothesis> list = new List<SpanHypothesis>();
			list.Capacity = _hypotheses.Count;
			int num = 0;
			int i = 0;
			while (num < _hypotheses.Count)
			{
				if (_hypotheses[num] == null || _hypotheses[num].ComputedConfidence == 0f)
				{
					num++;
				}
				else if (i < rescoredHyps.Count && rescoredHyps[i].ComputedConfidence > _hypotheses[num].ComputedConfidence)
				{
					list.Add(rescoredHyps[i]);
					i++;
				}
				else
				{
					list.Add(_hypotheses[num]);
					num++;
				}
			}
			for (; i < rescoredHyps.Count; i++)
			{
				list.Add(rescoredHyps[i]);
			}
			_hypotheses = list;
		}

		private void ScoreHypotheses()
		{
			foreach (SpanHypothesis hypothesis in _hypotheses)
			{
				ScoreHypothesis(hypothesis);
			}
		}

		private bool InvolvedInSkipped(List<SpanHypothesis> hypsSkipped, SpanHypothesis hyp)
		{
			foreach (SpanHypothesis item in hypsSkipped)
			{
				LiftAlignedSpanPair tempThis = hyp.TempThis;
				LiftAlignedSpanPair tempThis2 = item.TempThis;
				if (tempThis.SourceSpan.SpanEquals(tempThis2.SourceSpan))
				{
					return true;
				}
				if (tempThis.TargetSpan.SpanEquals(tempThis2.TargetSpan))
				{
					return true;
				}
			}
			return false;
		}

		private static bool HypothesesIntersectWithRemainder(SpanHypothesis hhypA, SpanHypothesis hhypB, out LiftAlignedSpanPair unionHyp, out LiftAlignedSpanPair intersectionHyp)
		{
			unionHyp = null;
			intersectionHyp = null;
			LiftAlignedSpanPair tempThis = hhypA.TempThis;
			LiftAlignedSpanPair tempThis2 = hhypB.TempThis;
			if (!tempThis.SourceSpan.Overlaps(tempThis2.SourceSpan))
			{
				return false;
			}
			if (!tempThis.TargetSpan.Overlaps(tempThis2.TargetSpan))
			{
				return false;
			}
			if (tempThis.SourceSpan.Covers(tempThis2.SourceSpan, 0))
			{
				return false;
			}
			if (tempThis2.SourceSpan.Covers(tempThis.SourceSpan, 0))
			{
				return false;
			}
			if (tempThis.TargetSpan.Covers(tempThis2.TargetSpan, 0))
			{
				return false;
			}
			if (tempThis2.TargetSpan.Covers(tempThis.TargetSpan, 0))
			{
				return false;
			}
			if (tempThis.SourceStartIndex < tempThis2.SourceStartIndex)
			{
				if (tempThis.TargetStartIndex >= tempThis2.TargetStartIndex)
				{
					return false;
				}
			}
			else if (tempThis2.TargetStartIndex >= tempThis.TargetStartIndex)
			{
				return false;
			}
			unionHyp = new LiftAlignedSpanPair();
			intersectionHyp = new LiftAlignedSpanPair();
			unionHyp.SourceSpan = tempThis.SourceSpan.Union(tempThis2.SourceSpan);
			unionHyp.TargetSpan = tempThis.TargetSpan.Union(tempThis2.TargetSpan);
			intersectionHyp.SourceSpan = tempThis.SourceSpan.Intersection(tempThis2.SourceSpan);
			intersectionHyp.TargetSpan = tempThis.TargetSpan.Intersection(tempThis2.TargetSpan);
			return true;
		}

		[Conditional("DEBUG")]
		private void DebugHypothesis(SpanHypothesis h)
		{
			DebugAlignedSpanPair(h.TempThis);
		}

		public void DebugAlignedSpanPair(LiftAlignedSpanPair spanPair)
		{
		}

		private void ScoreHypothesis(SpanHypothesis h)
		{
			LiftAlignedSpanPair spanPair = h.TempThis;
			List<AlignmentEvidence> list = new List<AlignmentEvidence>();
			List<AlignmentEvidence> list2 = new List<AlignmentEvidence>();
			foreach (AlignmentEvidence item in _evidence)
			{
				if (item.Concerns(spanPair, outside: false))
				{
					list.Add(item);
				}
				else if (item.Concerns(spanPair, outside: true))
				{
					list2.Add(item);
				}
			}
			list.Sort((AlignmentEvidence a, AlignmentEvidence b) => (a.ConfidenceFor(spanPair, outside: false) == b.ConfidenceFor(spanPair, outside: false)) ? ((a.FirstSourceTokenIndex == b.FirstSourceTokenIndex) ? a.FirstTargetTokenIndex.CompareTo(b.FirstTargetTokenIndex) : a.FirstSourceTokenIndex.CompareTo(b.FirstSourceTokenIndex)) : b.ConfidenceFor(spanPair, outside: false).CompareTo(a.ConfidenceFor(spanPair, outside: false)));
			list2.Sort((AlignmentEvidence a, AlignmentEvidence b) => (a.ConfidenceFor(spanPair, outside: true) == b.ConfidenceFor(spanPair, outside: true)) ? ((a.FirstSourceTokenIndex == b.FirstSourceTokenIndex) ? a.FirstTargetTokenIndex.CompareTo(b.FirstTargetTokenIndex) : a.FirstSourceTokenIndex.CompareTo(b.FirstSourceTokenIndex)) : b.ConfidenceFor(spanPair, outside: true).CompareTo(a.ConfidenceFor(spanPair, outside: true)));
			bool[] sourceTokenUseFlags = new bool[_pair.SourceTokens.Count];
			bool[] targetTokenUseFlags = new bool[_pair.TargetTokens.Count];
			float num = 0f;
			float num2 = 0f;
			foreach (AlignmentEvidence item2 in list)
			{
				if (item2.AttemptToUse(spanPair, sourceTokenUseFlags, targetTokenUseFlags, outside: false))
				{
					num += item2.SourceConfidenceFor(spanPair, outside: false);
					num2 += item2.TargetConfidenceFor(spanPair, outside: false);
				}
			}
			int sourceLength = spanPair.SourceLength;
			int targetLength = spanPair.TargetLength;
			float num3 = (num / (float)sourceLength + num2 / (float)targetLength) / 2f;
			num = 0f;
			num2 = 0f;
			foreach (AlignmentEvidence item3 in list2)
			{
				if (item3.AttemptToUse(spanPair, sourceTokenUseFlags, targetTokenUseFlags, outside: true))
				{
					num += item3.SourceConfidenceFor(spanPair, outside: true);
					num2 += item3.TargetConfidenceFor(spanPair, outside: true);
				}
			}
			int num4 = _pair.SourceTokens.Count - spanPair.SourceLength;
			int num5 = _pair.TargetTokens.Count - spanPair.TargetLength;
			for (int i = 0; i < _pair.SourceTokens.Count; i++)
			{
				if (!spanPair.SourceSpan.Covers(i) && !_sourceTokenHasEvidence[i])
				{
					num4--;
				}
			}
			for (int j = 0; j < _pair.TargetTokens.Count; j++)
			{
				if (!spanPair.TargetSpan.Covers(j) && !_targetTokenHasEvidence[j])
				{
					num5--;
				}
			}
			double num6 = -1.0;
			if (num4 > 0 && num5 > 0)
			{
				num6 = (num / (float)num4 + num2 / (float)num5) / 2f;
			}
			h.Confidence = num3;
			if (num6 > -1.0)
			{
				h.Confidence = (float)((double)num3 + num6) / 2f;
			}
		}

		private void CreateHypotheses(List<LiftSpan> sourceConstraintsList, List<LiftSpan> targetConstraintsList)
		{
			_sourceToTargetRatio = -1.0;
			if (_pair.SourceTokens.Count >= _minSourceLengthForAbsurditySkip && _pair.TargetTokens.Count >= _minTargetLengthForAbsurditySkip)
			{
				_sourceToTargetRatio = (double)_pair.SourceTokens.Count * 1.0 / (double)_pair.TargetTokens.Count;
			}
			if (sourceConstraintsList == null)
			{
				for (short num = 0; num <= _pair.SourceTokens.Count - _minHypothesisSourceLength; num = (short)(num + 1))
				{
					for (short num2 = _minHypothesisSourceLength; num2 <= _pair.SourceTokens.Count - num; num2 = (short)(num2 + 1))
					{
						if (num2 != _pair.SourceTokens.Count)
						{
							CreateHypothesesForSourceSpan(num, num2, targetConstraintsList);
						}
					}
				}
			}
			else
			{
				foreach (LiftSpan sourceConstraints in sourceConstraintsList)
				{
					CreateHypothesesForSourceSpan(sourceConstraints.StartIndex, sourceConstraints.Length, targetConstraintsList);
				}
			}
		}

		private void CreateHypothesesForSourceSpan(short sourceStartIndex, short sourceLength, List<LiftSpan> targetConstraintsList)
		{
			short num = MinTargetSpanLengthGivenSourceSpanLength(_sourceToTargetRatio, sourceLength);
			if (targetConstraintsList == null)
			{
				for (short num2 = 0; num2 <= _pair.TargetTokens.Count - num; num2 = (short)(num2 + 1))
				{
					short maxUnconditionalTargetLength = (short)(_pair.TargetTokens.Count - num2);
					maxUnconditionalTargetLength = MaxTargetSpanLengthGiveSourceSpanLength(_sourceToTargetRatio, sourceLength, maxUnconditionalTargetLength);
					for (short num3 = num; num3 <= maxUnconditionalTargetLength; num3 = (short)(num3 + 1))
					{
						if (num3 != _pair.TargetTokens.Count)
						{
							CreateHypothesisIfValid(sourceStartIndex, sourceLength, num2, num3);
						}
					}
				}
			}
			else
			{
				foreach (LiftSpan targetConstraints in targetConstraintsList)
				{
					CreateHypothesisIfValid(sourceStartIndex, sourceLength, targetConstraints.StartIndex, targetConstraints.Length);
				}
			}
		}

		private void CreateHypothesisIfValid(short sourceStartIndex, short sourceLength, short targetStartIndex, short targetLength)
		{
			LiftAlignedSpanPair spanPair = new LiftAlignedSpanPair(sourceStartIndex, sourceLength, targetStartIndex, targetLength);
			spanPair.Provenance = 1;
			if (_pair.AlignmentData.Contradicts(spanPair, repetitionIsContradiction: true))
			{
				return;
			}
			bool flag = false;
			List<LiftAlignedSpanPair> alignedSpanPairsCoveredBySpan = _pair.AlignmentData.GetAlignedSpanPairsCoveredBySpan(spanPair.SourceSpan, searchTargetText: false, 0);
			if (alignedSpanPairsCoveredBySpan.Any((LiftAlignedSpanPair x) => spanPair.TargetSpan.Covers(x.TargetSpan, 0)))
			{
				flag = true;
			}
			if (!flag)
			{
				for (int i = 0; i < sourceLength; i++)
				{
					for (int j = 0; j < targetLength; j++)
					{
						if (_evidenceMatrix[sourceStartIndex + i, targetStartIndex + j] != null)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				CreateHypothesis(sourceStartIndex, sourceLength, targetStartIndex, targetLength);
			}
		}

		private short MinTargetSpanLengthGivenSourceSpanLength(double sourceToTargetRatio, short sourceSpanLength)
		{
			if (sourceToTargetRatio < 0.0)
			{
				return _minHypothesisTargetLength;
			}
			int num = 0;
			if (sourceSpanLength > 19)
			{
				num = sourceSpanLength / 2;
			}
			else
			{
				switch (sourceSpanLength)
				{
				case 19:
					num = 8;
					break;
				case 18:
					num = 7;
					break;
				case 17:
					num = 6;
					break;
				case 16:
					num = 5;
					break;
				case 15:
					num = 5;
					break;
				case 14:
					num = 4;
					break;
				case 13:
					num = 3;
					break;
				case 12:
					num = 3;
					break;
				case 11:
					num = 3;
					break;
				default:
					num = 2;
					break;
				}
			}
			num = (int)((double)num * sourceToTargetRatio);
			return (short)Math.Max(num, _minHypothesisTargetLength);
		}

		private static short MaxTargetSpanLengthGiveSourceSpanLength(double sourceToTargetRatio, short sourceSpanLength, short maxUnconditionalTargetLength)
		{
			if (sourceToTargetRatio < 0.0)
			{
				return maxUnconditionalTargetLength;
			}
			int num = 0;
			switch (sourceSpanLength)
			{
			case 2:
				num = 8;
				break;
			case 3:
				num = 11;
				break;
			case 4:
				num = 14;
				break;
			case 5:
				num = 15;
				break;
			case 6:
				num = 17;
				break;
			case 7:
				num = 18;
				break;
			case 8:
				num = 19;
				break;
			case 9:
				num = 20;
				break;
			case 10:
				num = 20;
				break;
			default:
				num = sourceSpanLength * 2;
				break;
			}
			num = (int)((double)num * sourceToTargetRatio);
			return (short)Math.Min(maxUnconditionalTargetLength, num);
		}

		private void CreateHypothesis(int sourceStartOffset, int sourceLength, int targetStartOffset, int targetLength)
		{
			SpanHypothesis spanHypothesis = new SpanHypothesis(sourceStartOffset, sourceLength, targetStartOffset, targetLength);
			_hypotheses.Add(spanHypothesis);
			_hypArray[sourceStartOffset, sourceLength - 1, targetStartOffset, targetLength - 1] = spanHypothesis;
		}
	}
}
