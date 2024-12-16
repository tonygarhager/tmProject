using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualContentMessageReporter : IBasicMessageReporter
	{
		void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation);
	}
}
