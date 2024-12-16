using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IParser : IDisposable
	{
		event EventHandler<ProgressEventArgs> Progress;

		bool ParseNext();
	}
}
