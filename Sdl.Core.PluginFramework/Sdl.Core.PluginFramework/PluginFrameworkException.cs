using System;
using System.Runtime.Serialization;

namespace Sdl.Core.PluginFramework
{
	[Serializable]
	public class PluginFrameworkException : Exception
	{
		public PluginFrameworkException()
		{
		}

		public PluginFrameworkException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public PluginFrameworkException(string message)
			: base(message)
		{
		}

		protected PluginFrameworkException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
