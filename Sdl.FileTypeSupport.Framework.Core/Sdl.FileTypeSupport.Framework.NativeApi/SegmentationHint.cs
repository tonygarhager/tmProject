using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public enum SegmentationHint
	{
		[Obsolete]
		Undefined,
		Include,
		MayExclude,
		IncludeWithText,
		Exclude
	}
}
