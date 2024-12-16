using Sdl.FileTypeSupport.Framework.Formatting;
using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IStartTagProperties : IAbstractTagProperties, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable, IAbstractInlineTagProperties
	{
		SegmentationHint SegmentationHint
		{
			get;
			set;
		}

		IFormattingGroup Formatting
		{
			get;
			set;
		}
	}
}
