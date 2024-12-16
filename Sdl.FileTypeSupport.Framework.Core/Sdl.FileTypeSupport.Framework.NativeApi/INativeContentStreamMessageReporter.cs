namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeContentStreamMessageReporter : IBasicMessageReporter
	{
		void ReportMessage(object source, string origin, ErrorLevel level, string message, LocationMarkerId fromLocation, LocationMarkerId uptoLocation);
	}
}
