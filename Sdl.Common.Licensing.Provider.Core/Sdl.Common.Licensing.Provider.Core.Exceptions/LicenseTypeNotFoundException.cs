using System;

namespace Sdl.Common.Licensing.Provider.Core.Exceptions
{
	public class LicenseTypeNotFoundException : Exception
	{
		public LicenseTypeNotFoundException(string message)
			: base(message)
		{
		}
	}
}
