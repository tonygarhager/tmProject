namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public abstract class AbstractNativeFileTypeComponent : INativeFileTypeComponent, INativeContentStreamMessageReporter, IBasicMessageReporter
	{
		private IPropertiesFactory _factory;

		private INativeContentStreamMessageReporter _messageReporter;

		public virtual IPropertiesFactory PropertiesFactory
		{
			get
			{
				return _factory;
			}
			set
			{
				_factory = value;
			}
		}

		public virtual INativeContentStreamMessageReporter MessageReporter
		{
			get
			{
				return _messageReporter;
			}
			set
			{
				_messageReporter = value;
			}
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, LocationMarkerId fromLocation, LocationMarkerId uptoLocation)
		{
			if (MessageReporter != null)
			{
				MessageReporter.ReportMessage(source, origin, level, message, fromLocation, uptoLocation);
			}
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			if (MessageReporter != null)
			{
				MessageReporter.ReportMessage(source, origin, level, message, locationDescription);
			}
		}
	}
}
