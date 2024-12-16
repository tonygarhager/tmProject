using Sdl.FileTypeSupport.Framework.Formatting;
using System;
using System.Drawing;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IContextInfo : IMetaDataContainer, ICloneable
	{
		string ContextType
		{
			get;
			set;
		}

		string DisplayName
		{
			get;
			set;
		}

		string DisplayCode
		{
			get;
			set;
		}

		ContextPurpose Purpose
		{
			get;
			set;
		}

		string Description
		{
			get;
			set;
		}

		Color DisplayColor
		{
			get;
			set;
		}

		IFormattingGroup DefaultFormatting
		{
			get;
			set;
		}
	}
}
