using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	[Serializable]
	public class Framework1ExternalPreviewException : Exception
	{
		public Framework1ExternalPreviewException()
		{
		}

		public Framework1ExternalPreviewException(string message)
			: base(message)
		{
		}

		public Framework1ExternalPreviewException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected Framework1ExternalPreviewException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
