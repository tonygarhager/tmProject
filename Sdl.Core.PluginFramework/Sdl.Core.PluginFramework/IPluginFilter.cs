namespace Sdl.Core.PluginFramework
{
	public interface IPluginFilter
	{
		bool ShouldLoadPlugin(string pluginName);

		bool ShouldLoadExtension(IPlugin plugin, string extensionId);
	}
}
