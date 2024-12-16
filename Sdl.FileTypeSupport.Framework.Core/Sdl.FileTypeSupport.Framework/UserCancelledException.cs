using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework
{
	[Serializable]
	public class UserCancelledException : Exception
	{
		public UserCancelledException()
		{
		}

		public UserCancelledException(string message)
			: base(message)
		{
		}

		public UserCancelledException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected UserCancelledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
