namespace Sdl.Desktop.Logger
{
	public class LoggerFactoryAdapter : ILoggerFactory
	{
		public ILog GetLogger(string name)
		{
			return LoggerFactory.GetLogger(name);
		}
	}
}
