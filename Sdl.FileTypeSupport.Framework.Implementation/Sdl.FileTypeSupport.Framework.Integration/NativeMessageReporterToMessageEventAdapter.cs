using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class NativeMessageReporterToMessageEventAdapter : INativeContentStreamMessageReporter, IBasicMessageReporter
	{
		private ILocationMarkerLocator _markerLocator;

		private EventHandler<MessageEventArgs> _messageHandler;

		public EventHandler<MessageEventArgs> MessageHandler
		{
			get
			{
				return _messageHandler;
			}
			set
			{
				_messageHandler = value;
			}
		}

		public ILocationMarkerLocator MarkerLocator
		{
			get
			{
				return _markerLocator;
			}
			set
			{
				_markerLocator = value;
			}
		}

		public NativeMessageReporterToMessageEventAdapter(ILocationMarkerLocator markerLocator, EventHandler<MessageEventArgs> messageHandler)
		{
			_markerLocator = markerLocator;
			_messageHandler = messageHandler;
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, LocationMarkerId fromLocation, LocationMarkerId uptoLocation)
		{
			if (_messageHandler == null)
			{
				return;
			}
			IMessageLocation fromLocation2 = null;
			IMessageLocation uptoLocation2 = null;
			if (_markerLocator != null)
			{
				if (fromLocation != null)
				{
					fromLocation2 = _markerLocator.GetLocation(fromLocation);
				}
				if (uptoLocation != null)
				{
					uptoLocation2 = _markerLocator.GetLocation(uptoLocation);
				}
			}
			_messageHandler(source, new MessageEventArgs(null, origin, level, message, fromLocation2, uptoLocation2));
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			if (_messageHandler != null)
			{
				BilingualMessageLocation bilingualMessageLocation = new BilingualMessageLocation();
				bilingualMessageLocation.LocationDescription = locationDescription;
				_messageHandler(source, new MessageEventArgs(null, origin, level, message, bilingualMessageLocation, null));
			}
		}
	}
}
