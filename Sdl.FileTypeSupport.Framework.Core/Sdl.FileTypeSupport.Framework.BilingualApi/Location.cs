using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public class Location : ICloneable
	{
		private List<LevelLocation> _levels = new List<LevelLocation>();

		public List<LevelLocation> Levels => _levels;

		public bool IsAtEnd
		{
			get
			{
				if (_levels.Count == 0)
				{
					return true;
				}
				if (_levels.Count > 1)
				{
					return false;
				}
				if (BottomLevel.Parent == null)
				{
					return true;
				}
				if (BottomLevel.Index >= BottomLevel.Parent.Count)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsAtStart
		{
			get
			{
				if (_levels.Count == 0)
				{
					return true;
				}
				if (_levels.Count > 1)
				{
					return false;
				}
				if (BottomLevel.Parent == null)
				{
					return true;
				}
				if (BottomLevel.Index == 0)
				{
					return true;
				}
				return false;
			}
		}

		public LevelLocation BottomLevel
		{
			get
			{
				if (_levels.Count > 0)
				{
					return _levels[Levels.Count - 1];
				}
				return null;
			}
		}

		public IAbstractMarkupData ItemAtLocation
		{
			get
			{
				if (!IsValid)
				{
					return null;
				}
				return BottomLevel.ItemAtLocation;
			}
		}

		public int Depth => _levels.Count;

		public bool IsValid
		{
			get
			{
				if (_levels.Count == 0)
				{
					return false;
				}
				LevelLocation levelLocation = null;
				foreach (LevelLocation level in _levels)
				{
					if (!level.IsValid)
					{
						return false;
					}
					if (levelLocation != null && levelLocation.ItemAtLocation != level.Parent)
					{
						return false;
					}
					levelLocation = level;
				}
				return true;
			}
		}

		public Location()
		{
		}

		public Location(LevelLocation location)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			_levels.Add(location);
		}

		public Location(IAbstractMarkupDataContainer rootContainer, bool firstLocation)
		{
			if (rootContainer == null)
			{
				throw new ArgumentNullException("rootContainer");
			}
			_levels.Add(new LevelLocation(rootContainer, (!firstLocation) ? rootContainer.Count : 0));
		}

		public Location(IAbstractMarkupDataContainer rootContainer, IAbstractMarkupData itemAtLocation)
		{
			if (rootContainer == null)
			{
				throw new ArgumentNullException("rootContainer");
			}
			if (itemAtLocation == null)
			{
				throw new ArgumentNullException("itemAtLocation");
			}
			IAbstractMarkupDataContainer parent = itemAtLocation.Parent;
			IAbstractMarkupData abstractMarkupData = itemAtLocation;
			do
			{
				_levels.Insert(0, new LevelLocation(parent, abstractMarkupData.IndexInParent));
				if (parent != rootContainer)
				{
					abstractMarkupData = (parent as IAbstractMarkupData);
					if (abstractMarkupData == null)
					{
						throw new ArgumentException("The item is not in the root container.", "itemAtLocation");
					}
					parent = abstractMarkupData.Parent;
					continue;
				}
				return;
			}
			while (parent != null);
			throw new ArgumentException("The item is not in the root container.", "itemAtLocation");
		}

		public Location(Location other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (LevelLocation level in other._levels)
			{
				_levels.Add((LevelLocation)level.Clone());
			}
		}

		public bool MoveNext()
		{
			if (!MoveFirstChild() && !MoveNextSibling())
			{
				if (!MoveParent())
				{
					return false;
				}
				return MoveNextSibling();
			}
			return true;
		}

		public bool MovePrevious()
		{
			if (!MovePreviousSibling())
			{
				if (!MoveParent())
				{
					return false;
				}
				return true;
			}
			MoveLastChild();
			return true;
		}

		public bool MoveNextSibling()
		{
			if (Depth == 0)
			{
				return false;
			}
			if (BottomLevel.Index < BottomLevel.Parent.Count)
			{
				int num = ++BottomLevel.Index;
				return true;
			}
			return false;
		}

		public bool MovePreviousSibling()
		{
			if (Depth == 0)
			{
				return false;
			}
			if (BottomLevel.Index > 0)
			{
				int num = --BottomLevel.Index;
				return true;
			}
			return false;
		}

		public bool MoveParent()
		{
			if (Depth > 1)
			{
				_levels.RemoveAt(_levels.Count - 1);
				return true;
			}
			return false;
		}

		public bool MoveFirstChild()
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = ItemAtLocation as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer == null)
			{
				return false;
			}
			_levels.Add(new LevelLocation(abstractMarkupDataContainer, 0));
			return true;
		}

		public bool MoveLastChild()
		{
			IAbstractMarkupDataContainer abstractMarkupDataContainer = ItemAtLocation as IAbstractMarkupDataContainer;
			if (abstractMarkupDataContainer == null)
			{
				return false;
			}
			_levels.Add(new LevelLocation(abstractMarkupDataContainer, abstractMarkupDataContainer.Count));
			return true;
		}

		public object Clone()
		{
			return new Location(this);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Location location = (Location)obj;
			if (_levels.Count != location._levels.Count)
			{
				return false;
			}
			if (!HasMatchingLevels(location))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _levels.GetHashCode();
		}

		public override string ToString()
		{
			if (!IsValid)
			{
				return "[invalid]";
			}
			if (IsAtEnd)
			{
				return "[at end]";
			}
			if (BottomLevel != null)
			{
				if (BottomLevel.IsAtEndOfParent)
				{
					return "[at end of '" + BottomLevel.Parent.ToString() + "']";
				}
				return BottomLevel.ToString();
			}
			return "[unknown location]";
		}

		private bool HasMatchingLevels(Location other)
		{
			for (int i = 0; i < Math.Min(Depth, other.Depth); i++)
			{
				if (!_levels[i].Equals(other._levels[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
