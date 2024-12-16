using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Sdl.Desktop.Logger
{
	public class Log : ILog
	{
		private readonly log4net.ILog _log;

		public bool IsDebugEnabled => _log.IsDebugEnabled;

		public bool IsInfoEnabled => _log.IsInfoEnabled;

		public bool IsWarnEnabled => _log.IsWarnEnabled;

		public bool IsErrorEnabled => _log.IsErrorEnabled;

		public bool IsFatalEnabled => _log.IsFatalEnabled;

		public Log(string name)
		{
			_log = LogManager.GetLogger(name);
		}

		public void Debug(object message)
		{
			_log.Debug(message);
		}

		public void DebugFormat(string format, params object[] args)
		{
			_log.DebugFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.DebugFormat(provider, format, args);
		}

		public void Info(object message)
		{
			_log.Info(message);
		}

		public void Info(object message, Exception exception)
		{
			_log.Info(message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			_log.InfoFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.InfoFormat(provider, format, args);
		}

		public void Resources(object message, Action action)
		{
			if (!_log.IsInfoEnabled)
			{
				action();
				return;
			}
			Guid guid = Guid.NewGuid();
			Guid guid2 = guid;
			Resources("Start#Id=" + guid2.ToString() + "#" + message?.ToString());
			action();
			guid2 = guid;
			Resources("Finish#Id=" + guid2.ToString() + "#" + message?.ToString());
		}

		public void Resources(object message)
		{
			if (_log.IsInfoEnabled)
			{
				try
				{
					Info(GetResourcesMessage(message));
				}
				catch (Exception exception)
				{
					try
					{
						Warn("Could not get system resources statistics for current process", exception);
					}
					catch
					{
					}
				}
			}
		}

		private string GetResourcesMessage(object message)
		{
			return GetResourcesMessage(message, Process.GetCurrentProcess());
		}

		private string GetResourcesMessage(object message, Process process)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(message);
			SystemResourcesStatistics systemResourcesStatistics = new SystemResourcesStatistics(process);
			IDictionary<string, object> systemResourcesStatisticsDictionary = systemResourcesStatistics.GetSystemResourcesStatisticsDictionary();
			foreach (KeyValuePair<string, object> item in systemResourcesStatisticsDictionary)
			{
				stringBuilder.Append("#");
				stringBuilder.Append(item.Key);
				stringBuilder.Append("=");
				stringBuilder.Append(item.Value);
			}
			return stringBuilder.ToString();
		}

		public void ResourcesForAllProcesses(object message)
		{
			if (_log.IsInfoEnabled)
			{
				Process[] processes;
				try
				{
					processes = Process.GetProcesses();
				}
				catch (Exception exception)
				{
					try
					{
						Warn("Could not get processes for system resoures statistics", exception);
					}
					catch
					{
					}
					return;
				}
				Process[] array = processes;
				foreach (Process process in array)
				{
					try
					{
						Info(GetResourcesMessage(message, process));
					}
					catch (Exception exception2)
					{
						try
						{
							Warn("Could not get system resources statistics for '" + process.ProcessName + "'", exception2);
						}
						catch
						{
						}
					}
				}
			}
		}

		public void Warn(object message)
		{
			_log.Warn(message);
		}

		public void Warn(object message, Exception exception)
		{
			_log.Warn(message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			_log.WarnFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.WarnFormat(provider, format, args);
		}

		public void Error(object message)
		{
			_log.Error(message);
		}

		public void Error(object message, Exception exception)
		{
			_log.Error(message, exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			_log.ErrorFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.ErrorFormat(provider, format, args);
		}

		public void Fatal(object message)
		{
			_log.Fatal(message);
		}

		public void Fatal(object message, Exception exception)
		{
			_log.Fatal(message, exception);
		}

		public void FatalFormat(string format, params object[] args)
		{
			_log.FatalFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.FatalFormat(provider, format, args);
		}
	}
}
