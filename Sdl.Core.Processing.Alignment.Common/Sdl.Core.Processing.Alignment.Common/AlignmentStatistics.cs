using System;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Common
{
	public class AlignmentStatistics
	{
		private readonly int _sourceSegmentsCount;

		private readonly int _targetSegmentsCount;

		private readonly int _totalAlignmentsCount;

		private readonly IDictionary<AlignmentQuality, int> _alignmentsCountByQuality;

		private readonly IDictionary<AlignmentType, int> _alignmentsCountByType;

		internal IDictionary<AlignmentQuality, int> AlignmentsCountByQuality => _alignmentsCountByQuality;

		internal IDictionary<AlignmentType, int> AlignmentsCountByType => _alignmentsCountByType;

		public int SourceSegmentsCount => _sourceSegmentsCount;

		public int TargetSegmentsCount => _targetSegmentsCount;

		public int TotalAlignmentsCount => _totalAlignmentsCount;

		public AlignmentStatistics(int sourceSegmentsCount, int targetSegmentsCount, int totalAlignmentsCount, IDictionary<AlignmentQuality, int> alignmentsCountByQuality, IDictionary<AlignmentType, int> alignmentsCountByType)
		{
			if (sourceSegmentsCount < 0)
			{
				throw new ArgumentOutOfRangeException("sourceSegmentsCount", "sourceSegmentsCount cannot be negative");
			}
			if (targetSegmentsCount < 0)
			{
				throw new ArgumentOutOfRangeException("targetSegmentsCount", "targetSegmentsCount cannot be negative");
			}
			if (totalAlignmentsCount < 0)
			{
				throw new ArgumentOutOfRangeException("totalAlignmentsCount", "totalAlignmentsCount cannot be negative");
			}
			if (alignmentsCountByQuality == null)
			{
				throw new ArgumentNullException("alignmentsCountByQuality");
			}
			if (!IsValid(alignmentsCountByQuality))
			{
				throw new ArgumentException("alignmentsCountByQuality");
			}
			if (alignmentsCountByType == null)
			{
				throw new ArgumentNullException("alignmentsCountByType");
			}
			if (!IsValid(alignmentsCountByType))
			{
				throw new ArgumentException("alignmentsCountByType");
			}
			_sourceSegmentsCount = sourceSegmentsCount;
			_targetSegmentsCount = targetSegmentsCount;
			_totalAlignmentsCount = totalAlignmentsCount;
			_alignmentsCountByQuality = alignmentsCountByQuality;
			_alignmentsCountByType = alignmentsCountByType;
		}

		private bool IsValid<T>(IDictionary<T, int> alignmentsCountDictionary)
		{
			foreach (T value2 in Enum.GetValues(typeof(T)))
			{
				if (alignmentsCountDictionary.TryGetValue(value2, out int value) && value < 0)
				{
					return false;
				}
			}
			return true;
		}

		public int GetAlignmentsCount(AlignmentQuality alignmentQuality)
		{
			if (!_alignmentsCountByQuality.TryGetValue(alignmentQuality, out int value))
			{
				return 0;
			}
			return value;
		}

		public int GetAlignmentsCount(AlignmentType alignmentType)
		{
			if (!_alignmentsCountByType.TryGetValue(alignmentType, out int value))
			{
				return 0;
			}
			return value;
		}
	}
}
