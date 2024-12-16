using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ISubContentPublisher
	{
		event EventHandler<ProcessSubContentEventArgs> ProcessSubContent;
	}
}
