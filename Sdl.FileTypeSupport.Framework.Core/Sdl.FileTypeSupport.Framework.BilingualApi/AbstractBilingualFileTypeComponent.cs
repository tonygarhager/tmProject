using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public class AbstractBilingualFileTypeComponent : IBilingualFileTypeComponent, IBilingualContentMessageReporter, IBasicMessageReporter, IBilingualContentMessageReporterWithExtendedData, IBasicMessageReporterWithExtendedData
	{
		private IDocumentItemFactory _ItemFactory;

		private IBilingualContentMessageReporter _messageReporter;

		public virtual IDocumentItemFactory ItemFactory
		{
			get
			{
				return _ItemFactory;
			}
			set
			{
				_ItemFactory = value;
			}
		}

		public IPropertiesFactory PropertiesFactory
		{
			get
			{
				if (ItemFactory != null)
				{
					return ItemFactory.PropertiesFactory;
				}
				return null;
			}
		}

		public virtual IBilingualContentMessageReporter MessageReporter
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

		protected AbstractBilingualFileTypeComponent()
		{
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation)
		{
			ReportMessage(source, origin, level, message, fromLocation, uptoLocation, null);
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation, ExtendedMessageEventData extendedData)
		{
			if (MessageReporter != null)
			{
				BilingualContentMessageReporterProxy bilingualContentMessageReporterProxy = new BilingualContentMessageReporterProxy(MessageReporter);
				bilingualContentMessageReporterProxy.ReportMessage(source, origin, level, message, fromLocation, uptoLocation, extendedData);
			}
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			ReportMessage(source, origin, level, message, locationDescription, null);
		}

		public void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription, ExtendedMessageEventData extendedData)
		{
			if (MessageReporter != null)
			{
				BilingualContentMessageReporterProxy bilingualContentMessageReporterProxy = new BilingualContentMessageReporterProxy(MessageReporter);
				bilingualContentMessageReporterProxy.ReportMessage(source, origin, level, message, locationDescription, extendedData);
			}
		}
	}
}
