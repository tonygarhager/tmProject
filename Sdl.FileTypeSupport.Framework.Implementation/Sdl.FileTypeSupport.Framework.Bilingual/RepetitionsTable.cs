using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class RepetitionsTable : IRepetitionsTable, ICloneable, ISupportsPersistenceId
	{
		private Dictionary<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> _Repetitions;

		[NonSerialized]
		private int _persistenceId;

		public IEnumerable<RepetitionId> RepetitionIds
		{
			get
			{
				foreach (RepetitionId key in _Repetitions.Keys)
				{
					yield return key;
				}
			}
		}

		public int Count => _Repetitions.Count;

		[XmlIgnore]
		public int PersistenceId
		{
			get
			{
				return _persistenceId;
			}
			set
			{
				_persistenceId = value;
			}
		}

		public RepetitionsTable()
		{
			_Repetitions = new Dictionary<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>>();
		}

		protected RepetitionsTable(RepetitionsTable other)
		{
			if (other._Repetitions != null)
			{
				_Repetitions = new Dictionary<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>>(other._Repetitions);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			RepetitionsTable repetitionsTable = (RepetitionsTable)obj;
			if (repetitionsTable._Repetitions == null != (_Repetitions == null))
			{
				return false;
			}
			if (_Repetitions != null)
			{
				if (_Repetitions.Count != repetitionsTable._Repetitions.Count)
				{
					return false;
				}
				foreach (KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> repetition in _Repetitions)
				{
					if (!repetitionsTable._Repetitions.TryGetValue(repetition.Key, out List<Pair<ParagraphUnitId, SegmentId>> value))
					{
						return false;
					}
					if (repetition.Value == null != (value == null))
					{
						return false;
					}
					if (repetition.Value != null && repetition.Value.Count != value.Count)
					{
						return false;
					}
					for (int i = 0; i < repetition.Value.Count; i++)
					{
						Pair<ParagraphUnitId, SegmentId> pair = repetition.Value[i];
						Pair<ParagraphUnitId, SegmentId> pair2 = value[i];
						if (pair == null != (pair2 == null))
						{
							return false;
						}
						if (pair != null && !pair.Equals(pair2))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (_Repetitions != null)
			{
				foreach (KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> repetition in _Repetitions)
				{
					int num2 = repetition.Value.Count;
					foreach (Pair<ParagraphUnitId, SegmentId> item in repetition.Value)
					{
						if (item != null)
						{
							num2 ^= item.GetHashCode();
						}
					}
					num ^= ((repetition.Key.GetHashCode() << 16) ^ num2);
				}
				return num;
			}
			return num;
		}

		public IList<Pair<ParagraphUnitId, SegmentId>> GetRepetitions(RepetitionId repetitions)
		{
			if (_Repetitions.ContainsKey(repetitions) && _Repetitions.TryGetValue(repetitions, out List<Pair<ParagraphUnitId, SegmentId>> value))
			{
				return value;
			}
			return null;
		}

		public RepetitionId GetRepetitionId(ParagraphUnitId paragraphUnitId, SegmentId segmentId)
		{
			Pair<ParagraphUnitId, SegmentId> lookFor = new Pair<ParagraphUnitId, SegmentId>(paragraphUnitId, segmentId);
			IEnumerable<RepetitionId> source = _Repetitions.Where(delegate(KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> repetition)
			{
				KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> keyValuePair2 = repetition;
				return keyValuePair2.Value.Contains(lookFor);
			}).Select(delegate(KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> repetition)
			{
				KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> keyValuePair = repetition;
				return keyValuePair.Key;
			});
			return source.FirstOrDefault();
		}

		public bool Remove(ParagraphUnitId paragraphUnitId, SegmentId segmentId)
		{
			Pair<ParagraphUnitId, SegmentId> lookFor = new Pair<ParagraphUnitId, SegmentId>(paragraphUnitId, segmentId);
			IEnumerable<KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>>> enumerable = _Repetitions.Where(delegate(KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> repetition)
			{
				KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> keyValuePair = repetition;
				return keyValuePair.Value.Contains(lookFor);
			});
			bool result = false;
			foreach (KeyValuePair<RepetitionId, List<Pair<ParagraphUnitId, SegmentId>>> item in enumerable)
			{
				item.Value.Remove(lookFor);
				result = true;
				if (item.Value.Count == 0)
				{
					_Repetitions.Remove(item.Key);
				}
			}
			return result;
		}

		public bool Add(RepetitionId key, ParagraphUnitId pu, SegmentId newRepetition)
		{
			if (_Repetitions.ContainsKey(key))
			{
				if (_Repetitions.TryGetValue(key, out List<Pair<ParagraphUnitId, SegmentId>> value))
				{
					value.Add(new Pair<ParagraphUnitId, SegmentId>(pu, newRepetition));
					return true;
				}
				return false;
			}
			List<Pair<ParagraphUnitId, SegmentId>> list = new List<Pair<ParagraphUnitId, SegmentId>>();
			list.Add(new Pair<ParagraphUnitId, SegmentId>(pu, newRepetition));
			_Repetitions.Add(key, list);
			return true;
		}

		public void DeleteKey(RepetitionId repetitions)
		{
			if (_Repetitions.ContainsKey(repetitions))
			{
				_Repetitions.Remove(repetitions);
			}
		}

		public void Clear()
		{
			_Repetitions.Clear();
		}

		public object Clone()
		{
			return new RepetitionsTable(this);
		}
	}
}
