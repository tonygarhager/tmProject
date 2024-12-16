using System;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeFileWriter : INativeGenerationContentHandler, IAbstractNativeContentHandler, INativeOutputSettingsAware, IDisposable
	{
		INativeTextLocationMessageReporter MessageReporter
		{
			get;
			set;
		}

		INativeLocationTracker LocationTracker
		{
			get;
			set;
		}
	}
}
