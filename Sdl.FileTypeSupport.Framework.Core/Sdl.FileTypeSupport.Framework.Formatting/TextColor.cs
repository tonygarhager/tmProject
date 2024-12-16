using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class TextColor : AbstractFormattingItem
	{
		private Color _Value = SystemColors.WindowText;

		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(TextColor));

		public override string LocalizedFormattingName => Resources.TextColorName;

		public Color Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}

		public override string StringValue
		{
			get
			{
				return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, CultureInfo.InvariantCulture, _Value);
			}
			set
			{
				if (value == null)
				{
					_Value = SystemColors.WindowText;
				}
				else
				{
					_Value = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(null, CultureInfo.InvariantCulture, value);
				}
			}
		}

		public override string LocalizedStringValue => TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, CultureInfo.CurrentUICulture, _Value);

		public TextColor(Color value)
		{
			_Value = value;
		}

		public TextColor()
		{
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}
			TextColor textColor = (TextColor)obj;
			if (Value.ToArgb() == textColor.Value.ToArgb())
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.ToArgb().GetHashCode();
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitTextColor(this);
		}

		public override object Clone()
		{
			return new TextColor(_Value);
		}
	}
}
