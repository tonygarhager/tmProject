namespace Sdl.Core.PluginFramework
{
	public interface IPluginCache
	{
		PluginState GetPluginState(string pluginId);

		void StorePluginState(IPlugin plugin);

		void Save();
	}
}
