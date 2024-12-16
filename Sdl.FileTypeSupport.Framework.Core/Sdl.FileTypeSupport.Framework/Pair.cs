using System;

namespace Sdl.FileTypeSupport.Framework
{
	[Serializable]
	public class Pair<FirstType, SecondType> : ICloneable
	{
		private FirstType _First;

		private SecondType _Second;

		public FirstType First
		{
			get
			{
				return _First;
			}
			set
			{
				_First = value;
			}
		}

		public SecondType Second
		{
			get
			{
				return _Second;
			}
			set
			{
				_Second = value;
			}
		}

		public Pair(FirstType first, SecondType second)
		{
			_First = first;
			_Second = second;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Pair<FirstType, SecondType> pair = (Pair<FirstType, SecondType>)obj;
			if (_First == null != (pair._First == null))
			{
				return false;
			}
			if (_First != null && !_First.Equals(pair._First))
			{
				return false;
			}
			if (_Second == null != (pair._Second == null))
			{
				return false;
			}
			if (_Second != null && !_Second.Equals(pair._Second))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_First != null) ? _First.GetHashCode() : 0) ^ ((_Second != null) ? _Second.GetHashCode() : 0);
		}

		public virtual object Clone()
		{
			FirstType first = _First;
			ICloneable cloneable = _First as ICloneable;
			if (cloneable != null)
			{
				first = (FirstType)cloneable.Clone();
			}
			SecondType second = _Second;
			cloneable = (_Second as ICloneable);
			if (cloneable != null)
			{
				second = (SecondType)cloneable.Clone();
			}
			return new Pair<FirstType, SecondType>(first, second);
		}
	}
}
