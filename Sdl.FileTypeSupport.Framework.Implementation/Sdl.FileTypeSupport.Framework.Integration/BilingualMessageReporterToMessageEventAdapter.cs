using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class BilingualMessageReporterToMessageEventAdapter : IBilingualContentMessageReporter, IBasicMessageReporter, IBilingualContentMessageReporterWithExtendedData, IBasicMessageReporterWithExtendedData
	{
		private string _filePath;

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

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
			}
		}

		public BilingualMessageReporterToMessageEventAdapter(EventHandler<MessageEventArgs> messageHandler)
		{
			_messageHandler = messageHandler;
		}

		public virtual void OnMessage(object source, MessageEventArgs args)
		{
			if (_messageHandler != null)
			{
				_messageHandler(source, args);
			}
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation)
		{
			ReportMessage(source, origin, level, message, fromLocation, uptoLocation, null);
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation, ExtendedMessageEventData extendedData)
		{
			BilingualMessageLocation fromLocation2 = null;
			if (fromLocation != null)
			{
				fromLocation2 = new BilingualMessageLocation(fromLocation);
			}
			BilingualMessageLocation uptoLocation2 = null;
			if (uptoLocation != null)
			{
				uptoLocation2 = new BilingualMessageLocation(uptoLocation);
			}
			OnMessage(source, new MessageEventArgs(_filePath, origin, level, message, fromLocation2, uptoLocation2, extendedData));
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			ReportMessage(source, origin, level, message, locationDescription, null);
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription, ExtendedMessageEventData extendedData)
		{
			BilingualMessageLocation bilingualMessageLocation = new BilingualMessageLocation();
			bilingualMessageLocation.LocationDescription = locationDescription;
			OnMessage(source, new MessageEventArgs(_filePath, origin, level, message, bilingualMessageLocation, null, extendedData));
		}
	}
}
