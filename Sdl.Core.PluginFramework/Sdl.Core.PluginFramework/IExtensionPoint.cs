using System;

namespace Sdl.Core.PluginFramework
{
	public interface IExtensionPoint
	{
		string Name
		{
			get;
		}

		Type ExtensionAttributeType
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

		ExtensionCollection AllExtensions
		{
			get;
		}

		event EventHandler<ExtensionEventArgs> ExtensionEnabledChanged;
	}
}
