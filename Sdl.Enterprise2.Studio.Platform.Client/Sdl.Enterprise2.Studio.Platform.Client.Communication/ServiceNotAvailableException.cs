using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.Studio.Platform.Client.Communication
{
	[Serializable]
	public class ServiceNotAvailableException : Exception
	{
		public ServiceNotAvailableException()
		{
		}

		public ServiceNotAvailableException(string message)
			: base(message)
		{
		}

		public ServiceNotAvailableException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ServiceNotAvailableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
