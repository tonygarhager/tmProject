using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public class RevisionTypeNotSupportedException : Exception
	{
		public RevisionTypeNotSupportedException(string message)
			: base(message)
		{
		}
	}
}
