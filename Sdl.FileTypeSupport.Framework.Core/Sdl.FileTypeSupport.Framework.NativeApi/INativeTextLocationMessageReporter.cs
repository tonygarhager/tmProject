namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeTextLocationMessageReporter : IBasicMessageReporter
	{
		void ReportMessage(object source, string origin, ErrorLevel level, string message, NativeTextLocation fromLocation, NativeTextLocation uptoLocation);
	}
}
