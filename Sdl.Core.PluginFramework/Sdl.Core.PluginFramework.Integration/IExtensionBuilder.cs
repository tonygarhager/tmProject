namespace Sdl.Core.PluginFramework.Integration
{
	public interface IExtensionBuilder
	{
		object Build(IExtension extension, object defaultInstance, ExtensionArguments arguments);
	}
}
