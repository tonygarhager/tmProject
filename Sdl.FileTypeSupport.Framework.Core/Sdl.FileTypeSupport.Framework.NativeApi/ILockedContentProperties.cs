using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ILockedContentProperties : ICloneable
	{
		LockTypeFlags LockType
		{
			get;
			set;
		}
	}
}
