using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public sealed class StorageException : LanguagePlatformException
	{
		public StorageException(ErrorCode code)
			: base(code)
		{
		}

		public StorageException(ErrorCode code, string data)
			: base(code, data)
		{
		}

		public StorageException(ErrorCode code, FaultStatus status, string data)
			: base(code, status, data)
		{
		}
	}
}
