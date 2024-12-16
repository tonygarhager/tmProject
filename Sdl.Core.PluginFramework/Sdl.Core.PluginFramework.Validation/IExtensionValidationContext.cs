using System;

namespace Sdl.Core.PluginFramework.Validation
{
	public interface IExtensionValidationContext
	{
		void ReportError(string code, string message);

		void ReportWarning(string code, string message);

		void ValidateRequiredInterface(Type interfaceType);

		void ValidateRequiredExtensionAttributeType(AuxiliaryExtensionAttribute auxiliaryExtensionAttribute, Type extensionAttributeType);
	}
}
