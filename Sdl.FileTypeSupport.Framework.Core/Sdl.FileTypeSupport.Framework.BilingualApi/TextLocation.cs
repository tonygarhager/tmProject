using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public class TextLocation : ICloneable
	{
		private Location _location;

		private int _textOffset;

		public Location Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public int TextOffset
		{
			get
			{
				return _textOffset;
			}
			set
			{
				_textOffset = value;
			}
		}

		public bool IsValid
		{
			get
			{
				if (_location == null || !_location.IsValid)
				{
					return false;
				}
				if (_textOffset != 0)
				{
					IText text = _location.ItemAtLocation as IText;
					if (text == null)
					{
						return false;
					}
					if (_textOffset > text.Properties.Text.Length)
					{
						return false;
					}
				}
				return true;
			}
		}

		public TextLocation()
		{
		}

		public TextLocation(IAbstractMarkupData item)
		{
			_location = new Location(item.ParentParagraph, item);
			_textOffset = 0;
		}

		public TextLocation(IText textItem, int offsetIntoText)
		{
			_location = new Location(textItem.ParentParagraph, textItem);
			_textOffset = offsetIntoText;
		}

		public TextLocation(Location location, int textOffset)
		{
			_location = location;
			_textOffset = textOffset;
		}

		protected TextLocation(TextLocation other)
		{
			if (other._location != null)
			{
				_location = (Location)other._location.Clone();
			}
			_textOffset = other._textOffset;
		}

		public override string ToString()
		{
			if (!IsValid)
			{
				return "[invalid]";
			}
			string text = (_location != null) ? _location.ToString() : "(no location)";
			if (_textOffset != 0)
			{
				if (_textOffset < text.Length)
				{
					return text.Substring(_textOffset);
				}
				return $"{_textOffset} chars into {text}";
			}
			return text;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			TextLocation textLocation = (TextLocation)obj;
			if (textLocation._location == null != (_location == null))
			{
				return false;
			}
			if (_location != null && !_location.Equals(textLocation._location))
			{
				return false;
			}
			if (_textOffset != textLocation._textOffset)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_location != null) ? _location.GetHashCode() : 0) ^ _textOffset.GetHashCode();
		}

		public object Clone()
		{
			return new TextLocation(this);
		}
	}
}
