using Sdl.Core.Processing.Alignment.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Core.Processing.Alignment.Core
{
	public class AlignmentData : ICloneable, IComparable<AlignmentData>
	{
		private readonly IList<DocumentSegmentId> _leftIds = new List<DocumentSegmentId>();

		private readonly IList<DocumentSegmentId> _rightIds = new List<DocumentSegmentId>();

		private AlignmentCost _cost;

		private bool _confirmed;

		private AlignmentType _AlignmentType;

		public int LeftOrder
		{
			get
			{
				if (_leftIds != null && _leftIds.Count != 0)
				{
					return _leftIds[0].Order;
				}
				return -1;
			}
		}

		public int RightOrder
		{
			get
			{
				if (_rightIds != null && _rightIds.Count != 0)
				{
					return _rightIds[0].Order;
				}
				return -1;
			}
		}

		public IList<DocumentSegmentId> LeftIds => _leftIds;

		public IList<DocumentSegmentId> RightIds => _rightIds;

		public AlignmentType AlignmentType
		{
			get
			{
				if (_AlignmentType != 0)
				{
					return _AlignmentType;
				}
				int leftSegmentsCount = (_leftIds != null) ? _leftIds.Count : 0;
				int rightSegmentsCount = (_rightIds != null) ? _rightIds.Count : 0;
				return AlignmentHelper.GetAlignmentType(leftSegmentsCount, rightSegmentsCount);
			}
		}

		internal AlignmentCost Cost
		{
			get
			{
				return _cost;
			}
			set
			{
				_cost = value;
			}
		}

		public AlignmentQuality Quality
		{
			get;
			set;
		}

		public bool Confirmed
		{
			get
			{
				return _confirmed;
			}
			set
			{
				_confirmed = value;
				if (_confirmed)
				{
					_cost = AlignmentCost.MinValue;
					Quality = AlignmentQuality.Good;
				}
			}
		}

		public AlignmentData()
		{
			_confirmed = false;
			_AlignmentType = AlignmentType.Invalid;
			Quality = AlignmentQuality.Average;
		}

		public AlignmentData(AlignmentType alignmentType)
			: this()
		{
			_AlignmentType = alignmentType;
		}

		public AlignmentData(IList<DocumentSegmentId> leftIds, IList<DocumentSegmentId> rightIds, AlignmentCost cost, bool confirmed = false)
		{
			if (leftIds == null)
			{
				throw new ArgumentNullException("leftIds");
			}
			if (rightIds == null)
			{
				throw new ArgumentNullException("rightIds");
			}
			_leftIds = leftIds;
			_rightIds = rightIds;
			_confirmed = confirmed;
			_cost = (confirmed ? AlignmentCost.MinValue : cost);
			if ((double)_cost == (double)AlignmentCost.MinValue)
			{
				Quality = AlignmentQuality.Good;
			}
		}

		private AlignmentData(AlignmentData other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			_leftIds = new List<DocumentSegmentId>();
			foreach (DocumentSegmentId leftId in other._leftIds)
			{
				_leftIds.Add(new DocumentSegmentId(leftId.ParagraphUnitId, leftId.SegmentId, leftId.Order));
			}
			_rightIds = new List<DocumentSegmentId>();
			foreach (DocumentSegmentId rightId in other._rightIds)
			{
				_rightIds.Add(new DocumentSegmentId(rightId.ParagraphUnitId, rightId.SegmentId, rightId.Order));
			}
			_cost = other._cost;
			_confirmed = other.Confirmed;
		}

		public override bool Equals(object obj)
		{
			AlignmentData alignmentData = obj as AlignmentData;
			if (alignmentData != null && EqualIds(alignmentData.LeftIds, LeftIds))
			{
				return EqualIds(alignmentData.RightIds, RightIds);
			}
			return false;
		}

		private static bool EqualIds(IList<DocumentSegmentId> ids0, IList<DocumentSegmentId> ids1)
		{
			if (ids0.Count != ids1.Count)
			{
				return false;
			}
			for (int i = 0; i < ids0.Count; i++)
			{
				if (!ids0[i].Equals(ids1[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 31;
			for (int i = 0; i < LeftIds.Count; i++)
			{
				num += (i + 47) * LeftIds[i].GetHashCode();
			}
			for (int j = 0; j < RightIds.Count; j++)
			{
				num += (j + 171) * RightIds[j].GetHashCode();
			}
			return num;
		}

		public void Write()
		{
			Console.Out.WriteLine(ToString());
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (_leftIds.Count > 0)
			{
				foreach (DocumentSegmentId leftId in _leftIds)
				{
					stringBuilder.Append(leftId.ToString().PadLeft(2) + " ");
				}
			}
			else
			{
				stringBuilder.Append("   ");
			}
			stringBuilder.Append("| ");
			if (_rightIds.Count > 0)
			{
				foreach (DocumentSegmentId rightId in _rightIds)
				{
					stringBuilder.Append(rightId.ToString().PadLeft(2) + " ");
				}
			}
			else
			{
				stringBuilder.Append("   ");
			}
			stringBuilder.Append(": ");
			stringBuilder.Append(Cost);
			return stringBuilder.ToString();
		}

		public object Clone()
		{
			return new AlignmentData(this);
		}

		public int CompareTo(AlignmentData other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (LeftOrder >= 0 && other.LeftOrder >= 0)
			{
				return LeftOrder - other.LeftOrder;
			}
			if (RightOrder >= 0 && other.RightOrder >= 0)
			{
				return RightOrder - other.RightOrder;
			}
			return 0;
		}
	}
}
