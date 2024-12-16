using Sdl.LanguagePlatform.Core.EditDistance;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua
{
	public class TagAssociations : IEnumerable<TagAssociation>, IEnumerable
	{
		private readonly List<TagAssociation> _associations;

		private readonly Dictionary<int, int> _srcPositionIdx;

		private readonly Dictionary<int, int> _trgPositionIdx;

		public int Count => _associations.Count;

		public TagAssociation this[int p] => _associations[p];

		public TagAssociations()
		{
			_associations = new List<TagAssociation>();
			_srcPositionIdx = new Dictionary<int, int>();
			_trgPositionIdx = new Dictionary<int, int>();
		}

		public void Add(PairedTag srcTag, PairedTag trgTag)
		{
			Add(srcTag, trgTag, EditOperation.Undefined);
		}

		public void Add(PairedTag srcTag, PairedTag trgTag, EditOperation op)
		{
			if (op == EditOperation.Undefined)
			{
				op = ((srcTag == null) ? EditOperation.Insert : ((trgTag != null) ? EditOperation.Change : EditOperation.Delete));
			}
			int count = _associations.Count;
			_associations.Add(new TagAssociation(srcTag, trgTag, op));
			if (srcTag != null)
			{
				_srcPositionIdx.Add(srcTag.Start, count);
				_srcPositionIdx.Add(srcTag.End, count);
			}
			if (trgTag != null)
			{
				_trgPositionIdx.Add(trgTag.Start, count);
				_trgPositionIdx.Add(trgTag.End, count);
			}
		}

		public bool AreAssociated(int sourcePosition, int targetPosition)
		{
			TagAssociation bySourcePosition = GetBySourcePosition(sourcePosition);
			if (bySourcePosition == null)
			{
				return false;
			}
			if (bySourcePosition.TargetTag == null)
			{
				return false;
			}
			if (bySourcePosition.TargetTag.Start != targetPosition)
			{
				return bySourcePosition.TargetTag.End == targetPosition;
			}
			return true;
		}

		public TagAssociation GetBySourcePosition(int p)
		{
			if (_srcPositionIdx.TryGetValue(p, out int value))
			{
				return _associations[value];
			}
			return null;
		}

		public EditOperation GetOperationBySourcePosition(int p)
		{
			if (_srcPositionIdx.TryGetValue(p, out int value))
			{
				return _associations[value].Operation;
			}
			return EditOperation.Undefined;
		}

		public TagAssociation GetByTargetPosition(int p)
		{
			if (!_trgPositionIdx.TryGetValue(p, out int value))
			{
				return null;
			}
			return _associations[value];
		}

		public EditOperation GetOperationByTargetPosition(int p)
		{
			if (!_trgPositionIdx.TryGetValue(p, out int value))
			{
				return EditOperation.Undefined;
			}
			return _associations[value].Operation;
		}

		public IEnumerator<TagAssociation> GetEnumerator()
		{
			return _associations.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _associations.GetEnumerator();
		}
	}
}
