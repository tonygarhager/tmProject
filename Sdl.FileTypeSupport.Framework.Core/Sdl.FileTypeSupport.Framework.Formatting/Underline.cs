using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class Underline : AbstractBooleanFormatting
	{
		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(Underline));

		public override string LocalizedFormattingName => Resources.UnderlineName;

		public Underline(bool value)
			: base(value)
		{
		}

		public Underline()
		{
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitUnderline(this);
		}

		public override object Clone()
		{
			return new Underline(Value);
		}
	}
}
