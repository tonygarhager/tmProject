using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.MultiTerm.Client.Licensing
{
	[Serializable]
	public class LicensingException : Exception
	{
		public bool ProductFeatureNotLicensed
		{
			get;
			set;
		}

		public LicensingException()
		{
		}

		public LicensingException(string message)
			: base(message)
		{
		}

		public LicensingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected LicensingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
