using System;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class LogManager
	{
		private readonly ILogLogger _logger;

		private static readonly Lazy<LogManager> Instance = new Lazy<LogManager>(() => new LogManager());

		public static ILogLogger Logger => Instance.Value._logger;

		private LogManager()
		{
			_logger = new NLogLogger();
		}
	}
}
