using NLog;
using System;

namespace Sdl.Core.LanguageProcessing
{
	public class NLogLogger : ILogLogger
	{
		private const string LoggerName = "logger";

		private readonly Logger _logger;

		public NLogLogger()
		{
			_logger = NLog.LogManager.GetCurrentClassLogger();
		}

		public void Log(string msg)
		{
			LogInternal(LogLevel.Info, msg);
		}

		public void Log(Exception ex)
		{
			LogInternal(LogLevel.Error, null, ex);
		}

		public void LogError(string msg)
		{
			LogInternal(LogLevel.Error, msg);
		}

		public void LogWarn(string msg)
		{
			LogInternal(LogLevel.Warn, msg);
		}

		public void LogDebug(string msg)
		{
			LogInternal(LogLevel.Debug, msg);
		}

		private void LogInternal(LogLevel logLevel, string message, Exception exception = null)
		{
			try
			{
				LogEventInfo logEvent = new LogEventInfo(logLevel, "logger", null, message, null, exception);
				_logger.Log(logEvent);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}
	}
}
