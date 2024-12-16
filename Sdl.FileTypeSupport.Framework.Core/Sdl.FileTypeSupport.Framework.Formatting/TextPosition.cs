using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class TextPosition : AbstractFormattingItem
	{
		public enum SuperSub
		{
			Normal,
			Superscript,
			Subscript,
			Invalid
		}

		private SuperSub _Value;

		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(TextPosition));

		public override string LocalizedFormattingName => Resources.TextPositionName;

		public SuperSub Value
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
				return _Value.ToString();
			}
			set
			{
				_Value = (SuperSub)Enum.Parse(typeof(SuperSub), value);
			}
		}

		public override string LocalizedStringValue
		{
			get
			{
				switch (_Value)
				{
				case SuperSub.Invalid:
					return Resources.InvalidSuperSubName;
				case SuperSub.Normal:
					return Resources.NormalSuperSubName;
				case SuperSub.Subscript:
					return Resources.SubscriptName;
				case SuperSub.Superscript:
					return Resources.SuperscriptName;
				default:
					return "";
				}
			}
		}

		public TextPosition(SuperSub value)
		{
			_Value = value;
		}

		public TextPosition()
		{
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitTextPosition(this);
		}

		public override object Clone()
		{
			return new TextPosition(_Value);
		}
	}
}
