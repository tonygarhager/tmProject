using Sdl.FileTypeSupport.Framework;
using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	[Serializable]
	public class XliffParseException : FileTypeSupportException
	{
		public XliffParseException()
		{
		}

		public XliffParseException(string message)
			: base(message)
		{
		}

		public XliffParseException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected XliffParseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
