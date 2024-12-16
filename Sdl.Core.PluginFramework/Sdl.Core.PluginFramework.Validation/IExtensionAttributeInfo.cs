using System;

namespace Sdl.Core.PluginFramework.Validation
{
	public interface IExtensionAttributeInfo
	{
		Type ExtensionType
		{
			get;
		}

		ExtensionAttribute ExtensionAttribute
		{
			get;
		}

		AuxiliaryExtensionAttribute[] AuxiliaryExtensionAttributes
		{
			get;
		}
	}
}
