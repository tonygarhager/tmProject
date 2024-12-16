using System.IO;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ISubContentWriter
	{
		void InitializeSubContentWriter(Stream originalSubContent);

		Stream GetSubContentStream();
	}
}
