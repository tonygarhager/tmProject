using Sdl.FileTypeSupport.Framework.Core.Properties;
using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	[Serializable]
	public class UnknownFormatting : AbstractFormattingItem
	{
		private string _Name = string.Empty;

		private string _Value = string.Empty;

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

		public override string FormattingName => _Name;

		public override string LocalizedFormattingName => Resources.UnknownFormatting;

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

		public UnknownFormatting()
		{
		}

		public UnknownFormatting(string name, string value)
		{
			_Name = name;
			_Value = value;
		}

		public void SetFormattingName(string name)
		{
			_Name = name;
		}

		public override void AcceptVisitor(IFormattingVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitUnknownFormatting(this);
		}

		public override object Clone()
		{
			return new UnknownFormatting(_Name, _Value);
		}
	}
}
