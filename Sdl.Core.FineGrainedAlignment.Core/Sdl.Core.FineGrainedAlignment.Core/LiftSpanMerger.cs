using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.FineGrainedAlignment.Core
{
	public class LiftSpanMerger
	{
		private readonly BitArray _bitarray;

		private double[] _confidenceVals = new double[100];

		private int _highestIndexSet = -1;

		private int _lowestIndexSet = -1;

		public LiftSpanMerger(int size = 0)
		{
			_bitarray = new BitArray(size);
		}

		public BitArray ToBitArray()
		{
			return new BitArray(_bitarray);
		}

		public bool Overlaps(BitArray bitarray)
		{
			BitArray bitArray = new BitArray(_bitarray);
			BitArray bitArray2 = new BitArray(bitarray);
			if (bitArray.Length < bitArray2.Length)
			{
				bitArray.Length = bitArray2.Length;
			}
			if (bitArray2.Length < bitArray.Length)
			{
				bitArray2.Length = bitArray.Length;
			}
			return bitArray.And(bitArray2).Cast<bool>().Contains(value: true);
		}

		public bool Covers(BitArray bitarray)
		{
			int num = bitarray.Cast<bool>().Count((bool x) => x);
			BitArray bitArray = new BitArray(_bitarray);
			BitArray bitArray2 = new BitArray(bitarray);
			if (bitArray.Length < bitArray2.Length)
			{
				bitArray.Length = bitArray2.Length;
			}
			if (bitArray2.Length < bitArray.Length)
			{
				bitArray2.Length = bitArray.Length;
			}
			int num2 = bitArray.And(bitArray2).Cast<bool>().Count((bool x) => x);
			return num == num2;
		}

		public bool Covers(LiftSpan span)
		{
			if (span.Length == 0)
			{
				return false;
			}
			if (GetHighestIndex() < span.StartIndex)
			{
				return false;
			}
			if (GetLowestIndex() > span.StartIndex + span.Length - 1)
			{
				return false;
			}
			List<LiftSpan> spans = GetSpans(span.StartIndex, span.Length);
			return spans.Count == 1;
		}

		public bool Overlaps(LiftSpan span)
		{
			if (span.Length == 0)
			{
				return false;
			}
			if (GetHighestIndex() < span.StartIndex)
			{
				return false;
			}
			if (GetLowestIndex() > span.StartIndex + span.Length - 1)
			{
				return false;
			}
			List<LiftSpan> spans = GetSpans(span.StartIndex, span.Length);
			return spans.Count > 0;
		}

		public void Clear()
		{
			_highestIndexSet = -1;
			_lowestIndexSet = -1;
			_bitarray.SetAll(value: false);
		}

		public int GetLowestIndex()
		{
			return _lowestIndexSet;
		}

		public int GetHighestIndex()
		{
			return _highestIndexSet;
		}

		private void GrowIfNeeded(int lengthRequired)
		{
			if (lengthRequired > _bitarray.Length)
			{
				_bitarray.Length = lengthRequired;
			}
			if (_confidenceVals.Length < _bitarray.Length)
			{
				Array.Resize(ref _confidenceVals, _bitarray.Length + 50);
			}
		}

		public void SetAt(int pos, bool set = true, double Confidence = 0.0)
		{
			GrowIfNeeded(pos + 1);
			_bitarray.Set(pos, set);
			_confidenceVals[pos] = Confidence;
			if (set)
			{
				_highestIndexSet = Math.Max(_highestIndexSet, pos);
				if (_lowestIndexSet == -1)
				{
					_lowestIndexSet = pos;
				}
				else
				{
					_lowestIndexSet = Math.Min(_lowestIndexSet, pos);
				}
				return;
			}
			_lowestIndexSet = -1;
			for (int i = 0; i < _bitarray.Length; i++)
			{
				if (_bitarray[i])
				{
					_lowestIndexSet = i;
					break;
				}
			}
			if (_lowestIndexSet == -1)
			{
				_highestIndexSet = -1;
				return;
			}
			int num = _bitarray.Length - 1;
			while (true)
			{
				if (num >= 0)
				{
					if (_bitarray[num])
					{
						break;
					}
					num--;
					continue;
				}
				return;
			}
			_highestIndexSet = num;
		}

		public void AddSpan(LiftSpan s, double Confidence)
		{
			GrowIfNeeded(s.StartIndex + s.Length);
			for (int i = 0; i < s.Length; i++)
			{
				_bitarray.Set(s.StartIndex + i, value: true);
				_confidenceVals[s.StartIndex + i] = Confidence;
			}
			if (s.Length > 0)
			{
				_highestIndexSet = Math.Max(_highestIndexSet, s.EndIndex);
				if (_lowestIndexSet == -1)
				{
					_lowestIndexSet = s.StartIndex;
				}
				else
				{
					_lowestIndexSet = Math.Min(_lowestIndexSet, s.StartIndex);
				}
			}
		}

		public List<LiftSpan> GetSpans()
		{
			if (_bitarray.Length == 0)
			{
				return new List<LiftSpan>();
			}
			return GetSpans(0, (short)_bitarray.Length, inverse: false);
		}

		public List<LiftSpan> GetSpans(short startFrom, short length)
		{
			return GetSpans(startFrom, length, inverse: false);
		}

		public double GetConfidence(int startFrom, int length)
		{
			GrowIfNeeded(startFrom + length);
			double num = 0.0;
			for (int i = 0; i < length; i++)
			{
				num += _confidenceVals[startFrom + i];
			}
			return num / (double)length;
		}

		public List<LiftSpan> GetInverseSpans(short startFrom, short length)
		{
			return GetSpans(startFrom, length, inverse: true);
		}

		public List<LiftSpan> GetInverseSpans()
		{
			if (_bitarray.Length == 0)
			{
				return new List<LiftSpan>();
			}
			return GetSpans(0, (short)_bitarray.Length, inverse: true);
		}

		private List<LiftSpan> GetSpans(short startFrom, short length, bool inverse)
		{
			GrowIfNeeded(startFrom + length);
			List<LiftSpan> list = new List<LiftSpan>();
			bool flag = !inverse;
			short num = startFrom;
			bool flag2 = false;
			for (short num2 = 0; num2 < length; num2 = (short)(num2 + 1))
			{
				bool flag3 = _bitarray.Get(startFrom + num2);
				if (flag3 != flag2 && num2 > 0)
				{
					if (flag2 == flag)
					{
						LiftSpan liftSpan = new LiftSpan();
						liftSpan.StartIndex = num;
						liftSpan.Length = (short)(num2 - (num - startFrom));
						list.Add(liftSpan);
					}
					num = (short)(startFrom + num2);
				}
				flag2 = flag3;
			}
			if (length > 0 && flag2 == flag)
			{
				LiftSpan liftSpan2 = new LiftSpan();
				liftSpan2.StartIndex = num;
				liftSpan2.Length = (short)(length - (num - startFrom));
				list.Add(liftSpan2);
			}
			return list;
		}
	}
}
