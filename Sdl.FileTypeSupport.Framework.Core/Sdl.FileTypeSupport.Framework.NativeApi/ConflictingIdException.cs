using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	[Serializable]
	public class ConflictingIdException : FileTypeSupportException
	{
		private string _id;

		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public ConflictingIdException()
		{
		}

		public ConflictingIdException(string message)
			: base(message)
		{
		}

		public ConflictingIdException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ConflictingIdException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
