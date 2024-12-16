using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Flags]
	public enum DependencyFileUsage
	{
		None = 0x0,
		Extraction = 0x1,
		Generation = 0x2,
		Final = 0x4
	}
}
