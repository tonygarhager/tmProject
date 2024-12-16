using System;
using System.Reflection;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public abstract class AbstractFormattingItem : IFormattingItem, ICloneable
	{
		public virtual string FormattingName => GetDefaultName(GetType());

		public virtual string LocalizedFormattingName => FormattingName;

		public abstract string StringValue
		{
			get;
			set;
		}

		public virtual string LocalizedStringValue => StringValue;

		protected static string GetDefaultName(MemberInfo type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return type.Name;
		}

		public abstract void AcceptVisitor(IFormattingVisitor visitor);

		public override bool Equals(object obj)
		{
			IFormattingItem formattingItem = obj as IFormattingItem;
			if (obj == null)
			{
				return false;
			}
			if (FormattingName != formattingItem.FormattingName)
			{
				return false;
			}
			if (StringValue != formattingItem.StringValue)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((FormattingName != null) ? FormattingName.GetHashCode() : 0) ^ ((StringValue != null) ? StringValue.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("{0} = {1}", (FormattingName != null) ? FormattingName : "(null)", (StringValue != null) ? StringValue : "(null)");
		}

		public abstract object Clone();
	}
}
