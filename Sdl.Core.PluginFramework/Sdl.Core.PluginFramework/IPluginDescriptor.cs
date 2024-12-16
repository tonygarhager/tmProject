using System.IO;

namespace Sdl.Core.PluginFramework
{
	public interface IPluginDescriptor
	{
		string Name
		{
			get;
		}

		Stream GetPluginManifestStream();

		object GetPluginResource(string name);
	}
}
