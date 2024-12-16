using System;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ITelemetrySupport
	{
		void TrackException(Exception e);

		void TrackUnhandledException(UnhandledExceptionEventArgs e);
	}
}
