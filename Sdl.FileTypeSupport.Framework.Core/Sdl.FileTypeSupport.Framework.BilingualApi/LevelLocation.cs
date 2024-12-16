using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public class LevelLocation : ICloneable
	{
		private IAbstractMarkupDataContainer _parent;

		private int _index;

		public IAbstractMarkupDataContainer Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value;
			}
		}

		public IAbstractMarkupData ItemAtLocation
		{
			get
			{
				if (_parent == null || _index >= _parent.Count)
				{
					return null;
				}
				return _parent[_index];
			}
		}

		public bool IsValid
		{
			get
			{
				if (_parent != null && _index >= 0)
				{
					return _index <= _parent.Count;
				}
				return false;
			}
		}

		public bool IsAtEndOfParent
		{
			get
			{
				if (IsValid)
				{
					return _index == _parent.Count;
				}
				return false;
			}
		}

		public bool IsAtStartOfParent
		{
			get
			{
				if (IsValid)
				{
					return _index == 0;
				}
				return false;
			}
		}

		public LevelLocation()
		{
		}

		public LevelLocation(IAbstractMarkupDataContainer parent, int index)
		{
			_parent = parent;
			_index = index;
		}

		public LevelLocation(LevelLocation other)
		{
			_parent = other._parent;
			_index = other._index;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			LevelLocation levelLocation = (LevelLocation)obj;
			if (levelLocation._index != _index)
			{
				return false;
			}
			if (levelLocation._parent != _parent)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _index.GetHashCode() ^ ((_parent != null) ? _parent.GetHashCode() : 0);
		}

		public override string ToString()
		{
			if (!IsValid)
			{
				return "[invalid]";
			}
			IAbstractMarkupData itemAtLocation = ItemAtLocation;
			if (itemAtLocation != null)
			{
				return itemAtLocation.ToString();
			}
			if (IsAtEndOfParent)
			{
				return "[at end]";
			}
			if (IsAtStartOfParent)
			{
				return "[at start]";
			}
			return "[unknown position]";
		}

		public object Clone()
		{
			return new LevelLocation(this);
		}
	}
}
