using log4net.Config;

namespace Sdl.Desktop.Logger
{
	public static class LoggerFactory
	{
		public static bool LoggerInitialized
		{
			get;
			private set;
		}

		public static ILog GetLogger(string name)
		{
			if (!LoggerInitialized)
			{
				XmlConfigurator.Configure();
				LoggerInitialized = true;
			}
			return new Log(name);
		}
	}
}
