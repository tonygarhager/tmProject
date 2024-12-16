namespace Sdl.Core.PluginFramework
{
	public interface IPlugin
	{
		PluginId Id
		{
			get;
		}

		string Version
		{
			get;
		}

		string Name
		{
			get;
		}

		IPluginDescriptor Descriptor
		{
			get;
		}

		bool Enabled
		{
			get;
		}

		bool CanEnable
		{
			get;
		}

		bool CanDisable
		{
			get;
		}

		bool IsDynamic
		{
			get;
		}

		ExtensionCollection Extensions
		{
			get;
		}

		PluginStatus Status
		{
			get;
		}

		bool SetEnabled(bool enabled);

		T GetPluginResource<T>(string resourceName) where T : class;

		void Validate();
	}
}
