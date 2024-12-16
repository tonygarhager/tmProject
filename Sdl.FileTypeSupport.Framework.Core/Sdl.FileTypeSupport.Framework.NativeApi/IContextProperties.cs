using Sdl.FileTypeSupport.Framework.Formatting;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IContextProperties : ICloneable
	{
		IList<IContextInfo> Contexts
		{
			get;
		}

		IStructureInfo StructureInfo
		{
			get;
			set;
		}

		IFormattingGroup EffectiveDefaultFormatting
		{
			get;
		}
	}
}
