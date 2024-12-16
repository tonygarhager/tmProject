using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualContentMessageReporterWithExtendedData : IBasicMessageReporterWithExtendedData
	{
		void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation, ExtendedMessageEventData extendedData);
	}
}
