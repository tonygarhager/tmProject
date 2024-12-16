using System;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class SubContentProcessorDoesNotExistException : Exception
	{
		public SubContentProcessorDoesNotExistException(string message)
			: base(message)
		{
		}
	}
}
