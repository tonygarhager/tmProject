using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public class BilingualContentMessageReporterProxy : IBilingualContentMessageReporter, IBasicMessageReporter, IBilingualContentMessageReporterWithExtendedData, IBasicMessageReporterWithExtendedData
	{
		private readonly IBilingualContentMessageReporter _messageReporter;

		public BilingualContentMessageReporterProxy(IBilingualContentMessageReporter messageReporter)
		{
			if (messageReporter == null)
			{
				throw new ArgumentNullException("messageReporter");
			}
			_messageReporter = messageReporter;
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			_messageReporter.ReportMessage(source, origin, level, message, locationDescription);
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription, ExtendedMessageEventData extendedData)
		{
			IBasicMessageReporterWithExtendedData basicMessageReporterWithExtendedData = _messageReporter as IBasicMessageReporterWithExtendedData;
			if (basicMessageReporterWithExtendedData != null)
			{
				basicMessageReporterWithExtendedData.ReportMessage(source, origin, level, message, locationDescription, extendedData);
			}
			else
			{
				_messageReporter.ReportMessage(source, origin, level, message, locationDescription);
			}
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation)
		{
			_messageReporter.ReportMessage(source, origin, level, message, fromLocation, uptoLocation);
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation, ExtendedMessageEventData extendedData)
		{
			IBilingualContentMessageReporterWithExtendedData bilingualContentMessageReporterWithExtendedData = _messageReporter as IBilingualContentMessageReporterWithExtendedData;
			if (bilingualContentMessageReporterWithExtendedData != null)
			{
				bilingualContentMessageReporterWithExtendedData.ReportMessage(source, origin, level, message, fromLocation, uptoLocation, extendedData);
			}
			else
			{
				_messageReporter.ReportMessage(source, origin, level, message, fromLocation, uptoLocation);
			}
		}
	}
}
