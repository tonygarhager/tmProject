using System.IO;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ISubContentParser
	{
		void InitializeSubContentParser(Stream subContentStream);
	}
}
