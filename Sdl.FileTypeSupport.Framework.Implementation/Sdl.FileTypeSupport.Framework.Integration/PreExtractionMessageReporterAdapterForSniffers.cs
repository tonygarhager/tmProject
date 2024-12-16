using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PreExtractionMessageReporterAdapterForSniffers : PreExtractionMessageReporterAdapter
	{
		public FileTypeDefinitionId CurrentFileTypeDefinitionId
		{
			get;
			set;
		}

		public PreExtractionMessageReporterAdapterForSniffers(EventHandler<MessageEventArgs> messageHandler)
			: base(messageHandler)
		{
		}

		public override void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			origin = CurrentFileTypeDefinitionId.Id;
			base.ReportMessage(source, origin, level, message, locationDescription);
		}

		public override void ReportMessage(object source, string origin, ErrorLevel level, string message, NativeTextLocation fromLocation, NativeTextLocation uptoLocation)
		{
			origin = CurrentFileTypeDefinitionId.Id;
			base.ReportMessage(source, origin, level, message, fromLocation, uptoLocation);
		}
	}
}
