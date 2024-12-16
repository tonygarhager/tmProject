using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class FontName : AbstractFormattingItem
	{
		private string _Value = "Arial Unicode MS";

		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(FontName));

		public override string LocalizedFormattingName => Resources.FontName;

		public string Value
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
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}

		public FontName(string value)
		{
			_Value = value;
		}

		public FontName()
		{
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitFontName(this);
		}

		public override object Clone()
		{
			return new FontName(_Value);
		}
	}
}
