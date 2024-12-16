using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class Bold : AbstractBooleanFormatting
	{
		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(Bold));

		public override string LocalizedFormattingName => Resources.BoldName;

		public Bold(bool value)
			: base(value)
		{
		}

		public Bold()
		{
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitBold(this);
		}

		public override object Clone()
		{
			return new Bold(Value);
		}
	}
}
