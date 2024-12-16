using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class Italic : AbstractBooleanFormatting
	{
		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(Italic));

		public override string LocalizedFormattingName => Resources.ItalicName;

		public Italic(bool value)
			: base(value)
		{
		}

		public Italic()
		{
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitItalic(this);
		}

		public override object Clone()
		{
			return new Italic(Value);
		}
	}
}
