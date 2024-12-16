using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PostGenerationMessageReporterAdapter : INativeTextLocationMessageReporter, IBasicMessageReporter
	{
		private INativeTextLocationInfoProvider _textLocationInfoProvider;

		private INativeGenerationBilingualContentLocator _bilingualContentLocator;

		private EventHandler<MessageEventArgs> _externalMessageReporter;

		public PostGenerationMessageReporterAdapter(EventHandler<MessageEventArgs> externalMessageReporter, INativeTextLocationInfoProvider textLocationInfoProvider, INativeGenerationBilingualContentLocator bilingualContentLocator)
		{
			_externalMessageReporter = externalMessageReporter;
			_textLocationInfoProvider = textLocationInfoProvider;
			_bilingualContentLocator = bilingualContentLocator;
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, NativeTextLocation fromLocation, NativeTextLocation uptoLocation)
		{
			if (_externalMessageReporter != null)
			{
				if (_textLocationInfoProvider == null || _bilingualContentLocator == null)
				{
					string locationDescription = string.Format(StringResources.MessageLocationDescriptionForLineAndOffsetNumbers, fromLocation.Line, fromLocation.Offset, uptoLocation?.Line ?? fromLocation.Line, uptoLocation?.Offset ?? fromLocation.Offset);
					ReportMessage(source, origin, level, message, locationDescription);
					return;
				}
				LocationInfo locationInfo = _textLocationInfoProvider.GetLocationInfo(fromLocation);
				LocationInfo locationInfo2 = _textLocationInfoProvider.GetLocationInfo(uptoLocation);
				IMessageLocation fromLocation2 = _bilingualContentLocator.FindLocation(locationInfo);
				IMessageLocation uptoLocation2 = _bilingualContentLocator.FindLocation(locationInfo2);
				_externalMessageReporter(source, new MessageEventArgs(null, origin, level, message, fromLocation2, uptoLocation2));
			}
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			if (_externalMessageReporter != null)
			{
				BilingualMessageLocation bilingualMessageLocation = new BilingualMessageLocation();
				bilingualMessageLocation.LocationDescription = locationDescription;
				_externalMessageReporter(source, new MessageEventArgs(null, origin, level, message, bilingualMessageLocation, null));
			}
		}
	}
}
