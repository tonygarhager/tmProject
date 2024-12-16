using Sdl.Core.PluginFramework.Validation;
using System;

namespace Sdl.Core.PluginFramework.Implementation.Validation
{
	internal class ExtensionAttributeInfo : IExtensionAttributeInfo
	{
		private Type _extensionType;

		private ExtensionAttribute _extensionAttribute;

		private AuxiliaryExtensionAttribute[] _auxiliaryExtensionAttributes;

		public Type ExtensionType => _extensionType;

		public ExtensionAttribute ExtensionAttribute => _extensionAttribute;

		public AuxiliaryExtensionAttribute[] AuxiliaryExtensionAttributes => _auxiliaryExtensionAttributes;

		public ExtensionAttributeInfo(Type extensionType, ExtensionAttribute extensionAttribute, AuxiliaryExtensionAttribute[] auxiliaryExtensionAttributes)
		{
			_extensionType = extensionType;
			_extensionAttribute = extensionAttribute;
			_auxiliaryExtensionAttributes = auxiliaryExtensionAttributes;
		}
	}
}
