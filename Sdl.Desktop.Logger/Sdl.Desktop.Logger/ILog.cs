using System;

namespace Sdl.Desktop.Logger
{
	public interface ILog
	{
		bool IsDebugEnabled
		{
			get;
		}

		bool IsInfoEnabled
		{
			get;
		}

		bool IsWarnEnabled
		{
			get;
		}

		bool IsErrorEnabled
		{
			get;
		}

		bool IsFatalEnabled
		{
			get;
		}

		void Debug(object message);

		void DebugFormat(string format, params object[] args);

		void DebugFormat(IFormatProvider provider, string format, params object[] args);

		void Info(object message);

		void Info(object message, Exception exception);

		void InfoFormat(string format, params object[] args);

		void InfoFormat(IFormatProvider provider, string format, params object[] args);

		void Resources(object message);

		void Resources(object message, Action action);

		void ResourcesForAllProcesses(object message);

		void Warn(object message);

		void Warn(object message, Exception exception);

		void WarnFormat(string format, params object[] args);

		void WarnFormat(IFormatProvider provider, string format, params object[] args);

		void Error(object message);

		void Error(object message, Exception exception);

		void ErrorFormat(string format, params object[] args);

		void ErrorFormat(IFormatProvider provider, string format, params object[] args);

		void Fatal(object message);

		void Fatal(object message, Exception exception);

		void FatalFormat(string format, params object[] args);

		void FatalFormat(IFormatProvider provider, string format, params object[] args);
	}
}
