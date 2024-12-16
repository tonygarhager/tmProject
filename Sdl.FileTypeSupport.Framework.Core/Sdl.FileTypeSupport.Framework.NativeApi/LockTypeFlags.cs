using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Flags]
	public enum LockTypeFlags
	{
		Unlocked = 0x0,
		Structure = 0x1,
		Externalized = 0x2,
		Manual = 0x4
	}
}
