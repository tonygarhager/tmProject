using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.MultiTerm.Client.Licensing
{
	[Serializable]
	public class LicenseCountExceededException : LicensingException
	{
		public LicenseCountExceededException()
		{
		}

		public LicenseCountExceededException(string message)
			: base(message)
		{
		}

		public LicenseCountExceededException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected LicenseCountExceededException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
