using System;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	internal class GetLicenseCompletedEventArgs
	{
		public Exception Error
		{
			get;
			private set;
		}

		public GetLicenseCompletedEventArgs(Exception error)
		{
			Error = error;
		}
	}
}
