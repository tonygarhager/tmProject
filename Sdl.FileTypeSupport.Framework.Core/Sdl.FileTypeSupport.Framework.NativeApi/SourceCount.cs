using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public class SourceCount : ICloneable
	{
		public enum CountUnit
		{
			word,
			character
		}

		private CountUnit _unit;

		private long _value;

		public CountUnit Unit
		{
			get
			{
				return _unit;
			}
			set
			{
				_unit = value;
			}
		}

		public long Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public SourceCount()
		{
		}

		protected SourceCount(SourceCount other)
		{
			_unit = other._unit;
			_value = other._value;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SourceCount sourceCount = (SourceCount)obj;
			if (sourceCount._unit != _unit)
			{
				return false;
			}
			if (sourceCount._value != _value)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _unit.GetHashCode() ^ _value.GetHashCode();
		}

		public object Clone()
		{
			return new SourceCount(this);
		}
	}
}
