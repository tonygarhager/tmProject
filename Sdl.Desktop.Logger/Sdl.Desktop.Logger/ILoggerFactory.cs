namespace Sdl.Desktop.Logger
{
	public interface ILoggerFactory
	{
		ILog GetLogger(string name);
	}
}
