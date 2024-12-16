using Sdl.FileTypeSupport.Framework;
using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	[Serializable]
	public class XliffEncryptedException : FileTypeSupportException
	{
		public XliffEncryptedException()
		{
		}

		public XliffEncryptedException(string message)
			: base(message)
		{
		}

		public XliffEncryptedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected XliffEncryptedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
