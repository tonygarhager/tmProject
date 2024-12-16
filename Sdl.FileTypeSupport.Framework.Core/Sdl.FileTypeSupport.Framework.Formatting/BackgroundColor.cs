using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class BackgroundColor : AbstractFormattingItem
	{
		private Color _Value = SystemColors.Window;

		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(BackgroundColor));

		public override string LocalizedFormattingName => Resources.BackColorName;

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
					_Value = SystemColors.Window;
				}
				else
				{
					_Value = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(null, CultureInfo.InvariantCulture, value);
				}
			}
		}

		public override string LocalizedStringValue => TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, CultureInfo.CurrentUICulture, _Value);

		public BackgroundColor(Color value)
		{
			_Value = value;
		}

		public BackgroundColor()
		{
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}
			BackgroundColor backgroundColor = (BackgroundColor)obj;
			if (Value.ToArgb() == backgroundColor.Value.ToArgb())
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
			visitor.VisitBackgroundColor(this);
		}

		public override object Clone()
		{
			return new BackgroundColor(_Value);
		}
	}
}
