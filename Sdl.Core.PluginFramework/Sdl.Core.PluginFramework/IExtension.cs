using System;

namespace Sdl.Core.PluginFramework
{
	public interface IExtension
	{
		IPlugin Plugin
		{
			get;
		}

		bool Enabled
		{
			get;
		}

		IExtensionPoint ExtensionPoint
		{
			get;
		}

		ExtensionAttribute ExtensionAttribute
		{
			get;
		}

		Type ExtensionType
		{
			get;
		}

		AuxiliaryExtensionAttributeCollection AuxiliaryExtensionAttributes
		{
			get;
		}

		event EventHandler<ExtensionEventArgs> EnabledChanged;

		bool SetEnabled(bool enabled);

		object CreateInstance();

		T[] GetAuxiliaryExtensionAttributes<T>() where T : AuxiliaryExtensionAttribute;

		void Validate();
	}
}
