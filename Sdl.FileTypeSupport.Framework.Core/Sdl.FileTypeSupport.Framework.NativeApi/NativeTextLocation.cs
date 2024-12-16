using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public class NativeTextLocation : ICloneable, IComparable<NativeTextLocation>
	{
		private int _line;

		private int _offset;

		public virtual int Line
		{
			get
			{
				return _line;
			}
			set
			{
				_line = value;
			}
		}

		public virtual int Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
			}
		}

		public NativeTextLocation()
		{
			_line = 0;
			_offset = 0;
		}

		public NativeTextLocation(int line, int offset)
		{
			_line = line;
			_offset = offset;
		}

		protected NativeTextLocation(NativeTextLocation other)
		{
			_line = other._line;
			_offset = other._offset;
		}

		public override string ToString()
		{
			return $"({_line}, {_offset})";
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			NativeTextLocation nativeTextLocation = (NativeTextLocation)obj;
			if (_line != nativeTextLocation._line)
			{
				return false;
			}
			if (_offset != nativeTextLocation._offset)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return (_line << 16).GetHashCode() ^ (_line >> 16).GetHashCode() ^ _offset.GetHashCode();
		}

		public bool IsBefore(NativeTextLocation other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_line < other._line)
			{
				return true;
			}
			if (_line > other._line)
			{
				return false;
			}
			return _offset < other._offset;
		}

		public bool IsAfter(NativeTextLocation other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (_line > other._line)
			{
				return true;
			}
			if (_line < other._line)
			{
				return false;
			}
			return _offset > other._offset;
		}

		public object Clone()
		{
			return new NativeTextLocation(this);
		}

		public int CompareTo(NativeTextLocation other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (IsBefore(other))
			{
				return -1;
			}
			if (IsAfter(other))
			{
				return 1;
			}
			return 0;
		}
	}
}
