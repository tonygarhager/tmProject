using System.IO;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ISubContentAware
	{
		void AddSubContent(Stream subContentStream);
	}
}
