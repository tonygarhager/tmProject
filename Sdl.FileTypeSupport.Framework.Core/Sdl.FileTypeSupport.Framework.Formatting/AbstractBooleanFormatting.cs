using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public abstract class AbstractBooleanFormatting : AbstractFormattingItem
	{
		private bool _Value = true;

		public virtual bool Value
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
				_Value = bool.Parse(value);
			}
		}

		public override string LocalizedStringValue
		{
			get
			{
				if (!_Value)
				{
					return Resources.FalseName;
				}
				return Resources.TrueName;
			}
		}

		protected AbstractBooleanFormatting(bool value)
		{
			_Value = value;
		}

		protected AbstractBooleanFormatting()
		{
		}
	}
}
