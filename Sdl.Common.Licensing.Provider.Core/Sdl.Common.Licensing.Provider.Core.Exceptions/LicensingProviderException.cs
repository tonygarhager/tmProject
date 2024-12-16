using System;

namespace Sdl.Common.Licensing.Provider.Core.Exceptions
{
	public class LicensingProviderException : Exception
	{
		public override string Message
		{
			get
			{
				if (base.InnerException != null)
				{
					return $"{base.Message} - {base.InnerException.Message}";
				}
				return base.Message;
			}
		}

		public virtual long ErrorCode => ProviderErrorCode;

		public string ErrorMessage
		{
			get
			{
				long errorCode = ErrorCode;
				long num = errorCode;
				if (num <= 16)
				{
					long num2 = num - 1;
					if ((ulong)num2 <= 3uL)
					{
						switch (num2)
						{
						case 3L:
							return StringResources.LicenseStatusServerControl_Error_No_Seats;
						case 0L:
							return StringResources.LicenseStatusServerControl_Error_Cannot_Connect;
						case 1L:
							return StringResources.LicenseStatusServerControl_Error_NetworkError_MultipleInstances;
						case 2L:
							goto IL_00b7;
						}
					}
					switch (num)
					{
					case 8L:
						return StringResources.LicenseStatusServerControl_Error_NetworkError_LicenseNotBorrowable;
					case 16L:
						return StringResources.LicenseStatusServerControl_Error_NetworkError_LicenseAlreadyBorrowed;
					}
				}
				else
				{
					switch (num)
					{
					case 64L:
						return StringResources.LicenseStatusServerControl_NoLicenseAvailable;
					case 32L:
						return StringResources.Error_LicenseExpired;
					case 128L:
						return StringResources.LicenseStatusServerControl_Error_LicenseTamperingDetected;
					case 256L:
						return StringResources.LicenseStatusServerControl_Error_LicenseRevoked;
					}
				}
				goto IL_00b7;
				IL_00b7:
				return StringResources.LicenseStatusServerControl_Error_General;
			}
		}

		public virtual long ProviderErrorCode
		{
			get;
			protected set;
		}

		public LicensingProviderException()
		{
		}

		public LicensingProviderException(string message)
			: base(message)
		{
		}

		public LicensingProviderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
