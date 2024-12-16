using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;
using System.Globalization;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class FontSize : AbstractFormattingItem
	{
		private float _Value = 12f;

		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(FontSize));

		public override string LocalizedFormattingName => Resources.FontSizeName;

		public float Value
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
				return _Value.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				_Value = float.Parse(value, CultureInfo.InvariantCulture);
			}
		}

		public FontSize(float value)
		{
			_Value = value;
		}

		public FontSize()
		{
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitFontSize(this);
		}

		public override object Clone()
		{
			return new FontSize(_Value);
		}
	}
}
