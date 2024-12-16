using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface IBasicMessageReporterWithExtendedData
	{
		void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription, ExtendedMessageEventData extendedData);
	}
}
