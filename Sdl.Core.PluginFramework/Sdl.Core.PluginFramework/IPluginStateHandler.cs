namespace Sdl.Core.PluginFramework
{
	public interface IPluginStateHandler
	{
		bool CanEnable(IPlugin plugin);

		bool CanDisable(IPlugin plugin);
	}
}
