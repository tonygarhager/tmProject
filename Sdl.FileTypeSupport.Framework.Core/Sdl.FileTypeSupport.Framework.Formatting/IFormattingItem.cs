using System;

namespace Sdl.FileTypeSupport.Framework.Formatting
{
	public interface IFormattingItem : ICloneable
	{
		string FormattingName
		{
			get;
		}

		string StringValue
		{
			get;
			set;
		}

		string LocalizedFormattingName
		{
			get;
		}

		string LocalizedStringValue
		{
			get;
		}

		void AcceptVisitor(IFormattingVisitor visitor);
	}
}
