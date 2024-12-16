using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IPlaceholderTagProperties : IAbstractTagProperties, IAbstractBasicTagProperties, IMetaDataContainer, ICloneable, IAbstractInlineTagProperties
	{
		SegmentationHint SegmentationHint
		{
			get;
			set;
		}

		string TextEquivalent
		{
			get;
			set;
		}

		bool HasTextEquivalent
		{
			get;
		}

		bool IsBreakableWhiteSpace
		{
			get;
			set;
		}
	}
}
