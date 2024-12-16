using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	[Serializable]
	public class NotSplittableException : FileTypeSupportException
	{
		public NotSplittableException()
		{
		}

		public NotSplittableException(string message)
			: base(message)
		{
		}

		public NotSplittableException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected NotSplittableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
