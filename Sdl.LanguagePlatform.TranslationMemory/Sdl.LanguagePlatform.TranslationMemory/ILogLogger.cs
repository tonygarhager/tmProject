using System;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public interface ILogLogger
	{
		void Log(Exception ex);

		void Log(string msg);

		void LogDebug(string msg);

		void LogError(string msg);

		void LogWarn(string msg);
	}
}
