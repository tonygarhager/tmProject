using System;

namespace Sdl.Core.Api.DataAccess
{
	public class DataAccessException : Exception
	{
		public DataAccessException()
		{
		}

		public DataAccessException(string message)
			: base(message)
		{
		}

		public DataAccessException(Exception innerException)
			: base(innerException.Message, innerException)
		{
		}

		public DataAccessException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
