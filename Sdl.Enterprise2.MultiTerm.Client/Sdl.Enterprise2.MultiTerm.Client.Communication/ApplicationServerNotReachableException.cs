using System;
using System.Runtime.Serialization;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	[Serializable]
	public class ApplicationServerNotReachableException : Exception
	{
		public ApplicationServerNotReachableException()
		{
		}

		public ApplicationServerNotReachableException(string message)
			: base(message)
		{
		}

		public ApplicationServerNotReachableException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ApplicationServerNotReachableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
