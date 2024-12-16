using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class Strikethrough : AbstractBooleanFormatting
	{
		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(Strikethrough));

		public override string LocalizedFormattingName => Resources.StrikethroughName;

		public Strikethrough(bool value)
			: base(value)
		{
		}

		public Strikethrough()
		{
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitStrikethrough(this);
		}

		public override object Clone()
		{
			return new Strikethrough(Value);
		}
	}
}
