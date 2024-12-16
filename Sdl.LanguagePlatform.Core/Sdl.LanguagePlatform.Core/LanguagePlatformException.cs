using System;

namespace Sdl.LanguagePlatform.Core
{
	public class LanguagePlatformException : Exception
	{
		public FaultDescription Description
		{
			get;
		}

		public LanguagePlatformException(FaultDescription description, Exception e)
			: base(description.Message, e)
		{
			Description = description;
		}

		public LanguagePlatformException(FaultDescription description)
			: this(description, null)
		{
		}

		public LanguagePlatformException(ErrorCode code, FaultStatus status, string data)
			: this(new FaultDescription(code, status, data))
		{
		}

		public LanguagePlatformException(ErrorCode code, FaultStatus status)
			: this(new FaultDescription(code, status))
		{
		}

		public LanguagePlatformException(ErrorCode code)
			: this(new FaultDescription(code))
		{
		}

		public LanguagePlatformException(ErrorCode code, string data)
			: this(new FaultDescription(code, data))
		{
		}
	}
}
