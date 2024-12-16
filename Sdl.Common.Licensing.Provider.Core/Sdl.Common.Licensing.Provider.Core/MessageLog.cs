using log4net;

namespace Sdl.Common.Licensing.Provider.Core
{
	public class MessageLog
	{
		private static ILog _defaultLog;

		public static ILog DefaultLog
		{
			get
			{
				if (_defaultLog == null)
				{
					_defaultLog = LogManager.GetLogger("Licensing");
				}
				return _defaultLog;
			}
		}
	}
}
