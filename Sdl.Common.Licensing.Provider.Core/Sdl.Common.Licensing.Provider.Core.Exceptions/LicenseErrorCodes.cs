using System;

namespace Sdl.Common.Licensing.Provider.Core.Exceptions
{
	[Flags]
	internal enum LicenseErrorCodes : uint
	{
		NetworkError_NoConnectionToServer = 0x1,
		NetworkError_MultipleInstances = 0x2,
		Error_NoSeats = 0x4,
		Error_LicenseNotBorrowable = 0x8,
		Error_LicenseAlreadyBorrowed = 0x10,
		Error_LicenseExpired = 0x20,
		Error_NoLicenseAvailable = 0x40,
		Error_LicenseTamperingDetected = 0x80,
		Error_LicenseRevoked = 0x100,
		Error_InvalidLicense = 0x200,
		Error_BadHandle = 0x400,
		GeneralError = 0x7FFFFFFF
	}
}
