namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IBasicMessageReporter
	{
		void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription);
	}
}
