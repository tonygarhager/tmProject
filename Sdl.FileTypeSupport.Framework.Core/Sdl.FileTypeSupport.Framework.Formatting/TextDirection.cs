using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class TextDirection : AbstractFormattingItem
	{
		private Direction _Direction;

		public static string Name => AbstractFormattingItem.GetDefaultName(typeof(TextDirection));

		public override string LocalizedFormattingName => Resources.TextDirectionName;

		public override string StringValue
		{
			get
			{
				return _Direction.ToString();
			}
			set
			{
				_Direction = (Direction)Enum.Parse(typeof(Direction), value);
			}
		}

		public override string LocalizedStringValue
		{
			get
			{
				switch (_Direction)
				{
				case Direction.Inherit:
					return Resources.InheritName;
				case Direction.LeftToRight:
					return Resources.LeftToRightName;
				case Direction.RightToLeft:
					return Resources.RightToLeftName;
				default:
					return "";
				}
			}
		}

		public Direction Direction
		{
			get
			{
				return _Direction;
			}
			set
			{
				_Direction = value;
			}
		}

		public TextDirection()
		{
		}

		public TextDirection(Direction direction)
		{
			_Direction = direction;
		}

		public TextDirection(TextDirection other)
		{
			_Direction = other.Direction;
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitTextDirection(this);
		}

		public override object Clone()
		{
			return new TextDirection(this);
		}
	}
}
